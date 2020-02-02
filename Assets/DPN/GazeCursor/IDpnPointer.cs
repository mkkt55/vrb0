/************************************************************************************

Copyright   :   Copyright 2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;

namespace dpn
{
    /// <summary>
    /// DpnBasePointer calls this when it is created.
    /// If a pointer hasn't already been assigned, it
    /// will assign the newly created one by default.
    ///
    /// This simplifies the common case of having only one
    /// IDpnPointer so is can be automatically hooked up
    /// to the manager.  If multiple DpnGazePointers are in
    /// the scene, the app has to take responsibility for
    /// setting which one is active.
    /// </summary>
    public interface IDpnPointer
	{
        /// <summary>
        /// This is called when the 'BaseInputModule' system should be enabled.
        /// </summary>
        void OnInputModuleEnabled();

        /// <summary>
        /// This is called when the 'BaseInputModule' system should be disabled.
        /// </summary>
        void OnInputModuleDisabled();

        /// <summary>
        /// Called when [pointer enter].This is called when the 'BaseInputModule' system should be disabled.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="intersectionPosition">The intersection position.</param>
        /// <param name="intersectionRay">The intersection ray.</param>
        /// <param name="isInteractive">if set to <c>true</c> [is interactive].</param>
        void OnPointerEnter(GameObject targetObject, Vector3 intersectionPosition,
		   Ray intersectionRay, bool isInteractive);

        /// <summary>
        /// Called when [pointer hover].This is called when the 'BaseInputModule' system should be disabled.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="intersectionPosition">The intersection position.</param>
        /// <param name="intersectionRay">The intersection ray.</param>
        /// <param name="isInteractive">if set to <c>true</c> [is interactive].</param>
        void OnPointerHover(GameObject targetObject, Vector3 intersectionPosition,
			Ray intersectionRay, bool isInteractive);

        /// <summary>
        /// Called when the pointer no longer faces an object previously
		/// intersected with a ray projected from the camera.
		/// This is also called just before **OnInputModuleDisabled** and may have have any of
		/// the values set as **null**.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        void OnPointerExit(GameObject targetObject);

        /// <summary>
        /// Called when a click is initiated.
        /// </summary>
        void OnPointerClickDown();

        /// <summary>
        /// Called when click is finished.
        /// </summary>
        void OnPointerClickUp();

        /// <summary>
        /// Returns the max distance this pointer will be rendered at from the camera.
		/// This is used by DpnBasePointerRaycaster to calculate the ray when using
		/// the default "Camera" RaycastMode. See DpnBasePointerRaycaster.cs for details.
        /// </summary>
        /// <returns></returns>
        float GetMaxPointerDistance();

        /// <summary>
        /// Returns the transform that represents this pointer.
		/// It is used by DpnBasePointerRaycaster as the origin of the ray.
        /// </summary>
        /// <returns></returns>
        Transform GetPointerTransform();

        /// <summary>
        /// Return the radius of the pointer. This is currently
		/// only used by DpnGaze. It is used when searching for
		/// valid gaze targets. If a radius is 0, the DpnGaze will use a ray
		/// to find a valid gaze target. Otherwise it will use a SphereCast.
		/// The *innerRadius* is used for finding new targets while the *outerRadius*
		/// is used to see if you are still nearby the object currently looked at
		/// to avoid a flickering effect when just at the border of the intersection.
        /// </summary>
        /// <param name="innerRadius">The inner radius.</param>
        /// <param name="outerRadius">The outer radius.</param>
        void GetPointerRadius(out float innerRadius, out float outerRadius);

        /// <summary>
        /// get the position on the screen
        /// </summary>
        /// <returns></returns>
        Vector2 GetScreenPosition();
	}
}

