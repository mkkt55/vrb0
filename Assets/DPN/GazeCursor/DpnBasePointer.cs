/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace dpn
{
    /// <summary>
    /// Base implementation of IDpnPointer
    ///
    /// Automatically registers pointer with DpnPointerManager.
    /// Uses transform that this script is attached to as the pointer transform.
    ///
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="dpn.IDpnPointer" />
    public abstract class DpnBasePointer : MonoBehaviour, IDpnPointer
	{

		protected virtual void OnEnable()
		{
			DpnPointerManager.OnPointerCreated(this);
		}

        protected virtual void OnDisable()
        {
            DpnPointerManager.OnPointerDestroy(this);
        }

        /// <summary>
        /// Declare methods from IDpnPointer.
        /// </summary>
        public abstract void OnInputModuleEnabled();

		public abstract void OnInputModuleDisabled();

		public abstract void OnPointerEnter(GameObject targetObject, Vector3 intersectionPosition,
			Ray intersectionRay, bool isInteractive);

		public abstract void OnPointerHover(GameObject targetObject, Vector3 intersectionPosition,
			Ray intersectionRay, bool isInteractive);

		public abstract void OnPointerExit(GameObject targetObject);

		public abstract void OnPointerClickDown();

		public abstract void OnPointerClickUp();

		public abstract float GetMaxPointerDistance();

		public abstract void GetPointerRadius(out float innerRadius, out float outerRadius);

		public virtual Transform GetPointerTransform()
		{
			return transform;
		}

        virtual public Vector2 GetScreenPosition()
		{
			return DpnCameraRig.WorldToScreenPoint(transform.position);
		}

		protected float tiltedAngle = 0.0f;

        /// <summary>
        /// Sets the titled angle.
        /// </summary>
        /// <param name="degree">The degree.</param>
        virtual public void SetTitledAngle(float degree)
		{
			tiltedAngle = degree;
		}

        /// <summary>
        /// Gets the ray.
        /// </summary>
        /// <returns></returns>
        public Ray GetRay()
		{
			Transform pointerTransform = DpnPointerManager.Pointer.GetPointerTransform();

			Camera centerCamera = DpnCameraRig._instance._center_eye;
			Matrix4x4 matrixController =  pointerTransform.localToWorldMatrix;

			Ray castRay;
			Matrix4x4 matrixRayEnding = new Matrix4x4();
			matrixRayEnding.SetTRS(Vector3.zero, Quaternion.Euler(-tiltedAngle, 0,0), Vector3.one);
			matrixRayEnding = matrixController * matrixRayEnding;

			Vector3 rayPointerStart = matrixRayEnding.GetColumn(3);
			Vector3 rayPointerEnd = rayPointerStart + ((Vector3)matrixRayEnding.GetColumn(2) * DpnPointerManager.Pointer.GetMaxPointerDistance());

			Vector3 cameraLocation = centerCamera.transform.position;
			Vector3 finalRayDirection = rayPointerEnd - cameraLocation;
			finalRayDirection.Normalize();

			Vector3 finalRayStart = cameraLocation + (finalRayDirection * centerCamera.nearClipPlane);

			castRay = new Ray(finalRayStart, finalRayDirection);
			return castRay;
		}
	}
}
