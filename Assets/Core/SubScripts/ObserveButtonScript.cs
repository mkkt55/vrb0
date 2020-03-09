using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObserveButtonScript : MonoBehaviour, IPointerClickHandler
{
	public PlayerController pc;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!pc.observeMode)
		{
			GetComponent<Image>().color = new Color(1f, 0.6f, 0.6f);
		}
		else
		{
			GetComponent<Image>().color = Color.white;
		}
		pc.observeMode = !pc.observeMode;
	}

	// Start is called before the first frame update
	void Start()
    {
		pc = GameObject.Find("PlayerController").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
