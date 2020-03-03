using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VrbSettingData
{
	// Environment settings.
	public static Color elColor;
	public static float elIntensity;
	public static string skybox;

	// Project settings.
	public static string projectSavePath;
	public static string exportSavePath;

	// Render settings.
	public static string renderer;
	public static string renderSavePath;
}


public class VrbSetting : MonoBehaviour
{
	public GameObject projectSetting;
	public GameObject renderSetting;
	public GameObject environmentSetting;

	public GameObject projectPanel;
	public GameObject renderPanel;
	public GameObject environmentPanel;

	void Start()
	{
		projectSetting = transform.Find("LeftPanel/ProjectSetting").gameObject;
		renderSetting = transform.Find("LeftPanel/RenderSetting").gameObject;
		environmentSetting = transform.Find("LeftPanel/EnvironmentSetting").gameObject;

		projectPanel = transform.Find("RightPanel/ProjectPanel").gameObject;
		renderPanel = transform.Find("RightPanel/RenderPanel").gameObject;
		environmentPanel = transform.Find("RightPanel/EnvironmentPanel").gameObject;

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

	public void setEnvironmentLightColor()
	{

	}

	public void setEnvironmentLightIntensity()
	{

	}

	public void setProjectPath()
	{

	}

	public void setExportPath()
	{

	}

	public void setRenderPath()
	{

	}

	public void setSkybox()
	{

	}
}
