using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrbEditableFace : MonoBehaviour
{
	public int fIndex;
	public List<int> fVertexIndices; // 面的顶点在VrbModel中的索引是多少，即它是vertice中的哪一位。
	public List<Vector3> fVertices; // 面的顶点的具体信息

	public List<int> ftOriginal; // 面中的int代表三角面片在VrbModel.triangles数组的起始位置，即永远是3的倍数。
	public List<int> fTriangles; // 代表三角面片在自己的fVertices数组中的位置，是上一个的三倍
								 // Start is called before the first frame update
	void Start()
	{
		fVertexIndices = VrbFace.all[fIndex].fVertexIndices;
		fVertices = VrbFace.all[fIndex].fVertices;
		ftOriginal = VrbFace.all[fIndex].ftOriginal;
		fTriangles = VrbFace.all[fIndex].fTriangles;

	}

    // Update is called once per frame
    void Update()
    {

	}
}
