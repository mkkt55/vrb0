using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OpenProjectCanvasScript : MonoBehaviour
{
	public GameObject content;
	void OnEnable()
	{
		if (content == null)
		{
			content = transform.Find("OpenProjectPanel/ScrollView/Viewport/Content").gameObject;
			
		}

		for (int i = 0; i < content.transform.childCount; i++)
		{
			GameObject.Destroy(content.transform.GetChild(i).gameObject);
		}

		string fullPath = Application.persistentDataPath;  //路径

		//获取指定路径下面的所有资源文件  
		if (Directory.Exists(fullPath))
		{
			Debug.LogWarning(fullPath);
			DirectoryInfo dir = new DirectoryInfo(fullPath);
			DirectoryInfo[] projects = dir.GetDirectories();

			for (int i = 0; i < projects.Length; i++)
			{
				Debug.LogWarning("Name:" + projects[i].Name);

				GameObject g = Resources.Load<GameObject>("ProjectItem");
				GameObject rg = GameObject.Instantiate(g, content.transform);
				rg.GetComponent<ProjectItem>().dir = projects[i].Name;
			}
		}
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
