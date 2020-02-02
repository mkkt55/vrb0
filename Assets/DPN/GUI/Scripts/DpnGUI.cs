/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using System.Collections;

namespace dpn
{
	public abstract class DpnGUI : MonoBehaviour 
	{
		public Vector2 guiPosition      = new Vector2(0f, 0f);
		public float   guiSize          = 1f;
		public bool    useCurvedSurface = false;
		
		private GameObject    guiRenderPlane    = null;
		private RenderTexture guiRenderTexture  = null;

		void Start()
		{
			if (useCurvedSurface)
				guiRenderPlane = Instantiate(Resources.Load("DpnGuiCurvedSurface")) as GameObject;
			else
				guiRenderPlane = Instantiate(Resources.Load("DpnGuiFlatSurface")) as GameObject;

			guiRenderPlane.transform.parent        = this.transform;
			guiRenderPlane.transform.localPosition = new Vector3(guiPosition.x,guiPosition.y,1);
			guiRenderPlane.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			guiRenderPlane.transform.localScale    = new Vector3(guiSize, guiSize, guiSize);
			
			guiRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
#if UNITY_4
			guiRenderPlane.renderer.material.mainTexture = guiRenderTexture;
#else
            guiRenderPlane.GetComponent<Renderer>().material.mainTexture = guiRenderTexture;
#endif
		}
		
		protected void OnGUI()
		{
            if (Event.current.type == EventType.Repaint)
            {
                RenderTexture old = RenderTexture.active;
                RenderTexture.active = guiRenderTexture;
                GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                OnDpnGUI();
                RenderTexture.active = old;
            }
		}
		
		public abstract void OnDpnGUI();
	}
}