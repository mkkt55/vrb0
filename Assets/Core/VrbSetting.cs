using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public static class VrbSettingData
{
	public static string projectName = "Untitled";
	// Environment settings.
	public static Color elColor = Color.white;
	public static float elIntensity = 1;
	public static string skybox = "StarryNight";

	// Project settings.
	public static string projectSavePath = Application.persistentDataPath + "/" + projectName;
	public static string exportSavePath = Application.persistentDataPath + "/" + projectName + "/Object.obj";

	// Render settings.
	public static string renderer = "default";
	public static string renderSavePath =Application.persistentDataPath + "/" + projectName + "/Render/";
}


public class VrbSetting : MonoBehaviour
{
	public GameObject projectSetting;
	public GameObject renderSetting;
	public GameObject environmentSetting;

	public GameObject projectPanel;
	public GameObject renderPanel;
	public GameObject environmentPanel;

	public Dropdown skyboxDropdown;


	void Start()
	{
		//RenderSettings.ambientMode = AmbientMode.Flat;

		projectSetting = transform.Find("LeftPanel/ProjectSetting").gameObject;
		renderSetting = transform.Find("LeftPanel/RenderSetting").gameObject;
		environmentSetting = transform.Find("LeftPanel/EnvironmentSetting").gameObject;

		projectPanel = transform.Find("RightPanel/ProjectPanel").gameObject;
		
		renderPanel = transform.Find("RightPanel/RenderPanel").gameObject;
		
		environmentPanel = transform.Find("RightPanel/EnvironmentPanel").gameObject;

		skyboxDropdown = transform.Find("RightPanel/EnvironmentPanel/SkyboxDropdown").gameObject.GetComponent<Dropdown>();

		renderPanel.SetActive(false);
		environmentPanel.SetActive(false);
	}

	void Update()
	{
		
	}

	public void closeAllPanels()
	{
		projectPanel.SetActive(false);
		renderPanel.SetActive(false);
		environmentPanel.SetActive(false);
	}

	public void switchToProjectPanel()
	{
		closeAllPanels();
		projectPanel.SetActive(true);
	}

	public void switchToRenderPanel()
	{
		closeAllPanels();
		renderPanel.SetActive(true);
	}

	public void switchToEnvironmentPanel()
	{
		closeAllPanels();
		environmentPanel.SetActive(true);
	}

	public void setSkybox()
	{
		string path = "Beautify/" + skyboxDropdown.options[skyboxDropdown.value].text;
		RenderSettings.skybox = Resources.Load<Material>(path);
	}
}
