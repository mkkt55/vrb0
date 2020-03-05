using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TalesFromTheRift;
using dpn;

public class PlayerController : MonoBehaviour
{
	public GameObject editableModel;
	PointerEventData eventData;

	public List<VrbTarget> selected = new List<VrbTarget>();
	public bool isMultiSelect = false;

	public static GameObject colorPanel;

	public GameObject objectPanel;
	public GameObject scrollView;
	public GameObject scrollContent;

	public GameObject infoCanvas;
	public GameObject transformPanel;
	public GameObject positionPanel;
	public GameObject rotatePanel;
	public GameObject scalePanel;
	public GameObject lightPanel;

	public GameObject mainMenu;

	public GameObject moveButton;
	public GameObject moveButtonSubCanvas;
	public GameObject mbx;
	public GameObject mby;
	public GameObject mbz;
	public bool mbxe = true;
	public bool mbye = true;
	public bool mbze = true;

	public GameObject rotateButton;
	public GameObject rotateButtonSubCanvas;
	public GameObject rbx;
	public GameObject rby;
	public GameObject rbz;
	public bool rbxe = true;
	public bool rbye = true;
	public bool rbze = true;

	public GameObject scaleButton;
	public GameObject scaleButtonSubCanvas;
	public GameObject sbx;
	public GameObject sby;
	public GameObject sbz;
	public bool sbxe = true;
	public bool sbye = true;
	public bool sbze = true;


	public Color disableColor = Color.black;

	public GameObject projectButton;
	public GameObject projectButtonSubCanvas;

	public GameObject settingButtonSubCanvas;

	public GameObject placeButton;
	public GameObject placeButtonSubCanvas;

	public GameObject lightButton;
	public GameObject lightButtonSubCanvas;

	public GameObject editButton;
	public GameObject editButtonSubCanvas;

	public GameObject multiSelectButton;
	public GameObject multiSelectButtonSubCanvas;

	public GameObject dpnCamera;

	public GameObject orientationIndicator;

	public GameObject placementTarget;

	public GameObject textIndicator;

	public Text txt;

	public int oMode = 0; // 当前操作模式，0为平移模式，1为旋转模式，2为伸缩模式
	public bool isEditing = false;
	public bool isPlacement = false;

	public float moveSpeed; //操作物体时的移动速度
	public float moveSelfSpeed; //摄像机的移动速度
	public float rotateSelfSpeed;
	public float rotateSpeed;
	public float scaleSpeed;

	public Color defaultButtonColor = Color.white;
	public Color selectedButtonColor = Color.red;

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
		eventData = new PointerEventData(EventSystem.current);
		colorPanel = GameObject.Find("PlayerController/DpnCameraRig/ColorCanvas/ColorPanel");
		colorPanel.SetActive(false);

		moveSpeed = 30;
		moveSelfSpeed = 20;
		rotateSelfSpeed = 30;
		rotateSpeed = 20;
		scaleSpeed = 0.1f;
		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间
		
		editableModel = GameObject.Find("EditableModel");

		infoCanvas = GameObject.Find("PlayerController/InfoCanvas");

		objectPanel = GameObject.Find("PlayerController/InfoCanvas/ObjectPanel");

		scrollView = GameObject.Find("PlayerController/InfoCanvas/ObjectPanel/ScrollView");
		scrollContent = GameObject.Find("PlayerController/InfoCanvas/ObjectPanel/ScrollView/Viewport/Content");

		transformPanel = GameObject.Find("PlayerController/InfoCanvas/TransformPanel");
		transformPanel.SetActive(false);

		positionPanel = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/PositionPanel");
		rotatePanel = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/RotatePanel");
		scalePanel = GameObject.Find("PlayerController/InfoCanvas/TransformPanel/ScalePanel");

		lightPanel = GameObject.Find("PlayerController/InfoCanvas/LightPanel");
		lightPanel.SetActive(false);

		mainMenu = GameObject.Find("MainMenu");

		moveButton = GameObject.Find("MainMenu/MoveButton");
		moveButtonSubCanvas = GameObject.Find("MainMenu/MoveButton/SubCanvas");
		mbx = GameObject.Find("MainMenu/MoveButton/SubCanvas/X");
		mby = GameObject.Find("MainMenu/MoveButton/SubCanvas/Y");
		mbz = GameObject.Find("MainMenu/MoveButton/SubCanvas/Z");

