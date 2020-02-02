


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace dpn
{
    /// <summary>
    /// Main entry point for the DpnMultiControllerPeripheral API.
    /// </summary>
    /// <seealso cref="dpn.DpnBasePeripheral" />
    abstract public class DpnMultiControllerPeripheral :DpnBasePeripheral
    {
        public abstract DpnBasePeripheral GetController(string controllerName);

        protected DpnBasePeripheral[] _controllers = null;
        protected string[] _controllerNames = null;
        protected DpnBasePeripheral _mainController = null;
        protected Dictionary<string, DpnBasePeripheral> _connectedControllers = new Dictionary<string, DpnBasePeripheral>();

        void Start()
        {
            if (_controllers == null)
                return;

            for(int i = 0;i < _controllers.Length;++i)
            {
                DpnBasePeripheral controller = _controllers[i];
                controller.EnableModel(true);
                EnablePointer(controller.name);
            }
        }
        /// <summary>
        /// Enables the pointer.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public override void EnablePointer(bool enabled)
        {
            base.EnablePointer(enabled);
        }

        void EnablePointer(string name)
        {
            if (_controllers == null)
                return;

            if (_mainController && _mainController.name == name)
                return;

            DpnBasePeripheral mainController = null;

            for(int i = 0;i < _controllers.Length;++i)
            {
                DpnBasePeripheral controller = _controllers[i];
                if(controller && controller.name == name)
                {
                    mainController = controller;
                }
                else
                {
                    controller.EnablePointer(false);
                }
            }

            _mainController = mainController;

            if(_mainController != null)
                _mainController.EnablePointer(true);
        }
        /// <summary>
        /// Called when [enable].
        /// </summary>
        public void OnEnable()
        {
            if (_controllers == null)
                return;

            for(int i = 0;i < _controllers.Length;++i)
            {
                string name = _controllerNames[i];
                DpnBasePeripheral controller = GetController(name);
                if(controller)
                {
                    _controllers[i] = controller;
                    _connectedControllers[name] = controller;
                }
            }
        }

        void FixedUpdate()
        {
            if (_controllers == null)
                return;

            foreach (DpnBasePeripheral controller in _controllers)
            {
                if(controller && controller.BeingUsed())
                    EnablePointer(controller.name);
            }
        }

        /// <summary>
        /// Enables the internal objects.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        override public void EnableInternalObjects(bool enabled)
        {
            if (_controllers == null)
                return;

            foreach(DpnBasePeripheral controller in _controllers)
            {
                controller.EnableInternalObjects(enabled & controller.isValid);
            }
        }

        /// <summary>
        /// Called when [controller disconnected].
        /// </summary>
        /// <param name="controller">The controller.</param>
        virtual public void OnControllerDisconnected(DpnBasePeripheral controller)
        {
            if (controller == null)
                return;

            controller.EnableInternalObjects(false);

            _connectedControllers.Remove(controller.name);

            if (_connectedControllers.Count == 0)
            {
                _mainController = null;
                SendMessageUpwards("OnPeripheralDisconnected", this);
            }
            else
            {
                if(controller == _mainController)
                {
                    DpnBasePeripheral mainController = _connectedControllers.Values.First<DpnBasePeripheral>();
                    mainController.EnablePointer(true);
                    _mainController = mainController;
                }
                
            }
        }

        /// <summary>
        /// Called when [controller connected].
        /// </summary>
        /// <param name="controller">The controller.</param>
        virtual public void OnControllerConnected(DpnBasePeripheral controller)
        {
            if (controller == null)
                return;

            controller.EnableModel(true);

            if(_connectedControllers.Count == 0)
            {
                SendMessageUpwards("OnPeripheralConnected", this);
                controller.EnableInternalObjects(true);
                _mainController = controller;
            }

            _connectedControllers[controller.name] = controller;
        }
    }
}