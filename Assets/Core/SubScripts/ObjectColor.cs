using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectColor : MonoBehaviour, IPointerClickHandler
{
	private Color colorLastFrame;

	public VrbColor vrbc;
	public Image img;

	public GameObject pc;
	public List<VrbTarget> selected;

	public VrbObject o;

	public Dropdown dropdown;

	void OnEnable()
	{
		if (vrbc == null)
		{
			img = GetComponent<Image>();
			pc = GameObject.Find("PlayerController");
			selected = pc.GetComponent<PlayerController>().selected;
			dropdown = transform.parent.parent.GetComponentInChildren<Dropdown>();
		}
		if (selected != null && selected.Count > 0)
		{
			vrbc = ((VrbObject)selected[0]).vrbc;
			colorLastFrame = vrbc.color;
		}
	}
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!colorLastFrame.Equals(vrbc.color))
		{
			switch(dropdown.value)
			{
				case 0:
					img.color = vrbc.color;
					colorLastFrame = vrbc.color;
					break;
				case 1:
					img.color = vrbc.color;
					colorLastFrame = vrbc.color;
					o.material.SetColor("_RimColor", vrbc.color);
					break;
				default:
					break;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (pc.GetComponent<PlayerController>().selected[0].getType() == VrbTargetType.Object)
		{
			o = (VrbObject)pc.GetComponent<PlayerController>().selected[0];
			pc.GetComponent<PlayerController>().selected[0].deSelect();
			PlayerController.colorPanel.GetComponent<ColorPanel>().cRef = vrbc;
			PlayerController.colorPanel.SetActive(true);
		}
	}
}
