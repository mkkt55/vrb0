using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RottInput : MonoBehaviour
{
	public InputField xText;
	public InputField yText;
	public InputField zText;
	public PlayerController playerController;
	// Start is called before the first frame update
	void Start()
	{

	}

	public void OnEnable()
	{
		updateValue();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void init()
	{
		xText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/RotatePanel/InputX").GetComponent<InputField>();
		yText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/RotatePanel/InputY").GetComponent<InputField>();
		zText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/RotatePanel/InputZ").GetComponent<InputField>();
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		updateValue();
	}

	public void updateValue()
	{
		if (playerController && playerController.selected.Count > 0)
		{
			Vector3 v = playerController.selected[0].getRotate();
			xText.text = v.x.ToString();
			yText.text = v.y.ToString();
			zText.text = v.z.ToString();
		}
	}
}
