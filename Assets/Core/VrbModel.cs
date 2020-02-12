using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbFace
{
	public static List<VrbFace> all = new List<VrbFace>();

	private int index;
	public int getIndex()
	{
		return index;
	}

	private void addToStatic()
	{
		index = all.Count;
		all.Add(this);
	}

	public List<int> fVertexIndices; // 面的顶点在VrbModel中的索引是多少，即它是vertice中的哪一位。
	public List<Vector3> fVertices; // 面的顶点的具体信息

	public List<int> ftOriginal; // 面中的int代表三角面片在VrbModel.triangles数组的起始位置，即永远是3的倍数。
	public List<int> fTriangles; // 代表三角面片在自己的fVertices数组中的位置，是上一个的三倍

	public Mesh mesh; // 用来展示的mesh

	public VrbFace(List<int> t)
	{
		addToStatic();

		ftOriginal = t;
		calSelf();
	}

	private void calSelf()
	{
		fVertexIndices = new List<int>();
		fVertices = new List<Vector3>();
		// 根据ftOriginal记录的原始面片索引，计算自己的所有顶点索引和顶点信息
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			int ti = ftOriginal[i]; // triangle index
			int vi0 = VrbModel.triangles[ti]; // index of vertex 0 of the triangle, in VrbModel.vertices
			int vi1 = VrbModel.triangles[ti + 1];
			int vi2 = VrbModel.triangles[ti + 2];

			if (fVertexIndices.IndexOf(vi0) == -1)
			{
				fVertexIndices.Add(vi0);
				fVertices.Add(VrbModel.vertices[vi0]);
			}

			if (fVertexIndices.IndexOf(vi1) == -1)
			{
				fVertexIndices.Add(vi1);
				fVertices.Add(VrbModel.vertices[vi1]);
			}

			if (fVertexIndices.IndexOf(vi2) == -1)
			{
				fVertexIndices.Add(vi2);
				fVertices.Add(VrbModel.vertices[vi2]);
			}
		}

		// 计算自己的三角面片的顶点，在自己的所有顶点中的索引。
		int[] temp = new int[ftOriginal.Count * 3];
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			int ti = ftOriginal[i]; // triangle index
			int vi0 = VrbModel.triangles[ti]; // index of vertex 0 of the triangle
			int vi1 = VrbModel.triangles[ti + 1];
			int vi2 = VrbModel.triangles[ti + 2];

			Debug.LogWarning("第" + index + "个面在算第" + i + "个三角形，vi0vi1vi2分别" + vi0 + "," + vi1 + "," + vi2);
			temp[3 * i] = fVertexIndices.IndexOf(vi0);
			temp[3 * i + 1] = fVertexIndices.IndexOf(vi1);
			temp[3 * i + 2] = fVertexIndices.IndexOf(vi2);
		}
		fTriangles = new List<int>(temp);

		mesh = new Mesh();
		mesh.SetVertices(fVertices);
		mesh.SetTriangles(fTriangles, 0);
	}
	
	public void constructModel()
	{
		GameObject go = new GameObject("DynamicallyAdded");
		go.transform.parent = GameObject.Find("CustomModel").transform;

		MeshFilter mf = go.AddComponent<MeshFilter>();
		if (mf != null)
		{
			mf.mesh = mesh;
		}
		
		// 展示Mesh，且可操作。
		VrbEditableFace ef = go.AddComponent<VrbEditableFace>();
		ef.fIndex = index;
		// 可选择面
		VrbSelectable s = go.AddComponent<VrbSelectable>();
		

		MeshRenderer meshRender = go.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = go.AddComponent<MeshCollider>();

		// 使用shader设置双面，并成为白色
		Material mat = new Material(Shader.Find("Custom/DoubleSided"));
		meshRender.material = mat;
		meshRender.material.color = Color.white;
	}

	public void updateMesh()
	{
		for(int i = 0; i < fVertexIndices.Count; i++)
		{
			fVertices[i] = VrbModel.vertices[fVertexIndices[i]];
		}
		mesh.SetVertices(fVertices);
	}
}


public class VrbObject
{
	public static List<VrbObject> all = new List<VrbObject>();

	private int index;
	public int getIndex()
	{
		return index;
	}

	private void addToStatic()
	{
		index = all.Count;
		all.Add(this);
	}

	public int x, y, z; // 位置
	public int rx, ry, rz; // 旋转
	public int sx, sy, sz; // 三方向scale

	public List<int> faces = new List<int>();

	public VrbObject()
	{
		addToStatic();
	}

	public VrbObject(List<int> _faces)
	{
		addToStatic();
	}
}


// 便于Unity中调试
public class VrbModel: MonoBehaviour
{
	public static List<Vector3> vertices = new List<Vector3>();
	public static List<int> triangles = new List<int>(); // 三个一组保存顶点坐标
	public static List<VrbObject> objects = new List<VrbObject>();

