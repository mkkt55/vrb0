using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSavePathButton : MonoBehaviour
{
	string pathLastFrame;
	public Text path;
	void OnEnable()
	{
		path = transform.Find("ProjectSavePath").GetComponent<Text>();
		path.text = VrbSettingData.projectSavePath;
		pathLastFrame = VrbSettingData.projectSavePath;
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!path.text.Equals(pathLastFrame))
		{
			VrbSettingData.projectSavePath = path.text;
			pathLastFrame = path.text;
		}
	}
}
