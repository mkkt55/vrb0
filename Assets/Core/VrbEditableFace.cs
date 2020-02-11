using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbEditableFace : MonoBehaviour
{
	public List<Vector3> fVertices;
	public List<int> fTriangles;
	public Mesh mesh;
	
	// Start is called before the first frame update
	void Start()
    {
		constructMesh();
    }

    // Update is called once per frame
    void Update()
    {
		// 这两句是必要的，因为此时unity传递的是复制而不是引用，因为这些数据最终被保存在Unity的C++数据结构中而不是C#。
		// mesh.Clear();
		mesh.SetVertices(fVertices);
		mesh.SetTriangles(fTriangles, 0);
	}

	void constructMesh()
	{
		// 可选择面
		VrbSelectable s = gameObject.AddComponent<VrbSelectable>();

		transform.parent = GameObject.Find("CustomModel").transform;
		
		//int pNum = fVertices.Count;
		//int tNum = fTriangles.Count;

		//// 查找所有面，三个点分别遍历所有确定这些点新的索引，计入indices。
		//for (int tIndex = 0; tIndex < tNum; tIndex++)
		//{
		//	for (int vIndex = 0; vIndex < 3; vIndex++)
		//	{
		//		for (int pIndex = 0; pIndex < pNum; pIndex++)
		//		{
		//			if (fTriangles[tIndex].vertices[vIndex] == fVertices[pIndex])
		//			{
		//				indices.Add(pIndex);
		//			}
		//		}
		//	}

		//}


		mesh = new Mesh();
		mesh.SetVertices(fVertices);
		mesh.SetTriangles(fTriangles, 0);

		//for (int i = 0; i < tNum; i++)
		//{
		//	Debug.Log("Triangle index: " + i);
		//	Debug.Log(indices[3 * i] + " " + indices[3 * i + 1] + " " + indices[3 * i + 2]);
		//}

		MeshFilter mf = gameObject.AddComponent<MeshFilter>();
		if (mf != null)
		{
			mf.mesh = mesh;
		}

		MeshRenderer meshRender = gameObject.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

		// 使用shader设置双面，并成为白色
		Material mat = new Material(Shader.Find("Custom/DoubleSided"));
		meshRender.material = mat;
		meshRender.material.color = Color.white;
	}
}
