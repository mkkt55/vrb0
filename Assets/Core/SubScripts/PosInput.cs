using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PosInput : MonoBehaviour
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
		xText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/PositionPanel/InputX").GetComponent<InputField>();
		yText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/PositionPanel/InputY").GetComponent<InputField>();
		zText = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/PositionPanel/InputZ").GetComponent<InputField>();
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
		updateValue();
	}

	public void updateValue()
	{
		if (playerController && playerController.selected.Count > 0)
		{
			Vector3 v = playerController.selected[0].getPosition();
			xText.text = v.x.ToString();
			yText.text = v.y.ToString();
			zText.text = v.z.ToString();
		}
    }
}
