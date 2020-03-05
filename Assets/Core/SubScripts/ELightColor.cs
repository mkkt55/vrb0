using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ELightColor : MonoBehaviour, IPointerClickHandler
{
	private Color colorLastFrame;

	public VrbColor vrbc;
	public Image img;

	void OnEnable()
	{
		if (vrbc != null)
		{
			vrbc.color = VrbSettingData.elColor;
		}
	}
	// Start is called before the first frame update
	void Start()
    {
		vrbc = new VrbColor();
		img = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
		if (vrbc.color != colorLastFrame)
		{
			img.color = vrbc.color;
			VrbSettingData.elColor = vrbc.color;
			RenderSettings.ambientLight = vrbc.color;

			colorLastFrame = vrbc.color;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		PlayerController.colorPanel.GetComponent<ColorPanel>().cRef = vrbc;
		PlayerController.colorPanel.SetActive(true);
	}
}
