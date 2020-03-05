using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VrbSelectableLight : MonoBehaviour, IPointerClickHandler
{
	public VrbLight l;
	public Material arrowMat;
	// Start is called before the first frame update
	void Start()
    {
		arrowMat = GetComponentInChildren<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
		if (l == null)
		{
			return;
		}
		arrowMat.color = l.color;
    }

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		GameObject.Find("PlayerController").GetComponent<PlayerController>().select(l);
	}
}