	public List<int> dt;

	void Start()
	{
		dt = triangles;
		createCube(0, 100, 0, 200, 200, 200);
		DisplayPoint();
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			VrbFace.all[i].constructModel();
		}
	}

	void Update()
	{
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			VrbFace.all[i].updateMesh();
		}
	}

	public int createPoint(float x, float y, float z)
	{
		int startIndex = vertices.Count;
		Vector3 v = new Vector3(x, y, z);
		vertices.Add(v);
		return startIndex;
	}

	public int createTriangle(int p1, int p2, int p3)
	{
		int startIndex = triangles.Count;
		triangles.Add(p1);
		triangles.Add(p2);
		triangles.Add(p3);
		return startIndex;
	}

	public int createFace(List<int> fTriangles)
	{
		VrbFace f = new VrbFace(fTriangles);
		return f.getIndex();
	}
	
	public int createObject(List<int> oFaces)
	{
		int oIndex = objects.Count;
		objects.Add(new VrbObject(oFaces));
		return oIndex;
	}

	public void DisplayPoint()
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			GameObject vp = Resources.Load("VrbPoint") as GameObject;
			GameObject go = Instantiate(vp);

			VrbEditablePoint ep = go.AddComponent<VrbEditablePoint>();
			ep.vertexIndex = i;
		}
		
	}

	// 三角形面片，记录顶点索引
	//public void createQuad()
	//{
	//	Vector3 p0 = createPoint(100, 200, 0);
	//	Vector3 p1 = createPoint(100, 0, 0);
	//	Vector3 p2 = createPoint(-100, 0, 0);
	//	Vector3 p3 = createPoint(-100, 200, 0);

	//	int t0 = createTriangle(0, 1, 2);
	//	int t1 = createTriangle(0, 2, 3);

	//	List<int> f0 = createFace(new List<int>());
	//	f0.Add(t0);
	//	f0.Add(t1);

	//	List<int> fList = new List<int>();
	//	fList.Add(f0);
	//	new VrbObject(fList);
	//}

	// 前三个是位置坐标，后三个是大小
	public void createCube(float xp, float yp, float zp, float xl, float yl, float zl)
	{
		int p0 = createPoint(xp + xl / 2, yp + yl / 2, zp + zl / 2);
		int p1 = createPoint(xp + xl / 2, yp + yl / 2, zp - zl / 2);
		int p2 = createPoint(xp + xl / 2, yp - yl / 2, zp + zl / 2);
		int p3 = createPoint(xp + xl / 2, yp - yl / 2, zp - zl / 2);
		int p4 = createPoint(xp - xl / 2, yp + yl / 2, zp + zl / 2);
		int p5 = createPoint(xp - xl / 2, yp + yl / 2, zp - zl / 2);
		int p6 = createPoint(xp - xl / 2, yp - yl / 2, zp + zl / 2);
		int p7 = createPoint(xp - xl / 2, yp - yl / 2, zp - zl / 2);

		int t0 = createTriangle(p0, p1, p3);
		int t1 = createTriangle(p0, p2, p3);
		int t2 = createTriangle(p0, p1, p4);
		int t3 = createTriangle(p1, p4, p5);
		int t4 = createTriangle(p1, p3, p5);
		int t5 = createTriangle(p3, p5, p7);
		int t6 = createTriangle(p4, p5, p6);
		int t7 = createTriangle(p5, p6, p7);
		int t8 = createTriangle(p0, p2, p4);
		int t9 = createTriangle(p2, p4, p6);
		int t10 = createTriangle(p2, p3, p7);
		int t11 = createTriangle(p2, p6, p7);

		List<int> list = new List<int>();
		list.Add(t0);
		list.Add(t1);
		int f0 = createFace(list);

		list = new List<int>();
		list.Add(t2);
		list.Add(t3);
		int f1 = createFace(list);

		list = new List<int>();
		list.Add(t4);
		list.Add(t5);
		int f2 = createFace(list);

		list = new List<int>();
		list.Add(t6);
		list.Add(t7);
		int f3 = createFace(list);

		list = new List<int>();
		list.Add(t8);
		list.Add(t9);
		int f4 = createFace(list);

		list = new List<int>();
		list.Add(t10);
		list.Add(t11);
		int f5 = createFace(list);



		List<int> fList = new List<int>();
		fList.Add(f0);
		fList.Add(f1);
		fList.Add(f2);
		fList.Add(f3);
		fList.Add(f4);
		fList.Add(f5);
		createObject(fList);
	}

	public static bool readObj(string path)
	{
		return true;
	}

	public static bool saveObj(string path)
	{
		return true;
	}

}
