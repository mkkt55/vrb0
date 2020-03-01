using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VrbObjectUI : MonoBehaviour, IPointerClickHandler
{
	public VrbObject o;
	public GameObject inf;
	public GameObject text;

	public bool isSelected = false;

	// Start is called before the first frame update
	void Start()
	{
		inf = transform.Find("InputField").gameObject;
		inf.GetComponent<InputField>().text = o.name;
		inf.GetComponent<InputField>().onEndEdit.AddListener(delegate { o.setName(inf.GetComponent<InputField>().text); inf.SetActive(false);text.SetActive(true);text.GetComponent<Text>().text = o.name; });

		text = transform.Find("Text").gameObject;
		text.GetComponent<Text>().text = o.name;

		inf.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if (!isSelected)
		{
			GameObject.Find("PlayerController").GetComponent<PlayerController>().select(o);
		}
		else
		{
			inf.SetActive(true);
			text.SetActive(false);
		}
	}
}
