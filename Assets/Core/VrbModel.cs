using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public interface VrbTarget
{
	string getType();
	void move(Vector3 m);
	void rotate(Vector3 a);
	void scale(Vector3 s);

	Vector3 getPosition();
	Vector3 getRotate();
	Vector3 getScale();

	void select();
	void deSelect();
	GameObject getGameObject();
}

public class VrbVertex : VrbTarget
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
	private Color defaultColor;
	private bool constructed = false;
	private bool displayed = false;

	public VrbVertex(float _x, float _y, float _z)
	{
		addToStatic();

		vector3 = new Vector3(_x, _y, _z);
	}

	private void constructModel()
	{
		GameObject r = Resources.Load("VrbVertex") as GameObject;
		gameObject = GameObject.Instantiate(r);

		transform = gameObject.transform;
		transform.parent = GameObject.Find("EditableModel").transform;
		transform.position = vector3;

		material = gameObject.GetComponent<MeshRenderer>().material;
		defaultColor = material.color;

		VrbEditableVertex ep = gameObject.GetComponent<VrbEditableVertex>();
		ep.v = this;

		constructed = true;
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = defaultColor;
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

	public string getType()
	{
		return "vertex";
	}

	public void move(Vector3 dv)
	{
		vector3 += dv;
	}

	public void rotate(Vector3 a)
	{
		return;
	}

	public void scale(Vector3 s)
	{
		return;
	}

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public Vector3 getPosition()
	{
		return vector3;
	}

	public Vector3 getRotate()
	{
		return Vector3.zero;
	}

	public Vector3 getScale()
	{
		return Vector3.zero;
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
	public VrbEdge e0;
	public VrbEdge e1;
	public VrbEdge e2;

	public VrbTriangle(VrbVertex _v0, VrbVertex _v1, VrbVertex _v2)
	{
		v0 = _v0;
		v1 = _v1;
		v2 = _v2;

		addToStatic();

		e0 = VrbEdge.addEdge(v0, v1);
		e1 = VrbEdge.addEdge(v1, v2);
		e2 = VrbEdge.addEdge(v0, v2);
	}
}


public class VrbEdge : VrbTarget
{
	public static List<VrbEdge> all = new List<VrbEdge>();

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
	private GameObject gameObject;
	private Transform transform;
	private Material material;
	private Color defaultColor;
	private bool constructed = false;
	private bool displayed = false;

	private VrbEdge(VrbVertex _v0, VrbVertex _v1)
	{
		addToStatic();
		v0 = _v0;
		v1 = _v1;
	}
	public static VrbEdge addEdge(VrbVertex _v0, VrbVertex _v1)
	{
		VrbEdge e = findEdgeByVertices(_v0, _v1);
		if (e == null)
		{
			return new VrbEdge(_v0, _v1);
		}
		else
		{
			return e;
		}
	}
	public static VrbEdge findEdgeByVertices(VrbVertex _v0, VrbVertex _v1)
	{
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].v0 == _v0 && all[i].v1 == _v1)
			{
				return all[i];
			}
			if (all[i].v0 == _v1 && all[i].v1 == _v0)
			{
				return all[i];
			}
		}
		return null;
	}

	private void constructModel()
	{
		GameObject r = Resources.Load("VrbEdge") as GameObject;
		gameObject = GameObject.Instantiate(r);

		transform = gameObject.transform;
		transform.parent = GameObject.Find("EditableModel").transform;
		
		material = gameObject.GetComponent<MeshRenderer>().material;
		defaultColor = material.color;

		VrbEditableEdge ep = gameObject.GetComponent<VrbEditableEdge>();
		ep.e = this;

		constructed = true;
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = defaultColor;
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
	
	public string getType()
	{
		return "edge";
	}

	public void move(Vector3 dv)
	{
		v0.vector3 += dv;
		v1.vector3 += dv;
	}

	public void rotate(Vector3 a)
	{
		Vector3 center = (v0.vector3 + v1.vector3) / 2;
		v0.vector3 = Quaternion.Euler(a) * (v0.vector3 - center) + v0.vector3;
		v1.vector3 = Quaternion.Euler(a) * (v1.vector3 - center) + v1.vector3;
	}

	public void scale(Vector3 s)
	{
		Vector3 center = (v0.vector3 + v1.vector3) / 2;
		Vector3 dv = v0.vector3 - center;
		dv.x *= s.x;
		dv.y *= s.y;
		dv.z *= s.z;

		v0.vector3 = dv + center;
		v1.vector3 = -dv + center;
	}

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public Vector3 getPosition()
	{
		return (v0.vector3 + v1.vector3) / 2;
	}

	public Vector3 getRotate()
	{
		return gameObject.transform.rotation * Vector3.forward;
	}

	public Vector3 getScale()
	{
		return Vector3.one;
	}
}


