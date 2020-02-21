using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using dpn;

public class PlayerController : MonoBehaviour
{
	private GameObject editableModel;
	public VrbTarget selected;

	private GameObject modeCanvas;

	private GameObject mainMenu;
	private bool isShowingMainMenu = false;

	private GameObject orientationIndicator;

	private Text txt;

	private int oMode = 0; // 当前操作模式，0为平移模式，1为旋转模式，2为伸缩模式
	private bool isEditing = false;

	private float moveSpeed; //操作物体时的移动速度
	private float moveSelfSpeed; //摄像机的移动速度
	private float rotateSpeed;
	private float scaleSpeed;

	// 获取手柄的输入参数
	Vector3 rp;
	Vector3 acx;
	Quaternion orientation;
	Quaternion orientationLastFrame;
	Vector2 touchPos;
	Vector2 touchVector; // 手柄触摸板中心到手指触摸处的2D向量
	Vector3 pointerVector; // 手柄在3D世界中的指向
	Vector3 targetVector; // 手柄触摸板中心到手指触摸处位置在3D世界中的指向

	void Start()
	{
		moveSpeed = 30;
		moveSelfSpeed = 20;
		rotateSpeed = 30;
		scaleSpeed = 30;
		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间
		
		editableModel = GameObject.Find("EditableModel");

		modeCanvas = GameObject.Find("ModeCanvas");

		mainMenu = GameObject.Find("MainMenu");

		txt = GameObject.Find("DebugText").GetComponent<Text>();
		orientationIndicator = GameObject.Find("OrientationIndicator");

		VrbObject o = VrbModel.createCube(0, 100, 0, 200, 200, 200);
		o.displayModel();
	}

	void Update()
	{
		updateInputValue();
		
		
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

		//txt.text = "Orientation: x = " + ori.x + ", y = " + ori.y + ", z = " + ori.z + ", w = " + ori.w;


		//键盘鼠标控制
		if (Input.GetKey(KeyCode.A))
		{
			selected.move(Vector3.left);
		}
		if (Input.GetKey(KeyCode.D))
		{
			selected.move(Vector3.right);
		}
		if (Input.GetKey(KeyCode.W))
		{
			selected.move(Vector3.forward);
		}
		if (Input.GetKey(KeyCode.S))
		{
			selected.move(Vector3.back);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			selected.move(Vector3.up);
		}
		if (Input.GetKey(KeyCode.E))
		{
			selected.move(Vector3.down);
		}





		// VR手柄操作
		// 平移模式
		if (selected != null)
		{
			if (oMode == 0)
			{
				if (DpnDaydreamController.TriggerButton && !DpnDaydreamController.TriggerButtonDown && !DpnDaydreamController.TriggerButtonUp)
				{
					Vector3 v = new Vector3(0, 0, 1);
					Vector3 d = (orientation * v - orientationLastFrame * v) * selected.getGameObject().transform.position.magnitude;
					d.z = 0;
					selected.move(d * 2);
				}
				if (DpnDaydreamController.IsTouching && touchVector.y >= 0 && !DpnDaydreamController.TriggerButton)
				{
					selected.move(Vector3.forward * Time.deltaTime * moveSpeed);
				}
				if (DpnDaydreamController.IsTouching && touchVector.y < 0 && !DpnDaydreamController.TriggerButton)
				{
					selected.move(Vector3.back * Time.deltaTime * moveSpeed);
				}
			}
			// 旋转模式，绕手柄射线的延长轴旋转，触屏的左右决定的旋转方向，距离决定速度
			else if (oMode == 1)
			{
				if (DpnDaydreamController.TriggerButton && !DpnDaydreamController.TriggerButtonDown && !DpnDaydreamController.TriggerButtonUp)
				{
					Vector3 v = new Vector3(0, 0, 1);
					Vector3 d = (orientation * v - orientationLastFrame * v) * selected.getGameObject().transform.position.magnitude;
					d.z = 0;
					if (Mathf.Abs(d.y) >= Mathf.Abs(d.x))
					{
						Vector3 ra = new Vector3(0, d.y, 0);
						selected.rotate(ra * rotateSpeed);
					}
					else
					{
						Vector3 ra = new Vector3(d.x, 0, 0);
						selected.rotate(ra * rotateSpeed);
					}
				}
				if (DpnDaydreamController.TouchGestureLeft)
				{
					selected.rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
				}
				if (DpnDaydreamController.TouchGestureRight)
				{
					selected.rotate(Vector3.back * Time.deltaTime * rotateSpeed);
				}
			}
			// 缩放模式
			else if (oMode == 2)
			{
				if (DpnDaydreamController.TriggerButton && !DpnDaydreamController.TriggerButtonDown && !DpnDaydreamController.TriggerButtonUp)
				{
					Vector3 v = new Vector3(0, 0, 1);
					Vector3 d = (orientation * v - orientationLastFrame * v) * selected.getGameObject().transform.position.magnitude;
					d.z = 0;
					if (Mathf.Abs(d.y) >= Mathf.Abs(d.x))
					{
						Vector3 s = new Vector3(0, d.y, 0);
						selected.scale(d * scaleSpeed);
					}
					else
					{
						Vector3 s = new Vector3(d.x, 0, 0);
						selected.scale(d * scaleSpeed);
					}
				}
				if (DpnDaydreamController.TouchGestureUp)
				{
					selected.scale(Vector3.forward * Time.deltaTime * scaleSpeed);
				}
				if (DpnDaydreamController.TouchGestureDown)
				{
					selected.scale(Vector3.back * Time.deltaTime * scaleSpeed);
				}
			}
		}
		// 未选中物体，移动自己
		else
		{
			if (DpnDaydreamController.TriggerButton && !DpnDaydreamController.TriggerButtonDown && !DpnDaydreamController.TriggerButtonUp)
			{
				Vector3 v = new Vector3(0, 0, 1);
				Vector3 d = (orientation * v - orientationLastFrame * v) * selected.getGameObject().transform.position.magnitude;
				d.z = 0;
				if (Mathf.Abs(d.y) >= Mathf.Abs(d.x))
				{
					Vector3 ra = new Vector3(0, d.y, 0);
					gameObject.transform.Rotate(ra * rotateSpeed);
				}
				else
				{
					Vector3 ra = new Vector3(d.x, 0, 0);
					gameObject.transform.Rotate(ra * rotateSpeed);
				}
			}
			// 触摸触摸板但未按下后键，旋转
			if (DpnDaydreamController.IsTouching && !DpnDaydreamController.TriggerButton)
			{
				if (touchVector.y >= 0)
				{
					gameObject.transform.Rotate(0, 0, -Time.deltaTime * moveSelfSpeed);
				}
				else
				{
					gameObject.transform.Rotate(0, 0, Time.deltaTime * moveSelfSpeed);
				}
			}
			// 触摸触摸板，按下后键，移动
			if (DpnDaydreamController.IsTouching && DpnDaydreamController.TriggerButton)
			{
				if (touchVector.y >= 0)
				{
					gameObject.transform.Translate(0, 0, -Time.deltaTime * moveSelfSpeed);
				}
				else
				{
					gameObject.transform.Translate(0, 0, Time.deltaTime * moveSelfSpeed);
				}
			}
		}

		if (DpnDaydreamController.BackButtonUp)
		{
			Application.Quit();
		}
	}

