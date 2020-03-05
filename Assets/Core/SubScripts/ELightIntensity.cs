using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TalesFromTheRift;

public class ELightIntensity : MonoBehaviour, IPointerClickHandler
{
	private string textLastFrame;
	public Text t;
	public GameObject sp;
	public GameObject pc;
	public VrbSetting vrbs;

	void OnEnable()
	{
		if (t == null)
		{
			t = GetComponentInChildren<Text>();
		}
		t.text = VrbSettingData.elIntensity.ToString();
	}

	// Start is called before the first frame update
	void Start()
	{
		t = GetComponentInChildren<Text>();
		sp = GameObject.Find("PlayerController/SettingCanvas/SettingPanel");
		vrbs = sp.GetComponent<VrbSetting>();
		pc = GameObject.Find("PlayerController");
	}

	// Update is called once per frame
	void Update()
	{
		if (!t.text.Equals(textLastFrame))
		{
			vrbs.setEnvironmentLightIntensity(t.text);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		t.text = "";
		pc.GetComponent<OpenCanvasKeyboard>().inputObject = t.gameObject;
		pc.GetComponent<OpenCanvasKeyboard>().OpenKeyboard();
	}
}
