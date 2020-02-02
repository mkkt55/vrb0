


using System;
using System.Collections.Generic;
using UnityEngine;

namespace dpn
{
    public class DpnMultiControllerPeripheralNolo :DpnMultiControllerPeripheral
    {
        static string[] s_controllerNames = {"controller(left)","controller(right)" };

        public void Awake()
        {
            _controllerNames = s_controllerNames;

            _controllers = new DpnBasePeripheral[2];
        }

        public override DpnBasePeripheral GetController(string controllerName)
        {
            return GetNoloController(controllerName);    
        }

        NoloController GetNoloController(string controllerName)
        {
            NoloController noloCtrller = null;
            Transform noloTransf = gameObject.transform.Find(controllerName);
            if (noloTransf != null)
            {
                noloCtrller = noloTransf.GetComponent<NoloController>();
            }
            return noloCtrller;
        }

        override public void OnControllerDisconnected(DpnBasePeripheral controller)
        {
            base.OnControllerDisconnected(controller);

            if (_connectedControllers.Count == 0 && gameObject.transform && gameObject.transform.parent)
            {
                Transform transform = gameObject.transform.parent.Find("DpnBoundary(Clone)");
                if (transform)
                    transform.gameObject.SetActive(false);
            }
        }

        public override void OnControllerConnected(DpnBasePeripheral controller)
        {
            base.OnControllerConnected(controller);

            if (_connectedControllers.Count == 1 && gameObject.transform && gameObject.transform.parent)
            {
                Transform transform = gameObject.transform.parent.Find("DpnBoundary(Clone)");
                if (transform)
                    transform.gameObject.SetActive(true);
            }
        }
    }
}