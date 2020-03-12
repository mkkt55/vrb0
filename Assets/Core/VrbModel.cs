using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum VrbTargetType
{
	Vertex,Edge,Face,Object,Light,PlaceTarget,Measurer,LeftMeasurer,RightMeasurer
}

public interface VrbTarget
{
	VrbTargetType getType();
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

	List<VrbFace> rFaces = new List<VrbFace>();

	public Vector3 vector3;
	public GameObject gameObject;
	public Transform transform;
	public Material material;
	public Color defaultColor;
	public bool constructed = false;
	public bool displayed = false;

	public VrbVertex(float _x, float _y, float _z)
	{
		addToStatic();
		vector3 = new Vector3(_x, _y, _z);
	}

	public void constructModel()
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

	public bool isIsolate()
	{
		for (int i = 0; i < VrbTriangle.all.Count; i++)
		{
			if (VrbTriangle.all[i].v0 == this || VrbTriangle.all[i].v1 == this || VrbTriangle.all[i].v2 == this)
			{
				return false;
			}
		}
		return true;
	}

	public void select()
	{
		material.color = new Color(1f, 0.6f, 0.6f);
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

	public VrbTargetType getType()
	{
		return VrbTargetType.Vertex;
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

	public void addToStatic()
	{
		all.Add(this);
	}

	public VrbVertex v0;
	public VrbVertex v1;
	public VrbVertex v2;
	public VrbEdge e0;
	public VrbEdge e1;
	public VrbEdge e2;

	public List<Vector3> vectors
	{
		get
		{
			Vector3 normal_0 = Vector3.Cross(v0.vector3 - v1.vector3, v0.vector3 - v2.vector3).normalized;

			Vector3 normal_1 = -normal_0;

			List<Vector3> list = new List<Vector3>();

			list.Add(v0.vector3 + 0.1f * normal_0);
			list.Add(v1.vector3 + 0.1f * normal_0);
			list.Add(v2.vector3 + 0.1f * normal_0);

			list.Add(v0.vector3 + 0.1f * normal_1);
			list.Add(v2.vector3 + 0.1f * normal_1);
			list.Add(v1.vector3 + 0.1f * normal_1);
			return list;
		}
	}
	
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
	
	public void addToStatic()
	{
		all.Add(this);
	}

	public VrbVertex v0;
	public VrbVertex v1;
	public GameObject gameObject;
	public Transform transform;
	public Material material;
	public Color defaultColor;
	public bool constructed = false;
	public bool displayed = false;

	public VrbEdge(VrbVertex _v0, VrbVertex _v1)
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

	public void constructModel()
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

		ep.init();
	}

	public void select()
	{
		material.color = new Color(1f, 0.6f, 0.6f);
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
	
	public VrbTargetType getType()
	{
		return VrbTargetType.Edge;
	}

	public void move(Vector3 dv)
	{
		v0.vector3 += dv;
		v1.vector3 += dv;
	}

	public void rotate(Vector3 a)
	{
		Vector3 center = (v0.vector3 + v1.vector3) / 2;
		v0.vector3 = Quaternion.Euler(a) * (v0.vector3 - center) + center;
		v1.vector3 = Quaternion.Euler(a) * (v1.vector3 - center) + center;
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
		return Vector3.zero;
	}

	public Vector3 getScale()
	{
		return Vector3.one;
	}
}


public class VrbFace : VrbTarget
{
	public static List<VrbFace> all = new List<VrbFace>();
	
	public void addToStatic()
	{
		all.Add(this);
	}

	public VrbColor vrbc = new VrbColor();
	public VrbColor matVrbc = new VrbColor();

	public void updateMatColor(Color c)
	{
		material.color = c;
		matVrbc.color = c;
	}

	public List<VrbTriangle> ftOriginal; // 面中的int代表三角面片在VrbModel.triangles数组的起始位置，即永远是3的倍数。

	public List<VrbVertex> fVertices // 面的顶点
	{
		get
		{
			List<VrbVertex> list = new List<VrbVertex>();
			for (int i = 0; i < ftOriginal.Count; i++)
			{
				if (list.IndexOf(ftOriginal[i].v0) == -1)
				{
					list.Add(ftOriginal[i].v0);
				}
				if (list.IndexOf(ftOriginal[i].v1) == -1)
				{
					list.Add(ftOriginal[i].v1);
				}
				if (list.IndexOf(ftOriginal[i].v2) == -1)
				{
					list.Add(ftOriginal[i].v2);
				}
			}
			return list;
		}
	}

	public List<VrbEdge> fEdges;

	public List<Vector3> fVectors // 面的顶点
	{
		get
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < ftOriginal.Count; i++)
			{
				list.AddRange(ftOriginal[i].vectors);
			}
			return list;
		}
	}
	public List<int> fTriangles // 代表三角面片在自己的fVertices数组中的位置，是上一个的三倍
	{
		get
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 6 * ftOriginal.Count; i++)
			{
				list.Add(i);
			}
			return list;
		}
	}

	public List<Color> fColors // 三角形的颜色
	{
		get
		{
			Color[] c = new Color[6 * ftOriginal.Count];
			for (int i = 0; i < c.Length; i++)
			{
				c[i] = vrbc.color;
			}
			return new List<Color>(c);
		}
	}

	public Mesh mesh; // 用来展示的mesh
	public MeshCollider meshCollider;
	public GameObject gameObject;
	public Material material;

	public bool constructed = false;
	public bool displayed = false;

	public VrbFace(List<VrbTriangle> t)
	{
		addToStatic();

		ftOriginal = t;
		calSelf();
	}

	public void calSelf()
	{
		calEdge();
		calMesh();
	}

	public void calEdge()
	{
		// 计算所有边。
		fEdges = new List<VrbEdge>();
		// 面的所有边
		List<VrbEdge> allEdges = new List<VrbEdge>();
		List<int> fEdgeCount = new List<int>();
		for (int i = 0; i < ftOriginal.Count; i++)
		{
			VrbTriangle t = ftOriginal[i];

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

		for (int i = 0; i < allEdges.Count; i++)
		{
			if (fEdgeCount[i] == 1)
			{
				fEdges.Add(allEdges[i]);
			}
		}
	}

	public void calMesh()
	{
		vrbc.color = new Color(1f, 1f, 1f);
		mesh = new Mesh();
		mesh.SetVertices(fVectors);
		mesh.SetTriangles(fTriangles, 0);
		mesh.RecalculateNormals();
	}

	public void constructModel()
	{
		mesh.SetColors(fColors);
		GameObject r = Resources.Load("VrbFace") as GameObject;

		gameObject = GameObject.Instantiate(r);
		VrbEditableFace ef = gameObject.GetComponent<VrbEditableFace>();
		ef.f = this;

		gameObject.transform.parent = GameObject.Find("EditableModel").transform;

		MeshFilter mf = gameObject.GetComponent<MeshFilter>();
		if (mf != null)
		{
			mf.mesh = mesh;
		}

		MeshRenderer meshRender = gameObject.GetComponent<MeshRenderer>();

		meshCollider = gameObject.GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;

		meshRender.material.color = matVrbc.color;
		material = meshRender.material;

		constructed = true;

		ef.init();
	}

	public void select()
	{
		material.color = new Color(1f, 0.6f, 0.6f);
	}

	public void deSelect()
	{
		material.color = matVrbc.color;
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

	public VrbTargetType getType()
	{
		return VrbTargetType.Face;
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

	public void addToStatic()
	{
		all.Add(this);
	}

	public VrbColor vrbc = new VrbColor(Color.white);

	public Vector3 positionVector; // 位置
	public Vector3 rotationVector = Vector3.zero; // 旋转
	public Vector3 scaleVector = Vector3.one; // 三方向scale

	public string name;
	public string matStr = VrbMat.types[0];

	public List<VrbFace> faces;
	public List<VrbVertex> vertices;
	public List<VrbEdge> edges;
	public List<Vector3> vectors;
	public List<int> triangles;
	public GameObject gameObject;
	public Mesh mesh;
	public MeshCollider meshCollider;
	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	public Material material;
	public Material defaultMat;
	public Color defaultColor;
	public Material selectedMat;
	public Color selectedColor;

	public GameObject UiItem;

	public bool constructed = false;
	public bool displayed = false;

	public VrbObject(Vector3 p, string _name = "")
	{
		name = _name;
		positionVector = p;
		faces = new List<VrbFace>();
		addToStatic();
		calSelf();
	}

	public VrbObject(float x, float y, float z, string _name = "")
	{
		name = _name;
		positionVector = new Vector3(x, y, z);
		faces = new List<VrbFace>();
		addToStatic();
		calSelf();
	}


	public VrbObject(Vector3 p, List<VrbFace> _faces, string _name = "")
	{
		name = _name;
		positionVector = p;
		faces = _faces;
		addToStatic();
		calSelf();
	}

	public VrbObject(float x, float y, float z, List<VrbFace> _faces, string _name = "")
	{
		name = _name;
		positionVector = new Vector3(x, y, z);
		faces = _faces;
		addToStatic();
		calSelf();
	}

	public void setName(string _name)
	{
		name = _name;
	}

	public void calSelf()
	{
		vertices = new List<VrbVertex>();
		vectors = new List<Vector3>();
		triangles = new List<int>();
		edges = new List<VrbEdge>();
		List<Color> color = new List<Color>();

		for (int i = 0; i < faces.Count; i++)
		{
			List<VrbVertex> list = faces[i].fVertices;
			int n = list.Count;
			for (int j = 0; j < n; j++)
			{
				if (vertices.IndexOf(list[j]) == -1)
				{
					vertices.Add(list[j]);
				}
			}
		}

		// 一个三角面片是不可能出现在两个面里的，所以可以直接遍历所有
		for (int i = 0; i < faces.Count; i++)
		{
			for (int j = 0; j < faces[i].ftOriginal.Count; j++)
			{
				vectors.AddRange(faces[i].ftOriginal[j].vectors);

				triangles.Add(triangles.Count);
				triangles.Add(triangles.Count);
				triangles.Add(triangles.Count);

				triangles.Add(triangles.Count);
				triangles.Add(triangles.Count);
				triangles.Add(triangles.Count);
			}
			color.AddRange(faces[i].fColors);

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
		mesh.RecalculateNormals();
		mesh.SetColors(color);
	}

	public virtual void constructModel()
	{
		GameObject r = Resources.Load("VrbObject") as GameObject;
		gameObject = GameObject.Instantiate(r);

		gameObject.transform.parent = GameObject.Find("Layout").transform;
		gameObject.transform.position = positionVector;

		meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			meshFilter.mesh = mesh;
		}

		// 展示Mesh，且可操作。
		VrbSelectableObject so = gameObject.GetComponent<VrbSelectableObject>();
		so.o = this;
		so.colorLastFrame = vrbc.color;

		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshCollider = gameObject.GetComponent<MeshCollider>();

		meshCollider.sharedMesh = mesh;
		material = meshRenderer.material;
		material.color = vrbc.color;
		defaultColor = material.color;
		defaultMat = material;
		selectedColor = new Color(1f, 0.6f, 0.6f);

		GameObject rui = Resources.Load("Object-UI") as GameObject;
		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		constructed = true;
	}

	public virtual void select()
	{
		material.color = selectedColor;
		UiItem.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
		UiItem.GetComponent<VrbObjectUI>().isSelected = true;
	}

	public virtual void deSelect()
	{
		material.color = vrbc.color;
		UiItem.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
		UiItem.GetComponent<VrbObjectUI>().isSelected = false;
	}

	public virtual void displayModel()
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

	public virtual void hideModel()
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
			faces[i].updateMatColor(vrbc.color);
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
			VrbModel.closeAllPanel();
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
			GameObject.Destroy(editingObject.gameObject);
			GameObject.Destroy(editingObject.UiItem);
			editingObject.constructed = false;
			editingObject.calSelf();
			editingObject.displayModel();
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
			if (all[i] == VrbMeasurer.m || all[i] == VrbMeasurer.l || all[i] == VrbMeasurer.r || all[i] == VrbPlaceTarget.pt)
			{
				continue;
			}
			all[i].displayModel();
		}
	}

	public virtual VrbTargetType getType()
	{
		return VrbTargetType.Object;
	}

	public void move(Vector3 dv)
	{
		gameObject.transform.position += dv;
		positionVector += dv;
	}

	public void rotate(Vector3 a)
	{
		gameObject.transform.Rotate(a);
		rotationVector += a;

		Vector3 totalPos = Vector3.zero;
		for (int i = 0; i < vertices.Count; i++)
		{
			totalPos += vertices[i].vector3;
		}
		Vector3 center = (totalPos) / vertices.Count;
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i].vector3 = Quaternion.Euler(a) * (vertices[i].vector3 - center) + center;
		}
	}

	public void scale(Vector3 s)
	{
		Vector3 sv = gameObject.transform.localScale;
		sv.x *= s.x;
		sv.y *= s.y;
		sv.z *= s.z;
		gameObject.transform.localScale = sv;
		scaleVector = sv;


		Vector3 totalPos = Vector3.zero;
		for (int i = 0; i < vertices.Count; i++)
		{
			totalPos += vertices[i].vector3;
		}
		Vector3 center = (totalPos) / vertices.Count;
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector3 dv = vertices[i].vector3 - center;
			dv.x *= s.x;
			dv.y *= s.y;
			dv.z *= s.z;
			vertices[i].vector3 = dv + center;
		}
	}

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public virtual Vector3 getPosition()
	{
		return gameObject.transform.position;
	}

	public Vector3 getRotate()
	{
		return gameObject.transform.rotation.eulerAngles;
	}

	public Vector3 getScale()
	{
		return gameObject.transform.localScale;
	}
}


