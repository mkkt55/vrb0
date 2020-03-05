using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TalesFromTheRift;

public class LightIntensity : MonoBehaviour, IPointerClickHandler
{
	private string textLastFrame;
	public Text t;
	public GameObject pc;
	public List<VrbTarget> selected;

	void OnEnable()
	{
		if (t != null && selected != null && selected.Count>0)
		{
			Debug.LogWarning(((VrbLight)selected[0]).intensity);
			t.text = ((VrbLight)selected[0]).intensity.ToString();
		}
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
		if (!t.text.Equals(textLastFrame) && selected != null && selected.Count > 0)
		{
			float f;
			if (float.TryParse(t.text, out f))
			{
				((VrbLight)selected[0]).intensity = f;
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
