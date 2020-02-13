using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using dpn;

public class PlayerController : MonoBehaviour
{
	private GameObject editableModel;
	public VrbObject selectedObject = null; // Currently selected models.
	public VrbEdge selectedEdge = null;
	public VrbVertex selectedVertex = null;
	public VrbFace selectedFace = null;

	private GameObject modeCanvas;

	private GameObject mainMenu;
	private bool isShowingMainMenu = false;

	private GameObject orientationIndicator;

	private Text txt;

	private int oMode = 0; // 当前操作模式，0为平移模式，1为旋转模式，2为伸缩模式
	private bool isEditing = false;

	private float moveSpeed;//摄像机的移动速度
	private float rotateSpeed;

	void Start()
	{
		moveSpeed = 30;
		rotateSpeed = 30;
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
		Vector3 rp = DpnDaydreamController.Gyro;
		Vector3 acx = DpnDaydreamController.Accel;
		Quaternion ori = DpnDaydreamController.Orientation;

		Vector2 tp = DpnDaydreamController.TouchPos;
		Vector2 touchVector = new Vector2((tp.x - 0.5f), (0.5f - tp.y));
		Vector3 d0 = new Vector3();
		d0.z = touchVector.y;
		d0.x = touchVector.x;
		d0.y = 0;
		Vector3 pointerVector = ori * new Vector3(0, 0, 1);
		Vector3 targetVector = ori * d0;

		txt.text = "touchPos: " + tp
			+ "\ntouchVector: " + touchVector
			+ "\ntargetVector: " + targetVector
			+ "\neditableModel: " + editableModel.transform;


		orientationIndicator.transform.rotation = ori;
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
			if (selectedVertex != null)
			{
				selectedVertex.move(Vector3.left * Time.deltaTime * moveSpeed);
			}
			if (selectedEdge != null)
			{
				selectedEdge.move(Vector3.left * Time.deltaTime * moveSpeed);
			}
			if (selectedFace != null)
			{
				selectedFace.move(Vector3.left * Time.deltaTime * moveSpeed);
			}
			if (selectedObject != null)
			{
				selectedObject.move(Vector3.left * Time.deltaTime * moveSpeed);
			}
		}
		if (Input.GetKey(KeyCode.D))
		{
			if (selectedVertex != null)
			{
				selectedVertex.move(Vector3.right * Time.deltaTime * moveSpeed);
			}
			if (selectedEdge != null)
			{
				selectedEdge.move(Vector3.right * Time.deltaTime * moveSpeed);
			}
			if (selectedFace != null)
			{
				selectedFace.move(Vector3.right * Time.deltaTime * moveSpeed);
			}
			if (selectedObject != null)
			{
				selectedObject.move(Vector3.right * Time.deltaTime * moveSpeed);
			}
		}
		



		// VR手柄操作
		// 显示隐藏界面菜单：按住TriggerButton时，按下ClickButton。
		// 更改模式操作：未按住TriggerButton时，按下ClickButton。
		if (DpnDaydreamController.TriggerButtonDown || Input.GetKeyDown(KeyCode.Q))
		{
			
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (selectedObject != null)
			{
				selectedObject.enterEdit();
			}
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			VrbObject.exitEdit();
		}

		// 平移功能,使用positon属性，直接修改世界坐标
		if (oMode == 0 && DpnDaydreamController.IsTouching)
		{
			//selectedObject.transform.position += targetVector * moveSpeed * Time.deltaTime;
		}

		// 旋转功能，绕手柄射线的延长轴旋转，触屏的左右决定的旋转方向，距离决定速度
		if (oMode == 1 && DpnDaydreamController.IsTouching)
		{
			//selectedObject.transform.Rotate(pointerVector * touchVector.x);
		}
		
		if (DpnDaydreamController.BackButtonUp)
		{
			Application.Quit();
		}
	}

	public void selectVertex(VrbVertex v)
	{
		clearSelection();
		selectedVertex = v;
		v.select();
	}

	public void selectEdge(VrbEdge e)
	{
		clearSelection();
		selectedEdge = e;
		e.select();
	}

	public void selectFace(VrbFace f)
	{
		clearSelection();
		selectedFace = f;
		f.select();
	}

	public void selectObject(VrbObject o)
	{
		clearSelection();
		selectedObject = o;
		o.select();
	}

	public void clearSelection()
	{
		if (selectedVertex != null)
		{
			selectedVertex.deSelect();
			selectedVertex = null;
		}
		if (selectedEdge != null)
		{
			selectedEdge.deSelect();
			selectedEdge = null;
		}
		if (selectedFace != null)
		{
			selectedFace.deSelect();
			selectedFace = null;
		}
		if (selectedObject != null)
		{
			selectedObject.deSelect();
			selectedObject = null;
		}
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
			if (selectedObject != null)
			{
				selectedObject.enterEdit();
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