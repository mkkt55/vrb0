using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using dpn;

public class PlayerController : MonoBehaviour
{
	private GameObject customModel;
	private List<GameObject> curModels = new List<GameObject>(); //currently selected models.

	private GameObject canvas;
	private CanvasGroup canvasGroup;
	private GameObject shouBing;

	private Text txt;

	private bool isShowingCanvas = true;

	private float moveSpeed;//摄像机的移动速度
	private float rotateSpeed;
	void Start()
	{
		moveSpeed = 20;
		rotateSpeed = 20;
		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间

		customModel = GameObject.Find("CustomModel");
		canvas = GameObject.Find("Canvas");
		canvasGroup = canvas.GetComponent<CanvasGroup>();
		shouBing = GameObject.Find("ShouBing");

		txt = GameObject.Find("DebugText").GetComponent<Text>();


		if (customModel == null || canvas == null || canvasGroup == null)
		{
			// 显然程序出错了
			Application.Quit();
		}

		curModels.Add(customModel);
		
	}
	void FixedUpdate()
	{
		// emmmm...
	}
	void Update()
	{
		Vector3 rp = DpnDaydreamController.Gyro;
		Vector3 acx = DpnDaydreamController.Accel;
		Quaternion ori = DpnDaydreamController.Orientation;

		shouBing.transform.Rotate(new Vector3(1, 0, 0));
		shouBing.transform.rotation = ori;

		txt.text = "Gyro: x = " + rp.x + ", y = " + rp.y + ", z = " + rp.z 
			+ "\nAccel: x = " + acx.x + ", y = " + acx.y + ", z = " + acx.z
			+ "\nOrientation: x = " + ori.x + ", y = " + ori.y + ", z = " + ori.z + ", w = " + ori.w;
		
		//键盘鼠标控制
		if (Input.GetKey(KeyCode.A))
		{
			customModel.transform.Translate(-Vector3.right * Time.deltaTime * moveSpeed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			customModel.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		}
		// To show and hide canvas, must use "GetKeyDown" not "GetKey".
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (isShowingCanvas)
			{
				hideCanvas();
			}
			else
			{
				showCanvas();
			}
		}
		

		// VR interaction
		// ClickButton is used to show and hide operation panel.
		if (DpnDaydreamController.ClickButtonDown)
		{
			if (isShowingCanvas)
			{
				hideCanvas();
			}
			else
			{
				showCanvas();
			}
		}
		// Select a model.
		if (DpnDaydreamController.TriggerButton)
		{
			curModels[0].transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
		}
		if (DpnDaydreamController.BackButton)
		{
			curModels[0].transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
		}

		// Rotate the models.
		if (DpnDaydreamController.TouchGestureUp)
		{
			curModels[0].transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
		}
		if (DpnDaydreamController.TouchGestureDown)
		{
			curModels[0].transform.Rotate(Vector3.down * Time.deltaTime * rotateSpeed);
		}
		if (DpnDaydreamController.TouchGestureLeft)
		{
			curModels[0].transform.Rotate(Vector3.left * Time.deltaTime * rotateSpeed);
		}
		if (DpnDaydreamController.TouchGestureRight)
		{
			curModels[0].transform.Rotate(Vector3.right * Time.deltaTime * rotateSpeed);
		}
	}

	void showCanvas()
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		isShowingCanvas = true;
	}
	void hideCanvas()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		isShowingCanvas = false;
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