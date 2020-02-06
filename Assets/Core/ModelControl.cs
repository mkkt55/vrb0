using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelControl : MonoBehaviour, IPointerClickHandler
{
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
		GetComponent<Renderer>().material.color = Color.red;
		GameObject.Find("CustomModelController").GetComponent<PlayerController>().select(gameObject);
	}
}
