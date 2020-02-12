using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrbVertex
{
	public static List<VrbVertex> all = new List<VrbVertex>();

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

	public Vector3 vector3;
	public Transform transform;
	public Material material;

	public VrbVertex(float _x, float _y, float _z)
	{
		addToStatic();

		vector3 = new Vector3(_x, _y, _z);
	}

	public void constructModel()
	{
		GameObject vp = Resources.Load("VrbPoint") as GameObject;
		GameObject go = GameObject.Instantiate(vp);

		transform = go.transform;
		transform.parent = GameObject.Find("CustomModel").transform;

		material = go.GetComponent<MeshRenderer>().material;

		transform.position = vector3;
		transform.localScale = new Vector3(5, 5, 5);

		VrbEditableVertex ep = go.AddComponent<VrbEditableVertex>();
		ep.v = this;
	}

	public void updateMesh()
	{
		transform.position = vector3;
	}

	public void move(Vector3 dv)
	{
		vector3 += dv;
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = Color.green;
	}
}

public class VrbTriangle
{
	public static List<VrbTriangle> all = new List<VrbTriangle>();

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

	public VrbVertex v0;
	public VrbVertex v1;
	public VrbVertex v2;

	public VrbTriangle(VrbVertex _v0, VrbVertex _v1, VrbVertex _v2)
	{
		v0 = _v0;
		v1 = _v1;
		v2 = _v2;

		addToStatic();
	}
}

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

	public List<VrbVertex> fVertices; // 面的顶点
	public List<Vector3> fVectors; // 面的顶点

	public List<VrbTriangle> ftOriginal; // 面中的int代表三角面片在VrbModel.triangles数组的起始位置，即永远是3的倍数。
	public List<int> fTriangles; // 代表三角面片在自己的fVertices数组中的位置，是上一个的三倍

	public Mesh mesh; // 用来展示的mesh
	public Material material;

	public VrbFace(List<VrbTriangle> t)
	{
		addToStatic();

		ftOriginal = t;
		calSelf();
	}

	private void calSelf()
	{
		fVertices = new List<VrbVertex>();
		fVectors = new List<Vector3>();
		// 根据ftOriginal记录的原始面片索引，计算自己的所有顶点索引和顶点信息
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			VrbTriangle t = ftOriginal[i];

			if (fVertices.IndexOf(t.v0) == -1)
			{
				fVertices.Add(t.v0);
				fVectors.Add(t.v0.vector3);
			}

			if (fVertices.IndexOf(t.v1) == -1)
			{
				fVertices.Add(t.v1);
				fVectors.Add(t.v1.vector3);
			}

			if (fVertices.IndexOf(t.v2) == -1)
			{
				fVertices.Add(t.v2);
				fVectors.Add(t.v2.vector3);
			}
		}

		// 计算自己的三角面片的顶点，在自己的所有顶点中的索引。
		int[] temp = new int[ftOriginal.Count * 3];
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			VrbTriangle t = ftOriginal[i];
			
			temp[3 * i] = fVertices.IndexOf(t.v0);
			temp[3 * i + 1] = fVertices.IndexOf(t.v1);
			temp[3 * i + 2] = fVertices.IndexOf(t.v2);
		}
		fTriangles = new List<int>(temp);

		mesh = new Mesh();
		mesh.SetVertices(fVectors);
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
		ef.f = this;

		MeshRenderer meshRender = go.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = go.AddComponent<MeshCollider>();

		// 使用shader设置双面，并成为白色
		Material mat = new Material(Shader.Find("Custom/DoubleSided"));
		meshRender.material = mat;
		meshRender.material.color = Color.white;

		material = meshRender.material;
	}

	public void updateMesh()
	{
		for (int i = 0; i < fVertices.Count; i++)
		{
			fVectors[i] = fVertices[i].vector3;
		}
		mesh.SetVertices(fVectors);
	}

	public void move(Vector3 dv)
	{
		for (int i = 0; i < fVertices.Count; i++)
		{
			fVertices[i].move(dv);
		}
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = Color.white;
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

	public List<VrbFace> faces = new List<VrbFace>();

	public VrbObject()
	{
		addToStatic();
	}

	public VrbObject(List<VrbFace> _faces)
	{
		faces = _faces;
		addToStatic();
	}

	public void select()
	{

	}

	public void deSelect()
	{

	}
}


