/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace dpn
{
    /// <summary>
    /// Communicates with native plugin functions that run on the rendering thread.
    /// </summary>
    public class Composer
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		public static void PostRender(RENDER_EVENT eventType, int timewarp_view_number)
		{
			IssueWithData(eventType, timewarp_view_number);
		}
		private static void IssueWithData(RENDER_EVENT eventType, int eventData)
		{
			// Encode and send-two-bytes of data
			GL.IssuePluginEvent(GetRenderEventFunc(), EncodeData((int)eventType, eventData, 0));
			
			// Encode and send remaining two-bytes of data
			GL.IssuePluginEvent(GetRenderEventFunc(), EncodeData((int)eventType, eventData, 1));
			
			// Explicit event that uses the data
			GL.IssuePluginEvent(GetRenderEventFunc(), EncodeType((int)eventType));
		}
		private const UInt32 IS_DATA_FLAG = 0x80000000;
		private const UInt32 DATA_POS_MASK = 0x40000000;
		private const int DATA_POS_SHIFT = 30;
		private const UInt32 EVENT_TYPE_MASK = 0x3E000000;
		private const int EVENT_TYPE_SHIFT = 25;
		private const UInt32 PAYLOAD_MASK = 0x0000FFFF;
		private const int PAYLOAD_SHIFT = 16;
		
		private static int EncodeType(int eventType)
		{
			return (int)((UInt32)eventType & ~IS_DATA_FLAG); // make sure upper bit is not set
		}
		
		private static int EncodeData(int eventId, int eventData, int pos)
		{
			UInt32 data = 0;
			data |= IS_DATA_FLAG;
			data |= (((UInt32)pos << DATA_POS_SHIFT) & DATA_POS_MASK);
			data |= (((UInt32)eventId << EVENT_TYPE_SHIFT) & EVENT_TYPE_MASK);
			data |= (((UInt32)eventData >> (pos * PAYLOAD_SHIFT)) & PAYLOAD_MASK);
			
			return (int)data;
		}
		
		private static int DecodeData(int eventData)
		{
			//bool hasData = (((UInt32)eventData & IS_DATA_FLAG) != 0);
			UInt32 pos = (((UInt32)eventData & DATA_POS_MASK) >> DATA_POS_SHIFT);
			//UInt32 eventId = (((UInt32)eventData & EVENT_TYPE_MASK) >> EVENT_TYPE_SHIFT);
			UInt32 payload = (((UInt32)eventData & PAYLOAD_MASK) << (PAYLOAD_SHIFT * (int)pos));
			
			return (int)payload;
		}
#endif
        public static void Resume()
        {
            IssuePluginEvent(RENDER_EVENT.Resume);
        }

        public static void Pause()
        {
            IssuePluginEvent(RENDER_EVENT.Pause);
        }
        private static bool _initialized = false;

        public static bool Init(bool resume = true)
        {
            if (_initialized)
                return true;


            //dpnuConfig( (int)dpnuCONFIG.dpnuCONFIG_INTERPUPILLARY , (int)( ipd * 1000.0f ) );
#if UNITY_ANDROID && UNITY_EDITOR
            bool ret = false;
            string path = System.Environment.GetEnvironmentVariable("DPVR_PLATFORM_DIR");
            bool fileExist = System.IO.File.Exists(path + "/bin/DpnSDKService.exe");
            if(fileExist)
            {
                ret = DpnuInit((int)dpnuUsageMode.DPNU_UM_DEFAULT, IntPtr.Zero);
            }
#else
            bool ret = DpnuInit((int)dpnuUsageMode.DPNU_UM_DEFAULT, IntPtr.Zero);
#endif
            if (false == ret)
            {
                int err = DpnuGetLastError();
                Debug.Log(err.ToString());
                return false;
            }

            _initialized = true;
            if(resume)
                Resume();
            return true;
        }

        public static void Uninit()
        {
            if (false == _initialized)
                return;
#if UNITY_ANDROID && !UNITY_EDITOR
            IssuePluginEvent(RENDER_EVENT.Deinit);
#else
            DpnuDeinit();
#endif
            _initialized = false;
        }

        public static bool SetTextures(IntPtr tex_ptr, dpncEYE eye)
        {
            bool ret;
            ret = DpnuSetTexture(tex_ptr, (int)eye, 3, // distortion time warp
                DpnManager.DeviceInfo.resolution_x, DpnManager.DeviceInfo.resolution_y);

            if (!ret)
                return false;
            return true;
        }


        public void Update()
        {
            //if (false == _initialized)
            //	return;
            //for( int i = 0 ; i < (int)dpncEYE.NUM ; ++i )
            //	dpnuRecordPose( i );
        }

        public void Compose()
        {
            if (false == _initialized)
                return;
            IssuePluginEvent(RENDER_EVENT.Compose);
        }
        UInt32 encodeEventId(byte id, UInt32 data)
        {
            UInt32 r = data;
            r = r << 8;

            r = r | (UInt32)(id & 0xff);

            r = r | 0x80000000;

            return r;
        }

		public dpnnPrediction Predict()
        {
			dpnnPrediction Prediction = new dpnnPrediction();
            IntPtr tempPtr = DpnuPredict();
			if (tempPtr == IntPtr.Zero)
			{
				Prediction.key = -1;
			}
			else
			{
				Prediction = (dpnnPrediction)Marshal.PtrToStructure(tempPtr, typeof(dpnnPrediction));
			}
			return Prediction;
        }

        public void UpdatePose(UInt32 cacheIndex)
        {
            UInt32 ev = encodeEventId((byte)RENDER_EVENT.UpdatePose, cacheIndex);
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)ev);
        }

        public static void UpdatePose(dpnQuarterion pose, dpnVector3 pos)
        {
            Composer.DpnuUpdatePose(pose, pos);
        }

		public static void IssuePluginEvent( RENDER_EVENT evt )
		{
			if( false == _initialized )
				return;
			GL.IssuePluginEvent (GetRenderEventFunc(), (int)evt);
		}

		//deepoon unity
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool DpnuInit
			( int mode,IntPtr userData);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuGetDeviceInfo(ref dpnnDeviceInfo dpnnDeviceInfo);
		
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static void DpnuDeinit();
		
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool DpnuSetTexture
			( IntPtr texture
			 , int eye
 			 , int mode
 			 , int width
 			 , int height
			 );
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool DpnuRecordPose(int eye);

		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static dpnQuarterion DpnuGetPose();
		
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static dpnVector3 DpnuGetPosition();
		
		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool DpnuGetSensorData(ref dpnSensorData sensorData);

		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static int DpnuGetLastError();

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static float DpnuGetFloatValue(IntPtr property_name, float default_value);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuSetFloatValue(IntPtr property_name, float value);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuSetPtrValue(IntPtr property_name, IntPtr value);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr DpnuGetPtrValue(IntPtr property_name, IntPtr default_value);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuSetIntValue(IntPtr property_name, int value);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static int DpnuGetIntValue(IntPtr property_name, int default_value);

		[DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool DpnuSetTextureEx(int index, IntPtr texture, int eye, int mode, dpnRect viewport);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuResetPose();

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool DpnuUpdatePose(dpnQuarterion pose, dpnVector3 position);


        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static dpnQuarterion DpnuGetPredictedPose(double timeInSeconds);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static dpnVector3 DpnuGetPredictedPosition(double timeInSeconds);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static double DpnuGetPredictedDisplayTime(int pipelineDepth);

        [DllImport(Common.LibDpn, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr DpnuPredict();

        [DllImport(Common.LibDpn)]
        private static extern IntPtr GetRenderEventFunc();
    }
}
