/************************************************************************************

Copyright   :   Copyright 2015 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using System.Collections;

public class SampleGUI : dpn.DpnGUI
{
	public GUISkin skin;
	public Texture tex;

	public override void OnDpnGUI()
	{
		GUI.skin = skin;
		GUI.Label(new Rect(250, 100, 200, 50), "Label");
		GUI.Button(new Rect(250,150,200,100),"Button");

		GUI.DrawTexture(new Rect(250,250,200,100),tex);
		GUI.Label(new Rect(250, 250, 200, 100), "Texture");
	}
}
