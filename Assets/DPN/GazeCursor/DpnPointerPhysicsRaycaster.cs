/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace dpn
{
/// This script provides a raycaster for use with the DpnPointerInputModule.
/// It behaves similarly to the standards Physics raycaster, except that it utilize raycast
/// modes specifically for Dpn.
///
/// View DpnBasePointerRaycaster.cs and DpnPointerInputModule.cs for more details.
	[System.Obsolete]
	public class DpnPointerPhysicsRaycaster : PhysicsRaycaster
	{
		///// Const to use for clarity when no event mask is set
		//protected const int NO_EVENT_MASK_SET = -1;

		///// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
		//[SerializeField]
		//protected LayerMask raycasterEventMask = NO_EVENT_MASK_SET;

		///// Stored reference to the event camera.
		//private Camera cachedEventCamera;

		///// eventCamera is used for masking layers and determining the distance of the raycast.
		///// It will use the camera on the same object as this script.
		///// If there is none, it will use the main camera.
		//public override Camera eventCamera
		//{
		//	get
		//	{
		//		if (cachedEventCamera == null)
		//		{
		//			cachedEventCamera = GetComponent<Camera>();
		//		}
		//		return (cachedEventCamera != null) ? cachedEventCamera : dpn.DpnCameraRig._instance._center_eye;
		//	}
		//}

		///// Event mask used to determine which objects will receive events.
		//public int finalEventMask
		//{
		//	get
		//	{
		//		return (eventCamera != null) ? eventCamera.cullingMask & eventMask : NO_EVENT_MASK_SET;
		//	}
		//}

		///// Layer mask used to filter events. Always combined with the camera's culling mask if a camera is used.
		//public LayerMask eventMask
		//{
		//	get
		//	{
		//		return raycasterEventMask;
		//	}
		//	set
		//	{
		//		raycasterEventMask = value;
		//	}
		//}

		//protected DpnPointerPhysicsRaycaster()
		//{
		//}

		//public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		//{
		//	if (eventCamera == null)
		//	{
  //              return;
  //          }

  //          if (!IsPointerAvailable())
		//	{
		//		return;
  //          }

  //          Ray ray = GetRay();
		//	float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

		//	RaycastHit[] hits = Physics.RaycastAll(ray, dist, finalEventMask);

		//	if (hits.Length > 1)
		//	{
		//		System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));
		//	}

		//	if (hits.Length != 0)
  //          {
  //              for (int b = 0, bmax = hits.Length; b < bmax; ++b)
		//		{
		//			RaycastResult result = new RaycastResult
		//			{
		//				gameObject = hits[b].collider.gameObject,
		//				module = this,
		//				distance = hits[b].distance,
		//				worldPosition = hits[b].point,
		//				worldNormal = hits[b].normal,
		//				#if UNITY_5_3_OR_NEWER || UNITY_5_3
		//				screenPosition = eventData.position,
		//				#endif
		//				index = resultAppendList.Count,
		//				sortingLayer = 0,
		//				sortingOrder = 0
		//			};
		//			resultAppendList.Add(result);
		//		}
		//	}
		//}
	}
}
