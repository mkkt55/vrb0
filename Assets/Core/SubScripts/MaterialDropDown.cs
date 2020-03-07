using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class VrbMat
{
	public static string[] types = { "Default", "Emissive-Light" };
	public static Material[] mats = new Material[types.Length];
}

public class MaterialDropDown : MonoBehaviour
{
	public GameObject pc;
	public List<VrbTarget> selected;
	public Dropdown mat;
	public int valueLastFrame;

	void Awake()
	{
		VrbMat.mats[0] = Resources.Load<Material>("Materials/Vrb-Default");
		VrbMat.mats[1] = Resources.Load<Material>("Materials/Vrb-LightSource");
	}

	void OnEnable()
	{
		if (pc == null)
		{
			pc = GameObject.Find("PlayerController");
			selected = pc.GetComponent<PlayerController>().selected;
			mat = GetComponent<Dropdown>();
			valueLastFrame = mat.value;
		}

		if (selected.Count > 0)
		{
			if (selected[0].getType().Equals("object")){
				if (((VrbObject)selected[0]).matStr.Equals(VrbMat.types[0]))
				{
					mat.value = 0;
				}
				else if (((VrbObject)selected[0]).matStr.Equals(VrbMat.types[1]))
				{
					mat.value = 1;
				}
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
        if (mat.value != valueLastFrame)
		{
			setMaterial();
			Debug.LogWarning("aa");
			valueLastFrame = mat.value;
		}
    }

	void setMaterial()
	{
		VrbObject vrbo = (VrbObject)selected[mat.value];
		vrbo.matStr = VrbMat.types[mat.value];
		vrbo.material = VrbMat.mats[mat.value];
		vrbo.gameObject.GetComponent<MeshRenderer>().material = vrbo.material;
	}
}