		rotateButton = GameObject.Find("MainMenu/RotateButton");
		rotateButtonSubCanvas = GameObject.Find("MainMenu/RotateButton/SubCanvas");
		rbx = GameObject.Find("MainMenu/RotateButton/SubCanvas/X");
		rby = GameObject.Find("MainMenu/RotateButton/SubCanvas/Y");
		rbz = GameObject.Find("MainMenu/RotateButton/SubCanvas/Z");

		scaleButton = GameObject.Find("MainMenu/ScaleButton");
		scaleButtonSubCanvas = GameObject.Find("MainMenu/ScaleButton/SubCanvas");
		sbx = GameObject.Find("MainMenu/ScaleButton/SubCanvas/X");
		sby = GameObject.Find("MainMenu/ScaleButton/SubCanvas/Y");
		sbz = GameObject.Find("MainMenu/ScaleButton/SubCanvas/Z");

		projectButton = GameObject.Find("MainMenu/ProjectButton");
		projectButtonSubCanvas = GameObject.Find("MainMenu/ProjectButton/SubCanvas");
		projectButtonSubCanvas.SetActive(false);

		settingButtonSubCanvas = GameObject.Find("PlayerController/SettingCanvas");
		settingButtonSubCanvas.SetActive(false);

		placeButton = GameObject.Find("MainMenu/PlaceButton");
		placeButtonSubCanvas = GameObject.Find("MainMenu/PlaceButton/SubCanvas");
		placeButtonSubCanvas.SetActive(false);

		lightButton = GameObject.Find("MainMenu/LightButton");
		lightButtonSubCanvas = GameObject.Find("MainMenu/LightButton/SubCanvas");
		lightButtonSubCanvas.SetActive(false);

		editButton = GameObject.Find("MainMenu/EditButton");
		editButtonSubCanvas = GameObject.Find("MainMenu/EditButton/SubCanvas");

		multiSelectButton = GameObject.Find("MainMenu/MultiSelectButton");
		multiSelectButtonSubCanvas = GameObject.Find("MainMenu/MultiSelectButton/SubCanvas");

		dpnCamera = GameObject.Find("PlayerController/DpnCameraRig");

		txt = GameObject.Find("DebugText").GetComponent<Text>();

		orientationIndicator = GameObject.Find("OrientationIndicator");

		placementTarget = GameObject.Find("PlacementTarget");
		placementTarget.SetActive(false);

		lightPanel.GetComponent<LightPanel>().init();

		textIndicator = GameObject.Find("PlayerController/DpnCameraRig/Canvas/Panel");
		textIndicator.SetActive(false);

