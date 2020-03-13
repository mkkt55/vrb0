using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TalesFromTheRift;

public class EnterProjectNameScript : MonoBehaviour,IPointerClickHandler
{
	GameObject pc;
	Text t;

	void OnEnable()
	{
		GetComponentInChildren<Text>().text = VrbSettingData.projectName;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		t.text = "";
		pc.GetComponent<OpenCanvasKeyboard>().inputObject = t.gameObject;
		pc.GetComponent<OpenCanvasKeyboard>().OpenKeyboard();
	}

	// Start is called before the first frame update
	void Start()
    {
		pc = GameObject.Find("PlayerController");
		t = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		if (t.text != "")
		{
			VrbSettingData.projectName = t.text;
		}
		else
		{
			VrbSettingData.projectName = "Untitled";
		}
    }


}