public class VrbLight : VrbObject
{
	public Light light;
	public LightType type
	{
		get
		{
			return light.type;
		}
		set
		{
			light.type = value;
		}
	}

	public Color color
	{
		get
		{
			return light.color;
		}
		set
		{
			light.color = value;
		}
	}

	public float intensity
	{
		get
		{
			return light.intensity;
		}
		set
		{
			light.intensity = value;
		}
	}

	public VrbLight(float x, float y, float z, string t = "Direction", float i = 0.1f):base(x,y,z,new List<VrbFace>())
	{
		GameObject g = Resources.Load("VrbLight") as GameObject;
		gameObject = GameObject.Instantiate(g, GameObject.Find("Layout").transform);
		light = gameObject.GetComponent<Light>();
		gameObject.GetComponent<VrbSelectableLight>().l = this;
		intensity = i;
		if (t.Equals("Directional"))
		{
			name = "Directional";
			light.type = LightType.Directional;
		}
		else if (t.Equals("Point"))
		{
			name = "Point";
			light.type = LightType.Point;
		}
		else if (t.Equals("Spot"))
		{
			name = "Spot";
			light.type = LightType.Spot;
		}
	}

	public override void constructModel()
	{
		GameObject rui = Resources.Load("Object-UI") as GameObject;
		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		material = new Material(Shader.Find("Custom/DoubleSided"));
		defaultColor = material.color;
		selectedColor = new Color(1f, 0.6f, 0.6f);

		constructed = true;
	}

