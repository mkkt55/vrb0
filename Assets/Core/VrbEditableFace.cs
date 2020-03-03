using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbEditableFace : MonoBehaviour, IPointerClickHandler
{
	public VrbFace f;
	void Start()
	{

	}

    // Update is called once per frame
    void Update()
    {
		f.mesh.SetVertices(f.fVectors);
		f.mesh.SetTriangles(f.fTriangles, 0);
		f.mesh.RecalculateNormals();
		f.meshCollider.sharedMesh = f.mesh;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		Debug.LogWarning("seleeef");
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(f);
	}
}
