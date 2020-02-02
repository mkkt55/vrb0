/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace dpn
{
    public struct PeripheralList
    {
        public DpnPeripheral peripheral;
        public List<DpnBasePeripheral> list;

        public PeripheralList (DpnPeripheral t)
        {
            peripheral = t;
            list = new List<DpnBasePeripheral>();
        }
    }
    
	public class DpnDevice : MonoBehaviour
	{	
		#if UNITY_ANDROID && !UNITY_EDITOR
		static private JavaActivity _java_activity = new JavaActivity();
		#endif

		
		private Buffers _buffers = new Buffers();
		private Composer _composer = new Composer();
        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private static readonly Dictionary<string, PeripheralList> s_Peripherals = new Dictionary<string, PeripheralList>();

		private const int BUFF_NUM = 2;

		public static DpnDevice _instance;

        public static bool bVR9 = false;
        public static bool bQCOM = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        private string PROPERTY_ISVR9 = "is_vr9";
        private string PROPERTY_ISQCOM = "is_qcom";
#endif

		private dpnnPrediction _prediction;
		
		void OnEnable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            IntPtr tempPtr = Marshal.StringToHGlobalAnsi(PROPERTY_ISVR9);
            bVR9 = Composer.DpnuGetIntValue(tempPtr, 0) == 0? false: true;
            IntPtr tempPtrQcom = Marshal.StringToHGlobalAnsi(PROPERTY_ISQCOM);
            bQCOM = Composer.DpnuGetIntValue(tempPtrQcom, 0) == 0? false: true;
            Debug.Log("DpnDevice Init bVR9 " + bVR9  + " bQCOM " + bQCOM);
#endif
            ConfigEngine.Init();
            if (!bVR9)
            {
                _buffers.Init
                    (DpnManager.eyeTextureDepth
                    , DpnManager.eyeTextureFormat
                    , DpnManager.DeviceInfo.resolution_x
                    , DpnManager.DeviceInfo.resolution_y);
#if UNITY_ANDROID && !UNITY_EDITOR
#else
            IntPtr[] buf_ptr = new IntPtr[(int)dpncEYE.NUM]
                    { _buffers.GetEyeTexturePtr( dpncEYE.LEFT )
                        , _buffers.GetEyeTexturePtr( dpncEYE.RIGHT ) };
            Composer.SetTextures(buf_ptr[0], dpncEYE.LEFT);
            Composer.SetTextures(buf_ptr[1], dpncEYE.RIGHT);
#endif
            }
            //
#if !UNITY_ANDROID
            string pcScreenOutput = "pcScreenOutput";
#if UNITY_EDITOR
			IntPtr OutputPtr = Marshal.StringToHGlobalAnsi(pcScreenOutput);
			Composer.DpnuSetIntValue(OutputPtr, (int)dpncOutputMode.NONE);
			Marshal.FreeHGlobal(OutputPtr);
#else
			IntPtr OutputPtr = Marshal.StringToHGlobalAnsi(pcScreenOutput);
			Composer.DpnuSetIntValue(OutputPtr, (int)DpnManager.pcScreenOutputMode);
			Marshal.FreeHGlobal(OutputPtr);
#endif
#endif

            Camera.onPreRender += CameraPreRender;
            Camera.onPreCull += CameraPreCull;

            if (DpnManager.DPVRPointer)
            {
                gameObject.AddComponent<DpnPointerManager>();
            }

            //cameras
            StartCoroutine(CallbackCoroutine());
		}

        void Start ()
        {
            if (!bVR9)
            {
                DpnCameraRig._instance._left_eye.targetTexture = _instance._buffers.GetEyeTexture(dpncEYE.LEFT);
                DpnCameraRig._instance._right_eye.targetTexture = _instance._buffers.GetEyeTexture(dpncEYE.RIGHT);
            }
        }

		void OnDisable()
		{
			IntPtr tempPtr = Marshal.StringToHGlobalAnsi("OnDisable");
			Composer.DpnuSetIntValue(tempPtr, 1);
            Camera.onPreRender -= CameraPreRender;
            Camera.onPreCull -= CameraPreCull;
        }

#if UNITY_ANDROID && !UNITY_EDITOR

       static public void InitJavaActivity()
       {
            _java_activity.Init();
       }
#endif

        /// <summary>
        /// Run DeePoon HMD
        /// </summary>
        public static void create()
        {
            if (_instance != null)
            {
                _instance.gameObject.SetActive(true);
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
			_java_activity.Init();
#endif
            Composer.Init();

            //create a new DeviceObject
            GameObject device_object = new GameObject
                (Common.DeePoonDeviceGameObjectName
                    , typeof(DpnDevice));

            if (DpnManager.DPVRPointer)
            {
                EventSystem eventSystem = FindObjectOfType<EventSystem>();
                if (eventSystem == null)
                {
                    Debug.LogWarning("DpnEventSystem is needed after version 0.7.5.");
                    GameObject gameObject = new GameObject("DpnEventSystem", typeof(EventSystem));
                    eventSystem = gameObject.GetComponent<EventSystem>();
                }
                {
                    DpnStandaloneInputModule inputModule = eventSystem.GetComponent<DpnStandaloneInputModule>();
                    if(inputModule == null)
                    {
                        Debug.LogWarning("DpnStandaloneInputModule component must be in the EventSystem, after version 0.7.5.");
                        eventSystem.gameObject.AddComponent<DpnStandaloneInputModule>();
                    }

                }
            }

            //Don't destroy it.
            DontDestroyOnLoad(device_object);

            //get device
            _instance = device_object.GetComponent<DpnDevice>();
        }
		
		private void OnDestroy()
		{
			StopAllCoroutines();

#if UNITY_ANDROID && !UNITY_EDITOR
			RenderTexture.active = null;
#else
            // do nothing
#endif

            Composer.Uninit();
            if (!bVR9)
            {
                _buffers.clear();
            }
        }

		/// <summary>
		/// event functions 
		/// </summary>
		private void CameraPreRender(Camera cam)
		{
		    if (!bVR9)
            {      
                UpdateCameraTexture(cam);
            }
		}

        static int lastFrameCount = -1;
        private void CameraPreCull(Camera cam)
        {
            // Only update poses on the first camera per frame.
            if (Time.frameCount != lastFrameCount)
            {
                lastFrameCount = Time.frameCount;
                DeviceUpdate();
                foreach (KeyValuePair<string, PeripheralList> i in s_Peripherals)
                {
                    if (i.Value.list.Count > 0)
                    {
                        foreach (DpnBasePeripheral s in i.Value.list)
                        {
                            s.DpnpUpdate();
                        }
                    }
                }
            }
        }
        
		private void DeviceUpdate()
		{
		    if (!bVR9)
            {      
			    if (false == _buffers.SwapBuffers())
			    {
#if UNITY_ANDROID && !UNITY_EDITOR
#else
				    Debug.Log("DPVR texture is recreated.");
				    IntPtr[] buf_ptr = new IntPtr[(int)dpncEYE.NUM]
				    { _buffers.GetEyeTexturePtr( dpncEYE.LEFT )
					    , _buffers.GetEyeTexturePtr( dpncEYE.RIGHT ) };
                    Composer.SetTextures(buf_ptr[0], dpncEYE.LEFT);
                    Composer.SetTextures(buf_ptr[1], dpncEYE.RIGHT);
#endif
			    }
            }
        }

        private void UpdateCameraTexture(Camera cam)
        {
            if (cam == DpnCameraRig._instance._left_eye)
            {
                cam.targetTexture = _instance._buffers.GetEyeTexture(dpncEYE.LEFT);
            }
            if (cam == DpnCameraRig._instance._right_eye)
            {
                cam.targetTexture = _instance._buffers.GetEyeTexture(dpncEYE.RIGHT);
            }
        }

        private IEnumerator CallbackCoroutine()
		{
			while (true)
			{
				yield return waitForEndOfFrame;
				if( null != DpnCameraRig._instance)
				{
                    DpnCameraRig._instance.UpdatePose();
                    _composer.Compose();
                    HideSplash();
                }
			}
		}

        private bool hasSplash = true;

        private void HideSplash()
        {
            if (!hasSplash)
                return;

            Debug.Log("HideSplash");
            if (Application.platform.Equals(RuntimePlatform.Android))
            {
                using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        hasSplash = false;
                        jo.Call("HideSplash");
                    }
                }
            }
        }

		private void OnApplicationPause(bool pause)
		{
            if (pause)
            {
                foreach (KeyValuePair<string, PeripheralList> i in s_Peripherals)
                {
                    if (i.Value.list.Count > 0)
                    {
                        foreach (DpnBasePeripheral s in i.Value.list)
                        {
                            s.DpnpPause();
                        }
                    }
                }
                Composer.Pause();
            }
            else
                StartCoroutine(_OnResume());
        }

        private void OnApplicationFocus(bool focus)
		{
			// OnApplicationFocus() does not appear to be called 
			// consistently while OnApplicationPause is. Moved
			// functionality to OnApplicationPause().
		}
 
		//triggered by OnApplicationPause and OnApplicationFocus
		private IEnumerator _OnResume()
		{
			yield return null;
            Composer.Resume();
            foreach (KeyValuePair<string, PeripheralList> i in s_Peripherals)
            {
                if (i.Value.list.Count > 0)
                {
                    foreach (DpnBasePeripheral s in i.Value.list)
                    {
                        s.DpnpResume();
                    }
                }
            }
        }

        public static DpnPeripheral OpenPeripheral(string deviceId, DpnBasePeripheral basePeripheral)
        {
            if (deviceId == null)
            {
                return null;
            }
            if (s_Peripherals.ContainsKey(deviceId))
            {
                if (s_Peripherals[deviceId].list.Contains(basePeripheral))
                {
                    return basePeripheral.peripheral;
                }
                else
                {
                    basePeripheral.peripheral = s_Peripherals[deviceId].peripheral;
                    s_Peripherals[deviceId].list.Add(basePeripheral);
                    return basePeripheral.peripheral;
                }
            }
            DpnPeripheral temp = DpnPeripheral.OpenPeripheralDevice(deviceId);
            if (temp == null)
            {
                return null;
            }
            basePeripheral.peripheral = temp;
            s_Peripherals.Add(deviceId, new PeripheralList(temp));
            s_Peripherals[deviceId].list.Add(basePeripheral);
            return basePeripheral.peripheral;
        }

        public static Dictionary<string, PeripheralList> GetPeripherals()
        {
            return s_Peripherals;
        }

        public static void ClosePeripheral(DpnBasePeripheral basePeripheral)
        {
            if (!s_Peripherals[basePeripheral.peripheral._deviceId].list.Contains(basePeripheral))
            {
                return;
            }
            s_Peripherals[basePeripheral.peripheral._deviceId].list.Remove(basePeripheral);
            if (s_Peripherals[basePeripheral.peripheral._deviceId].list.Count == 0)
            {
                DpnPeripheral.ClosePeripheralDevice(basePeripheral.peripheral);
                s_Peripherals.Remove(basePeripheral.peripheral._deviceId);
            }
            basePeripheral.peripheral = null;
            return;
        }
    }
}
