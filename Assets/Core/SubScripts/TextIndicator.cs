using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextIndicator : MonoBehaviour
{
	public Text t;
	// Start is called before the first frame update
	void Start()
    {
		t = GetComponentInChildren<Text>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void display(string s)
	{
		if (t == null)
		{
			t = GetComponentInChildren<Text>();
		}
		if (t != null)
		{
			gameObject.SetActive(true);
			t.text = s;
			Invoke("disappear", 5);
		}
	}

	public void disappear()
	{
		t.text = "";
		gameObject.SetActive(false);
	}
}
