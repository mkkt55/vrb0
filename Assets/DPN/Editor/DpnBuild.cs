/************************************************************************************

Copyright: Copyright(c) 2015-2017 Deepoon LLC. All Rights reserved.

DPVR Developer Website: http://developer.dpvr.cn

************************************************************************************/
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

class ProjectBuild : Editor {
	[MenuItem("DPVR/DpnBuild/Android")]
    public static void BuildForAndroid()
    {
        QualitySettings.vSyncCount = 0;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		string[] levels = new string[] { "Assets/DPN/Scenes/Cubes.unity", "Assets/DPN/Scenes/Spheres.unity" };
		BuildPipeline.BuildPlayer(levels, projectPath + "/DPVRUnityForAndroid.apk", BuildTarget.Android, BuildOptions.None);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
    }

	[MenuItem("DPVR/DpnBuild/Win32")]
    public static void BuildForWin32()
    {
        QualitySettings.vSyncCount = 0;
		string[] levels = new string[] { "Assets/DPN/Scenes/Cubes.unity", "Assets/DPN/Scenes/Spheres.unity" };
		BuildPipeline.BuildPlayer(levels, projectPath + "/DPVRUnityForWin32.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }

	[MenuItem("DPVR/DpnBuild/Win64")]
    public static void BuildForx64()
    {
        QualitySettings.vSyncCount = 0;
		string[] levels = new string[] { "Assets/DPN/Scenes/Cubes.unity", "Assets/DPN/Scenes/Spheres.unity" };
		BuildPipeline.BuildPlayer(levels, projectPath + "/DPVRUnityForWin64.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static string projectPath
    {
        get
        {
			string path = Path.GetFullPath(".");
			if (path.Contains("DeePoonUnity"))
			{
				path += "/../Release";
			}
			else
			{
				path += "/Release";
			}
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string releasepath = path + "/" + Application.unityVersion;
            if (!Directory.Exists(releasepath))
            {
                Directory.CreateDirectory(releasepath); ;
            }
            return releasepath;
        }
    }
}
