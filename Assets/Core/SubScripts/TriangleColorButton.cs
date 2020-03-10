using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TriangleColorButton : MonoBehaviour, IPointerClickHandler
{
	public GameObject pc;
	public List<VrbTarget> selected;
	// Start is called before the first frame update
	void Start()
	{
		pc = GameObject.Find("PlayerController");
		selected = pc.GetComponent<PlayerController>().selected;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (selected != null && selected.Count > 0 && selected[0].getType() == VrbTargetType.Face)
		{
			PlayerController.colorPanel.GetComponent<ColorPanel>().cRef = ((VrbFace)selected[0]).vrbc;
			PlayerController.colorPanel.SetActive(true);
			pc.GetComponent<PlayerController>().clearAllSelection();
		}
	}
}