	public override VrbTargetType getType()
	{
		return VrbTargetType.Light;
	}
}


// 便于Unity中调试
public static class VrbModel
{
	public static void saveProject(string path)
	{
		StringBuilder sb = new StringBuilder();
		foreach (VrbVertex v in VrbVertex.all)
		{
			sb.AppendLine(string.Format("v {0} {1} {2}", v.vector3.x, v.vector3.y, v.vector3.z));
		}
		sb.AppendLine();
		sb.AppendLine();
		foreach (VrbObject o in VrbObject.all)
		{
			if (o.getType() != VrbTargetType.Light && o.getType() != VrbTargetType.Object)
			{
				continue;
			}
			sb.AppendLine("o-start");
			sb.AppendLine("name " + o.name);
			sb.AppendLine("mat " + o.matStr);
			sb.AppendLine(string.Format("mat-color {0} {1} {2} {3}", o.vrbc.color.r, o.vrbc.color.g, o.vrbc.color.b, o.vrbc.color.a));
			sb.AppendLine(string.Format("position {0} {1} {2}", o.getPosition().x, o.getPosition().y, o.getPosition().z));
			sb.AppendLine(string.Format("rotation {0} {1} {2}", o.getRotate().x, o.getRotate().y, o.getRotate().z));
			sb.AppendLine(string.Format("scale {0} {1} {2}", o.getScale().x, o.getScale().y, o.getScale().z));
			sb.AppendLine();
			foreach (VrbFace f in o.faces)
			{
				sb.AppendLine("f-start");
				sb.AppendLine(string.Format("color {0} {1} {2} {3}", f.vrbc.color.r, f.vrbc.color.g, f.vrbc.color.b, f.vrbc.color.a));
				foreach (VrbTriangle t in f.ftOriginal)
				{
					sb.AppendLine(string.Format("t {0} {1} {2}", t.v0.getIndex(), t.v1.getIndex(), t.v2.getIndex()));
				}
				sb.AppendLine("f-end");
				sb.AppendLine();
			}
			sb.AppendLine("o-end");
			sb.AppendLine();
		}


		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		StreamWriter sw = new StreamWriter(path + "/model.vrbp", false);
		sw.Write(sb.ToString());
		sw.Close();


		sb = new StringBuilder();
		sb.AppendLine(string.Format("elc {0} {1} {2} {3}", VrbSettingData.elColor.r, VrbSettingData.elColor.g, VrbSettingData.elColor.b, VrbSettingData.elColor.a));
		sb.AppendLine(string.Format("eli {0}", VrbSettingData.elIntensity));
		sb.AppendLine(string.Format("skybox {0}", VrbSettingData.skybox));

		StreamWriter sw2 = new StreamWriter(path + "/setting.conf", false);
		sw2.Write(sb.ToString());
		sw2.Close();
	}

