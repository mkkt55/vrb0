using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExportSavePathButton : MonoBehaviour
{
	string pathLastFrame;
	public Text path;
	void OnEnable()
	{
		path = transform.Find("ExportSavePath").GetComponent<Text>();
		path.text = VrbSettingData.exportSavePath;
		pathLastFrame = VrbSettingData.exportSavePath;
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
	}
}
