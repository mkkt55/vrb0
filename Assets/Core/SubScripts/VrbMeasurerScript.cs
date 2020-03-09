using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbMeasurerScript : MonoBehaviour
{
	public Transform left;
	public Transform right;
	public Transform center;
    // Start is called before the first frame update
    void Start()
	{
		left = transform.Find("Left/Measurer-side/default/PosMarker");
		right = transform.Find("Right/Measurer-side/default/PosMarker");
		center = transform.Find("length");
	}

    // Update is called once per frame
    void Update()
    {
		float length = (left.position - right.position).magnitude;
		center.position = (left.position + right.position) / 2;
		center.localScale = new Vector3(length, 4, 4);
	}
}
