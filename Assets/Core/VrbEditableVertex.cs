using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbEditableVertex: MonoBehaviour, IPointerClickHandler
{
	public VrbVertex v;
	// Start is called before the first frame update
	void Start()
	{
		
	}

    // Update is called once per frame
    void Update()
    {
		
    }

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("CustomModelController").GetComponent<PlayerController>().selectVertex(v);
	}

}
