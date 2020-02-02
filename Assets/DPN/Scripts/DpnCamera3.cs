using System;
using UnityEngine;

namespace dpn
{

    public class DpnCamera3 : MonoBehaviour
    {
        private Camera _leftCamera { get; set; }

        private Camera _rightCamera { get; set; }

        private Camera _centerCamera { get; set; }

        private Camera _leftEyeCamera = null;
        private Camera _rightEyeCamera = null;
        private Camera _centerEyeCamera = null;

        [SerializeField]
        private Transform _leftTransform = null;

        public Transform leftTransform
        {
            get
            {
                return _leftTransform;
            }
            set
            {
                _leftTransform = value;
            }
        }

        [SerializeField]
        private Transform _rightTransform = null;

        public Transform rightTransform
        {
            get
            {
                return _rightTransform;
            }
            set
            {
                _rightTransform = value;
            }
        }

        [SerializeField]
        private Transform _centerTransform = null;

        public Transform centerTransform
        {
            get
            {
                return _centerTransform;
            }
            set
            {
                _centerTransform = value;
            }
        }

        [SerializeField]
        private LayerMask _cullingMask = -1;

        public LayerMask cullingMask
        {
            get
            {
                return _cullingMask;
            }
            set
            {
                _cullingMask = value;
                _leftCamera.cullingMask = value;
                _rightCamera.cullingMask = value;
                _centerCamera.cullingMask = value;
            }
        }

        [SerializeField]
        [Range(-100.0f, 100.0f)]
        private float _depth = 0.0f;

        public float depth
        {
            set
            {
                _depth = value;
                _leftCamera.depth = value;
                _rightCamera.depth = value;
                _centerCamera.depth = value;
            }
            get
            {
                return _depth;
            }
        }

        [SerializeField]
        private CameraClearFlags _clearFlags = CameraClearFlags.Skybox;
        
        public CameraClearFlags clearFlags
        {
            set
            {
                _clearFlags = value;
                _leftCamera.clearFlags = value;
                _rightCamera.clearFlags = value;
                _centerCamera.clearFlags = value;
            }
            get
            {
                return _clearFlags;    
            }
        }

        float farClipPlane = 0.0f;

        public float FarClipPlane
        {
            set
            {
                farClipPlane = value;
                if (_leftCamera)
                    _leftCamera.farClipPlane = value;
                if (_rightCamera)
                    _rightCamera.farClipPlane = value;
                if (_centerCamera)
                    _centerCamera.farClipPlane = value;
            }
            get
            {
                return farClipPlane;
            }
        }
        void Awake()
        {
            // left camera
            GameObject leftEye = new GameObject("LeftEyeCamera");
            _leftCamera = leftEye.AddComponent<Camera>();
            
            leftEye.transform.SetParent(this.transform, false);
            _leftCamera.cullingMask = _cullingMask;
            _leftCamera.depth = depth;
            _leftCamera.clearFlags = _clearFlags;
           

            // right camera
            GameObject rightEye = new GameObject("RightEyeCamera");
            _rightCamera = rightEye.AddComponent<Camera>();
            rightEye.transform.SetParent(this.transform, false);
            _rightCamera.cullingMask = _cullingMask;
            _rightCamera.depth = depth;
            _rightCamera.clearFlags = _clearFlags;

            // center camera
            GameObject centerEye = new GameObject("CenterEyeCamera");
            _centerCamera = centerEye.AddComponent<Camera>();
            centerEye.transform.SetParent(this.transform, false);
            _centerCamera.cullingMask = _cullingMask;
            _centerCamera.depth = depth;
            _centerCamera.clearFlags = clearFlags;

            farClipPlane = DpnCameraRig._instance._center_eye.farClipPlane;

            _leftCamera.enabled = false;
            _rightCamera.enabled = false;
            _centerCamera.enabled = false;
        }

        protected void Start()
        {
            _leftEyeCamera = DpnCameraRig._instance._left_eye;
            _rightEyeCamera = DpnCameraRig._instance._right_eye;
            _centerEyeCamera = DpnCameraRig._instance._center_eye;

            _leftCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;
            _leftCamera.rect = _leftEyeCamera.rect;

            _rightCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;
            _rightCamera.rect = _rightEyeCamera.rect;

            _centerCamera.fieldOfView = DpnManager.DeviceInfo.fov_y;
            _centerCamera.rect = _centerEyeCamera.rect;

            FarClipPlane = farClipPlane;

            bool vrSupport = DpnCameraRig.VRsupport;
            if (vrSupport
#if UNITY_ANDROID && UNITY_EDITOR
                || !DpnManager.androidEditorUseHmd
#endif
                )
            {
                _leftCamera.gameObject.SetActive(false);
                _rightCamera.gameObject.SetActive(false);
                _centerCamera.gameObject.SetActive(true);
            }
            else
            {
                _leftCamera.gameObject.SetActive(true);
                _rightCamera.gameObject.SetActive(true);
                _centerCamera.gameObject.SetActive(false);
            }

#if UNITY_EDITOR
            _centerCamera.gameObject.SetActive(true);
#endif

            Camera.onPreRender += OnCameraPreRender;
#if UNITY_ANDROID && !(UNITY_5_6_1 || UNITY_5_6_0)
            Camera.onPostRender += OnCameraPostRender;
#endif
        }

        void Update()
        {
            if (_rightCamera != null
                && _rightCamera.enabled != _rightEyeCamera.enabled)
            {
                _rightCamera.enabled = _rightEyeCamera.enabled;
            }

            if (_centerCamera != null
                && _centerCamera.enabled != _centerEyeCamera.enabled)
            {
                _centerCamera.enabled = _centerEyeCamera.enabled;
            }

            if (_leftEyeCamera != null
                && _leftEyeCamera.enabled != _leftCamera.enabled)
            {
                _leftCamera.enabled = _leftEyeCamera.enabled;
            }
        }

        void OnCameraPreRender(Camera camera)
        {
            if(camera == _leftCamera)
            {
                if (leftTransform)
                {
                    _leftCamera.transform.position = leftTransform.position;
                    _leftCamera.transform.rotation = leftTransform.rotation;
                }
                _leftCamera.targetTexture = DpnCameraRig._instance._left_eye.targetTexture;
            }
            else if (camera == _rightCamera)
            {
                if (rightTransform)
                {
                    _rightCamera.transform.position = rightTransform.position;
                    _rightCamera.transform.rotation = rightTransform.rotation;
                }
                _rightCamera.targetTexture = DpnCameraRig._instance._right_eye.targetTexture;
            }
            else if(camera == _centerCamera)
            {
                if (centerTransform)
                {
                    _centerCamera.transform.position = centerTransform.position;
                    _centerCamera.transform.rotation = centerTransform.rotation;
                }
                _centerCamera.targetTexture = DpnCameraRig._instance._center_eye.targetTexture;
            }
        }

#if UNITY_ANDROID && !(UNITY_5_6_1 || UNITY_5_6_0)
        void OnCameraPostRender(Camera camera)
        {
            if (camera == _leftCamera
                || camera == _rightCamera
                || camera == _centerCamera)
            {
                camera.targetTexture = null;
            }
        }
#endif

        public bool ContainsCamera(Camera camera)
        {
            return camera == _leftCamera
                || camera == _rightCamera
                || camera == _centerCamera;
        }

        void OnDisable()
        {
            if (_leftCamera)
                _leftCamera.targetTexture = null;

            if (_rightCamera)
                _rightCamera.targetTexture = null;

            if (_centerCamera)
                _centerCamera.targetTexture = null;
        }
    }
}
