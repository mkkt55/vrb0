using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TalesFromTheRift;

public class ScaleButtonZ : MonoBehaviour, IPointerClickHandler
{
	private string textLastFrame;
	public Text t;
	public List<VrbTarget> selected;
	public GameObject pc;

	void OnEnable()
	{
		updateFromTarget();
	}
	// Start is called before the first frame update
	void Start()
	{
		t = GetComponentInChildren<Text>();
		pc = GameObject.Find("PlayerController");
		selected = pc.GetComponent<PlayerController>().selected;
	}

	// Update is called once per frame
	void Update()
	{
		if (!t.text.Equals(textLastFrame))
		{
			pc.GetComponent<PlayerController>().updateScaZfromInput(t.text);
			textLastFrame = t.text;
		}
		if (!OpenCanvasKeyboard.isOpening)
		{
			updateFromTarget();
		}
	}

	public void updateFromTarget()
	{
		if (selected !=null && selected.Count > 0)
		{
			float f = selected[0].getScale().z;
			float tf;
			if (float.TryParse(t.text, out tf) && f - tf > 0.1)
			{
				t.text = f.ToString();
				textLastFrame = f.ToString();
			}
			else
			{
				t.text = f.ToString();
				textLastFrame = f.ToString();
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		t.text = "";
		pc.GetComponent<OpenCanvasKeyboard>().inputObject = t.gameObject;
		pc.GetComponent<OpenCanvasKeyboard>().OpenKeyboard();
	}
}
