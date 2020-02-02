/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace dpn
{
    /// <summary>
    /// Main entry point for the DpnCameraRig API.
    /// </summary>
    /// <seealso cref="dpn.DpnBasePeripheralDevicetype" />
    public class DpnCameraRig : DpnBasePeripheralDevicetype
    {
        private Camera leftCamera = null;
        private Camera rightCamera = null;
        private Camera centerCamera = null;
        private Transform leftTranfrom = null;
        private Transform rightTransform = null;
        private Transform centerTransform = null;
        private Transform trackerTransform = null;
        private Transform trackingSpace = null;

        public Camera _left_eye
        {
            get
            {
                if(leftCamera == null)
                {
                    leftCamera = transform.Find("TrackingSpace/LeftEyeAnchor").GetComponent<Camera>();
                }
                return leftCamera;
            }
        }

        public Camera _right_eye
        {
            get
            {
                if (rightCamera == null)
                {
                    rightCamera = transform.Find("TrackingSpace/RightEyeAnchor").GetComponent<Camera>();
                }
                return rightCamera;
            }
        }
		public Camera _center_eye
        {
            get
            {
                if (centerCamera == null)
                {
                    centerCamera = transform.Find("TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
                }
                return centerCamera;
            }
        }

        public Transform _left_transform
        {
            get
            {
                if (leftTranfrom == null)
                {
                    leftTranfrom = transform.Find("TrackingSpace/LeftEyeAnchor");
                }
                return leftTranfrom;
            }
        }

        public Transform _right_transform
        {
            get
            {
                if (rightTransform == null)
                {
                    rightTransform = transform.Find("TrackingSpace/RightEyeAnchor");
                }
                return rightTransform;
            }
        }

        public Transform _center_transform
        {
            get
            {
                if (centerTransform == null)
                {
                    centerTransform = transform.Find("TrackingSpace/CenterEyeAnchor");
                }
                return centerTransform;
            }
        }

        public Transform _tracker_transform
        {
            get
            {
                if (trackerTransform == null)
                {
                    trackerTransform = transform.Find("TrackingSpace/TrackerAnchor");
                }
                return trackerTransform;
            }
        }

        public Transform _tracking_space
        {
            get
            {
                if (trackingSpace == null)
                {
                    trackingSpace = transform.Find("TrackingSpace");
                }
                return trackingSpace;
            }
        }

        public static DpnCameraRig _instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DpnCameraRig>();
                }
                return instance;
            }
        }

        private static DpnCameraRig instance = null;

        private Sensor _sensor = new Sensor();
        private bool _freezed = false;
        private bool _monoscopic = false;

     
        public void Freeze(bool enabled) { _freezed = enabled; }
        public bool GetFreezed() { return _freezed; }

        public void MonoScopic(bool enabled) { _monoscopic = enabled; }
        public bool GetMonoScopic() { return _monoscopic; }

        private dpnQuarterion pose;
        private dpnVector3 position;
        public Quaternion GetPose() { return pose.ToQuaternion(); }
        public Vector3 GetPosition() { return new Vector3(position.x, position.y, position.z); }

        static public bool VRsupport;
        static private bool CameraRigInit = false;

       
        public Transform Polaris;
        public Transform Nolo;
        public Transform Flip;
       

        public Transform Boundary;
        public Transform reticlePointer;
        private GameObject _nonePointer = null;

        /// <summary>
        /// Called when [enable].
        /// </summary>
        public override void OnEnable()
        {
            if (!CameraRigInit)
            {
                if (_instance != this && _instance != null)
                {
                    Debug.LogWarning("There is another active DpnCameraRig in a scene, set it unactive");
                    _instance.gameObject.SetActive(false);
                }

            }

            // version adaptation : Unity 2018
#if UNITY_ANDROID && (!UNITY_EDITOR) && UNITY_2018
            InitComponent(this.transform);
            StartCoroutine(InitPeripheral_Coroutine());
#else
            InitPeripheral();
#endif

#if !UNITY_EDITOR
            StartCoroutine(_WaitForEndOfFrame());
#endif
        }


        /// <summary>
        /// Recenters this instance.
        /// </summary>
        public void Recenter()
        {
            _sensor.RecenterPose();
        }

        /// <summary>
        /// Disables the base peripheral.
        /// invoked by Unity Engine
        /// </summary>
        public override void OnDisable ()
		{
            IntPtr tempPtr = Marshal.StringToHGlobalAnsi("OnDisable");
            Composer.DpnuSetIntValue(tempPtr, 1);
            base.OnDisable();
		}

        private bool _initialized = false;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            if (_initialized)
            {
                if (VRsupport
#if UNITY_ANDROID && UNITY_EDITOR
                || !DpnManager.androidEditorUseHmd
#endif
                )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            InitComponent(this.transform);
            if (VRsupport
#if UNITY_ANDROID && UNITY_EDITOR
				|| !DpnManager.androidEditorUseHmd
#endif
                )
            {
                _left_eye.enabled = false;
                _right_eye.enabled = false;
                _center_eye.enabled = true;
                return false;
            }
            else
            {
                _left_eye.enabled = true;
                _right_eye.enabled = true;
                _center_eye.enabled = false;
            }
#if UNITY_EDITOR
            _center_eye.enabled = true;
#endif

            Freeze(false);
            MonoScopic(false);

            _initialized = true;
            return true;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void InitComponent(Transform parent)
		{
			
				CameraEnsure( _left_transform, true );
			
				CameraEnsure(_right_transform, true);

				CameraEnsure(_center_transform, false);
        }

        /// <summary>
        /// Updates the state of the device.
        /// </summary>
        public override void DpnpUpdate()
        {
            base.DpnpUpdate();
            double displayTime = Composer.DpnuGetPredictedDisplayTime(DpnManager.minimumVsync);
            pose = Composer.DpnuGetPredictedPose(displayTime); // 右手螺旋, 左手系, room坐标系 或者 惯性系
            if (DpnManager.peripheral == DPVRPeripheral.Polaris)
            {
                float[] temp_position = DpnpGetDeviceCurrentStatus().position_state[0];
                position = new dpnVector3(temp_position[0], temp_position[1], temp_position[2]);
                position.z = -position.z;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            else if (DpnManager.peripheral == DPVRPeripheral.Nolo && NoloController._instance[(int)NoloController.NoloDevice.Nolo_Hmd] != null)
            {
                Vector3 temp_position = NoloController._instance[(int)NoloController.NoloDevice.Nolo_Hmd].transform.localPosition;
                position = new dpnVector3(temp_position.x, temp_position.y, temp_position.z);
            }
#endif
            else
            {
                position = Composer.DpnuGetPredictedPosition(displayTime);
                position.z = -position.z;
            }


            position.x = position.x * DpnManager.worldScale;
            position.y = position.y * DpnManager.worldScale;
            position.z = position.z * DpnManager.worldScale;

            Pose posel = _sensor.GetEyePose(dpncEYE.LEFT, pose, position
                                            , DpnManager.DeviceInfo.ipd);
            Pose poser = _sensor.GetEyePose(dpncEYE.RIGHT, pose, position
                                            , DpnManager.DeviceInfo.ipd);

            //After GetEyePose: dpnQuarterion.ToQuaternion, 变成左手螺旋, 左手系, room坐标系 或者 惯性系

            //update eye's render target and transform
            _Update(posel, poser
                    , _monoscopic, _freezed);

            //Unity 使用的是左手螺旋，左手系

            UpdatePeripheral();
        }

        /// <summary>
        /// Updates the pose.
        /// </summary>
        public void UpdatePose()
        {
            Composer.UpdatePose(pose, position);
        }


        public void _Update
            (Pose left_pose, Pose right_pose
             , bool monoscopic
             , bool freezed
             )
        {
            //entities
            if (false == freezed)
            {
                _left_transform.localRotation = left_pose.orientation;
                _right_transform.localRotation = monoscopic ? left_pose.orientation : right_pose.orientation; // using left eye for now
                _center_transform.localRotation = left_pose.orientation;
                _tracker_transform.localRotation = _center_transform.localRotation;

                Vector3 pos = 0.5f * (left_pose.position + right_pose.position);
                _left_transform.localPosition = monoscopic ? pos : left_pose.position;
                _right_transform.localPosition = monoscopic ? pos : right_pose.position;
                _center_transform.localPosition = pos;
                _tracker_transform.localPosition = _center_transform.localPosition;
            }
        }

        /// <summary>
        /// Updates the camera.
        /// </summary>
        /// <param name="fovy">The fovy.</param>
        /// <param name="aspect_xdy">The aspect xdy.</param>
        public void _UpdateCam(float fovy, float aspect_xdy)
		{
			CameraSetup(_left_eye, PLUGIN_EVENT_TYPE.LeftEyeEndFrame
						, fovy, aspect_xdy);
			CameraSetup(_right_eye, PLUGIN_EVENT_TYPE.RightEyeEndFrame
						, fovy, aspect_xdy);

            CameraSetup(_center_eye, 0, fovy, aspect_xdy);
		}
		
		private static Transform EntityEnsure( Transform parent , string name )
		{
			Transform entity = parent.Find( name );
			if( entity == null )
				entity = new GameObject( name ).transform;
			
			entity.name = name;
			entity.parent = parent;
			entity.localScale = Vector3.one;
			entity.localPosition = Vector3.zero;
			entity.localRotation = Quaternion.identity;
			
			return entity;
		}
		
		private static Camera CameraEnsure( Transform entity, bool render)
		{
			Camera cam = entity.GetComponent<Camera>();
			if( cam == null )
			{
				cam = entity.gameObject.AddComponent<Camera>();
			}

			// AA is documented to have no effect in deferred, but it causes black screens.
			if (cam.actualRenderingPath == RenderingPath.DeferredLighting ||
				cam.actualRenderingPath == RenderingPath.DeferredShading)
			{
				QualitySettings.antiAliasing = 0;
			}

			if (render && cam.GetComponent<DpnPostRender>() == null && !VRsupport)
			{
				cam.gameObject.AddComponent<DpnPostRender>();
			}
			return cam;
		}
		
		private static void CameraSetup
			( Camera cam , PLUGIN_EVENT_TYPE event_type
			 , float fovy , float aspect_xdy)
		{
			
			cam.fieldOfView = fovy;
			cam.aspect = aspect_xdy;
			if (DpnDevice.bVR9)
            {
                if (event_type == PLUGIN_EVENT_TYPE.LeftEyeEndFrame)
                {
                    cam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                }
                else if (event_type == PLUGIN_EVENT_TYPE.RightEyeEndFrame)
                {
                    cam.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                }
                else
                {
                    cam.rect = new Rect(0f, 0f, 1f, 1f);
                }
            }
            else
            {
                cam.rect = new Rect(0f, 0f, 1f, 1f);
            }
			
			// Enforce camera render order
			cam.depth = (int)event_type;

			//// AA is documented to have no effect in deferred, but it causes black screens.
			//if (cam.actualRenderingPath == RenderingPath.DeferredLighting ||
			//    cam.actualRenderingPath == RenderingPath.DeferredShading)
			//{
			//    QualitySettings.antiAliasing = 0;
			//    DpnManager.instance.eyeTextureAntiAliasing = TEXTURE_ANTIALIASING._1;
			//}
		}

        public override void DpnpPause()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
		Composer.Pause();
#endif
        }

        //triggered by OnApplicationPause and OnApplicationFocus
        public override void DpnpResume()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
	    Composer.Resume();
#endif
            //SetPeripheralFollowSystem();
            StartCoroutine(SetPeripheralFollowSystem_Delay());
        }

        // hmd Peripheral
        private DpnBasePeripheral _hmdPeripheral = null;
        // current Peripheral
        private DpnBasePeripheral _currentPeripheral = null;
        // default Peripheral by setting
        private DpnBasePeripheral _defaultPeripheral = null;

        /// <summary>
        /// Sets the peripheral.
        /// </summary>
        /// <param name="peripheral">The peripheral.</param>
        public void SetPeripheral(DpnBasePeripheral peripheral)
        {
            if (_currentPeripheral)
            {
                _currentPeripheral.EnableInternalObjects(false);
            }

            _currentPeripheral = peripheral;

            if (_currentPeripheral)
            {
                _currentPeripheral.EnableInternalObjects(true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [follow system].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [follow system]; otherwise, <c>false</c>.
        /// </value>
        static public bool followSystem
        {
            get
            {
                return _instance._interativeType != -1;
            }
        }
        private int _interativeType = -1;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            SetPeripheralFollowSystem();
#if UNITY_ANDROID && (UNITY_5_5_0 || UNITY_5_4_3)
            // In Unity 5.5.0 and Unity 5.4.3,
            // surface is deleted and rebuilt after the first frame is completed by Unity, the second frame will be black screen.
            // So, Skip the rendering of the first frame to avoid flickering.
            StartCoroutine(Coroutine_EnableCamera());
#endif
        }

        int GetInteractiveType()
        {
            if (_defaultPeripheral == null || _defaultPeripheral.peripheral == null)
                return -1;

            IntPtr buffer = Marshal.AllocHGlobal(sizeof(int));
            int ret = _defaultPeripheral.peripheral.DpnupReadDeviceAttribute(DPNP_VALUE_TYPE.DPNP_VALUE_TYPE_ATTRIBUTE_INTERACTIVE_TYPE - DPNP_VALUE_TYPE.DPNP_VALUE_TYPE_ATTRIBUTE
            , buffer, sizeof(int));
            int type = -1;
            if (ret == 1)
            {
                int[] value = new int[1];
                Marshal.Copy(buffer, value, 0, 1);
                type = value[0];
            }

            Marshal.FreeHGlobal(buffer);
            return type;
        }

        void SetInterctiveType(int type)
        {
            if (_defaultPeripheral == null || _defaultPeripheral.peripheral == null)
                return;

            if (type != 0 && type != 1)
                return;

            IntPtr buffer = Marshal.AllocHGlobal(sizeof(int));
            int[] values = new int[1];
            values[0] = type;
            Marshal.Copy(values, 0, buffer, 1);

            _defaultPeripheral.peripheral.DpnupSetDeviceAttribute(DPNP_VALUE_TYPE.DPNP_VALUE_TYPE_ATTRIBUTE_INTERACTIVE_TYPE - DPNP_VALUE_TYPE.DPNP_VALUE_TYPE_ATTRIBUTE
                , buffer, sizeof(int));
            Marshal.FreeHGlobal(buffer);
        }

        private void SetPeripheralFollowSystem()
        {
            _interativeType = GetInteractiveType();
            
            if (_interativeType == 0)
            {
                SetPeripheral(_hmdPeripheral);
            }
            else if (_interativeType == 1)
            {
                SetPeripheral(_defaultPeripheral);
            }
        }

        /// <summary>
        /// Enables the internal objects.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public override void EnableInternalObjects(bool enabled)
        {
            if (DpnManager.DPVRPointer)
            {
                if (_nonePointer)
                    _nonePointer.SetActive(enabled);

                if (_nonePointer && enabled)
                    DpnPointerManager.Pointer = (IDpnPointer)_nonePointer.GetComponent<DpnBasePointer>();
            }
        }

        void UpdatePeripheral()
        {
            if (!DpnCameraRig.followSystem)
                return;

            if (_defaultPeripheral && _defaultPeripheral.BeingUsed())
            {
                // peripheral
                if (_defaultPeripheral != _currentPeripheral)
                {
                    SetPeripheral(_defaultPeripheral);
                    SetInterctiveType(1);
                }
            }
            else if (DpnManager.hmdPointerSwitchable)
            {
                // touch pad
                int count = Input.touchCount;
                if (count != 0)
                {
                    if (_currentPeripheral != _hmdPeripheral)
                    {
                        SetPeripheral(_hmdPeripheral);
                        SetInterctiveType(0);
                    }
                }
            }
        }

        void OnPeripheralConnected(DpnBasePeripheral peripheral)
        {
            if (DpnCameraRig.followSystem)
                SetPeripheralFollowSystem();
            else
                SetPeripheral(peripheral);
        }

        void OnPeripheralDisconnected(DpnBasePeripheral peripheral)
        {
            if (DpnCameraRig.followSystem)
                SetPeripheralFollowSystem();
            else
                SetPeripheral(_hmdPeripheral);
        }

        void OnPeripheralUnbind(DpnBasePeripheral peripheral)
        {
            SetInterctiveType(0);
            SetPeripheral(_hmdPeripheral);
        }
        
        IEnumerator SetPeripheralFollowSystem_Delay()
        {
            yield return new WaitForSeconds(1.0f);
            SetPeripheralFollowSystem();
        }

        void EnableCamera(bool enabled)
        {
            if (_left_eye)
                _left_eye.enabled = enabled;

            if (_right_eye)
                _right_eye.enabled = enabled;

#if UNITY_EDITOR
            if (_center_eye)
                _center_eye.enabled = enabled;
#endif
        }

#if UNITY_ANDROID && (UNITY_5_5_0 || UNITY_5_4_3)

        IEnumerator Coroutine_EnableCamera()
        {
            EnableCamera(false);

            yield return new WaitForEndOfFrame();

            EnableCamera(true);
        }
#endif

        void InitPeripheral()
        {
            if (!CameraRigInit)
            {
#if UNITY_5_3_OR_NEWER || UNITY_5
                VRsupport = UnityEngine.XR.XRSettings.enabled;
#else
			    VRsupport = false;
#endif
            }
            CameraRigInit = true;
            if (!Init())
            {
                return;
            }
            base.OnEnable();
            _UpdateCam(DpnManager.DeviceInfo.fov_y
                , DpnManager.DeviceInfo.fov_x / (float)DpnManager.DeviceInfo.fov_y);

            if (DpnManager.DPVRPointer)
            {

                Transform Pointer = Instantiate(reticlePointer);
                Pointer.SetParent(_center_transform, false);
                Pointer.localPosition = new Vector3(0, 0, 2);
                Pointer.gameObject.SetActive(true);

                _nonePointer = Pointer.gameObject;
                _nonePointer.transform.Find("Ray").gameObject.SetActive(false);
            }
            _hmdPeripheral = this;
            _hmdPeripheral.EnableInternalObjects(false);

            switch (DpnManager.peripheral)
            {
                case DPVRPeripheral.Polaris:
                    {
                        Transform Peripheral = Instantiate(Polaris);
                        Peripheral.parent = this.transform;

                        _defaultPeripheral = Peripheral.GetComponent<DpnMultiControllerPeripheralPolaris>();
                        _defaultPeripheral.EnableInternalObjects(false);
                        break;
                    }
                case DPVRPeripheral.Nolo:
                    {
                        Transform Peripheral = Instantiate(Nolo);
                        Peripheral.parent = this.transform;

                        Transform boundary = Instantiate(Boundary);
                        boundary.parent = this.transform;

                        _defaultPeripheral = Peripheral.GetComponent<DpnMultiControllerPeripheralNolo>();
                        _defaultPeripheral.EnableInternalObjects(false);
                        break;
                    }
                case DPVRPeripheral.Flip:
                    {
                        Transform Peripheral = Instantiate(Flip);
                        Peripheral.parent = this.transform;
                        Peripheral.localPosition = Vector3.zero;

                        Transform controller_right = Peripheral.Find("controller(right)");
                        if (controller_right == null)
                            break;

                        _defaultPeripheral = controller_right.GetComponent<DpnDaydreamController>();
                        _defaultPeripheral.EnableInternalObjects(false);

                        DpnDaydreamController.onUnbind += OnPeripheralUnbind;
                        break;
                    }
                case DPVRPeripheral.None:
                    {
                        _defaultPeripheral = _hmdPeripheral;
                        break;
                    }
                default:
                    break;
            }

            SetPeripheral(_defaultPeripheral);
        }

#if UNITY_ANDROID && (!UNITY_EDITOR) && UNITY_2018 

        IEnumerator InitPeripheral_Coroutine()
        {
            // Wait two frames until the Unity3d initialization is complete.
            EnableCamera(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            EnableCamera(true);

            InitPeripheral();

            SetPeripheralFollowSystem();
        }

#endif

        static public Vector3 WorldToScreenPoint(Vector3 position)
        {
            if (_instance && _instance._center_eye)
                return _instance._center_eye.WorldToScreenPoint(position);
            else
                return Vector3.zero;
        }

#if !UNITY_EDITOR
        IEnumerator _WaitForEndOfFrame()
        {
            while(true)
            {
                yield return new WaitForEndOfFrame();
                OnEndOfFrame();
            }
        }

        void LateUpdate()
        {
            if(_center_eye)
                _center_eye.enabled = false;
        }

        void OnEndOfFrame()
        {
            if(_center_eye)
                _center_eye.enabled = true;
        }
#endif

        private void OnDestroy()
        {
            instance = null;
        }
    }

}
