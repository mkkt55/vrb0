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
    /// This script extends the standard Unity EventSystem events with Dpn specific events.
    /// </summary>
    public static class DpnExecuteEventsExtension
	{
		private static readonly ExecuteEvents.EventFunction<IDpnPointerHoverHandler> s_HoverHandler = Execute;

		private static void Execute(IDpnPointerHoverHandler handler, BaseEventData eventData)
		{
			handler.OnDpnPointerHover(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		public static ExecuteEvents.EventFunction<IDpnPointerHoverHandler> pointerHoverHandler
		{
			get { return s_HoverHandler; }
		}
	}
}
