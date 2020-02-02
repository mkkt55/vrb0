/************************************************************************************

Copyright   :   Copyright 2015-2017 DeePoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Diagnostics;

[InitializeOnLoad]
public class DpnShimLoader
{
	static DpnShimLoader()
	{

	}

	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string project_path)
	{


	}

	private static void OverwriteFile( string src , string dest )
	{
		if (File.Exists(dest))
			File.Delete(dest);
		
		File.Copy(src, dest);
	}

	[MenuItem("DPVR/Developer Website")]
    static void OpenDeepoonDeveloperSite()
    {
        UnityEngine.Debug.Log("Open developer.dpvr.cn");
        Process foo = new Process();
		foo.StartInfo.FileName = "Assets\\DPN\\Utilities\\Dpnwebsite\\OpenDeveloperSiteOfDpvr.bat";
        foo.StartInfo.Arguments = "";
        foo.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        foo.Start();
    }

}
