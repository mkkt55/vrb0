/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

namespace dpn
{
/// This script provides an implemention of Unity's `BaseInputModule` class, so
/// that Canvas-based (_uGUI_) UI elements and 3D scene objects can be
/// interacted with in a Dpn Application.
///
/// This script is intended for use with either a
/// 3D pointer with the Daydream Controller (Recommended for Daydream),
/// or a Gaze-based-pointer (Recommended for Cardboard).
///
/// To use, attach to the scene's **EventSystem** object.  Be sure to move it above the
/// other modules, such as _TouchInputModule_ and _StandaloneInputModule_, in order
/// for the pointer to take priority in the event system.
///
/// If you are using a **Canvas**, set the _Render Mode_ to **World Space**,
/// and add the _DpnPointerGraphicRaycaster_ script to the object.
///
/// If you'd like pointers to work with 3D scene objects, add a _DpnPointerPhysicsRaycaster_ to the main camera,
/// and add a component that implements one of the _Event_ interfaces (_EventTrigger_ will work nicely) to
/// an object with a collider.
///
/// DpnPointerInputModule emits the following events: _Enter_, _Exit_, _Down_, _Up_, _Click_, _Select_,
/// _Deselect_, _UpdateSelected_, and _DpnPointerHover_.  Scroll, move, and submit/cancel events are not emitted.
///
/// To use a 3D Pointer with the Daydream Controller:
///   - Add the prefab GoogleVR/Prefabs/UI/DpnControllerPointer to your scene.
///   - Set the parent of DpnControllerPointer to the same parent as the main camera
///     (With a local position of 0,0,0).
///
/// To use a Gaze-based-pointer:
///   - Add the prefab GoogleVR/Prefabs/UI/DpnReticlePointer to your scene.
///   - Set the parent of DpnReticlePointer to the main camera.
///
	[System.Obsolete]
	public class DpnPointerInputModule : BaseInputModule
	{
		/// Deepoon button event
		/// developer need to change it depending by games
		private bool ButtonState
		{
			get
			{
				return Input.GetMouseButton(0) || DpnDaydreamController.ClickButton || Input.GetKey(KeyCode.Joystick1Button0);
			}
		}
		private bool ButtonDown
		{
			get
			{
				return Input.GetMouseButtonDown(0) || DpnDaydreamController.ClickButtonDown || Input.GetKeyDown(KeyCode.Joystick1Button0);
			}
		}

        public static DpnPointerInputModule _instance = null;

        /// Determines whether pointer input is active in VR Mode only (`true`), or all of the
        /// time (`false`).  Set to false if you plan to use direct screen taps or other
        /// input when not in VR Mode.
        [Tooltip("Whether pointer input is active in VR Mode only (true), or all the time (false).")]
		public bool vrModeOnly = false;

		public PointerEventData pointerData { get; private set; }
		private Vector2 lastHeadPose;

		// Active state
		private bool isActive = false;

		/// Time in seconds between the pointer down and up events sent by a trigger.
		/// Allows time for the UI elements to make their state transitions.
		private const float clickTime = 0.1f;
		// Based on default time for a button to animate to Pressed.

		/// The IDpnPointer which will be responding to pointer events.
		private IDpnPointer pointer
		{
			get
			{
				return DpnPointerManager.Pointer;
			}
		}

        protected override void OnEnable()
        {
            _instance = this;
            base.OnEnable();
        }

        /// @cond
        public override bool ShouldActivateModule()
		{

			bool isVrModeEnabled = !vrModeOnly;
			bool activeState = base.ShouldActivateModule() && isVrModeEnabled;

			if (activeState != isActive)
			{
				isActive = activeState;

				// Activate pointer
				if (pointer != null)
				{
					if (isActive)
					{
						pointer.OnInputModuleEnabled();
					}
				}
			}

			return activeState;
		}

		/// @endcond

