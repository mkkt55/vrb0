using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbSelectableObject : MonoBehaviour, IPointerClickHandler
{
	public Color colorLastFrame;
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
		if (!o.vrbc.color.Equals(colorLastFrame))
		{
			o.material.color = o.vrbc.color;
			colorLastFrame = o.vrbc.color;
			for (int i = 0; i < o.faces.Count; i++)
			{
				o.faces[i].matVrbc.color = o.vrbc.color;
			}
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(o);
	}
}
