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

	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("CustomModelController").GetComponent<PlayerController>().selectFace(f);
	}
}