	public static void closeAllPanel()
	{
		PlayerController pc = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		pc.openProjectCanvas.SetActive(false);
		pc.saveProjectCanvas.SetActive(false);
		pc.transformPanel.SetActive(false);
		pc.lightPanel.SetActive(false);
		pc.matPanel.SetActive(false);
		pc.exportModelCanvas.SetActive(false);


		pc.projectButtonSubCanvas.SetActive(false);
		pc.settingButtonSubCanvas.SetActive(false);
		pc.placeButtonSubCanvas.SetActive(false);
		pc.lightButtonSubCanvas.SetActive(false);
		pc.textIndicator.SetActive(false);
		pc.distanceDisplayer.SetActive(false);
	}

	public static void deleteAll()
	{
		VrbObject.exitEdit();
		closeAllPanel();

		foreach (VrbFace f in VrbFace.all)
		{
			foreach (VrbEdge e in f.fEdges)
			{
				GameObject.Destroy(e.gameObject);
			}
			foreach (VrbVertex v in f.fVertices)
			{
				GameObject.Destroy(v.gameObject);
			}
			GameObject.Destroy(f.gameObject);
		}
		VrbFace.all.Clear();
		VrbEdge.all.Clear();
		VrbTriangle.all.Clear();
		VrbVertex.all.Clear();

		foreach (VrbObject o in VrbObject.all)
		{
			if (o.getType() == VrbTargetType.Object || o.getType() == VrbTargetType.Light)
			{
				GameObject.Destroy(o.UiItem);
				GameObject.Destroy(o.gameObject);
			}
		}
		VrbObject.all.Clear();
	}

