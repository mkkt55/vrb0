

using UnityEngine;
using UnityEngine.EventSystems;

namespace dpn
{
    /// <summary>
    /// Main entry point for the DpnStandaloneInputModule API.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.StandaloneInputModule" />
    public class DpnStandaloneInputModule : StandaloneInputModule
    {
        public int touchScrollThreshold = 50;
        public float touchScrollFactor = 0.1f;

        new protected static PointerEventData.FramePressState StateForMouseButton(int buttonId)
        {
            // about controller click button event
            if (buttonId == 0)
            {
                if ((DpnDaydreamController.ClickButtonDown && DpnDaydreamController.ClickButtonUp)
                    || (DpnDaydreamController.TriggerButtonDown && DpnDaydreamController.TriggerButtonUp))
                {
                    return PointerEventData.FramePressState.PressedAndReleased;
                }
                if (DpnDaydreamController.ClickButtonDown || DpnDaydreamController.TriggerButtonDown)
                {
                    return PointerEventData.FramePressState.Pressed;
                }
                if (DpnDaydreamController.ClickButtonUp || DpnDaydreamController.TriggerButtonUp)
                {
                    return PointerEventData.FramePressState.Released;
                }
            }

            bool mouseButtonDown = Input.GetMouseButtonDown(buttonId);
            bool mouseButtonUp = Input.GetMouseButtonUp(buttonId);

            if (mouseButtonDown && mouseButtonUp)
            {
                return PointerEventData.FramePressState.PressedAndReleased;
            }
            if (mouseButtonDown)
            {
                return PointerEventData.FramePressState.Pressed;
            }
            if (mouseButtonUp)
            {
                return PointerEventData.FramePressState.Released;
            }
            return PointerEventData.FramePressState.NotChanged;
        }

        /// <summary>
        /// See BaseInputModule.
        /// </summary>
        public override void Process()
        {
            bool flag = this.SendUpdateEventToSelectedObject();
            if (base.eventSystem.sendNavigationEvents)
            {
                if (!flag)
                {
                    flag |= this.SendMoveEventToSelectedObject();
                }
                if (!flag)
                {
                    this.SendSubmitEventToSelectedObject();
                }
            }
            if (!this.ProcessTouchEvents())
            {
                this.ProcessMouseEvent();
            }
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                bool pressed;
                bool flag;
                PointerEventData touchPointerEventData = GetTouchPointerEventData(touch, out pressed, out flag);
                this.ProcessTouchPress(touchPointerEventData, pressed, flag);
                if (!flag)
                {
                    this.ProcessMove(touchPointerEventData);
                    this.ProcessDrag(touchPointerEventData);

                    if (Mathf.Abs(deltaTouchPos.x) > touchScrollThreshold || Mathf.Abs(deltaTouchPos.y) > touchScrollThreshold)
                    {
                        GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(touchPointerEventData.pointerCurrentRaycast.gameObject);
                        if (eventHandler != null)
                        {
                            if (touchPointerEventData.pointerPress != null)
                            {
                                ExecuteEvents.ExecuteHierarchy<IPointerUpHandler>(touchPointerEventData.pointerPress, touchPointerEventData, ExecuteEvents.pointerUpHandler);
                                touchPointerEventData.pointerPress = null;
                            }
                            ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, touchPointerEventData, ExecuteEvents.scrollHandler);
                        }
                    }
                }
                else
                {
                    base.RemovePointerData(touchPointerEventData);
                }
            }
            
