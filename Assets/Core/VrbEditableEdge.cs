using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbEditableEdge : MonoBehaviour, IPointerClickHandler
{
	public VrbEdge e;
	public Vector3 v0;
	public Vector3 v1;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		v0 = e.v0.vector3;
		v1 = e.v1.vector3;
		transform.position = (e.v0.vector3 + e.v1.vector3) / 2;

		Vector3 dv = e.v1.vector3 - e.v0.vector3;
		float length = dv.magnitude;
		transform.localScale = new Vector3(3, length / 2 - 3, 3);

		transform.rotation = new Quaternion();

		if (dv.x != 0 && dv.y != 0 & dv.z != 0)
		{
			// ok
			if (dv.y > 0)
			{
				transform.Rotate(new Vector3(Mathf.Atan(dv.z / dv.y) * 180 / Mathf.PI, 0, 0));
			}
			else
			{
				transform.Rotate(new Vector3(180 + Mathf.Atan(dv.z / dv.y) * 180 / Mathf.PI, 0, 0));
			}
			transform.Rotate(new Vector3(0, 0, -Mathf.Atan(dv.x / Mathf.Sqrt(dv.y * dv.y + dv.z * dv.z)) * 180 / Mathf.PI));
		}
		else if (dv.x != 0 && dv.y != 0 && dv.z == 0)
		{
			// ok
			transform.Rotate(new Vector3(0, 0, -Mathf.Atan(dv.x / dv.y) * 180 / Mathf.PI));
		}
		else if (dv.x != 0 && dv.y == 0 && dv.z != 0)
		{
			// ok
			transform.Rotate(new Vector3(90, Mathf.Atan(dv.x / dv.z) * 180 / Mathf.PI, 0));
		}
		else if (dv.x == 0 && dv.y != 0 && dv.z != 0)
		{
			// ok
			transform.Rotate(new Vector3(Mathf.Atan(dv.z / dv.y) * 180 / Mathf.PI, 0, 0));
		}
		else if (dv.x == 0 && dv.y == 0 && dv.z != 0)
		{
			// ok
			transform.Rotate(new Vector3(90, 0, 0));
		}
		else if (dv.x == 0 && dv.y != 0 && dv.z == 0)
		{
			// ok
			transform.Rotate(new Vector3());
		}
		else if (dv.x != 0 && dv.y == 0 && dv.z == 0)
		{
			// ok
			transform.Rotate(new Vector3(0, 0, 90));
		}
		else
		{
			transform.localScale = new Vector3();
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().selectEdge(e);
	}
}
