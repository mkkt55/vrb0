using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbEditableFace : MonoBehaviour
{
	public List<Vector3> fVertices;
	public List<int> fVertexIndices; // 面的顶点在VrbModel中原来的索引是多少，即它是vertice中的哪一位。
	public List<int> fTriangles;
	public Mesh mesh;
	private VrbModel vrbm;

	// Start is called before the first frame update
	void Start()
    {
		vrbm = GameObject.Find("CustomModel").GetComponent<VrbModel>();
		constructMesh();
    }

    // Update is called once per frame
    void Update()
    {
		// 这两句是必要的，因为此时unity传递的是复制而不是引用，因为Vector3是struct。
		// mesh.Clear();
		for (int i = 0; i < fVertices.Count; i++)
		{
			fVertices[i] = vrbm.vertices[fVertexIndices[i]];
		}
		mesh.SetVertices(fVertices);
		mesh.SetTriangles(fTriangles, 0);
	}

	public void updateVertices()
	{

	}

	public void updateFace()
	{

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
