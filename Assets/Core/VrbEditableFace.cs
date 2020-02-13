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
		for (int i = 0; i < f.fVertices.Count; i++)
		{
			f.fVectors[i] = f.fVertices[i].vector3;
		}
		f.mesh.SetVertices(f.fVectors);
		f.mesh.RecalculateNormals();
		f.meshCollider.sharedMesh = f.mesh;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		Debug.LogWarning("seleeef");
		GameObject.Find("PlayerController").GetComponent<PlayerController>().selectFace(f);
	}
}
