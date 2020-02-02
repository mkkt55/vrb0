/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using System.Threading;

namespace dpn
{
    /// <summary>
    /// Main entry point for the DpnUICamera API.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class DpnUICamera : MonoBehaviour
    {
        public enum UITYPE
        {
            GUI = 1,
            CURSOR = 2,
        }
        public UITYPE UIType = UITYPE.GUI;

        private Rect viewport = new Rect(0, 0, 1, 1);
        public float depth = 0.0f;
#if UNITY_EDITOR
        const int cameraNum = 3;
#else
        const int cameraNum = 2;
#endif
        public Camera leftCamera;
        public Camera rightCamera;
        public Camera centerCamera;

        private RenderTexture[] _canvas_texture = new RenderTexture[Common.NUM_BUFFER * cameraNum];
        private IntPtr[] _canvas_ptr = new IntPtr[Common.NUM_BUFFER * cameraNum];
        private int index = 0;

        static public DpnUICamera instance = null;

        public void DpnSetViewport(Rect view)
        {
            viewport = view;
            Reshape();
        }
        public Rect DpnGetViewport()
        {
            return viewport;
        }

        private void Awake()
        {
            instance = this;
        }
        // Use this for initialization
        void Start()
        {
            Reshape();
#if UNITY_ANDROID && !UNITY_EDITOR
#else
            if (0.0f == depth)
            {
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index], 0, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index + 1], 1, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
            }
            else
            {
                // Rect viewport_left = new Rect(viewport.x + DpnManager.DeviceInfo.ipd / (4 * depth * (float)Math.Tan((Math.PI / 360) * DpnManager.DeviceInfo.fov_x)), viewport.y, viewport.width, viewport.height);
                // Rect viewport_right = new Rect(viewport.x - DpnManager.DeviceInfo.ipd / (4 * depth * (float)Math.Tan((Math.PI / 360) * DpnManager.DeviceInfo.fov_x)), viewport.y, viewport.width, viewport.height);
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index], 0, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index + 1], 1, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
            }
#endif

#if UNITY_ANDROID && (UNITY_5_5_0 || UNITY_5_4_3)
            // In Unity 5.5.0 and Unity 5.4.3,
            // surface is deleted and rebuilt after the first frame is completed by Unity, the second frame will be black screen.
            // So, Skip the rendering of the first frame to avoid flickering.
            StartCoroutine(Coroutine_EnableCamera());
#endif

            SetSrcBlendFactor(1);

            SetCommandBufferEnabled(true);

#if UNITY_ANDROID && !UNITY_EDITOR
            Camera.onPostRender += OnCameraPostRender;
#endif

            leftCamera.depth = depth;
            leftCamera.transform.localPosition = new Vector3(DpnManager.DeviceInfo.ipd * -0.5f, 0, 0);
            leftCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;

            rightCamera.depth = depth + 0.1f;
            rightCamera.transform.localPosition = new Vector3(DpnManager.DeviceInfo.ipd * 0.5f, 0, 0);
            rightCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;

            centerCamera.depth = depth + 0.2f;
            centerCamera.transform.localPosition = new Vector3(0, 0, 0);
            centerCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;

#if !UNITY_EDITOR
              centerCamera.enabled = false;