	public void updateInputValue()
	{
		rp = DpnDaydreamController.Gyro;
		acx = DpnDaydreamController.Accel;
		orientationLastFrame = orientation;
		orientation = DpnDaydreamController.Orientation;
		touchPos = DpnDaydreamController.TouchPos;
		if (!touchPos.Equals(Vector2.zero))
		{
			touchVector = new Vector2((touchPos.x - 0.5f), (0.5f - touchPos.y));
		}
		else
		{
			touchVector = Vector2.zero;
		}
		Vector3 d0 = new Vector3();
		d0.z = touchVector.y;
		d0.x = touchVector.x;
		d0.y = 0;
		pointerVector = orientation * new Vector3(0, 0, 1);
		targetVector = orientation * d0;

		txt.text = "touchPos: " + touchPos
			+ "\ntouchVector: " + touchVector
			+ "\ntargetVector: " + targetVector
			+ "\neditableModel: " + editableModel.transform;


		orientationIndicator.transform.rotation = orientation;
	}

	public void moveCamera(Vector3 direction)
	{
		gameObject.transform.position += direction * Time.deltaTime * moveSpeed;
	}


	public void select(VrbTarget t)
	{
		clearSelection();
		selected = t;
		t.select();
	}

	public void clearSelection()
	{
		if (selected != null)
		{
			selected.deSelect();
		}
		selected = null;
	}

	// 对应模式面板的四个模式，一个Cancel按钮
	public void setMoveMode()
	{
		oMode = 0;
	}

	public void setRotateMode()
	{
		oMode = 1;
	}

	public void setScaleMode()
	{
		oMode = 2;
	}

	public void switchEditMode()
	{
		if (!isEditing)
		{
			if (selected != null && selected.getType() == "object")
			{
				((VrbObject)selected).enterEdit();
			}
		}
		else
		{
			VrbObject.exitEdit();
		}
		isEditing = !isEditing;
	}

	public void saveModel()
	{
		VrbModel.saveModel("/");
	}

	public void openModel()
	{
		VrbModel.openModel("/");
	}

	public void placeObject()
	{

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