public class VrbFace : VrbTarget
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
	public List<VrbEdge> fEdges;
	public List<int> fTriangles; // 代表三角面片在自己的fVertices数组中的位置，是上一个的三倍

	public Mesh mesh; // 用来展示的mesh
	public MeshCollider meshCollider;
	private GameObject gameObject;
	public Material material;
	private Color defaultColor;

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
		int[] temp = new int[ftOriginal.Count * 6];
		fEdges = new List<VrbEdge>();
		List<VrbEdge> allEdges = new List<VrbEdge>();
		List<int> fEdgeCount = new List<int>();
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			VrbTriangle t = ftOriginal[i];
			
			temp[6 * i] = fVertices.IndexOf(t.v0);
			temp[6 * i + 1] = fVertices.IndexOf(t.v1);
			temp[6 * i + 2] = fVertices.IndexOf(t.v2);
			//temp[6 * i + 3] = fVertices.IndexOf(t.v2);
			//temp[6 * i + 4] = fVertices.IndexOf(t.v1);
			//temp[6 * i + 5] = fVertices.IndexOf(t.v0);

			int eIndex = allEdges.IndexOf(t.e0);
			if (eIndex == -1) {
				allEdges.Add(t.e0);
				fEdgeCount.Add(1);
			}
			else
			{
				fEdgeCount[eIndex] += 1;
			}

			eIndex = allEdges.IndexOf(t.e1);
			if (eIndex == -1)
			{
				allEdges.Add(t.e1);
				fEdgeCount.Add(1);
			}
			else
			{
				fEdgeCount[eIndex] += 1;
			}

			eIndex = allEdges.IndexOf(t.e2);
			if (eIndex == -1)
			{
				allEdges.Add(t.e2);
				fEdgeCount.Add(1);
			}
			else
			{
				fEdgeCount[eIndex] += 1;
			}
		}
		fTriangles = new List<int>(temp);

		for (int i = 0; i < allEdges.Count; i++)
		{
			if (fEdgeCount[i] == 1)
			{
				fEdges.Add(allEdges[i]);
			}
		}

		mesh = new Mesh();
		mesh.SetVertices(fVectors);
		mesh.SetTriangles(fTriangles, 0);
		mesh.RecalculateNormals();
	}

	public void constructModel()
	{
		GameObject r = Resources.Load("VrbFace") as GameObject;
		gameObject = GameObject.Instantiate(r);

		gameObject.transform.parent = GameObject.Find("EditableModel").transform;

		MeshFilter mf = gameObject.GetComponent<MeshFilter>();
		if (mf != null)
		{
			mf.sharedMesh = mesh;
		}
		// 展示Mesh，且可操作。
		VrbEditableFace ef = gameObject.GetComponent<VrbEditableFace>();
		ef.f = this;

		MeshRenderer meshRender = gameObject.GetComponent<MeshRenderer>();

		meshCollider = gameObject.GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;

		material = meshRender.material;
		defaultColor = material.color;

		constructed = true;
	}

	public void select()
	{
		material.color = Color.red;
	}

	public void deSelect()
	{
		material.color = defaultColor;
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

	public string getType()
	{
		return "face";
	}

	public void move(Vector3 dv)
	{
		for (int i = 0; i < fVertices.Count; i++)
		{
			fVertices[i].move(dv);
		}
	}

	public void rotate(Vector3 a)
	{
		Vector3 totalPos = Vector3.zero;
		for (int i = 0; i < fVertices.Count; i++)
		{
			totalPos += fVertices[i].vector3;
		}
		Vector3 center = (totalPos) / fVertices.Count;
		for (int i = 0; i < fVertices.Count; i++)
		{
			fVertices[i].vector3= Quaternion.Euler(a) * (fVertices[i].vector3 - center) + center;
		}
	}

	public void scale(Vector3 s)
	{
		Vector3 totalPos = Vector3.zero;
		for (int i = 0; i < fVertices.Count; i++)
		{
			totalPos += fVertices[i].vector3;
		}
		Vector3 center = (totalPos) / fVertices.Count;
		for (int i = 0; i < fVertices.Count; i++)
		{
			Vector3 dv = fVertices[i].vector3 - center;
			dv.x *= s.x;
			dv.y *= s.y;
			dv.z *= s.z;
			fVertices[i].vector3 = dv + center;
		}
	}

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public Vector3 getPosition()
	{
		Vector3 totalPos = Vector3.zero;
		for (int i = 0; i < fVertices.Count; i++)
		{
			totalPos += fVertices[i].vector3;
		}
		Vector3 center = (totalPos) / fVertices.Count;
		return center;
	}

	public Vector3 getRotate()
	{
		return Vector3.zero;
	}

	public Vector3 getScale()
	{
		return Vector3.one;
	}
}