		setMoveMode();
		exitEdit();
		exitMultiSelect();

		
		VrbObject o = VrbModel.createCube(0, -60, 0, 100, 100, 100);
		o.displayModel();
	}

	void Update()
	{
		updateInputValue();
		
		//RaycastHit hit;
		//Physics.Raycast(new Vector3(0, -20, -400), dpnCamera.transform.rotation * Vector3.forward, out hit);

		//#if UNITY_ANDROID || UNITY_IPHONE
		//if (EventSystem.current.IsPointerOverGameObject())
		//#else
		//if (EventSystem.current.IsPointerOverGameObject())
		//#endif
		//	Debug.Log("当前触摸在UI上");
		//else
		//	Debug.Log("当前没有触摸在UI上");
		//if (hit.collider != null)
		//{
		//	Debug.LogWarning(hit.point);
		//	Debug.LogWarning(hit.collider.gameObject.name);
		//	Debug.LogWarning(dpnCamera.transform.rotation* Vector3.forward);
		//	Debug.DrawRay(new Vector3(0, -20, -400), dpnCamera.transform.rotation * Vector3.forward, Color.red);
		//}

		//if (Input.GetKeyDown(KeyCode.G))
		//{
		//	ExecuteEvents.Execute<IPointerClickHandler>(hit.collider.gameObject, eventData, ExecuteEvents.pointerClickHandler);
		//}

		//txt.text = "Orientation: x = " + ori.x + ", y = " + ori.y + ", z = " + ori.z + ", w = " + ori.w;


		//键盘鼠标控制
		if (Input.GetKey(KeyCode.A))
		{
			moveSelected(Vector3.left);
		}
		if (Input.GetKey(KeyCode.D))
		{
			moveSelected(Vector3.right);
		}
		if (Input.GetKey(KeyCode.W))
		{
			moveSelected(Vector3.forward);
		}
		if (Input.GetKey(KeyCode.S))
		{
			moveSelected(Vector3.back);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			moveSelected(Vector3.up);
		}
		if (Input.GetKey(KeyCode.E))
		{
			moveSelected(Vector3.down);
		}



		if (isPlacement)
		{
			if (DpnDaydreamController.TriggerButton)
			{
				// 假设手柄指向前方距离为1的x-y平面上，手柄与平面的交点
				Vector3 pointOnXyPlane = orientation * Vector3.forward * (selected[0].getGameObject().transform.position - dpnCamera.transform.position).magnitude;
				pointOnXyPlane.x = pointOnXyPlane.x * Mathf.Sqrt(pointOnXyPlane.x * pointOnXyPlane.x + pointOnXyPlane.z * pointOnXyPlane.z) / Mathf.Abs(pointOnXyPlane.z);
				pointOnXyPlane.y = pointOnXyPlane.y * Mathf.Sqrt(pointOnXyPlane.y * pointOnXyPlane.y + pointOnXyPlane.z * pointOnXyPlane.z) / Mathf.Abs(pointOnXyPlane.z);
				pointOnXyPlane.z = 0;
				placementTarget.transform.position = pointOnXyPlane;
			}
		}
		else if (!OpenCanvasKeyboard.isOpening)
		{
			// VR手柄操作
			// 平移模式
			if (selected.Count > 0)
			{
				if (oMode == 0)
				{
					if (DpnDaydreamController.TriggerButton && !DpnDaydreamController.TriggerButtonDown && !DpnDaydreamController.TriggerButtonUp)
					{
						// 假设手柄指向前方距离为1的x-y平面上，手柄射线与这个面的交点和上一帧相比，在平面上的向量移动长度。
						Vector3 dvPerDist = (orientation * Vector3.forward - orientationLastFrame * Vector3.forward);
						dvPerDist.x = dvPerDist.x * Mathf.Sqrt(dvPerDist.x * dvPerDist.x + dvPerDist.z * dvPerDist.z) / Mathf.Abs(dvPerDist.z);
						dvPerDist.y = dvPerDist.y * Mathf.Sqrt(dvPerDist.y * dvPerDist.y + dvPerDist.z * dvPerDist.z) / Mathf.Abs(dvPerDist.z);
						dvPerDist.z = 0;
						// 乘以z方向的距离即可
						Vector3 d = dvPerDist * (selected[0].getGameObject().transform.position - dpnCamera.transform.position).magnitude;
						moveSelected(d);
					}
					if (DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton && !DpnDaydreamController.TriggerButton)
					{
						if (touchVector.y >= 0)
						{
							moveSelected(Vector3.forward * Time.deltaTime * moveSpeed);
						}
						else
						{
							moveSelected(Vector3.back * Time.deltaTime * moveSpeed);
						}
					}
				}
				// 旋转模式，绕手柄射线的延长轴旋转，触屏的左右决定的旋转方向，距离决定速度
				else if (oMode == 1)
				{
					if (DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton)
					{
						if (Mathf.Abs(touchVector.y) >= Mathf.Abs(touchVector.x))
						{
							rotateSelected(touchVector.y * Vector3.right * Time.deltaTime * rotateSpeed);
						}
						else
						{
							rotateSelected(touchVector.x * Vector3.down * Time.deltaTime * rotateSpeed);
						}
					}
				}
				// 缩放模式
				else if (oMode == 2)
				{
					if (DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton)
					{
						if (touchVector.x < 0)
						{
							scaleSelected(Vector3.one * (1 - Time.deltaTime * scaleSpeed));
						}
						else
						{
							scaleSelected(Vector3.one * (1 + Time.deltaTime * scaleSpeed));
						}
					}
				}
			}
			// 未选中物体，控制自己
			else
			{
				if (DpnDaydreamController.TriggerButton && DpnDaydreamController.IsTouching)
				{
					if (touchVector.y > 0)
					{
						dpnCamera.transform.Translate(0, 0, Time.deltaTime * moveSelfSpeed);
					}
					else
					{
						dpnCamera.transform.Translate(0, 0, -Time.deltaTime * moveSelfSpeed);
					}
				}
				if (DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton)
				{
					if (Mathf.Abs(touchVector.y) < Mathf.Abs(touchVector.x))
					{
						gameObject.transform.Rotate(0, -touchVector.x * Time.deltaTime * rotateSelfSpeed, 0);
					}
					else
					{
						gameObject.transform.Rotate(touchVector.y * Time.deltaTime * rotateSelfSpeed, 0, 0);
					}
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

		orientationIndicator.transform.rotation = orientation;
	}

	public void moveSelected(Vector3 _v)
	{
		if (!mbxe)
		{
			_v.x = 0;
		}
		if (!mbye)
		{
			_v.y = 0;
		}
		if (!mbze)
		{
			_v.z = 0;
		}
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].move(_v);
		}
	}

	public void rotateSelected(Vector3 _v)
	{
		if (!rbxe)
		{
			_v.x = 0;
		}
		if (!rbye)
		{
			_v.y = 0;
		}
		if (!rbze)
		{
			_v.z = 0;
		}
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].rotate(_v);
			selected[i].rotate(_v);
		}
	}

	public void scaleSelected(Vector3 _v)
	{
		if (!sbxe)
		{
			_v.x = 0;
		}
		if (!sbye)
		{
			_v.y = 0;
		}
		if (!sbze)
		{
			_v.z = 0;
		}
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].scale(_v);
		}
	}

	public void select(VrbTarget t)
	{
		// 如果在编辑模式下选中，则只能是选中了InfoCanvas上的物体。
		if (isEditing && t.getType().Equals("object"))
		{
			return;
		}
		if (isMultiSelect)
		{
			selected.Add(t);
		}
		else
		{
			clearAllSelection();
			selected.Add(t);
		}
		if (t.getType().Equals("object"))
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType().Equals("light"))
		{
			transformPanel.SetActive(true);
			lightPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType().Equals("face"))
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		else if (t.getType().Equals("edge"))
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		else if (t.getType().Equals("vertex"))
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		t.select();
	}

	public void clearSingleSelection()
	{
		transformPanel.SetActive(false);
		lightPanel.SetActive(false);
		if (!isMultiSelect && selected.Count > 0)
		{
			selected[0].deSelect();
			selected.Clear();
		}
	}

	public void clearAllSelection()
	{
		transformPanel.SetActive(false);
		lightPanel.SetActive(false);
		for (int i = 0; i < selected.Count; i++)
		{
			selected[i].deSelect();
		}
		selected.Clear();
	}

	public void switchMultiSelect()
	{
		if (!isMultiSelect)
		{
			enterMultiSelect();
		}
		else
		{
			exitMultiSelect();
		}
	}

	public void enterMultiSelect()
	{
		multiSelectButton.GetComponent<Image>().color = selectedButtonColor;
		multiSelectButtonSubCanvas.SetActive(true);
		isMultiSelect = true;
	}

	public void exitMultiSelect()
	{
		multiSelectButton.GetComponent<Image>().color = defaultButtonColor;
		multiSelectButtonSubCanvas.SetActive(false);
		isMultiSelect = false;
	}

	// 对应模式面板的四个模式，一个Cancel按钮
	public void setMoveMode()
	{
		clearMRSButton();
		moveButton.GetComponent<Image>().color = selectedButtonColor;
		moveButtonSubCanvas.SetActive(true);
		oMode = 0;
	}

	public void setRotateMode()
	{
		clearMRSButton();
		rotateButton.GetComponent<Image>().color = selectedButtonColor;
		rotateButtonSubCanvas.SetActive(true);
		oMode = 1;
	}

	public void setScaleMode()
	{
		clearMRSButton();
		scaleButton.GetComponent<Image>().color = selectedButtonColor;
		scaleButtonSubCanvas.SetActive(true);
		oMode = 2;
	}

	public void switchMBX()
	{
		if (!mbxe)
		{
			mbx.GetComponent<Image>().color = defaultButtonColor;
			mbxe = true;
		}
		else
		{
			mbx.GetComponent<Image>().color = disableColor;
			mbxe = false;
		}
	}
	public void switchMBY()
	{
		if (!mbye)
		{
			mby.GetComponent<Image>().color = defaultButtonColor;
			mbye = true;
		}
		else
		{
			mby.GetComponent<Image>().color = disableColor;
			mbye = false;
		}
	}
	public void switchMBZ()
	{
		if (!mbze)
		{
			mbz.GetComponent<Image>().color = defaultButtonColor;
			mbze = true;
		}
		else
		{
			mbz.GetComponent<Image>().color = disableColor;
			mbze = false;
		}
	}

	public void switchRBX()
	{
		if (!rbxe)
		{
			rbx.GetComponent<Image>().color = defaultButtonColor;
			rbxe = true;
		}
		else
		{
			rbx.GetComponent<Image>().color = disableColor;
			rbxe = false;
		}
	}
	public void switchRBY()
	{
		if (!rbye)
		{
			rby.GetComponent<Image>().color = defaultButtonColor;
			rbye = true;
		}
		else
		{
			rby.GetComponent<Image>().color = disableColor;
			rbye = false;
		}
	}
	public void switchRBZ()
	{
		if (!rbze)
		{
			rbz.GetComponent<Image>().color = defaultButtonColor;
			rbze = true;
		}
		else
		{
			rbz.GetComponent<Image>().color = disableColor;
			rbze = false;
		}
	}

	public void switchSBX()
	{
		if (!sbxe)
		{
			sbx.GetComponent<Image>().color = defaultButtonColor;
			sbxe = true;
		}
		else
		{
			sbx.GetComponent<Image>().color = disableColor;
			sbxe = false;
		}
	}
	public void switchSBY()
	{
		if (!sbye)
		{
			sby.GetComponent<Image>().color = defaultButtonColor;
			sbye = true;
		}
		else
		{
			sby.GetComponent<Image>().color = disableColor;
			sbye = false;
		}
	}
	public void switchSBZ()
	{
		if (!sbze)
		{
			sbz.GetComponent<Image>().color = defaultButtonColor;
			sbze = true;
		}
		else
		{
			sbz.GetComponent<Image>().color = disableColor;
			sbze = false;
		}
	}

	public void clearMRSButton()
	{
		moveButton.GetComponent<Image>().color = defaultButtonColor;
		moveButtonSubCanvas.SetActive(false);
		rotateButton.GetComponent<Image>().color = defaultButtonColor;
		rotateButtonSubCanvas.SetActive(false);
		scaleButton.GetComponent<Image>().color = defaultButtonColor;
		scaleButtonSubCanvas.SetActive(false);
	}

	public void switchEditMode()
	{
		if (!isEditing)
		{
			enterEdit();
		}
		else
		{
			exitEdit();
		}
	}

	public void enterEdit()
	{
		if (selected.Count > 0 && selected[0].getType() == "object")
		{
			editButton.GetComponent<Image>().color = selectedButtonColor;
			editButtonSubCanvas.SetActive(true);
			((VrbObject)selected[0]).enterEdit();
			isEditing = true;
		}
	}

	public void exitEdit()
	{
		editButton.GetComponent<Image>().color = defaultButtonColor;
		editButtonSubCanvas.SetActive(false);
		VrbObject.exitEdit();
		isEditing = false;
	}

	public void saveModel()
	{
		string filePath;
		filePath = "d:/zz.obj";
		VrbModel.saveModel(filePath);
		textIndicator.GetComponent<TextIndicator>().display("Object exported to " + filePath);
	}

	public void openModel()
	{
		string filePath;
		filePath = "/vrb/zz.obj";
		VrbModel.openModel(filePath);
	}

	public void placeObject(string s)
	{
		if (s.Equals("Cube"))
		{
			VrbModel.createCube(placementTarget.transform.position.x, placementTarget.transform.position.y, 0, 100, 100, 100).displayModel();
		}

		if (s.Equals("Directional") || s.Equals("Point") || s.Equals("Spot"))
		{
			(new VrbLight(placementTarget.transform.position.x, placementTarget.transform.position.y, 0, s)).displayModel();
		}
	}

	public void switchLightButton()
	{
		if (!lightButtonSubCanvas.activeSelf)
		{
			isPlacement = true;
			placementTarget.SetActive(true);
			lightButtonSubCanvas.SetActive(true);
		}
		else
		{
			if (!placeButtonSubCanvas.activeSelf)
			{
				isPlacement = false;
				placementTarget.SetActive(false);
			}
			lightButtonSubCanvas.SetActive(false);
		}
	}

	public void switchProjectSubCanvas()
	{
		if (!projectButtonSubCanvas.activeSelf)
		{
			projectButtonSubCanvas.SetActive(true);
		}
		else
		{
			projectButtonSubCanvas.SetActive(false);
		}
	}

	public void switchSettingSubCanvas()
	{
		if (!settingButtonSubCanvas.activeSelf)
		{
			settingButtonSubCanvas.SetActive(true);
		}
		else
		{
			settingButtonSubCanvas.SetActive(false);
		}
	}

	public void switchPlacement()
	{
		if (!placeButtonSubCanvas.activeSelf)
		{
			isPlacement = true;
			placementTarget.SetActive(true);
			placeButtonSubCanvas.SetActive(true);
		}
		else
		{
			if (!lightButtonSubCanvas.activeSelf)
			{
				isPlacement = false;
				placementTarget.SetActive(false);
			}
			placeButtonSubCanvas.SetActive(false);
		}
	}

	public void updatePosXfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getPosition();
			t.x = n - t.x;
			t.y = 0;
			t.z = 0;
			selected[0].move(t);
		}
	}

	public void updatePosYfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			//Debug.LogWarning("now move, " + s + "->" + selected[0].getPosition());
			float n = float.Parse(s);
			Vector3 t = selected[0].getPosition();
			t.x = 0;
			t.y = n - t.y;
			t.z = 0;
			selected[0].move(t);
		}
	}

	public void updatePosZfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getPosition();
			t.x = 0;
			t.y = 0;
			t.z = n - t.z;
			selected[0].move(t);
		}
	}

	public void updateRottXfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getRotate();
			t.x = n - t.x;
			t.y = 0;
			t.z = 0;
			selected[0].rotate(t);
		}
	}

	public void updateRottYfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getRotate();
			t.x = 0;
			t.y = n - t.y;
			t.z = 0;
			selected[0].rotate(t);
		}
	}

	public void updateRottZfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getRotate();
			t.x = 0;
			t.y = 0;
			t.z = n - t.z;
			selected[0].rotate(t);
		}
	}

	public void updateScaXfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getScale();
			t.x = n / t.x;
			t.y = 1;
			t.z = 1;
			selected[0].scale(t);
		}
	}

	public void updateScaYfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getScale();
			t.x = 1;
			t.y = n / t.y;
			t.z = 1;
			selected[0].scale(t);
		}
	}

	public void updateScaZfromInput(string s)
	{
		if (s.Equals("") || s.Equals("-"))
		{
			return;
		}
		if (selected.Count > 0)
		{
			float n = float.Parse(s);
			Vector3 t = selected[0].getScale();
			t.x = 1;
			t.y = 1;
			t.z = n / t.z;
			selected[0].scale(t);
		}
	}
	
	public void deleteSelection()
	{
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType().Equals("light") || selected[i].getType().Equals("object"))
			{
				VrbModel.deleteObject((VrbObject)selected[i]);
			}
			else if (selected[i].getType().Equals("face"))
			{
				VrbModel.deleteFace((VrbFace)selected[i]);
			}
			else if (selected[i].getType().Equals("vertex"))
			{
				VrbModel.deleteVertex((VrbVertex)selected[i]);
			}
			else if (selected[i].getType().Equals("edge"))
			{
				VrbModel.deleteEdge((VrbEdge)selected[i]);
			}
		}

		transformPanel.SetActive(false);
		lightPanel.SetActive(false);
		selected.Clear();
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