		public override void DeactivateModule()
		{
			DisablePointer();
			base.DeactivateModule();
			if (pointerData != null)
			{
				HandlePendingClick();
				HandlePointerExitAndEnter(pointerData, null);
				pointerData = null;
			}
			eventSystem.SetSelectedGameObject(null, GetBaseEventData());
		}

		public override bool IsPointerOverGameObject(int pointerId)
		{
			return pointerData != null && pointerData.pointerEnter != null;
		}

		public override void Process()
		{
			// Save the previous Game Object
			GameObject previousObject = GetCurrentGameObject();

			CastRay();
			UpdateCurrentObject(previousObject);
			UpdateReticle(previousObject);

			bool isDpnTriggered = ButtonDown;
			bool handlePendingClickRequired = !ButtonState;

			// Handle input
			if (!ButtonDown && ButtonState)
			{
				HandleDrag();
			}
			else if (Time.unscaledTime - pointerData.clickTime < clickTime)
			{
				// Delay new events until clickTime has passed.
			}
			else if (!pointerData.eligibleForClick &&
					 (isDpnTriggered || ButtonDown))
			{
				// New trigger action.
				HandleTrigger();
			}
			else if (handlePendingClickRequired)
			{
				// Check if there is a pending click to handle.
				HandlePendingClick();
			}
		}

		/// @endcond

		private void CastRay()
		{
			Quaternion headOrientation;
			headOrientation = DpnCameraRig._instance.GetPose();
			Vector2 headPose = NormalizedCartesianToSpherical(headOrientation * Vector3.forward);

			if (pointerData == null)
			{
				pointerData = new PointerEventData(eventSystem);
				lastHeadPose = headPose;
			}

			// Cast a ray into the scene
			pointerData.Reset();
			pointerData.position = GetPointerPosition();
			eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
			RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);
			if (raycastResult.worldPosition == Vector3.zero)
			{
				raycastResult.worldPosition = GetIntersectionPosition(pointerData.enterEventCamera, raycastResult);
			}

			pointerData.pointerCurrentRaycast = raycastResult;
			m_RaycastResultCache.Clear();
			pointerData.delta = headPose - lastHeadPose;
			lastHeadPose = headPose;
		}

		private void UpdateCurrentObject(GameObject previousObject)
		{
			// Send enter events and update the highlight.
			GameObject currentObject = GetCurrentGameObject(); // Get the pointer target
			HandlePointerExitAndEnter(pointerData, previousObject);

			// Update the current selection, or clear if it is no longer the current object.
			var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(currentObject);
			if (selected == eventSystem.currentSelectedGameObject)
			{
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
				  ExecuteEvents.updateSelectedHandler);
			}
			else
			{
				eventSystem.SetSelectedGameObject(null, pointerData);
			}

