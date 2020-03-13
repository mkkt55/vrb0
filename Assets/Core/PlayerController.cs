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
	public bool performedSomeOperation = false;
	public GameObject openProjectCanvas;
	public GameObject saveProjectCanvas;

	public GameObject exportModelCanvas;

	public bool observeMode = false;
	public GameObject editableModel;
	PointerEventData eventData;

	public VrbMeasurer measurer;
	public VrbLeftMeasurer leftMeasurer;
	public VrbRightMeasurer rightMeasurer;
	public GameObject distanceDisplayer;

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
	public GameObject matPanel;

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
	public GameObject placeButtonSubCanvas2;

	public GameObject lightButton;
	public GameObject lightButtonSubCanvas;

	public GameObject editButton;
	public GameObject editButtonSubCanvas;

	public GameObject multiSelectButton;

	public GameObject dpnCamera;
	public GameObject dpnCameraRig;

	public GameObject orientationIndicator;

	public VrbPlaceTarget placementTarget;

	public GameObject textIndicator;

	public Text txt;

	public int oMode = 0; // 当前操作模式，0为平移模式，1为旋转模式，2为伸缩模式
	public bool isEditing = false;
	public bool isPlacement = false;

	// 在这里改是没有用的，挂进unity就已经是当时的值了
	public float moveSpeed = 200; //操作物体时的移动速度
	public float moveSelfSpeed = 300; //摄像机的移动速度
	public float rotateSpeed = 60;
	public float scaleSpeed = 0.3f;

	public Color defaultButtonColor = Color.white;
	public Color selectedButtonColor = new Color(1f, 0.6f, 0.6f);

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
		moveSpeed = 200; //操作物体时的移动速度
		moveSelfSpeed = 200; //摄像机的移动速度
		rotateSpeed = 90;
		scaleSpeed = 0.1f;

		exportModelCanvas = GameObject.Find("PlayerController/CameraUI/SaveModelCanvas");
		exportModelCanvas.SetActive(false);

		eventData = new PointerEventData(EventSystem.current);
		colorPanel = GameObject.Find("PlayerController/CameraUI/DpnCameraRig/ColorCanvas");
		colorPanel.SetActive(false);

		//Cursor.visible = false;//隐藏鼠标
		//Cursor.lockState = CursorLockMode.Locked;//把鼠标锁定到屏幕中间


		openProjectCanvas = GameObject.Find("PlayerController/CameraUI/OpenProjectCanvas");
		openProjectCanvas.SetActive(false);
		saveProjectCanvas = GameObject.Find("PlayerController/CameraUI/SaveProjectCanvas");
		saveProjectCanvas.SetActive(false);

		editableModel = GameObject.Find("EditableModel");

		infoCanvas = GameObject.Find("PlayerController/CameraUI/InfoCanvas");

		objectPanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/ObjectPanel");

		scrollView = GameObject.Find("PlayerController/CameraUI/InfoCanvas/ObjectPanel/ScrollView");
		scrollContent = GameObject.Find("PlayerController/CameraUI/InfoCanvas/ObjectPanel/ScrollView/Viewport/Content");

		transformPanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/TransformPanel");
		transformPanel.SetActive(false);

		positionPanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/TransformPanel/PositionPanel");
		rotatePanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/TransformPanel/RotatePanel");
		scalePanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/TransformPanel/ScalePanel");

		lightPanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/LightPanel");
		lightPanel.SetActive(false);
		
		matPanel = GameObject.Find("PlayerController/CameraUI/InfoCanvas/MaterialPanel");
		matPanel.SetActive(false);

		mainMenu = GameObject.Find("PlayerController/CameraUI/MainMenu");

		moveButton = GameObject.Find("PlayerController/CameraUI/MainMenu/MoveButton");
		moveButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/MoveButton/SubCanvas");
		mbx = GameObject.Find("PlayerController/CameraUI/MainMenu/MoveButton/SubCanvas/X");
		mby = GameObject.Find("PlayerController/CameraUI/MainMenu/MoveButton/SubCanvas/Y");
		mbz = GameObject.Find("PlayerController/CameraUI/MainMenu/MoveButton/SubCanvas/Z");

		rotateButton = GameObject.Find("PlayerController/CameraUI/MainMenu/RotateButton");
		rotateButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/RotateButton/SubCanvas");
		rbx = GameObject.Find("PlayerController/CameraUI/MainMenu/RotateButton/SubCanvas/X");
		rby = GameObject.Find("PlayerController/CameraUI/MainMenu/RotateButton/SubCanvas/Y");
		rbz = GameObject.Find("PlayerController/CameraUI/MainMenu/RotateButton/SubCanvas/Z");

		scaleButton = GameObject.Find("PlayerController/CameraUI/MainMenu/ScaleButton");
		scaleButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/ScaleButton/SubCanvas");
		sbx = GameObject.Find("PlayerController/CameraUI/MainMenu/ScaleButton/SubCanvas/X");
		sby = GameObject.Find("PlayerController/CameraUI/MainMenu/ScaleButton/SubCanvas/Y");
		sbz = GameObject.Find("PlayerController/CameraUI/MainMenu/ScaleButton/SubCanvas/Z");

		projectButton = GameObject.Find("PlayerController/CameraUI/MainMenu/ProjectButton");
		projectButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/ProjectButton/SubCanvas");
		projectButtonSubCanvas.SetActive(false);

		settingButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/SettingCanvas");
		settingButtonSubCanvas.SetActive(false);

		placeButton = GameObject.Find("PlayerController/CameraUI/MainMenu/PlaceButton");
		placeButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/PlaceButton/SubCanvas");
		placeButtonSubCanvas2 = GameObject.Find("PlayerController/CameraUI/MainMenu/PlaceButton/SubCanvas2");
		placeButtonSubCanvas.SetActive(false);
		placeButtonSubCanvas2.SetActive(false);

		lightButton = GameObject.Find("PlayerController/CameraUI/MainMenu/LightButton");
		lightButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/LightButton/SubCanvas");
		lightButtonSubCanvas.SetActive(false);

		editButton = GameObject.Find("PlayerController/CameraUI/MainMenu/EditButton");
		editButtonSubCanvas = GameObject.Find("PlayerController/CameraUI/MainMenu/EditButton/SubCanvas");

		multiSelectButton = GameObject.Find("PlayerController/CameraUI/MainMenu/MultiSelectButton");

		dpnCamera = GameObject.Find("PlayerController/CameraUI");
		dpnCameraRig = GameObject.Find("PlayerController/CameraUI/DpnCameraRig");

		txt = GameObject.Find("DebugText").GetComponent<Text>();

		orientationIndicator = GameObject.Find("PlayerController/CameraUI/OrientationIndicator");

		placementTarget = new VrbPlaceTarget();
		placementTarget.constructModel();
		placementTarget.hideModel();

		lightPanel.GetComponent<LightPanel>().init();

		textIndicator = GameObject.Find("PlayerController/CameraUI/DpnCameraRig/TextIndicatorCanvas");
		textIndicator.SetActive(false);

		setMoveMode();
		exitEdit();
		exitMultiSelect();

		measurer = new VrbMeasurer();
		leftMeasurer = VrbMeasurer.l;
		rightMeasurer = VrbMeasurer.r;
		measurer.hideModel();

		distanceDisplayer = GameObject.Find("PlayerController/CameraUI/InfoCanvas/DistancePanel");
		distanceDisplayer.SetActive(false);

		VrbObject o = VrbModel.createCube(0, -100, 0, 100, 100, 100);
		o.displayModel();
	}

	void Update()
	{
		performedSomeOperation = false;
		
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
		
		if (observeMode)
		{
			controlCamera();
		}
		else if (true)
		{
			// VR手柄操作
			// 平移模式
			if (selected.Count > 0)
			{
				if (oMode == 0)
				{

					moveControl();
				}
				// 旋转模式，绕手柄射线的延长轴旋转，触屏的左右决定的旋转方向，距离决定速度
				else if (oMode == 1)
				{
					rotateControl();
				}
				// 缩放模式
				else if (oMode == 2)
				{
					scaleControl();
				}
			}
			// 未选中物体，控制自己
			else
			{
				controlCamera();
			}
		}

		if (DpnDaydreamController.BackButtonUp)
		{
			Application.Quit();
		}

		//saveProject();
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

	void moveControl()
	{
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

		if (DpnDaydreamController.TriggerButton && DpnDaydreamController.ClickButton)
		{
			// 假设手柄指向前方距离为1的x-y平面上，手柄射线与这个面的交点和上一帧相比，在平面上的向量移动长度。
			Vector3 dvPerDist = (orientation * Vector3.forward - orientationLastFrame * Vector3.forward);
			//dvPerDist.x = dvPerDist.x * Mathf.Sqrt(dvPerDist.x * dvPerDist.x + dvPerDist.z * dvPerDist.z) / Mathf.Abs(dvPerDist.z);
			//dvPerDist.y = dvPerDist.y * Mathf.Sqrt(dvPerDist.y * dvPerDist.y + dvPerDist.z * dvPerDist.z) / Mathf.Abs(dvPerDist.z);
			dvPerDist.z = 0;
			// 乘以z方向的距离即可
			Vector3 d = dvPerDist * (selected[0].getGameObject().transform.position - dpnCameraRig.transform.position).magnitude;
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

	void rotateControl()
	{
		if (Input.GetKey(KeyCode.A))
		{
			rotateSelected(Vector3.left);
		}
		if (Input.GetKey(KeyCode.D))
		{
			rotateSelected(Vector3.right);
		}
		if (Input.GetKey(KeyCode.W))
		{
			rotateSelected(Vector3.forward);
		}
		if (Input.GetKey(KeyCode.S))
		{
			rotateSelected(Vector3.back);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			rotateSelected(Vector3.up);
		}
		if (Input.GetKey(KeyCode.E))
		{
			rotateSelected(Vector3.down);
		}

		if (!DpnDaydreamController.TriggerButton && DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton)
		{
			if (Mathf.Abs(touchVector.y) >= Mathf.Abs(touchVector.x))
			{
				rotateSelected(touchVector.y * Vector3.right * Time.deltaTime * rotateSpeed);
			}
			else
			{
				rotateSelected(touchVector.x * Vector3.back * Time.deltaTime * rotateSpeed);
			}
		}
		else if (DpnDaydreamController.TriggerButton && DpnDaydreamController.ClickButton)
		{
			Vector3 d0 = orientation * Vector3.forward;
			//float dist = (selected[0].getGameObject().transform.position - dpnCamera.transform.position).magnitude;
			//Vector3 d = d0 * dist;
			Vector3 d = d0;
			if (Mathf.Abs(d0.x) >= Mathf.Abs(d0.y))
			{
				rotateSelected(d.x * Vector3.down * Time.deltaTime * rotateSpeed);
			}
			else
			{
				rotateSelected(d.y * Vector3.right * Time.deltaTime * rotateSpeed);
			}
		}
	}

	void scaleControl()
	{
		//键盘鼠标控制
		if (Input.GetKey(KeyCode.Q))
		{
			scaleSelected(new Vector3(1.01f, 1.01f, 1.01f));
		}
		if (Input.GetKey(KeyCode.E))
		{
			scaleSelected(new Vector3(0.99f, 0.99f, 0.99f));
		}

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


	void controlCamera()
	{
		//键盘鼠标控制
		if (Input.GetKey(KeyCode.A))
		{
			dpnCamera.transform.Translate(Vector3.left);
		}
		if (Input.GetKey(KeyCode.D))
		{
			dpnCamera.transform.Translate(Vector3.right);
		}
		if (Input.GetKey(KeyCode.W))
		{
			dpnCamera.transform.Translate(Vector3.forward);
		}
		if (Input.GetKey(KeyCode.S))
		{
			dpnCamera.transform.Translate(Vector3.back);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			dpnCamera.transform.Translate(Vector3.up);
		}
		if (Input.GetKey(KeyCode.E))
		{
			dpnCamera.transform.Translate(Vector3.down);
		}


		if (DpnDaydreamController.IsTouching && DpnDaydreamController.ClickButton)
		{
			if (Mathf.Abs(touchVector.y) < Mathf.Abs(touchVector.x))
			{
				dpnCamera.transform.Translate(touchVector.x * Time.deltaTime * moveSelfSpeed, 0, 0);
			}
			else
			{
				Vector3 vd = DpnDaydreamController.Orientation * Vector3.forward;
				if (Mathf.Abs(vd.y) > Mathf.Abs(vd.z))
				{
					dpnCamera.transform.Translate(0, 0, touchVector.y * Time.deltaTime * moveSelfSpeed);
				}
				else
				{
					dpnCamera.transform.Translate(0, touchVector.y * Time.deltaTime * moveSelfSpeed, 0);
				}
			}
		}
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
		if (selected[0].getType() == VrbTargetType.LeftMeasurer || selected[0].getType() == VrbTargetType.RightMeasurer)
		{
			_v.y = 0;
			_v.z = 0;
		}
		// 避免重复移动
		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Face)
			{
				foreach(VrbVertex v in ((VrbFace)selected[i]).fVertices)
				{
					if (vList.IndexOf(v) == -1)
					{
						vList.Add(v);
					}
				}
			}
			else if (selected[i].getType() == VrbTargetType.Edge)
			{
				if (vList.IndexOf(((VrbEdge)selected[i]).v0) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v0);
				}

				if (vList.IndexOf(((VrbEdge)selected[i]).v1) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v1);
				}
			}
			else if (selected[i].getType() == VrbTargetType.Vertex)
			{
				if (vList.IndexOf((VrbVertex)selected[i]) == -1)
				{
					vList.Add((VrbVertex)selected[i]);
				}
			}
			else
			{
				selected[i].move(_v);
			}
		}
		foreach(VrbVertex v in vList)
		{
			v.move(_v);
		}
		performedSomeOperation = true;
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

		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Face)
			{
				foreach (VrbVertex v in ((VrbFace)selected[i]).fVertices)
				{
					if (vList.IndexOf(v) == -1)
					{
						vList.Add(v);
					}
				}
			}
			else if (selected[i].getType() == VrbTargetType.Edge)
			{
				if (vList.IndexOf(((VrbEdge)selected[i]).v0) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v0);
				}

				if (vList.IndexOf(((VrbEdge)selected[i]).v1) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v1);
				}
			}
			else if (selected[i].getType() == VrbTargetType.Vertex)
			{
				if (vList.IndexOf((VrbVertex)selected[i]) == -1)
				{
					vList.Add((VrbVertex)selected[i]);
				}
			}
			else
			{
				selected[i].rotate(_v);
			}
		}

		if (vList.Count != 0)
		{

			Vector3 center = Vector3.zero;
			foreach (VrbVertex v in vList)
			{
				center += v.vector3;
			}
			center = center / vList.Count;

			foreach (VrbVertex v in vList)
			{
				v.vector3 = Quaternion.Euler(_v) * (v.vector3 - center) + center;
			}
		}

		performedSomeOperation = true;
	}

	public void scaleSelected(Vector3 _v)
	{
		if (!sbxe)
		{
			_v.x = 1;
		}
		if (!sbye)
		{
			_v.y = 1;
		}
		if (!sbze)
		{
			_v.z = 1;
		}


		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Face)
			{
				foreach (VrbVertex v in ((VrbFace)selected[i]).fVertices)
				{
					if (vList.IndexOf(v) == -1)
					{
						vList.Add(v);
					}
				}
			}
			else if (selected[i].getType() == VrbTargetType.Edge)
			{
				if (vList.IndexOf(((VrbEdge)selected[i]).v0) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v0);
				}

				if (vList.IndexOf(((VrbEdge)selected[i]).v1) == -1)
				{
					vList.Add(((VrbEdge)selected[i]).v1);
				}
			}
			else if (selected[i].getType() == VrbTargetType.Vertex)
			{
				if (vList.IndexOf((VrbVertex)selected[i]) == -1)
				{
					vList.Add((VrbVertex)selected[i]);
				}
			}
			else
			{
				selected[i].scale(_v);
			}
		}

		if (vList.Count != 0)
		{

			Vector3 center = Vector3.zero;
			foreach (VrbVertex v in vList)
			{
				center += v.vector3;
			}
			center = center / vList.Count;

			foreach (VrbVertex v in vList)
			{
				Vector3 temp = v.vector3 - center;
				temp.x *= _v.x;
				temp.y *= _v.y;
				temp.z *= _v.z;
				v.vector3 = temp + center;
			}
		}


		performedSomeOperation = true;
	}

	public void select(VrbTarget t)
	{
		// 如果在编辑模式下选中，则只能是选中了InfoCanvas上的物体。
		if (isEditing && t.getType() == VrbTargetType.Object)
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
		if (t.getType() == VrbTargetType.Object)
		{
			transformPanel.SetActive(true);
			matPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType() == VrbTargetType.Measurer)
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType() == VrbTargetType.Light)
		{
			transformPanel.SetActive(true);
			lightPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType() == VrbTargetType.Face)
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(true);
			scalePanel.SetActive(true);
		}
		else if (t.getType() == VrbTargetType.Edge)
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		else if (t.getType() == VrbTargetType.Vertex)
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		else if (t.getType() == VrbTargetType.LeftMeasurer || t.getType() == VrbTargetType.RightMeasurer)
		{
			transformPanel.SetActive(true);
			rotatePanel.SetActive(false);
			scalePanel.SetActive(false);
		}
		else if (t.getType() == VrbTargetType.PlaceTarget)
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
		matPanel.SetActive(false);
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
		matPanel.SetActive(false);
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
		isMultiSelect = true;
	}

	public void exitMultiSelect()
	{
		multiSelectButton.GetComponent<Image>().color = defaultButtonColor;
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
		if (selected.Count > 0 && selected[0].getType() == VrbTargetType.Object)
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
		exportModelCanvas.SetActive(true);
	}

	public void closeSaveModelCanvas()
	{
		exportModelCanvas.SetActive(false);
	}

	public void realSaveModel()
	{
		if (selected.Count > 0 && selected[0].getType() == VrbTargetType.Object)
		{
			VrbModel.saveModel(VrbSettingData.exportSavePath, ((VrbObject)selected[0]).mesh);
			textIndicator.GetComponent<TextIndicator>().display("Object exported to " + VrbSettingData.exportSavePath);
			exportModelCanvas.SetActive(false);
		}
	}

	public void openModel()
	{
		string filePath;
		filePath = "/vrb/test.obj";
		VrbModel.openModel(filePath);
	}

	public void placeObject(string s)
	{
		if (isEditing && s.Equals("Vertex"))
		{
			new VrbVertex(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, placementTarget.gameObject.transform.position.z).displayModel();
		}
		else if (s.Equals("Cube"))
		{
			VrbModel.createCube(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, 100, 100, 100).displayModel();
		}
		else if (s.Equals("Quad"))
		{
			VrbModel.createQuad(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, 100, 100).displayModel();
		}
		else if (s.Equals("Circle"))
		{
			VrbModel.createCircle(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, 50).displayModel();
		}
		else if (s.Equals("Sphere"))
		{
			VrbModel.createSphere(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, 50).displayModel();
		}
		else if (s.Equals("Cylinder"))
		{
			VrbModel.createCylinder(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, 50, 200).displayModel();
		}
		else if (s.Equals("Directional") || s.Equals("Point") || s.Equals("Spot"))
		{
			(new VrbLight(placementTarget.gameObject.transform.position.x, placementTarget.gameObject.transform.position.y, 0, s)).displayModel();
		}
	}

	public void switchLightButton()
	{
		if (!lightButtonSubCanvas.activeSelf)
		{
			isPlacement = true;
			placementTarget.displayModel();
			
			lightButtonSubCanvas.SetActive(true);
		}
		else
		{
			if (!placeButtonSubCanvas.activeSelf && !placeButtonSubCanvas2.activeSelf)
			{
				isPlacement = false;
				placementTarget.hideModel();
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
		if (!placeButtonSubCanvas.activeSelf && !placeButtonSubCanvas2.activeSelf)
		{
			isPlacement = true;
			placementTarget.displayModel();
			if (isEditing)
			{
				placeButtonSubCanvas2.SetActive(true);
				placeButtonSubCanvas.SetActive(false);
			}
			else
			{
				placeButtonSubCanvas.SetActive(true);
				placeButtonSubCanvas2.SetActive(false);
			}
		}
		else
		{
			if (!lightButtonSubCanvas.activeSelf)
			{
				isPlacement = false;
				placementTarget.hideModel();
			}
			placeButtonSubCanvas.SetActive(false);
			placeButtonSubCanvas2.SetActive(false);
		}
	}

	public void updatePosXfromInput(string s)
	{
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
		performedSomeOperation = true;
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
			if (selected[i].getType() == VrbTargetType.Light || selected[i].getType() == VrbTargetType.Object)
			{
				VrbModel.deleteObject((VrbObject)selected[i]);
			}
			else if (selected[i].getType() == VrbTargetType.Face)
			{
				VrbModel.deleteFace((VrbFace)selected[i]);
			}
			else if (selected[i].getType() == VrbTargetType.Vertex)
			{
				VrbModel.deleteVertex((VrbVertex)selected[i]);
			}
			else if (selected[i].getType() == VrbTargetType.Edge)
			{
				VrbModel.deleteEdge((VrbEdge)selected[i]);
			}
		}

		transformPanel.SetActive(false);
		lightPanel.SetActive(false);
		matPanel.SetActive(false);
		selected.Clear();
	}

	public void switchMeasurer()
	{
		if (measurer.gameObject.activeSelf)
		{
			measurer.hideModel();
			distanceDisplayer.SetActive(false);
		}
		else
		{
			measurer.displayModel();
			distanceDisplayer.SetActive(true);
		}
	}

	public void openToSetEnvironmentLight()
	{
		if (!settingButtonSubCanvas.activeSelf)
		{
			settingButtonSubCanvas.SetActive(true);
			transform.Find("SettingCanvas/SettingPanel").GetComponent<VrbSetting>().switchToEnvironmentPanel();
		}
		else
		{
			settingButtonSubCanvas.SetActive(false);
		}
	}

	public void saveProject()
	{
		saveProjectCanvas.SetActive(true);
	}

	public void realSaveProject()
	{
		VrbModel.saveProject(VrbSettingData.projectSavePath);
		textIndicator.GetComponent<TextIndicator>().display("Project saved to: " + VrbSettingData.projectSavePath);
		saveProjectCanvas.SetActive(false);
	}

	public void closeSaveProjectCanvas()
	{
		saveProjectCanvas.SetActive(false);
	}

	public void openProject()
	{
		openProjectCanvas.SetActive(true);
	}

	public void closeOpenProjectCanvas()
	{
		openProjectCanvas.SetActive(false);
	}

	public void newProject()
	{
		VrbModel.deleteAll();

		VrbObject o = VrbModel.createCube(0, -100, 0, 100, 100, 100);
		o.displayModel();
	}

	public void connetVertexToEdge()
	{
		performedSomeOperation = true;
		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Vertex)
			{
				vList.Add((VrbVertex)selected[i]);
			}
		}
		if (vList.Count > 1)
		{
			for (int i = 0; i < vList.Count - 1; i++)
			{
				VrbEdge.addEdge(vList[i], vList[i + 1]).displayModel();
			}
		}
		transformPanel.SetActive(false);
		clearAllSelection();
	}

	public void connetVertexToFace()
	{
		performedSomeOperation = true;
		List<VrbVertex> vList = new List<VrbVertex>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Vertex)
			{
				vList.Add((VrbVertex)selected[i]);
			}
		}
		if (vList.Count > 2)
		{
			List<VrbTriangle> tList = new List<VrbTriangle>();
			for (int i = 0; i < vList.Count - 2; i++)
			{
				tList.Add(new VrbTriangle(vList[i], vList[i + 1], vList[i + 2]));
			}
			VrbFace nf = new VrbFace(tList);
			nf.matVrbc.color = VrbObject.editingObject.vrbc.color;
			nf.displayModel();
			VrbObject.editingObject.faces.Add(nf);
		}
		transformPanel.SetActive(false);
		clearAllSelection();
	}

	public void divideEdge()
	{
		performedSomeOperation = true;
		List<VrbEdge> eList = new List<VrbEdge>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Edge)
			{
				eList.Add((VrbEdge)selected[i]);
			}
		}

		foreach (VrbEdge e in eList)
		{
			GameObject.Destroy(e.gameObject);
			VrbEdge.all.Remove(e);

			Vector3 pos = e.getPosition();
			VrbVertex nv = new VrbVertex(pos.x, pos.y, pos.z);
			nv.displayModel();
			foreach (VrbFace f in VrbFace.all)
			{
				VrbTriangle tToRemove = null;
				if (f.fEdges.IndexOf(e) != -1)
				{
					foreach (VrbTriangle t in f.ftOriginal)
					{
						if (t.e0 == e)
						{
							tToRemove = t;
							VrbTriangle t1 = new VrbTriangle(t.v0, t.v2, nv);
							VrbTriangle t2 = new VrbTriangle(t.v1, t.v2, nv);
							f.ftOriginal.Add(t1);
							f.ftOriginal.Add(t2);
							f.calSelf();
							foreach (VrbEdge ee in f.fEdges)
							{
								ee.displayModel();
							}
							break;
						}
						if (t.e1 == e)
						{
							tToRemove = t;
							VrbTriangle t1 = new VrbTriangle(t.v0, t.v2, nv);
							VrbTriangle t2 = new VrbTriangle(t.v0, t.v1, nv);
							f.ftOriginal.Add(t1);
							f.ftOriginal.Add(t2);
							f.calSelf();
							foreach (VrbEdge ee in f.fEdges)
							{
								ee.displayModel();
							}
							break;
						}
						if (t.e2 == e)
						{
							tToRemove = t;
							VrbTriangle t1 = new VrbTriangle(t.v1, t.v2, nv);
							VrbTriangle t2 = new VrbTriangle(t.v1, t.v0, nv);
							f.ftOriginal.Add(t1);
							f.ftOriginal.Add(t2);
							f.calSelf();
							foreach (VrbEdge ee in f.fEdges)
							{
								ee.displayModel();
							}
							break;
						}
					}
				}
				if (tToRemove != null)
				{
					f.ftOriginal.Remove(tToRemove);
					VrbTriangle.all.Remove(tToRemove);
				}
			}
		}
		VrbObject.editingObject.calSelf();
		transformPanel.SetActive(false);
		clearAllSelection();
	}

	public void mergeFace()
	{
		performedSomeOperation = true;
		List<VrbFace> fList = new List<VrbFace>();
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i].getType() == VrbTargetType.Face)
			{
				fList.Add((VrbFace)selected[i]);
			}
		}

		List<VrbTriangle> tList = new List<VrbTriangle>();
		foreach (VrbFace f in fList)
		{
			GameObject.Destroy(f.gameObject);
			foreach(VrbEdge e in f.fEdges)
			{
				e.hideModel();
			}

			foreach (VrbTriangle t in f.ftOriginal)
			{
				tList.Add(t);
			}

			VrbFace.all.Remove(f);
			VrbObject.editingObject.faces.Remove(f);
		}
		VrbFace nf = new VrbFace(tList);
		nf.matVrbc.color = VrbObject.editingObject.vrbc.color;
		foreach (VrbEdge ee in nf.fEdges)
		{
			ee.displayModel();
		}
		nf.displayModel();
		VrbObject.editingObject.faces.Add(nf);

		transformPanel.SetActive(false);
		clearAllSelection();
	}

	public void divideFace()
	{
		if (selected.Count > 0 && selected[0].getType() == VrbTargetType.Face)
		{
			VrbFace oldf = (VrbFace)selected[0];

			VrbFace.all.Remove(oldf);
			VrbObject.editingObject.faces.Remove(oldf);
			GameObject.Destroy(oldf.gameObject);
			
			foreach(VrbTriangle t in oldf.ftOriginal)
			{
				List<VrbTriangle> ntList = new List<VrbTriangle>();
				ntList.Add(t);

				VrbFace nf = new VrbFace(ntList);
				nf.matVrbc.color = VrbObject.editingObject.vrbc.color;
				foreach (VrbEdge ee in nf.fEdges)
				{
					ee.displayModel();
				}
				nf.displayModel();
				VrbObject.editingObject.faces.Add(nf);
			}

		}
		transformPanel.SetActive(false);
		clearAllSelection();
	}
}