// 便于Unity中调试
public class VrbModel : MonoBehaviour
{
	public static List<VrbObject> objects = new List<VrbObject>();
	

	void Start()
	{
		createCube(0, 100, 0, 200, 200, 200);
		for (int i = 0; i < VrbVertex.all.Count; i++)
		{
			VrbVertex.all[i].constructModel();
		}
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			VrbFace.all[i].constructModel();
		}
	}

	void Update()
	{
		for (int i = 0; i < VrbVertex.all.Count; i++)
		{
			VrbVertex.all[i].updateMesh();
		}
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			VrbFace.all[i].updateMesh();
		}
	}

	

	// 三角形面片，记录顶点索引
	//public void createQuad()
	//{
	//	Vector3 p0 = new VrbVertex(100, 200, 0);
	//	Vector3 p1 = new VrbVertex(100, 0, 0);
	//	Vector3 p2 = new VrbVertex(-100, 0, 0);
	//	Vector3 p3 = new VrbVertex(-100, 200, 0);

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
		VrbVertex p0 = new VrbVertex(xp + xl / 2, yp + yl / 2, zp + zl / 2);
		VrbVertex p1 = new VrbVertex(xp + xl / 2, yp + yl / 2, zp - zl / 2);
		VrbVertex p2 = new VrbVertex(xp + xl / 2, yp - yl / 2, zp + zl / 2);
		VrbVertex p3 = new VrbVertex(xp + xl / 2, yp - yl / 2, zp - zl / 2);
		VrbVertex p4 = new VrbVertex(xp - xl / 2, yp + yl / 2, zp + zl / 2);
		VrbVertex p5 = new VrbVertex(xp - xl / 2, yp + yl / 2, zp - zl / 2);
		VrbVertex p6 = new VrbVertex(xp - xl / 2, yp - yl / 2, zp + zl / 2);
		VrbVertex p7 = new VrbVertex(xp - xl / 2, yp - yl / 2, zp - zl / 2);

		VrbTriangle t0 = new VrbTriangle(p0, p1, p3);
		VrbTriangle t1 = new VrbTriangle(p0, p2, p3);
		VrbTriangle t2 = new VrbTriangle(p0, p1, p4);
		VrbTriangle t3 = new VrbTriangle(p1, p4, p5);
		VrbTriangle t4 = new VrbTriangle(p1, p3, p5);
		VrbTriangle t5 = new VrbTriangle(p3, p5, p7);
		VrbTriangle t6 = new VrbTriangle(p4, p5, p6);
		VrbTriangle t7 = new VrbTriangle(p5, p6, p7);
		VrbTriangle t8 = new VrbTriangle(p0, p2, p4);
		VrbTriangle t9 = new VrbTriangle(p2, p4, p6);
		VrbTriangle t10 = new VrbTriangle(p2, p3, p7);
		VrbTriangle t11 = new VrbTriangle(p2, p6, p7);

		List<VrbTriangle> list = new List<VrbTriangle>();
		list.Add(t0);
		list.Add(t1);
		VrbFace f0 = new VrbFace(list);

		list = new List<VrbTriangle>();
		list.Add(t2);
		list.Add(t3);
		VrbFace f1 = new VrbFace(list);

		list = new List<VrbTriangle>();
		list.Add(t4);
		list.Add(t5);
		VrbFace f2 = new VrbFace(list);

		list = new List<VrbTriangle>();
		list.Add(t6);
		list.Add(t7);
		VrbFace f3 = new VrbFace(list);

		list = new List<VrbTriangle>();
		list.Add(t8);
		list.Add(t9);
		VrbFace f4 = new VrbFace(list);

		list = new List<VrbTriangle>();
		list.Add(t10);
		list.Add(t11);
		VrbFace f5 = new VrbFace(list);



		List<VrbFace> fList = new List<VrbFace>();
		fList.Add(f0);
		fList.Add(f1);
		fList.Add(f2);
		fList.Add(f3);
		fList.Add(f4);
		fList.Add(f5);
		new VrbObject(fList);
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
