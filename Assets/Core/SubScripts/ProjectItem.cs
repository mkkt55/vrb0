using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProjectItem : MonoBehaviour, IPointerClickHandler
{
	public string dir;
	public Text t;
	public void OnPointerClick(PointerEventData eventData)
	{
		VrbModel.openProject(Application.persistentDataPath + "/" + dir);
		GameObject.Find("PlayerController/OpenProjectCanvas").SetActive(false);
	}

	void OnEnable()
	{
		t = GetComponent<Text>();
	}

	// Start is called before the first frame update
	void Start()
	{
		t.text = "  " + dir;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