            return Input.touchCount > 0;
        }

        new protected void ProcessMouseEvent()
        {
            this.ProcessMouseEvent(0);
        }

        private readonly PointerInputModule.MouseState m_MouseState = new PointerInputModule.MouseState();

        protected override MouseState GetMousePointerEventData(int id)
        {
            PointerEventData pointerEventData;
            bool pointerData = this.GetPointerData(-1, out pointerEventData, true);
            pointerEventData.Reset();

            // use pointer position instead of mouse position
            Vector2 position;
            if (DpnManager.DPVRPointer)
                position = DpnPointerManager.Pointer != null ? DpnPointerManager.Pointer.GetScreenPosition() : new Vector2(-1,-1);
            else
                position = Input.mousePosition;

            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);

            pointerEventData.delta = position - pointerEventData.position;

            pointerEventData.position = position;
            pointerEventData.scrollDelta = Input.mouseScrollDelta;
            pointerEventData.button = PointerEventData.InputButton.Left;
            base.eventSystem.RaycastAll(pointerEventData, this.m_RaycastResultCache);
            RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
            pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
            this.m_RaycastResultCache.Clear();
            PointerEventData pointerEventData2;
            this.GetPointerData(-2, out pointerEventData2, true);
            this.CopyFromTo(pointerEventData, pointerEventData2);
            pointerEventData2.button = PointerEventData.InputButton.Right;
            PointerEventData pointerEventData3;
            this.GetPointerData(-3, out pointerEventData3, true);
            this.CopyFromTo(pointerEventData, pointerEventData3);
            pointerEventData3.button = PointerEventData.InputButton.Middle;

            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), pointerEventData);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), pointerEventData2);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), pointerEventData3);
            return m_MouseState;
        }



        new protected void ProcessMouseEvent(int id)
        {
            PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
            PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            this.ProcessMousePress(eventData);
            this.ProcessMove(eventData.buttonData);
            this.ProcessDrag(eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
            if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
            {
                GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, eventData.buttonData, ExecuteEvents.scrollHandler);
            }
     
        }

        Vector2 deltaTouchPos;
        Vector2 prevTouchPos;

        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
                base.DeselectIfSelectionChanged(gameObject, pointerEvent);
                if (pointerEvent.pointerEnter != gameObject)
                {
                    base.HandlePointerExitAndEnter(pointerEvent, gameObject);
                    pointerEvent.pointerEnter = gameObject;
                }
                GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
                if (gameObject2 == null)
                {
                    gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                }
                float unscaledTime = Time.unscaledTime;
                if (gameObject2 == pointerEvent.lastPress)
                {
                    float num = unscaledTime - pointerEvent.clickTime;
                    if (num < 0.3f)
                    {
                        pointerEvent.clickCount++;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }
                    pointerEvent.clickTime = unscaledTime;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }
                pointerEvent.pointerPress = gameObject2;
                pointerEvent.rawPointerPress = gameObject;
                pointerEvent.clickTime = unscaledTime;
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                }
            }
            if (released)
            {
                ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
                }
                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;
                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.pointerDrag = null;
                ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }

        }

        new protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            // use pointer position instead of touch position
            Vector2 position;
            if (DpnManager.DPVRPointer)
                position = DpnPointerManager.Pointer.GetScreenPosition();
            else
                position = Input.mousePosition;

            PointerEventData pointerEventData;
            bool pointerData = this.GetPointerData(input.fingerId, out pointerEventData, true);
            pointerEventData.Reset();
            pressed = (pointerData || input.phase == 0);
            released = (input.phase == TouchPhase.Canceled || input.phase == TouchPhase.Ended) || DpnDaydreamController.ClickButtonUp;
            if (pointerData)
            {
                pointerEventData.position = position;
            }
            if (pressed)
            {
                pointerEventData.delta = Vector2.zero;
            }
            else
            {
                pointerEventData.delta = position - pointerEventData.position;
            }
            pointerEventData.position = position;
            pointerEventData.button = PointerEventData.InputButton.Left;
            if(Input.GetMouseButtonDown(0))
            {
                prevTouchPos = Input.GetTouch(0).position;
            }

            deltaTouchPos = (Vector2)Input.GetTouch(0).position - prevTouchPos;
            prevTouchPos = Input.GetTouch(0).position;

            float x = Mathf.Abs(deltaTouchPos.x);
            float y = Mathf.Abs(deltaTouchPos.y);

            Vector2 scrollDelta = Vector2.zero;

            if (x > y)
            {
                if (x > touchScrollThreshold)
                {
                    scrollDelta.x = deltaTouchPos.x * touchScrollFactor;
                }
            }
            else
            {
                if (y > touchScrollThreshold)
                {
                    scrollDelta.y = -deltaTouchPos.y * touchScrollFactor;
                }
            }

            pointerEventData.scrollDelta = scrollDelta;

            base.eventSystem.RaycastAll(pointerEventData, this.m_RaycastResultCache);
            RaycastResult pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
            pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
            this.m_RaycastResultCache.Clear();
            return pointerEventData;
        }

        GameObject _lastGameObejct = null;

        protected override void ProcessMove(PointerEventData pointerEvent)
        {
            // process pointer move
            if (DpnManager.DPVRPointer)
            {
                GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
                if (gameObject)
                {
                    BaseRaycaster raycaster = pointerEvent.pointerCurrentRaycast.module;
                    Ray intersectionRay = new Ray();
                    if (raycaster != null)
                    {
                        DpnBasePointer pointer = DpnPointerManager.Pointer as DpnBasePointer;
                        intersectionRay = pointer.GetRay();
                    }
                    else if (pointerEvent.enterEventCamera != null)
                    {
                        Camera cam = pointerEvent.enterEventCamera;
                        intersectionRay = new Ray(cam.transform.position, cam.transform.forward);
                    }

                    Vector3 intersectionPos = pointerEvent.pointerCurrentRaycast.worldPosition;
                    if (intersectionPos == Vector3.zero)
                    {
                        Camera camera = pointerEvent.enterEventCamera;
                        if (camera != null)
                        {
                            float intersectionDistance = pointerEvent.pointerCurrentRaycast.distance + camera.nearClipPlane;
                            intersectionPos = camera.transform.position + intersectionRay.direction * intersectionDistance;
                        }
                    }

                    bool interactived = pointerEvent.pointerPress != null ||
                                        ExecuteEvents.GetEventHandler<IPointerEnterHandler>(gameObject) != null;

                    if (_lastGameObejct == null)
                    {
                        DpnPointerManager.Pointer.OnPointerEnter(gameObject, intersectionPos, intersectionRay, interactived);

                        _lastGameObejct = gameObject;
                    }
                    else
                    {
                        DpnPointerManager.Pointer.OnPointerHover(gameObject, intersectionPos, intersectionRay, interactived);
                    }
                }
                else
                {
                    if (_lastGameObejct != null)
                    {
                        DpnPointerManager.Pointer.OnPointerExit(gameObject);
                        _lastGameObejct = null;
                    }
                }
            }

            base.ProcessMove(pointerEvent);
        }
    }
}
