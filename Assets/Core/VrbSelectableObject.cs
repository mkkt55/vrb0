using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbSelectableObject : MonoBehaviour, IPointerClickHandler
{
	public VrbObject o;
	// Start is called before the first frame update
	void Start()
	{

	}

	void OnEnable()
	{
		if (o != null && o.mesh != null && o.vertices != null)
		{
			List<Vector3> vs = new List<Vector3>();
			for (int i = 0; i < o.faces.Count; i++)
			{
				for (int j = 0; j < o.faces[i].ftOriginal.Count; j++)
				{
					o.vectors.Add(o.faces[i].ftOriginal[j].v0.vector3);
					o.triangles.Add(o.vectors.Count - 1);
					o.vectors.Add(o.faces[i].ftOriginal[j].v1.vector3);
					o.triangles.Add(o.vectors.Count - 1);
					o.vectors.Add(o.faces[i].ftOriginal[j].v2.vector3);
					o.triangles.Add(o.vectors.Count - 1);
					//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v2));
					//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v1));
					//triangles.Add(vertices.IndexOf(faces[i].ftOriginal[j].v0));
				}
				for (int j = 0; j < o.faces[i].fEdges.Count; j++)
				{
					if (o.edges.IndexOf(o.faces[i].fEdges[j]) == -1)
					{
						o.edges.Add(o.faces[i].fEdges[j]);
					}
				}
			}
			o.mesh.SetVertices(o.vectors);
			o.mesh.SetTriangles(o.triangles, 0);
			o.mesh.RecalculateBounds();
			o.mesh.RecalculateNormals();
			o.meshCollider.sharedMesh = o.mesh;
		}
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = o.position;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(o);
	}
}
