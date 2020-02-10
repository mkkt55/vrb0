using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vrb;

public class VrbModelDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		VrbModel.spawnCube(0, 200, 0, 200, 200, 200);
		DisplayModel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void DisplayModel()
	{
		foreach (VrbFace f in VrbFace.all)
		{
			List<VrbPoint> fPoints = f.getFacePoints();
			int pNum = fPoints.Count;
			List<VrbTriangle> fTriangles = f.triangles;
			int tNum = fTriangles.Count;

			Vector3[] vertices = new Vector3[pNum];
			int[] verticesPreIndex = new int[pNum];
			int[] indices = new int[tNum * 3];

			// 找面里的所有点，加入vertices数组，并查找所有面，确定这些点新的索引，计入indices数组对应位置。
			for (int i = 0; i < pNum; i++)
			{
				vertices[i] = new Vector3(fPoints[i].x, fPoints[i].y, fPoints[i].z);
				for (int j = 0; j < tNum; j++)
				{
					if (fTriangles[j].vertices[0] == fPoints[i])
					{
						indices[3 * j] = i;
					}
					if (fTriangles[j].vertices[1] == fPoints[i])
					{
						indices[3 * j + 1] = i;
					}
					if (fTriangles[j].vertices[2] == fPoints[i])
					{
						indices[3 * j + 2] = i;
					}
				}
			}

			GameObject go = new GameObject("DynamicallyAdded");

			Mesh m = new Mesh();
			m.vertices = vertices;
			m.triangles = indices;

			for (int i = 0; i < tNum; i++)
			{
				Debug.Log("Triangle index: " + i);
				Debug.Log(indices[3 * i] + " " + indices[3 * i + 1] + " " + indices[3 * i + 2]);
			}

			MeshFilter mf = go.AddComponent<MeshFilter>();
			if (mf != null)
			{
				mf.sharedMesh = m;
			}

			MeshRenderer meshRender = go.AddComponent<MeshRenderer>();
			MeshCollider meshCollider = go.AddComponent<MeshCollider>();
		}
		
	}
}