#endif

            Camera.onPreRender += OnCameraPreRender;

        }

        void Reshape()
        {
#if UNITY_EDITOR
            int width = (int)(Screen.width * viewport.width);
            int height = (int)(Screen.height * viewport.height);
#else
            int width = (int)(DpnManager.DeviceInfo.resolution_x * viewport.width);
            int height = (int)(DpnManager.DeviceInfo.resolution_y * viewport.height);
#endif
            for (int i = 0; i < Common.NUM_BUFFER; i++)
            {
                for (int j = 0; j < cameraNum; ++j)
                {
                    int index = i * cameraNum + j;
                    _canvas_texture[index] = new RenderTexture
                    (width,height
                     , 24, RenderTextureFormat.Default);
                    _canvas_texture[index].antiAliasing
                        = 4;
                    _canvas_texture[index].Create();
                    _canvas_ptr[index] = _canvas_texture[index].GetNativeTexturePtr();
                }

            }
        }

        void OnDestroy()
        {
            if (leftCamera != null)
                leftCamera.targetTexture = null;
            if (rightCamera != null)
                rightCamera.targetTexture = null;
            if (centerCamera != null)
                centerCamera.targetTexture = null;

            Composer.DpnuSetTextureEx((int)UIType, IntPtr.Zero, 0, (int)dpncTwType.NONE, new dpnRect(viewport));
            Composer.DpnuSetTextureEx((int)UIType, IntPtr.Zero, 1, (int)dpncTwType.NONE, new dpnRect(viewport));
            //Composer.DpnuSetTextureEx((int)UIType, IntPtr.Zero, 2, (int)dpncTwType.NONE, new dpnRect(viewport));
            for (int i = 0; i < Common.NUM_BUFFER; i++)
            {
                for (int j = 0; j < cameraNum; ++j)
                {
                    int index = i * cameraNum + j;
                    if (_canvas_texture[index].IsCreated())
                    {
                        _canvas_texture[index].Release();
                    }
                }

            }
        }

        void OnCameraPreRender(Camera camera)
        {
            if (camera == leftCamera)
            {
                leftCamera.targetTexture = _canvas_texture[index];
            }
            else if (camera == rightCamera)
            {
                rightCamera.targetTexture = _canvas_texture[index + 1];
            }


#if UNITY_EDITOR
            if (camera == centerCamera)
            {
                centerCamera.targetTexture = _canvas_texture[index + 2];
                dpnLayerMaterial.SetTexture("albedo", centerCamera.targetTexture);
            }
#endif
        }

        //Update is called once per frame
        void LateUpdate()
        {
            index = (index + cameraNum) % (Common.NUM_BUFFER * cameraNum);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        void OnCameraPostRender(Camera camera)
        {
            if(camera != rightCamera)
                return;

            dpnRect view = new dpnRect(new Rect(0, 0, _canvas_texture[index].width, _canvas_texture[index].height));
            IntPtr tempPtr = Marshal.AllocHGlobal(Marshal.SizeOf(view));
            Marshal.StructureToPtr(view, tempPtr, false);
            Composer.PostRender(RENDER_EVENT.Posttransparent, (int)tempPtr);

            {
                // Rect viewport_left = new Rect(viewport.x + DpnManager.DeviceInfo.ipd / (4 * depth * (float)Math.Tan((Math.PI / 360) * DpnManager.DeviceInfo.fov_x)), viewport.y, viewport.width, viewport.height);
                // Rect viewport_right = new Rect(viewport.x - DpnManager.DeviceInfo.ipd / (4 * depth * (float)Math.Tan((Math.PI / 360) * DpnManager.DeviceInfo.fov_x)), viewport.y, viewport.width, viewport.height);
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index], 0, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
                Composer.DpnuSetTextureEx((int)UIType, _canvas_ptr[index + 1], 1, (int)dpncTwType.DISTORTION, new dpnRect(viewport));
            }

        }
#endif

#if UNITY_ANDROID && (UNITY_5_5_0 || UNITY_5_4_3)
        IEnumerator Coroutine_EnableCamera()
        {
            if(leftCamera)
                leftCamera.enabled = false;
            if(rightCamera)
                rightCamera.enabled = false;

            yield return new WaitForEndOfFrame();

            if(leftCamera)
                leftCamera.enabled = true;
            if(rightCamera)
                rightCamera.enabled = true;
        }
#endif
        private void OnDisable()
        {
            SetCommandBufferEnabled(false);
        }

        private void OnEnable()
        {
            SetCommandBufferEnabled(true);
        }

        bool comandEnabled = false;
        CommandBuffer commandbuff;

        void SetCommandBufferEnabled(bool enabled)
        {
#if UNITY_EDITOR
            if (commandbuff == null)
            {
                InitEditorMultiLayer();
            }

            if (comandEnabled == enabled)
                return;

            if (DpnCameraRig._instance == null || DpnCameraRig._instance._center_eye == null)
                return;

            comandEnabled = enabled;

            if (enabled)
            {
                DpnCameraRig._instance._center_eye.AddCommandBuffer(CameraEvent.AfterEverything, commandbuff);
            }
            else
            {
                DpnCameraRig._instance._center_eye.RemoveCommandBuffer(CameraEvent.AfterEverything, commandbuff);
            }
#endif
        }

        static int blendSrcFactor = 0;

        void SetSrcBlendFactor(int value)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!DpnDevice.bVR9)
            {
                if(blendSrcFactor == value)
                    return;
                blendSrcFactor = value;

                IntPtr tempPtr = Marshal.StringToHGlobalAnsi("blend_srcfactor");
                Composer.DpnuSetIntValue(tempPtr, value);
            }
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (leftCamera)
                    leftCamera.enabled = false;
                if (rightCamera)
                    rightCamera.enabled = false;

            }
            else
            {
                StartCoroutine(EnableCameras_Delay());
            }
        }

        IEnumerator EnableCameras_Delay()
        {
            yield return new WaitForEndOfFrame();
            if (leftCamera)
                leftCamera.enabled = true;
            if (rightCamera)
                rightCamera.enabled = true;
        }
        [SerializeField]
        Material dpnLayerMaterial;

#if UNITY_EDITOR


        void InitEditorMultiLayer()
        {
            commandbuff = new CommandBuffer();

            Mesh mesh = new Mesh();

            Vector3[] vertices =
            {
                new Vector3(viewport.x, viewport.y),
                new Vector3(viewport.x + viewport.width, viewport.y),
                new Vector3(viewport.x + viewport.width, viewport.y + viewport.height),
                new Vector3(viewport.x, viewport.y + viewport.height),
            };

            int[] indices =
            {
                0,1,2,
                0,2,3,
            };

            Vector2[] texcoords =
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = texcoords;

            commandbuff.DrawMesh(mesh, Matrix4x4.identity, dpnLayerMaterial);
        }
#endif
    }
}