	public static void openProject(string path)
	{
		deleteAll();
		StreamReader sr = new StreamReader(path + "/model.vrbp");
		StreamReader sr2 = new StreamReader(path + "/setting.conf");

		string line = "";
		while ((line = sr.ReadLine()) != null)
		{
			Debug.LogWarning(line);
			string[] strArr = line.Split(' ');
			if (strArr[0] == "v")
			{
				float x = float.Parse(strArr[1]);
				float y = float.Parse(strArr[2]);
				float z = float.Parse(strArr[3]);
				new VrbVertex(x, y, z);
			}
			else if(strArr[0] == "o-start")
			{
				line = sr.ReadLine();
				string name = line.Split(' ')[1];
				line = sr.ReadLine();
				string mat = line.Split(' ')[1];
				line = sr.ReadLine();
				string[] cs = line.Split(' ');
				Color matColor = new Color(float.Parse(cs[1]), float.Parse(cs[2]), float.Parse(cs[3]), float.Parse(cs[4]));


				line = sr.ReadLine();
				string[] s = line.Split(' ');
				Vector3 position = new Vector3(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

				line = sr.ReadLine();
				s = line.Split(' ');
				Quaternion rotate = Quaternion.Euler(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

				line = sr.ReadLine();
				s = line.Split(' ');
				Vector3 scale = new Vector3(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));

				line = sr.ReadLine();
				List<VrbFace> fs = new List<VrbFace>();
				while (line != "o-end")
				{
					Debug.LogWarning(line);
					if (line == "f-start")
					{
						line = sr.ReadLine();
						string []cArr = line.Split(' ');
						float r = float.Parse(cArr[1]);
						float g = float.Parse(cArr[2]);
						float b = float.Parse(cArr[3]);
						float a = float.Parse(cArr[4]);

						Color c = new Color(r, g, b, a);

						List<VrbTriangle> temp = new List<VrbTriangle>();

						line = sr.ReadLine();
						while (line != "f-end")
						{
							string[] tArr = line.Split(' ');
							if (tArr[0] == "t")
							{
								temp.Add(new VrbTriangle(VrbVertex.all[int.Parse(tArr[1])], VrbVertex.all[int.Parse(tArr[2])], VrbVertex.all[int.Parse(tArr[3])]));
							}
							line = sr.ReadLine();
						}
						VrbFace f = new VrbFace(temp);
						f.vrbc.color = c;
						f.matVrbc.color = matColor;
						fs.Add(f);
					}
					line = sr.ReadLine();
				}
				VrbObject vrbo = new VrbObject(position, fs, name);
				vrbo.vrbc.color = matColor;
				vrbo.displayModel();
				vrbo.gameObject.transform.rotation = rotate;
				vrbo.gameObject.transform.localScale = scale;
			}
		}

		sr.Close();

		string ss;
		ss = sr.ReadLine();
		string[] sArr = ss.Split(' ');
		if (sArr[0] == "elc")
		{
			VrbSettingData.elColor = new Color(float.Parse(sArr[1]), float.Parse(sArr[2]), float.Parse(sArr[3]), float.Parse(sArr[4]));
			RenderSettings.ambientLight = VrbSettingData.elColor;
		}
		sr.ReadLine();
		if (sArr[1] == "eli")
		{
			VrbSettingData.elIntensity = float.Parse(sArr[1]);
			RenderSettings.ambientIntensity = VrbSettingData.elIntensity;
		}
		sr.ReadLine();
		if (sArr[0] == "skybox")
		{
			VrbSettingData.skybox = sArr[1];

			string rPath = "Beautify/" + sArr[1];
			RenderSettings.skybox = Resources.Load<Material>(rPath);
		}

		VrbSettingData.projectName = path.Substring(path.LastIndexOf("/"));

		sr2.Close();
	}

	// 三角形面片，记录顶点索引
	public static VrbObject createQuad(float xp, float yp, float zp, float xl, float yl)
	{
		VrbVertex p0 = new VrbVertex(xl/2, yl/2, 0);
		VrbVertex p1 = new VrbVertex(xl/2, -yl/2, 0);
		VrbVertex p2 = new VrbVertex(-xl/2, yl/2, 0);
		VrbVertex p3 = new VrbVertex(-xl/2, -yl/2, 0);

		VrbTriangle t0 = new VrbTriangle(p0, p1, p2);
		VrbTriangle t1 = new VrbTriangle(p1, p2, p3);

		List<VrbTriangle> tl = new List<VrbTriangle>();
		tl.Add(t0);
		tl.Add(t1);

		VrbFace f0 = new VrbFace(tl);

		List<VrbFace> fList = new List<VrbFace>();
		fList.Add(f0);
		return new VrbObject(xp, yp, zp, fList, "Quad");
	}

	public static VrbObject createCircle(float xp, float yp, float zp, float r)
	{
		int num = 100;
		float angle = 2 * Mathf.PI / num;
		VrbVertex p0 = new VrbVertex(xp, yp, zp);
		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < num; i++)
		{
			VrbVertex v = new VrbVertex(xp + r * Mathf.Cos(angle * i), yp + r * Mathf.Sin(angle * i), zp);
			vList.Add(v);
		}

		List<VrbTriangle> tList = new List<VrbTriangle>();
		for (int i = 0; i < num - 1; i++)
		{
			tList.Add(new VrbTriangle(p0, vList[i], vList[i + 1]));
		}
		tList.Add(new VrbTriangle(p0, vList[num - 1], vList[0]));

		VrbFace f0 = new VrbFace(tList);

		List<VrbFace> fList = new List<VrbFace>();
		fList.Add(f0);
		return new VrbObject(xp, yp, zp, fList, "Circle");
	}

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
		return new VrbObject(xp, yp, zp, fList, "Cube");
	}

	public static VrbObject createCylinder(float xp, float yp, float zp, float r, float h)
	{
		int num = 20;
		float angle = Mathf.PI * 2 / num;
		VrbVertex v0 = new VrbVertex(xp, yp + h / 2, zp);
		VrbVertex v1 = new VrbVertex(xp, yp - h / 2, zp);

		List<VrbVertex> vList0 = new List<VrbVertex>();
		List<VrbVertex> vList1 = new List<VrbVertex>();

		for (int i = 0; i < num; i++)
		{
			VrbVertex v = new VrbVertex(xp + r * Mathf.Cos(angle * i), yp + h /2, zp + r * Mathf.Sin(angle * i));
			vList0.Add(v);
			v = new VrbVertex(xp + r * Mathf.Cos(angle * i), yp - h / 2, zp + r * Mathf.Sin(angle * i));
			vList1.Add(v);
		}

		// 上下两面
		List<VrbTriangle> tList0 = new List<VrbTriangle>();
		List<VrbTriangle> tList1 = new List<VrbTriangle>();
		for (int i = 0; i < num - 1; i++)
		{
			tList0.Add(new VrbTriangle(v0, vList0[i], vList0[i + 1]));
			tList1.Add(new VrbTriangle(v1, vList1[i], vList1[i + 1]));
		}
		tList0.Add(new VrbTriangle(v0, vList0[num - 1], vList0[0]));
		tList1.Add(new VrbTriangle(v1, vList1[num - 1], vList1[0]));
		VrbFace f0 = new VrbFace(tList0);
		VrbFace f1 = new VrbFace(tList1);

		List<VrbFace> fList = new List<VrbFace>();
		fList.Add(f0);
		fList.Add(f1);
		for (int i = 0; i < num - 1; i++)
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vList0[i], vList0[i + 1], vList1[i]));
			temp.Add(new VrbTriangle(vList0[i + 1], vList1[i], vList1[i + 1]));
			fList.Add(new VrbFace(temp));
		}
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vList0[num - 1], vList0[0], vList1[num - 1]));
			temp.Add(new VrbTriangle(vList0[0], vList1[num - 1], vList1[0]));
			fList.Add(new VrbFace(temp));
		}

		return new VrbObject(xp, yp, zp, fList, "Cylinder");
	}

	public static VrbObject createSphere(float xp, float yp, float zp, float r)
	{
		List<List<VrbVertex>> vLists = new List<List<VrbVertex>>();
		int num = 20;
		float angleHorizontal = 2 * Mathf.PI / num;
		float angleVertical = Mathf.PI / num;
		VrbVertex vTop = new VrbVertex(xp, r, zp);
		VrbVertex vBottom = new VrbVertex(xp, -r, zp);
		for (int i = 1; i < num - 1; i++)
		{
			float rh = r * Mathf.Sin(angleVertical * i);
			float yh = r * Mathf.Cos(angleVertical * i);
			List<VrbVertex> vList = new List<VrbVertex>();
			for (int j = 0; j < num; j++)
			{
				vList.Add(new VrbVertex(rh * Mathf.Cos(angleHorizontal * j), yh, rh * Mathf.Sin(angleHorizontal * j)));
			}
			vLists.Add(vList);
		}

		List<VrbFace> fList = new List<VrbFace>();
		for (int i = 0; i < vLists[0].Count - 1; i++)
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vTop, vLists[0][i], vLists[0][i + 1]));
			fList.Add(new VrbFace(temp));
		}
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vTop, vLists[0][num - 1], vLists[0][0]));
			fList.Add(new VrbFace(temp));
		}

		for (int i = 0; i < vLists.Count - 1; i++)
		{
			for (int j = 0; j < vLists[i].Count - 1; j++)
			{
				List<VrbTriangle> temp = new List<VrbTriangle>();
				temp.Add(new VrbTriangle(vLists[i][j], vLists[i][j + 1], vLists[i + 1][j]));
				temp.Add(new VrbTriangle(vLists[i][j + 1], vLists[i + 1][j], vLists[i + 1][j + 1]));
				fList.Add(new VrbFace(temp));
			}
			{
				List<VrbTriangle> temp = new List<VrbTriangle>();
				temp.Add(new VrbTriangle(vLists[i][vLists[i].Count - 1], vLists[i][0], vLists[i + 1][vLists[i].Count - 1]));
				temp.Add(new VrbTriangle(vLists[i][0], vLists[i + 1][vLists[i].Count - 1], vLists[i + 1][0]));
				fList.Add(new VrbFace(temp));
			}
		}

		List<VrbVertex> last = vLists[vLists.Count - 1];
		for (int i = 0; i < last.Count - 1; i++)
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vBottom, last[i], last[i + 1]));
			fList.Add(new VrbFace(temp));
		}
		{
			List<VrbTriangle> temp = new List<VrbTriangle>();
			temp.Add(new VrbTriangle(vBottom, last[last.Count - 1], last[0]));
			fList.Add(new VrbFace(temp));
		}

		return new VrbObject(xp, yp, zp, fList, "Sphere");
	}

	public static bool openModel(string path)
	{
		return true;
	}

	public static bool saveModel(string path, Mesh mts)
	{
		FileInfo f = new FileInfo(path);

		if (mts != null)
		{
			string s = MeshToString(mts , null);
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

	public static void deleteVertex(VrbVertex v)
	{
		// 删掉面在object中的引用和面的gameObject，并记录位置以便从VrbFace.all中删除
		List<VrbFace> fToDelete = new List<VrbFace>();
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			for (int j = 0; j < VrbFace.all[i].fVertices.Count; j++)
			{
				if (VrbFace.all[i].fVertices[j] == v)
				{
					GameObject.Destroy(VrbFace.all[i].gameObject);
					VrbObject.editingObject.faces.Remove(VrbFace.all[i]);
					fToDelete.Add(VrbFace.all[i]);
					break;
				}
			}
		}
		// 从VrbFace.all中删掉，顺便删掉所有VrbTriangle
		for (int i=0; i < VrbFace.all.Count; i++)
		{
			for (int j = 0; j < fToDelete.Count; j++)
			{
				if (VrbFace.all[i] == fToDelete[j])
				{
					foreach (VrbTriangle t in VrbFace.all[i].ftOriginal)
					{
						VrbTriangle.all.Remove(t);
					}
					VrbFace.all.RemoveAt(i);
					break;
				}
			}
		}

		// 删除边的gameObject
		for (int i = 0; i < VrbEdge.all.Count; i++)
		{
			if (VrbEdge.all[i].v0 == v || VrbEdge.all[i].v1 == v)
			{
				GameObject.Destroy(VrbEdge.all[i].gameObject);
			}
		}
		// 删除边
		VrbEdge.all.RemoveAll(e => e.v0 == v || e.v1 == v);

		GameObject.Destroy(v.gameObject);
	}

	public static void deleteEdge(VrbEdge e)
	{
		List<VrbFace> fToDelete = new List<VrbFace>();
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			for (int j = 0; j < VrbFace.all[i].fEdges.Count; j++)
			{
				if (VrbFace.all[i].fEdges[j] == e)
				{
					GameObject.Destroy(VrbFace.all[i].gameObject);
					VrbObject.editingObject.faces.Remove(VrbFace.all[i]);
					fToDelete.Add(VrbFace.all[i]);
					break;
				}
			}
		}
		// 从VrbFace.all中删掉，顺便删掉所有VrbTriangle
		for (int i = 0; i < VrbFace.all.Count; i++)
		{
			for (int j = 0; j < fToDelete.Count; j++)
			{
				if (VrbFace.all[i] == fToDelete[j])
				{
					foreach (VrbTriangle t in VrbFace.all[i].ftOriginal)
					{
						VrbTriangle.all.Remove(t);
					}
					VrbFace.all.RemoveAt(i);
					break;
				}
			}
		}

		// 查找对triangle对vertex的引用，如果没有引用就删掉这个点
		if (e.v0.isIsolate())
		{
			deleteVertex(e.v0);
		}
		if (e.v1.isIsolate())
		{
			deleteVertex(e.v1);
		}

		GameObject.Destroy(e.gameObject);
	}

	public static void deleteFace(VrbFace f)
	{
		GameObject.Destroy(f.gameObject);
		VrbObject.editingObject.faces.Remove(f);
		// 从f中删掉所有VrbTriangle
		foreach (VrbTriangle t in f.ftOriginal)
		{
			VrbTriangle.all.Remove(t);
		}

		for (int i=0;i< f.fVertices.Count; i++)
		{
			if (f.fVertices[i].isIsolate())
			{
				deleteVertex(f.fVertices[i]);
			}
		}
		VrbFace.all.Remove(f);
	}

	public static void deleteObject(VrbObject o)
	{
		// 删面、三角和边
		for (int i = 0; i < o.faces.Count; i++)
		{
			foreach (VrbTriangle t in o.faces[i].ftOriginal)
			{
				VrbTriangle.all.Remove(t);
			}
			foreach (VrbEdge e in o.faces[i].fEdges)
			{
				GameObject.Destroy(e.gameObject);
				VrbEdge.all.Remove(e);
			}
			GameObject.Destroy(o.faces[i].gameObject);
			VrbFace.all.Remove(o.faces[i]);
		}
		// 删掉所有点就OK了
		for (int i = 0; i < o.vertices.Count; i++)
		{
			GameObject.Destroy(o.vertices[i].gameObject);
			VrbVertex.all.Remove(o.vertices[i]);
		}
		GameObject.Destroy(o.gameObject);
		GameObject.Destroy(o.UiItem);
		VrbObject.all.Remove(o);
	}

}

