using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbEditableVertex : MonoBehaviour, IPointerClickHandler
{
	public VrbVertex v;

	public GameObject pc;
	public PlayerController pcpc;

	void OnEnable()
	{
		if (pc == null)
		{
			pc = GameObject.Find("PlayerController");
			pcpc = pc.GetComponent<PlayerController>();
		}
	}
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (!pcpc.performedSomeOperation)
		{
			return;
		}
		transform.position = v.vector3;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(v);
	}

}
