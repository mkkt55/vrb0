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
		
	}

	// Update is called once per frame
	void Update()
	{

	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(o);
	}
}