public class VrbObject : VrbTarget
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
	public Vector3 scaleVector; // 三方向scale

	public List<VrbFace> faces;
	public List<VrbVertex> vertices;
	public List<VrbEdge> edges;
	public List<Vector3> vectors;
	public List<int> triangles;
	private GameObject gameObject;
	public Mesh mesh;
	public MeshCollider meshCollider;
	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	public Material material;
	public Material defaultMat;
	public Color defaultColor;
	private Material selectedMat;
	private Color selectedColor;

	public GameObject UiItem;

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
		edges = new List<VrbEdge>();

		for (int i = 0; i < faces.Count; i++)
		{
			for (int j = 0; j < faces[i].fVertices.Count; j++)
			{
				if (vertices.IndexOf(faces[i].fVertices[j]) == -1)
				{
					vertices.Add(faces[i].fVertices[j]);
				}
			}
		}

		// 一个三角面片是不可能出现在两个面里的，所以可以直接遍历所有
		for (int i = 0; i < faces.Count; i++)
		{
			for (int j = 0; j < faces[i].ftOriginal.Count; j++)
			{
				vectors.Add(faces[i].ftOriginal[j].v0.vector3);
				triangles.Add(vectors.Count - 1);
				vectors.Add(faces[i].ftOriginal[j].v1.vector3);
				triangles.Add(vectors.Count - 1);
				vectors.Add(faces[i].ftOriginal[j].v2.vector3);
				triangles.Add(vectors.Count - 1);
				//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v2));
				//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v1));
				//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v0));
			}
			for (int j = 0; j < faces[i].fEdges.Count; j++)
			{
				if (edges.IndexOf(faces[i].fEdges[j]) == -1)
				{
					edges.Add(faces[i].fEdges[j]);
				}
			}
		}

		mesh = new Mesh();
		mesh.SetVertices(vectors);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
	}

	public void constructModel()
	{
		GameObject r = Resources.Load("VrbObject") as GameObject;
		gameObject = GameObject.Instantiate(r);

		gameObject.transform.parent = GameObject.Find("Layout").transform;
		gameObject.transform.position = position;

		meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			meshFilter.sharedMesh = mesh;
		}

		// 展示Mesh，且可操作。
		VrbSelectableObject so = gameObject.GetComponent<VrbSelectableObject>();
		so.o = this;

		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshCollider = gameObject.GetComponent<MeshCollider>();

		meshCollider.sharedMesh = mesh;
		material = meshRenderer.material;
		defaultColor = material.color;
		defaultMat = material;
		selectedColor = Color.red;
		selectedMat = new Material(Shader.Find("Custom/SelectedEffect"));

		GameObject rui = Resources.Load("Object-UI") as GameObject;
		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		constructed = true;
	}

	public void select()
	{
		material.color = selectedColor;
	}

	public void deSelect()
	{
		material.color = defaultColor;
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
		for (int i = 0; i < edges.Count; i++)
		{
			edges[i].displayModel();
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
			for (int i = 0; i < editingObject.edges.Count; i++)
			{
				editingObject.edges[i].hideModel();
			}
			displayAll();
			editingObject = null;
		}
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

	public string getType()
	{
		return "object";
	}

	public void move(Vector3 dv)
	{
		position += dv;
	}

	public void rotate(Vector3 a)
	{
		gameObject.transform.Rotate(a);
	}

	public void scale(Vector3 s)
	{
		Vector3 sv = gameObject.transform.localScale;
		sv.x *= s.x;
		sv.y *= s.y;
		sv.z *= s.z;
		gameObject.transform.localScale = sv;
	}

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public Vector3 getPosition()
	{
		return gameObject.transform.position;
	}

	public Vector3 getRotate()
	{
		return gameObject.transform.rotation * Vector3.forward;
	}

	public Vector3 getScale()
	{
		return gameObject.transform.localScale;
	}
}


