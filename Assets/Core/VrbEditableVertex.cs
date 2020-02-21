using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbEditableVertex : MonoBehaviour, IPointerClickHandler
{
	public VrbVertex v;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.position = v.vector3;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(v);
	}

}
