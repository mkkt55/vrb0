using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightPanel : MonoBehaviour
{
	public GameObject playerController;
	public Dropdown typeDropdown;
	public InputField rInputfield;
	public InputField iInputfield;

	public void init()
	{
		playerController = GameObject.Find("PlayerController");
		typeDropdown = GameObject.Find("PlayerController/InfoCanvas/LightPanel/TypePanel/Dropdown").GetComponent<Dropdown>();
		rInputfield = GameObject.Find("PlayerController/InfoCanvas/LightPanel/RangePanel/InputField").GetComponent<InputField>();
		iInputfield = GameObject.Find("PlayerController/InfoCanvas/LightPanel/IntensityPanel/InputField").GetComponent<InputField>();
	}

	void OnEnable()
    {
		if (playerController)
		{
			updateValue();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void updateValue()
	{
		if (playerController.GetComponent<PlayerController>().selected.Count > 0 && playerController.GetComponent<PlayerController>().selected[0].getType().Equals("light"))
		{
			VrbLight vrbl = (VrbLight)playerController.GetComponent<PlayerController>().selected[0];
			for (int i = 0;i < typeDropdown.options.Count; i++)
			{
				if (typeDropdown.options[i].text.Equals(vrbl.type.ToString()))
				{
					typeDropdown.value = i;
					break;
				}
			}
			rInputfield.text = vrbl.range.ToString();
			iInputfield.text = vrbl.intensity.ToString();
		}
	}

	public void setValue()
	{
		List<VrbTarget> temp = playerController.GetComponent<PlayerController>().selected;
		if (temp.Count > 0 && temp[0].getType().Equals("light"))
		{
			VrbLight vrbl = (VrbLight)temp[0];
			if (typeDropdown.value == 0)
			{
				vrbl.type = LightType.Directional;
			}
			else if (typeDropdown.value == 1)
			{
				vrbl.type = LightType.Point;
			}
			else if (typeDropdown.value == 2)
			{
				vrbl.type = LightType.Spot;
			}
			try
			{
				vrbl.range = float.Parse(rInputfield.text);
			}
			catch (Exception e)
			{
				print(e);
			}
			try
			{
				vrbl.intensity = float.Parse(iInputfield.text);
			}
			catch (Exception e)
			{
				print(e);
			}

		}
	}
}