// 便于Unity中调试
public static class VrbModel
{
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

	public static VrbObject createCube(float xp, float yp, float zp, float xl, float yl, float zl)
	{
		VrbVertex p0 = new VrbVertex(xl / 2, yl / 2, zl / 2);
		VrbVertex p1 = new VrbVertex(xl / 2, yl / 2, -zl / 2);
		VrbVertex p2 = new VrbVertex(xl / 2, -yl / 2, zl / 2);
		VrbVertex p3 = new VrbVertex(xl / 2, -yl / 2, -zl / 2);
		VrbVertex p4 = new VrbVertex(-xl / 2, yl / 2, zl / 2);
		VrbVertex p5 = new VrbVertex(-xl / 2, yl / 2, -zl / 2);
		VrbVertex p6 = new VrbVertex(-xl / 2, -yl / 2, zl / 2);
		VrbVertex p7 = new VrbVertex(-xl / 2, -yl / 2, -zl / 2);

		VrbTriangle t0 = new VrbTriangle(p0, p3, p1);
		VrbTriangle t1 = new VrbTriangle(p0, p2, p3);
		VrbTriangle t2 = new VrbTriangle(p0, p1, p4);
		VrbTriangle t3 = new VrbTriangle(p1, p5, p4);
		VrbTriangle t4 = new VrbTriangle(p1, p3, p5);
		VrbTriangle t5 = new VrbTriangle(p3, p7, p5);
		VrbTriangle t6 = new VrbTriangle(p4, p5, p6);
		VrbTriangle t7 = new VrbTriangle(p5, p7, p6);
		VrbTriangle t8 = new VrbTriangle(p0, p4, p2);
		VrbTriangle t9 = new VrbTriangle(p2, p4, p6);
		VrbTriangle t10 = new VrbTriangle(p2, p7, p3);
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

	public static bool openModel(string path)
	{
		return true;
	}

	public static bool saveModel(string path)
	{
		if (VrbObject.all.Count > 0)
		{
			//string data = ObjExporterScript.MeshToString(VrbObject.all[0].meshFilter, VrbObject.all[0].meshRenderer, VrbObject.all[0].getGameObject().transform);
			//ObjExporter.WriteToFile(data, "D:/zz.obj");
			string s = MeshToString(VrbObject.all[0].mesh , null);
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(s);
			}
		}
		return true;
	}

	public static string MeshToString(Mesh m, List<Material> mats)
	{
		if (!m)
		{
			return "####Error####";
		}
		StringBuilder sb = new StringBuilder();

		foreach (Vector3 v in m.vertices)
		{
			sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
		}
		sb.Append("\n");

		foreach (Vector3 n in m.normals)
		{
			sb.Append(string.Format("vn {0} {1} {2}\n", n.x, n.y, n.z));
		}
		sb.Append("\n");

		foreach (Vector3 t in m.vertices)
		{
			sb.Append(string.Format("vt {0} {1}\n", "0.29", "0.74"));
		}
		sb.Append("\n");

		for (int i = 0; i < m.triangles.Length; i += 3)
		{
			sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", m.triangles[i] + 1, m.triangles[i + 1] + 1, m.triangles[i + 2] + 1));
		}
		sb.Append("\n");
		
		return sb.ToString();
	}

}
