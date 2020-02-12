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
	private GameObject gameObject;
	private Transform transform;
	private Material material;
	private bool constructed = false;
	private bool displayed = false;

	public VrbVertex(float _x, float _y, float _z)
	{
		addToStatic();

		vector3 = new Vector3(_x, _y, _z);
	}

	private void constructModel()
	{
		GameObject vp = Resources.Load("VrbPoint") as GameObject;
		gameObject = GameObject.Instantiate(vp);

		transform = gameObject.transform;
		transform.parent = GameObject.Find("EditableModel").transform;

		material = gameObject.GetComponent<MeshRenderer>().material;

		transform.position = vector3;
		transform.localScale = new Vector3(5, 5, 5);

		VrbEditableVertex ep = gameObject.AddComponent<VrbEditableVertex>();
		ep.v = this;

		constructed = true;
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

	public void displayModel()
	{
		if (constructed == false)
		{
			constructModel();
		}
		else
		{
			gameObject.SetActive(true);
		}
		displayed = true;
	}

	public void hideModel()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		displayed = false;
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
	private GameObject gameObject;
	public Material material;

	private bool constructed = false;
	private bool displayed = false;

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
		gameObject = new GameObject("face" + index);
		gameObject.transform.parent = GameObject.Find("EditableModel").transform;

		MeshFilter mf = gameObject.AddComponent<MeshFilter>();
		if (mf != null)
		{
			mf.mesh = mesh;
		}

		// 展示Mesh，且可操作。
		VrbEditableFace ef = gameObject.AddComponent<VrbEditableFace>();
		ef.f = this;

		MeshRenderer meshRender = gameObject.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

		// 使用shader设置双面，并成为白色
		Material mat = new Material(Shader.Find("Custom/DoubleSided"));
		meshRender.material = mat;
		meshRender.material.color = Color.white;

		material = meshRender.material;

		constructed = true;
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

	public void displayModel()
	{
		if (constructed == false)
		{
			constructModel();
		}
		else
		{
			gameObject.SetActive(true);
		}
		displayed = true;
	}

	public void hideModel()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		displayed = false;
	}
}


public class VrbObject
{
	public static List<VrbObject> all = new List<VrbObject>();
	public static VrbObject editingObject = null;

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

	public Vector3 position; // 位置
	public int rx, ry, rz; // 旋转
	public Vector3 scale; // 三方向scale

	public List<VrbFace> faces;

	public List<VrbVertex> vertices;
	public List<Vector3> vectors;
	public List<int> triangles;
	private GameObject gameObject;
	public Mesh mesh;

	public Material material;

	private bool constructed = false;
	private bool displayed = false;

	public VrbObject(Vector3 p)
	{
		position = p;
		faces = new List<VrbFace>();
		addToStatic();
		calSelf();
	}

	public VrbObject(float x, float y, float z)
	{
		position = new Vector3(x, y, z);
		faces = new List<VrbFace>();
		addToStatic();
		calSelf();
	}


	public VrbObject(Vector3 p, List<VrbFace> _faces)
	{
		position = p;
		faces = _faces;
		addToStatic();
		calSelf();
	}

	public VrbObject(float x, float y, float z, List<VrbFace> _faces)
	{
		position = new Vector3(x, y, z);
		faces = _faces;
		addToStatic();
		calSelf();
	}

	private void calSelf()
	{
		vertices = new List<VrbVertex>();
		vectors = new List<Vector3>();
		triangles = new List<int>();

		for (int i = 0; i < faces.Count; i++)
		{
			for (int j = 0; j < faces[i].fVertices.Count; j++)
			{
				if (vertices.IndexOf(faces[i].fVertices[j]) == -1)
				{
					vertices.Add(faces[i].fVertices[j]);
					vectors.Add(faces[i].fVertices[j].vector3);
				}
			}
		}

		// 一个三角面片是不可能出现在两个面里的，所以可以直接遍历所有
		for (int i = 0; i < faces.Count; i++)
		{
			for (int j = 0; j < faces[i].ftOriginal.Count; j++)
			{
				triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v0));
				triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v1));
				triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v2));
			}
		}

		mesh = new Mesh();
		mesh.SetVertices(vectors);
		mesh.SetTriangles(triangles, 0);
	}

	public void constructModel()
	{
		gameObject = new GameObject("object" + index);
		gameObject.transform.parent = GameObject.Find("Layout").transform;
		gameObject.transform.position = position;

		MeshFilter mf = gameObject.AddComponent<MeshFilter>();
		if (mf != null)
		{
			mf.mesh = mesh;
		}

		// 展示Mesh，且可操作。
		VrbSelectableObject so = gameObject.AddComponent<VrbSelectableObject>();
		so.o = this;

		MeshRenderer meshRender = gameObject.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

		// 使用shader设置双面，并成为白色
		Material mat = new Material(Shader.Find("Custom/DoubleSided"));
		meshRender.material = mat;
		meshRender.material.color = Color.white;

		material = meshRender.material;

		constructed = true;
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = Color.white;
	}

	public void displayModel()
	{
		if (constructed == false)
		{
			constructModel();
		}
		else
		{
			gameObject.SetActive(true);
		}
		displayed = true;
	}

	public void hideModel()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		displayed = false;
	}

	public void enterEdit()
	{
		hideAll();
		for (int i = 0; i < faces.Count; i++)
		{
			faces[i].displayModel();
		}
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i].displayModel();
		}
		editingObject = this;
	}

	public static void exitEdit()
	{
		if (editingObject != null)
		{
			for (int i = 0; i < editingObject.faces.Count; i++)
			{
				editingObject.faces[i].hideModel();
			}
			for (int i = 0; i < editingObject.vertices.Count; i++)
			{
				editingObject.vertices[i].hideModel();
			}
		}
		displayAll();
	}

	public static void hideAll()
	{
		for (int i = 0; i < all.Count; i++)
		{
			all[i].hideModel();
		}
	}

	public static void displayAll()
	{
		for (int i = 0; i < all.Count; i++)
		{
			all[i].displayModel();
		}
	}

	public void move(Vector3 dv)
	{
		position += dv;
	}
}


// 便于Unity中调试
public class VrbModel : MonoBehaviour
{
	void Start()
	{
		VrbObject o = createCube(0, 100, 0, 200, 200, 200);
		o.displayModel();
	}

	void Update()
	{

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

	public VrbObject createCube(float xp, float yp, float zp, float xl, float yl, float zl)
	{
		VrbVertex p0 = new VrbVertex(xl / 2, yl / 2, zl / 2);
		VrbVertex p1 = new VrbVertex(xl / 2, yl / 2, -zl / 2);
		VrbVertex p2 = new VrbVertex(xl / 2, -yl / 2, zl / 2);
		VrbVertex p3 = new VrbVertex(xl / 2, -yl / 2, -zl / 2);
		VrbVertex p4 = new VrbVertex(-xl / 2, yl / 2, zl / 2);
		VrbVertex p5 = new VrbVertex(-xl / 2, yl / 2, -zl / 2);
		VrbVertex p6 = new VrbVertex(-xl / 2, -yl / 2, zl / 2);
		VrbVertex p7 = new VrbVertex(-xl / 2, -yl / 2, -zl / 2);

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
		return new VrbObject(xp, yp, zp, fList);
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
