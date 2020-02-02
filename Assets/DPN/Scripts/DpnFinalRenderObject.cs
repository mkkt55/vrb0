
using System;
using UnityEngine;

namespace dpn
{
    public class DpnFinalRenderObject : MonoBehaviour
    {
        public class DpnFinalRenderCamera3
        {
            static public DpnCamera3 Camera3
            {
                get
                {
                    string name = "FinalRenderCamera";

                    Transform transf = DpnCameraRig._instance.transform.Find(name);

                    if (transf != null)
                    {
                        return transf.GetComponent<DpnCamera3>();
                    }
                    else
                    {
                        GameObject gameObject = new GameObject(name);

                        gameObject.transform.SetParent(DpnCameraRig._instance.transform);

                        DpnCamera3 camera3 = gameObject.AddComponent<DpnCamera3>();

                        camera3.leftTransform = DpnCameraRig._instance._left_transform;
                        camera3.rightTransform = DpnCameraRig._instance._right_transform;
                        camera3.centerTransform = DpnCameraRig._instance._center_transform;

                        camera3.clearFlags = CameraClearFlags.Nothing;
                        camera3.depth = 100.0f;
                        camera3.cullingMask = 1 << LAYER;
                        camera3.FarClipPlane = DpnCameraRig._instance._center_eye.farClipPlane;
                        return camera3;
                    }
                }
            }
            static public readonly int LAYER = 31;
        }

        Renderer[] _renderers = null;
        public bool applyToChildren = true;
        DpnCamera3 camera3 = null;

        void Start()
        {

            camera3 = DpnFinalRenderCamera3.Camera3;
            gameObject.layer = DpnFinalRenderCamera3.LAYER;

            if (applyToChildren)
            {
                _renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                {
                    gameObject.transform.GetChild(i).gameObject.layer = DpnFinalRenderCamera3.LAYER;
                }
            }
            else
            {
                _renderers = new Renderer[1];
                _renderers[0] = GetComponent<Renderer>();
            }

            Camera.onPreCull += OnCameraPreCull;
        }

        bool isVisable = true;

        void OnCameraPreCull(Camera camera)
        {
            if (_renderers == null)
                return;

            if (camera3 != null && camera3.ContainsCamera(camera))
            {
                if (!isVisable)
                {
                    EnableRenderers(true);
                    isVisable = true;
                }
            }
            else
            {
                if (isVisable)
                {
                    EnableRenderers(false);
                    isVisable = false;
                }
            }
        }

        void EnableRenderers(bool enabled)
        {
            foreach (Renderer renderer in _renderers)
            {
                renderer.enabled = enabled;
            }
        }

        void OnDestroy()
        {
            if (_renderers != null)
            {
                Array.Clear(_renderers, 0, _renderers.Length);
                _renderers = null;
            }
        }
    }
}