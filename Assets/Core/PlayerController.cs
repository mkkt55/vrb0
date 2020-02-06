using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using dpn;

public class PlayerController : MonoBehaviour
{
	private GameObject customModel;
	private GameObject selectedModel; // Currently selected models.

	private GameObject modeCanvas;
	private CanvasGroup modeCanvasGroup;
	private bool isShowingModeCanvas = true;

	private GameObject mainMenu;
	private CanvasGroup mainMenuGroup;
	private bool isShowingMainMenu = true;

	private Text txt;

	private int mode = 0; // 当前模式，0为平移模式，1为旋转模式，2为编辑模式

	private float moveSpeed;//摄像机的移动速度
	private float rotateSpeed;
	void Start()
	{
		moveSpeed = 20;
		rotateSpeed = 10;
		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间

		customModel = GameObject.Find("CustomModel");
		selectedModel = customModel;

		modeCanvas = GameObject.Find("ModeCanvas");
		modeCanvasGroup = modeCanvas.GetComponent<CanvasGroup>();
		hideModeCanvas();

		mainMenu = GameObject.Find("MainMenu");
		mainMenuGroup = modeCanvas.GetComponent<CanvasGroup>();
		txt = GameObject.Find("DebugText").GetComponent<Text>();

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
		Vector2 tp = DpnDaydreamController.TouchPos;
		
		
		/*
		 * 自己创建手柄交互缺少设备SDK提供的API
		shouBing.transform.rotation = ori;
		RaycastHit hit;
		Physics.Raycast(new Vector3(0, 100, -200), new Vector3(0, 0, 1), out hit);
		
		if (hit.collider != null)
		{
			Debug.Log(hit.point);
			Debug.DrawRay(new Vector3(0, 100, -200), hit.point, Color.red);
		}
		*/

		txt.text = "Gyro: x = " + rp.x + ", y = " + rp.y + ", z = " + rp.z
			+ "\nAccel: x = " + acx.x + ", y = " + acx.y + ", z = " + acx.z
			+ "\nOrientation: x = " + ori.x + ", y = " + ori.y + ", z = " + ori.z + ", w = " + ori.w
			+ "\nTouchPos: x = " + tp.x + ", y = " + tp.y;

		//键盘鼠标控制
		if (Input.GetKey(KeyCode.A))
		{
			customModel.transform.Translate(-Vector3.right * Time.deltaTime * moveSpeed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			customModel.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		}
		
		
		
		
		// VR手柄操作
		// 显示隐藏界面菜单：按住TriggerButton时，按下ClickButton。
		// 更改模式操作：未按住TriggerButton时，按下ClickButton。
		if (DpnDaydreamController.ClickButtonDown || Input.GetKeyDown(KeyCode.Q))
		{
			if (!isShowingModeCanvas)
			{
				showModeCanvas();
			}
		}

		// 平移功能：根据TouchPos直接平移
		if (mode == 0 && DpnDaydreamController.IsTouching)
		{
			customModel.transform.Translate(new Vector3((tp.x - 0.5f) * Time.deltaTime * moveSpeed, (tp.y - 0.5f) * Time.deltaTime * moveSpeed, 0));
		}

		// 旋转功能：按住TriggerButton
		if (mode == 1 && DpnDaydreamController.TriggerButton)
		{
			customModel.transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
		}



		if (DpnDaydreamController.BackButtonUp)
		{
			Application.Quit();
		}
		
	}

	public void select(GameObject gb)
	{
		selectedModel = gb;
	}

	void showModeCanvas()
	{
		modeCanvasGroup.alpha = 1;
		modeCanvasGroup.interactable = true;
		modeCanvasGroup.blocksRaycasts = true;
		isShowingModeCanvas = true;
	}
	void hideModeCanvas()
	{
		modeCanvasGroup.alpha = 0;
		modeCanvasGroup.interactable = false;
		modeCanvasGroup.blocksRaycasts = false;
		isShowingModeCanvas = false;
	}

	// 对应模式面板的四个模式，一个Cancel按钮
	public void setMoveMode()
	{
		mode = 0;
		hideModeCanvas();
	}

	public void setRotateMode()
	{
		mode = 1;
		hideModeCanvas();
	}

	public void setEditMode()
	{
		mode = 2;
		hideModeCanvas();
	}

	public void setMainMenu()
	{
		if (isShowingMainMenu)
		{
			mainMenuGroup.alpha = 0;
			mainMenuGroup.interactable = false;
			mainMenuGroup.blocksRaycasts = false;
			isShowingMainMenu = false;
		}
		else
		{
			mainMenuGroup.alpha = 1;
			mainMenuGroup.interactable = true;
			mainMenuGroup.blocksRaycasts = true;
			isShowingMainMenu = true;
		}
		hideModeCanvas();
	}

	public void cancelSetMode()
	{
		hideModeCanvas();
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