public class VrbPlaceTarget:VrbObject
{
	public static VrbPlaceTarget pt;
	public VrbPlaceTarget() : base(0, 0, 0, "Target")
	{
		pt = this;
	}

	public override void constructModel()
	{
		gameObject = GameObject.Instantiate(Resources.Load<GameObject>("VrbTargetPrefab"));
		GameObject rui = Resources.Load("Object-UI") as GameObject;
		gameObject.GetComponentInChildren<VrbTargetScript>().o = this;
		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		defaultColor = gameObject.GetComponentInChildren<MeshRenderer>().material.color;
	}

	public override VrbTargetType getType()
	{
		return VrbTargetType.PlaceTarget;
	}

	public override void select()
	{
		gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 0.6f, 0.6f);
		UiItem.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
	}

	public override void deSelect()
	{
		gameObject.GetComponentInChildren<MeshRenderer>().material.color = defaultColor;
		UiItem.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
	}

	public override void displayModel()
	{
		gameObject.SetActive(true);
		UiItem.SetActive(true);
	}

	public override void hideModel()
	{
		gameObject.SetActive(false);
		UiItem.SetActive(false);
	}
}

public class VrbMeasurer : VrbObject
{
	public static VrbMeasurer m;
	public static GameObject g;
	public static VrbLeftMeasurer l;
	public static VrbRightMeasurer r;
	public VrbMeasurer() : base(0, 0, 0, "Measurer")
	{
		constructModel();
		l = new VrbLeftMeasurer();
		r = new VrbRightMeasurer();
		
		l.constructModel();
		r.constructModel();
		m = this;
	}

