/************************************************************************************

Copyright   :   Copyright 2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/
using UnityEngine;
using System.Collections;

namespace dpn
{
    [RequireComponent(typeof(Renderer))]
    public class ControllerRay : MonoBehaviour
    {
        private Material materialComp;

        // Use this for initialization
        void Start()
        {
            CreateRayVertices();

            transform.localPosition = new Vector3(0,0,0);

            materialComp = gameObject.GetComponent<Renderer>().material;
            float color = 175.0f / 255.0f;
            materialComp.SetColor("_Color", new Color(color, color, color, 0.45f));
            materialComp.renderQueue = 4000;
        }

        private void CreateRayVertices()
        {
            Mesh mesh = new Mesh();
            gameObject.AddComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = mesh;

            Vector3[] vertices =
            {
                new Vector3(0.5f, 0.866025f, 0.0f),
                new Vector3(1,0, 0.0f),
                new Vector3(0.5f, -0.866025f, 0.0f),

                new Vector3(-0.5f, -0.866025f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-0.5f, 0.866025f, 0.0f),

                new Vector3(0.5f, 0.866025f, -1.0f),
                new Vector3(1, 0, -1.0f),
                new Vector3(0.5f, -0.866025f, -1.0f),

                new Vector3(-0.5f, -0.866025f, -1.0f),
                new Vector3(-1.0f, 0.0f, -1.0f),
                new Vector3(-0.5f, 0.866025f, -1.0f),
            };

            Vector3 scale = new Vector3(0.002f, 0.002f, 1.0f);
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector3 vertex = vertices[i];
                vertices[i] = new Vector3(vertex.x * scale.x, vertex.y * scale.y, vertex.z * scale.z);
            }

            int[] indices =
            {
                0, 6, 7, 0, 7, 1,
                1, 7, 8, 1, 8, 2,
                2, 8, 9, 2, 9, 3,
                3, 9,10, 3,10, 4,
                4,10,11, 4,11, 5,
                5,11, 6, 5, 6, 0
            };

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            ;
        }

        // Update is called once per frame
        void Update()
        {
            if (DpnPointerManager.Pointer == null)
                return;

            float length = 0.0f;

            do
            {
                var pointer = DpnPointerManager.Pointer;
                if (pointer == null)
                    break;

                Transform pointerTransform = pointer.GetPointerTransform();
                if (transform == null)
                    break;

                Vector3 Direction = transform.position - pointerTransform.localToWorldMatrix.MultiplyPoint(new Vector3(0,0,0.06f));

                transform.rotation = Quaternion.LookRotation(Direction.normalized, transform.up);

                length = Direction.magnitude;
                if (length < 0)
                {
                    length = 0;
                }
            } while (false);

            materialComp.SetFloat("_Length", length);
        }
    }
}
