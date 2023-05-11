
#if UNITY_IOS

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.Text.RegularExpressions;

using Adiscope.PostProcessor;

public class BuildPostProcessor
{
	[PostProcessBuildAttribute(1)]
	
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		switch (target)
		{
			case BuildTarget.iOS: BuildPostProcessorForIos.OnPostProcessBuild(path); break;
			default: break;
		}
	}
}

#endif