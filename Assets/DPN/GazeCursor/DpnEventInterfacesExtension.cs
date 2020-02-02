/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine.EventSystems;

namespace dpn
{
    /// <summary>
    /// Interface to implement if you wish to receive OnDpnPointerHover callbacks.
	/// Executed by GazeInputModule.cs.
    /// </summary>
    /// <seealso cref="UnityEngine.EventSystems.IEventSystemHandler" />
    public interface IDpnPointerHoverHandler : IEventSystemHandler
	{

        /// <summary>
        /// Called when pointer is hovering over GameObject.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        void OnDpnPointerHover(PointerEventData eventData);
	}
}