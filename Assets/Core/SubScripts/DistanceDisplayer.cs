using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceDisplayer : MonoBehaviour
{
	public Text t;
	public Transform lt;
	public Transform rt;
	void OnEnable()
	{
		if (t == null)
		{
			t = GetComponent<Text>();
		}
		if (lt == null)
		{
			if (VrbMeasurer.l != null)
			{
				lt = VrbMeasurer.l.gameObject.transform;
			} 
		}
		if (rt == null)
		{
			if (VrbMeasurer.r != null)
			{
				rt = VrbMeasurer.r.gameObject.transform;
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
		if (rt != null && lt != null)
		{
			t.text = (lt.position - rt.position).magnitude.ToString();
		}
	}
}
