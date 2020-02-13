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
			for (int i = 0; i < o.vertices.Count; i++)
			{
				vs.Add(o.vertices[i].vector3);
			}
			o.mesh.SetVertices(vs);
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
		Debug.LogWarning("aaaaaa");
		GameObject.Find("PlayerController").GetComponent<PlayerController>().selectObject(o);
	}
}