	public override void constructModel()
	{
		gameObject = GameObject.Instantiate(Resources.Load<GameObject>("Measurer"));
		g = gameObject;

		GameObject rui = Resources.Load("Object-UI") as GameObject;
		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		gameObject.transform.position = new Vector3(0, -200, -100);
	}

	public override VrbTargetType getType()
	{
		return VrbTargetType.Measurer;
	}

	public override void select()
	{
		l.select();
		r.select();
		UiItem.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
	}

	public override void deSelect()
	{
		l.deSelect();
		r.deSelect();
		UiItem.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
	}

	public override void displayModel()
	{
		gameObject.SetActive(true);
		UiItem.SetActive(true);
		l.displayModel();
		r.displayModel();
	}

	public override void hideModel()
	{
		gameObject.SetActive(false);
		UiItem.SetActive(false);
		l.hideModel();
		r.hideModel();
	}
}

public class VrbLeftMeasurer : VrbObject
{
	public VrbLeftMeasurer() : base(0, 0, 0, "LeftMeasurer")
	{

	}

	public override void constructModel()
	{
		gameObject = VrbMeasurer.g.transform.Find("Left/Measurer-side/default").gameObject;
		GameObject rui = Resources.Load("Object-UI") as GameObject;
		gameObject.GetComponent<VrbSideMeasurerScript>().o = this;

		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
	}

