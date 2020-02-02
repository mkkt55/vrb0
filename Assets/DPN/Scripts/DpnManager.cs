/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

//using System.IO;
//using System.Collections;
//using System.Runtime.InteropServices;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.IO;

namespace dpn
{
	/*
	[System.Serializable]
	public struct SettingInfo
	{
		public dpncOutputMode pcScreenOutputMode;
		public TEXTURE_DEPTH eyeTextureDepth;
		public float worldScale;
		public int minimumVsync;
		public bool resetTrackerOnLoad;
		public float eyeTextureScale;
		public bool polarisSupport;
		public bool androidEditorUseHmd;
	}
	*/

	public struct SettingFileInfo
	{
		public dpncOutputMode pcScreenOutputMode;
		public TEXTURE_DEPTH eyeTextureDepth;
		public float worldScale;
		public int minimumVsync;
		public bool resetTrackerOnLoad;
		public float pcEyeTextureScale;
		public float mobileEyeTextureScale;
        public DPVRPeripheral peripheral;
        public DPVRKeyMode controllerKeyMode;
        public bool androidEditorUseHmd;
        public bool useDPVRPointer;
        public bool hmdPointerSwitchable;
        public TouchPosOrig touchPosOrig;
	}

    public enum DPVRKeyMode
    {
        DPVR,
        STEAM
    }

    public enum TouchPosOrig
    {
        TOP_LEFT,
        CENTER,
    }

    public enum DPVRPeripheral
    {
        None = 0,
        Anything = 1,    //not support now
        Polaris = 2,     //pc support only
        Nolo = 3,        //mobile support only
        Flip = 4,        //mobile support only
        //Wisevision = 4   //mobile support only
    }

    /// <summary>
    /// Configuration for DeePoon virtual reality.
    /// Will be deprecated in the future
    /// </summary>
    public static class DpnManager
	{
		static DpnManager()
		{
			TextAsset file_info = Resources.Load("DPN/DPNUnityConfig") as TextAsset;
			if (!file_info)
			{
				Debug.LogError("DPNUnityConfig file don't exists");
				return;
			}
			if (file_info.text == string.Empty)
			{
				Debug.LogError("DPNUnityConfig file has no data");
				return;
			}
			SettingFileInfo settingInfo = JsonUtility.FromJson<SettingFileInfo>(file_info.text);
			#if !UNITY_ANDROID
			pcScreenOutputMode = settingInfo.pcScreenOutputMode;
			#endif
			eyeTextureDepth = settingInfo.eyeTextureDepth;
			worldScale = settingInfo.worldScale;
			minimumVsync = settingInfo.minimumVsync;
			resetTrackerOnLoad = settingInfo.resetTrackerOnLoad;
            peripheral = settingInfo.peripheral;
			#if !UNITY_ANDROID
			eyeTextureScale = settingInfo.pcEyeTextureScale;
            controllerKeyMode = settingInfo.controllerKeyMode;
            #else
			eyeTextureScale = settingInfo.mobileEyeTextureScale;
            #endif
            #if UNITY_ANDROID
			androidEditorUseHmd = settingInfo.androidEditorUseHmd;
            #else
            androidEditorUseHmd = true;
			#endif

            DPVRPointer = settingInfo.useDPVRPointer;

            touchPosOrig = settingInfo.touchPosOrig;
            hmdPointerSwitchable = settingInfo.hmdPointerSwitchable;
		}
		/// <summary>
		/// Dpvr unity version
		/// </summary>
		public static string DpvrUnityVersion = "0.7.8f";

		/// <summary>
		/// The format of each eye texture.
		/// </summary>
		//public RenderTextureFormat eyeTextureFormat = RenderTextureFormat.Default;
		public static RenderTextureFormat eyeTextureFormat
		{
			get
			{
				return RenderTextureFormat.ARGB32;
			}
		}

		#if !UNITY_ANDROID
		public static dpncOutputMode pcScreenOutputMode = dpncOutputMode.LEFT_EYE;
		#endif
		
		/// <summary>
		/// The depth of each eye texture in bits. Valid Unity render texture depths are 0, 16, and 24.
		/// </summary>
		public static TEXTURE_DEPTH eyeTextureDepth = TEXTURE_DEPTH._24;

		public static float worldScale = 1.0f;

		public static int minimumVsync = 1;
		
		/// <summary>
		/// If true, each scene load will cause the head pose to reset.
		/// </summary>
		public static bool resetTrackerOnLoad = true;

		/// <summary>
		/// Controls the size of the eye textures.
		/// Values must be above 0.
		/// Values below 1 permit sub-sampling for improved performance.
		/// Values above 1 permit super-sampling for improved sharpness.
		/// </summary>
		[Range(0.1f, 2.0f)]
		public static float eyeTextureScale = 1.5f;

		private static bool Bool_DeviceInfo = false;

		public static bool androidEditorUseHmd = false;

        public static DPVRKeyMode controllerKeyMode = DPVRKeyMode.DPVR;

        public static DPVRPeripheral peripheral = DPVRPeripheral.None;

        public static bool DPVRPointer = true;

        public static TouchPosOrig touchPosOrig = TouchPosOrig.TOP_LEFT;
        public static bool hmdPointerSwitchable = true;

        private static dpnnDeviceInfo _DeviceInfo;

		public static dpnnDeviceInfo DeviceInfo
		{
			get
			{
				if (!Bool_DeviceInfo)
				{
				#if UNITY_ANDROID && !UNITY_EDITOR
					DpnDevice.InitJavaActivity();
				#endif

					Composer.Init(false);

					bool ret = Composer.DpnuGetDeviceInfo(ref _DeviceInfo);
					if ((!ret) || (_DeviceInfo.resolution_x <= 0) || (_DeviceInfo.resolution_y <= 0))
					{
						//case: editor mode with no platform sdk installed
						_DeviceInfo.ipd = 0.064f * worldScale;
						#if UNITY_ANDROID && !UNITY_EDITOR
						_DeviceInfo.fov_x = 96.0f;
						_DeviceInfo.fov_y = 96.0f;
						#else
						_DeviceInfo.fov_x = 100.0f;
						_DeviceInfo.fov_y = 100.0f;
						#endif
						_DeviceInfo.resolution_x = 1024;
						_DeviceInfo.resolution_y = 1024;
					}
					_DeviceInfo.ipd = _DeviceInfo.ipd * worldScale;
					_DeviceInfo.resolution_x = (int)(_DeviceInfo.resolution_x * DpnManager.eyeTextureScale);
					_DeviceInfo.resolution_y = (int)(_DeviceInfo.resolution_y * DpnManager.eyeTextureScale);
					Bool_DeviceInfo = ret;
				}
				return _DeviceInfo;
			}
		}
	}
}