			// Execute hover event.
			if (currentObject == previousObject)
			{
				ExecuteEvents.Execute(currentObject, pointerData, DpnExecuteEventsExtension.pointerHoverHandler);
			}
		}

		private void UpdateReticle(GameObject previousObject)
		{
			if (pointer == null)
			{
				return;
			}

			//Camera camera = pointerData.enterEventCamera; // Get the camera
			GameObject currentObject = GetCurrentGameObject(); // Get the pointer target
			Vector3 intersectionPosition = pointerData.pointerCurrentRaycast.worldPosition;
			bool isInteractive = pointerData.pointerPress != null ||
								 ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject) != null;

			if (currentObject == previousObject)
			{
				if (currentObject != null)
				{
					pointer.OnPointerHover(currentObject, intersectionPosition, GetLastRay(), isInteractive);
				}
			}
			else
			{
				if (previousObject != null)
				{
					pointer.OnPointerExit(previousObject);
				}

				if (currentObject != null)
				{
					pointer.OnPointerEnter(currentObject, intersectionPosition, GetLastRay(), isInteractive);
				}
			}
		}

		private void HandleDrag()
		{
			bool moving = (pointerData.position - pointerData.pressPosition).magnitude > 10;

			if (moving && pointerData.pointerDrag != null && !pointerData.dragging)
			{
				ExecuteEvents.Execute(pointerData.pointerDrag, pointerData,
				  ExecuteEvents.beginDragHandler);
				pointerData.dragging = true;
			}

			// Drag notification
			if (pointerData.dragging && moving && pointerData.pointerDrag != null)
			{
				// Before doing drag we should cancel any pointer down state
				// And clear selection!
				if (pointerData.pointerPress != pointerData.pointerDrag)
				{
					ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

					pointerData.eligibleForClick = false;
					pointerData.pointerPress = null;
					pointerData.rawPointerPress = null;
				}
				ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
			}
		}

		private void HandlePendingClick()
		{
			if (!pointerData.eligibleForClick && !pointerData.dragging)
			{
				return;
			}

			if (pointer != null)
			{
				//Camera camera = pointerData.enterEventCamera;
				pointer.OnPointerClickUp();
			}

			var go = pointerData.pointerCurrentRaycast.gameObject;

			// Send pointer up and click events.
			ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
			if (pointerData.eligibleForClick)
			{
				ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
			}
			else if (pointerData.dragging)
			{
				ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.dropHandler);
				ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);
			}

			// Clear the click state.
			pointerData.pointerPress = null;
			pointerData.rawPointerPress = null;
			pointerData.eligibleForClick = false;
			pointerData.clickCount = 0;
			pointerData.clickTime = 0;
			pointerData.pointerDrag = null;
			pointerData.dragging = false;
		}

		private void HandleTrigger()
		{
			var go = pointerData.pointerCurrentRaycast.gameObject;

			// Send pointer down event.
			pointerData.pressPosition = pointerData.position;
			pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
			pointerData.pointerPress =
			  ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler)
			?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

			// Save the drag handler as well
			pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
			if (pointerData.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);
			}

			// Save the pending click state.
			pointerData.rawPointerPress = go;
			pointerData.eligibleForClick = true;
			pointerData.delta = Vector2.zero;
			pointerData.dragging = false;
			pointerData.useDragThreshold = true;
			pointerData.clickCount = 1;
			pointerData.clickTime = Time.unscaledTime;

			if (pointer != null)
			{
				pointer.OnPointerClickDown();
			}
		}

		private Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords)
		{
			cartCoords.Normalize();
			if (cartCoords.x == 0)
				cartCoords.x = Mathf.Epsilon;
			float outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
			if (cartCoords.x < 0)
				outPolar += Mathf.PI;
			float outElevation = Mathf.Asin(cartCoords.y);
			return new Vector2(outPolar, outElevation);
		}

		private GameObject GetCurrentGameObject()
		{
			if (pointerData != null)
			{
				return pointerData.pointerCurrentRaycast.gameObject;
			}

			return null;
		}

		private Ray GetLastRay()
		{
			if (pointerData != null)
			{
				DpnBasePointerRaycaster raycaster = pointerData.pointerCurrentRaycast.module as DpnBasePointerRaycaster;
				if (raycaster != null)
				{
					return raycaster.GetLastRay();
				}
				else if (pointerData.enterEventCamera != null)
				{
					Camera cam = pointerData.enterEventCamera;
					return new Ray(cam.transform.position, cam.transform.forward);
				}
			}

			return new Ray();
		}

		private Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
		{
			// Check for camera
			if (cam == null)
			{
				return Vector3.zero;
			}

			float intersectionDistance = raycastResult.distance + cam.nearClipPlane;
			Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;
			return intersectionPosition;
		}

		private void DisablePointer()
		{
			if (pointer == null)
			{
				return;
			}

			GameObject currentGameObject = GetCurrentGameObject();
			if (currentGameObject)
			{
				pointer.OnPointerExit(currentGameObject);
			}

			pointer.OnInputModuleDisabled();
		}

		private Vector2 GetPointerPosition()
		{
			return pointer != null ? pointer.GetScreenPosition() : Vector2.zero;
		}
	}
}