	public override VrbTargetType getType()
	{
		return VrbTargetType.LeftMeasurer;
	}

	public override Vector3 getPosition()
	{
		return gameObject.transform.Find("PosMarker").position;
	}

	public override void select()
	{
		gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.6f, 0.6f);
		UiItem.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
	}

	public override void deSelect()
	{
		gameObject.GetComponent<MeshRenderer>().material.color = defaultColor;
		UiItem.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
	}

	public override void displayModel()
	{
		UiItem.SetActive(true);
	}

	public override void hideModel()
	{
		UiItem.SetActive(false);
	}
}

public class VrbRightMeasurer : VrbObject
{
	public VrbRightMeasurer() : base(0, 0, 0, "RightMeasurer")
	{

	}

	public override void constructModel()
	{
		gameObject = VrbMeasurer.g.transform.Find("Right/Measurer-side/default").gameObject;
		GameObject rui = Resources.Load("Object-UI") as GameObject;
		gameObject.GetComponent<VrbSideMeasurerScript>().o = this;

		UiItem = GameObject.Instantiate(rui, GameObject.Find("PlayerController").GetComponent<PlayerController>().scrollContent.GetComponent<Transform>());
		UiItem.GetComponent<VrbObjectUI>().o = this;

		defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
	}

	public override VrbTargetType getType()
	{
		return VrbTargetType.RightMeasurer;
	}

	public override Vector3 getPosition()
	{
		return gameObject.transform.Find("PosMarker").position;
	}

	public override void select()
	{
		gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.6f, 0.6f);
		UiItem.GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
	}

	public override void deSelect()
	{
		gameObject.GetComponent<MeshRenderer>().material.color = defaultColor;
		UiItem.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
	}

	public override void displayModel()
	{
		UiItem.SetActive(true);
	}

	public override void hideModel()
	{
		UiItem.SetActive(false);
	}
}