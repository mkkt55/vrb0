﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleInput : MonoBehaviour
{
	public InputField xText;
	public InputField yText;
	public InputField zText;
	public PlayerController playerController;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnEnable()
	{
		updateValue();
	}

	public void init()
	{
		xText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/ScalePanel/InputX").GetComponent<InputField>();
		yText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/ScalePanel/InputY").GetComponent<InputField>();
		zText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/ScalePanel/InputZ").GetComponent<InputField>();
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		updateValue();
	}

	public void updateValue()
	{
		if (playerController && playerController.selected.Count > 0)
		{
			Vector3 v = playerController.selected[0].getScale();
			xText.text = v.x.ToString();
			yText.text = v.y.ToString();
			zText.text = v.z.ToString();
		}
	}
}