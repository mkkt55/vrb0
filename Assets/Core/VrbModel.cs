using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrbObject
{
	public static List<VrbObject> all = new List<VrbObject>();
	public int x, y, z; // 位置
	public int rx, ry, rz; // 旋转
	public int sx, sy, sz; // 三方向scale

	public List<int> faces = new List<int>();

	public VrbObject()
	{
		all.Add(this);
	}

	public VrbObject(List<int> _faces)
	{
		faces = _faces;
		all.Add(this);
	}
}


public class VrbModel: MonoBehaviour
{
	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>(); // 三个一组保存顶点坐标
	public List<List<int>> faces = new List<List<int>>(); // 里面的List<int>代表一个面，面中的int代表三角面片在triangle数组的起始位置，即永远是3的倍数。
	public List<List<int>> objects = new List<List<int>>();

	public Vector3 createPoint(float x, float y, float z)
	{
		Vector3 v = new Vector3(x, y, z);
		vertices.Add(v);
		return v;
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
		int fIndex = faces.Count;
		faces.Add(fTriangles);
		return fIndex;
	}

	public List<Vector3> getFaceVertices(List<int> face)
	{
		List<Vector3> fVertices = new List<Vector3>();
		foreach (int tIndex in face)
		{
			Vector3 p = vertices[triangles[tIndex]];
			if (fVertices.IndexOf(p) == -1)
			{
				fVertices.Add(p);
			}

			p = vertices[triangles[tIndex + 1]];
			if (fVertices.IndexOf(p) == -1)
			{
				fVertices.Add(p);
			}

			p = vertices[triangles[tIndex + 2]];
			if (fVertices.IndexOf(p) == -1)
			{
				fVertices.Add(p);
			}
		}
		return fVertices;
	}

	public int createObject(List<int> oFaces)
	{
		int oIndex = objects.Count;
		objects.Add(oFaces);
		return oIndex;
	}

	void Start()
	{
		createCube(0, 100, 0, 200, 200, 200);
		DisplayModel();
	}
	
	void Update()
	{

	}

	public void DisplayModel()
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			GameObject vp = Resources.Load("VrbPoint") as GameObject;
			GameObject go = Instantiate(vp);

			VrbEditablePoint ep = go.AddComponent<VrbEditablePoint>();
			ep.vertexIndex = i;
		}
		
		for (int i = 0; i < faces.Count; i++)
		{
			List<int> f = faces[i];
			GameObject go = new GameObject("DynamicallyAdded");

			// 展示Mesh，且可操作。
			VrbEditableFace ef = go.AddComponent<VrbEditableFace>();
			ef.fVertices = getFaceVertices(f);
			ef.fTriangles = new List<int>();
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
		Vector3 p0 = createPoint(xp + xl / 2, yp + yl / 2, zp + zl / 2);
		Vector3 p1 = createPoint(xp + xl / 2, yp + yl / 2, zp - zl / 2);
		Vector3 p2 = createPoint(xp + xl / 2, yp - yl / 2, zp + zl / 2);
		Vector3 p3 = createPoint(xp + xl / 2, yp - yl / 2, zp - zl / 2);
		Vector3 p4 = createPoint(xp - xl / 2, yp + yl / 2, zp + zl / 2);
		Vector3 p5 = createPoint(xp - xl / 2, yp + yl / 2, zp - zl / 2);
		Vector3 p6 = createPoint(xp - xl / 2, yp - yl / 2, zp + zl / 2);
		Vector3 p7 = createPoint(xp - xl / 2, yp - yl / 2, zp - zl / 2);

		int t0 = createTriangle(0, 1, 3);
		int t1 = createTriangle(0, 2, 3);
		int t2 = createTriangle(0, 1, 4);
		int t3 = createTriangle(1, 4, 5);
		int t4 = createTriangle(1, 3, 5);
		int t5 = createTriangle(3, 5, 7);
		int t6 = createTriangle(4, 5, 6);
		int t7 = createTriangle(5, 6, 7);
		int t8 = createTriangle(0, 2, 4);
		int t9 = createTriangle(2, 4, 6);
		int t10 = createTriangle(2, 3, 7);
		int t11 = createTriangle(2, 6, 7);


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
