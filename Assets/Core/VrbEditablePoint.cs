using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbEditablePoint: MonoBehaviour
{
	public int vertexIndex;
	private VrbModel vrbm;
	// Start is called before the first frame update
	void Start()
	{
		vrbm = GameObject.Find("CustomModel").GetComponent<VrbModel>();
		constructMesh();
	}

    // Update is called once per frame
    void Update()
    {
		transform.position = vrbm.vertices[vertexIndex];
    }

	public void move(float dx, float dy, float dz)
	{
		Vector3 v = vrbm.vertices[vertexIndex];
		v.x += dx;
		v.y += dy;
		v.z += dz;
	}

	public void move(Vector3 dv)
	{
		vrbm.vertices[vertexIndex] += dv;
	}
	public void constructMesh()
	{
		// 可选择面
		VrbSelectable s = gameObject.AddComponent<VrbSelectable>();

		transform.parent = GameObject.Find("CustomModel").transform;
		transform.position = vrbm.vertices[vertexIndex];
		transform.localScale = new Vector3(4, 4, 4);
	}

}
