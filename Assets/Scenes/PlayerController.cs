using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dpn;

public class PlayerController : MonoBehaviour
{

	private float moveSpeed;//摄像机的移动速度
	void Start()
	{
		moveSpeed = 20;
		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间
	}

	Vector3 rot = new Vector3(0, 0, 0);

	void Update()
	{
		
		//键盘鼠标控制，wsad控制前后左右，qe分别为上升下降
		if (Input.GetKey(KeyCode.W))
		{
			gameObject.transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			gameObject.transform.Translate(-Vector3.forward * Time.deltaTime * moveSpeed);
		}

		if (Input.GetKey(KeyCode.A))
		{
			gameObject.transform.Translate(-Vector3.right * Time.deltaTime * moveSpeed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		}

		if (Input.GetKey(KeyCode.Q))
		{
			gameObject.transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
		}
		if (Input.GetKey(KeyCode.E))
		{
			gameObject.transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
		}
		

		// vr control
		if (DpnDaydreamController.ClickButton)
		{
			gameObject.transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
		}
		if (DpnDaydreamController.TriggerButton)
		{
			gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		}
		if (DpnDaydreamController.BackButton)
		{
			gameObject.transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
		}
	}
	/*
	/// <summary>
	/// 鼠键控制player移动
	/// </summary>
	void WASD()
	{
		if (Input.GetMouseButton(1))
		{
			if (Input.GetAxis("Mouse X") != 0)
			{
				//Debug.Log(Input.GetAxis("Mouse X"));
				if (Input.GetAxis("Mouse X") < 0.1f && Input.GetAxis("Mouse X") > -0.1f)
				{
					// return;
				}
				this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 200, 0));//摄像机的旋转速度
																														  //clearArrow(false);
			}
			if (Input.GetAxis("Mouse Y") != 0)
			{
				if (Input.GetAxis("Mouse Y") < 0.1f && Input.GetAxis("Mouse Y") > -0.1f)
				{
					Debug.Log("返回");
					//  return;
				}
				Eye.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * -200, 0, 0));//摄像机的旋转速度
			}
		}
		
	}
	*/
}