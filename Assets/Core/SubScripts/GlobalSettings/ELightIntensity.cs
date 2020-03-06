using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TalesFromTheRift;

public class ELightIntensity : MonoBehaviour
{
	private float valueLastFrame;
	public Text t;
	public Slider s;
	public GameObject pc;

	void OnEnable()
	{
		if (t == null)
		{
			s = GetComponent<Slider>();
			t = transform.Find("ElIntensity").GetComponent<Text>();
			s.value = VrbSettingData.elIntensity;
		}
		t.text = VrbSettingData.elIntensity.ToString();
	}

	// Start is called before the first frame update
	void Start()
	{
		pc = GameObject.Find("PlayerController");
	}

	// Update is called once per frame
	void Update()
	{
		if (!s.value.Equals(valueLastFrame))
		{
			t.text = s.value.ToString();
			VrbSettingData.elIntensity = s.value;
			RenderSettings.ambientIntensity = s.value;
		}
	}
}
