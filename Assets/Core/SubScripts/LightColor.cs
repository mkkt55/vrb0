using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LightColor : MonoBehaviour, IPointerClickHandler
{
	private Color colorLastFrame;

	public VrbColor vrbc;
	public Image img;

	public GameObject pc;
	public List<VrbTarget> selected;

	void OnEnable()
	{
		if (vrbc != null && selected != null && selected.Count > 0)
		{
			vrbc.color = ((VrbLight)selected[0]).color;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		vrbc = new VrbColor();
		img = GetComponent<Image>();
		pc = GameObject.Find("PlayerController");
		selected = pc.GetComponent<PlayerController>().selected;
	}

	// Update is called once per frame
	void Update()
	{
		if (vrbc.color != colorLastFrame)
		{
			img.color = vrbc.color;
			if (selected != null && selected.Count > 0)
			{
				((VrbLight)selected[0]).color = vrbc.color;
				colorLastFrame = vrbc.color;
			}
			else
			{
				selected = pc.GetComponent<PlayerController>().selected;
			}
		}
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		PlayerController.colorPanel.GetComponent<ColorPanel>().cRef = vrbc;
		PlayerController.colorPanel.SetActive(true);
	}
}
