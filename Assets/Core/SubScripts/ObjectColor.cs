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

	void OnEnable()
	{
		if (vrbc == null)
		{
			img = GetComponent<Image>();
			pc = GameObject.Find("PlayerController");
			selected = pc.GetComponent<PlayerController>().selected;
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
			img.color = vrbc.color;
			colorLastFrame = vrbc.color;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		pc.GetComponent<PlayerController>().selected[0].deSelect();
		PlayerController.colorPanel.GetComponent<ColorPanel>().cRef = vrbc;
		PlayerController.colorPanel.SetActive(true);
	}
}
