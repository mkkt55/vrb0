using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderSavePathButton : MonoBehaviour
{
	string pathLastFrame;
	public Text path;
	void OnEnable()
	{
		path = transform.Find("RenderSavePath").GetComponent<Text>();
		path.text = VrbSettingData.renderSavePath;
		pathLastFrame = VrbSettingData.renderSavePath;
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
			pathLastFrame = path.text;
		}
    }
}
