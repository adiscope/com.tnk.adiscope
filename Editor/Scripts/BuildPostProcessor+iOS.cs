#if UNITY_IOS

using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Adiscope.Extension;
using SimpleJSON;

namespace Adiscope.PostProcessor
{

    class BuildPostProcessorForIos {

        public static string targetVersion = "2.1.1.0";
        private static string prefixURI = "https://github.com/adiscope/Adiscope-iOS-Sample/releases/download";
        
        /**
         * Unity Project내에 Adiscope.framework가 있는 폴더위치입니다. 
         * iOS위에 있지않고 한번 더 감싸여 있다면 (혹은 다른위치라면) 해당 위치를 수정 해 주세요.
         * Ex) /Plugins/Adiscope/iOS
        **/
        private static string adiscopeUnityPath = "Adiscope/Plugins/iOS";
        /**
         * Google Mobile Identifier를 업데이트 하기 위해서는 Media ID를 입력 해 주세요.
         * * Ex) "170"
         **/
        private static string adiscopeTargetMediaID = null;

        public static void OnPostProcessBuild(string path) {
            
            CopyAdiscopeFrameworks(path, new List<AdiscopeFrameworkType>() {
                AdiscopeFrameworkType.Core,
                AdiscopeFrameworkType.Admanager,
                AdiscopeFrameworkType.Admob,
                AdiscopeFrameworkType.Vungle,
                AdiscopeFrameworkType.ChartBoost,
                AdiscopeFrameworkType.FAN,
                AdiscopeFrameworkType.MobVista,
                AdiscopeFrameworkType.Tapjoy,
                AdiscopeFrameworkType.UnityAds,
                AdiscopeFrameworkType.Ironsource,
                AdiscopeFrameworkType.AppLovin
            });
            UpdateBuildSetting(path);
            UpdateInfoPlist(path);
        }

        private static string GetDefaultTarget(PBXProject project) {
            return project.GetUnityMainTargetGuid();
        }

        private static void CopyAdiscopeFrameworks(string path, List<AdiscopeFrameworkType> usingFrameworks) {

            string projectPath = PBXProject.GetPBXProjectPath(path);     
            string contents = File.ReadAllText(projectPath);

            PBXProject project = new PBXProject();
            project.ReadFromString(contents); 

            string frameworkPath = adiscopeUnityPath;

            string buildTargetGUID = GetDefaultTarget(project);
            string buildFrameworkTargetGUID = project.GetUnityFrameworkTargetGuid();

            string filePath = Application.dataPath;
            filePath       += "/" + frameworkPath + "/"; 

            string embedSectionID = project.AddCopyFilesBuildPhase(buildTargetGUID, "Embed Frameworks", "", "10");
            string embedFrameworkSectionID = project.AddCopyFilesBuildPhase(buildFrameworkTargetGUID, "Embed Frameworks", "", "10");
            string frameworkLinkPhaseGuid = project.GetFrameworksBuildPhaseByTarget(buildFrameworkTargetGUID);
            string linkPhaseGuid = project.GetFrameworksBuildPhaseByTarget(buildTargetGUID);
            project.AddBuildProperty(buildTargetGUID, "VALIDATE_WORKSPACE", "YES");
            project.AddBuildProperty(buildTargetGUID, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(buildTargetGUID, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/" + adiscopeUnityPath);
            project.AddBuildProperty(buildFrameworkTargetGUID, "VALIDATE_WORKSPACE", "YES");
            
            foreach (AdiscopeFrameworkType type in usingFrameworks) {
                string fileID = project.AddFile(
                    "Frameworks/" + adiscopeUnityPath + "/" + type.GetFileName(),
                    "Frameworks/" + adiscopeUnityPath + "/" + type.GetFileName()
                );

                if (false == type.IsEmbedFramework()) {
                    List<string> childFiles = type.GetChildFrameworkName();
                    if (childFiles == null) { continue; }

                    foreach (string childFileName in childFiles) {
                        string childFileID = project.AddFile(
                            "Frameworks/" + adiscopeUnityPath + "/" + childFileName,
                            "Frameworks/" + adiscopeUnityPath + "/" + childFileName
                        );

                        project.AddFileToBuildSection(buildTargetGUID, linkPhaseGuid, childFileID);
                        // Only used when FBAudienceNetwork are Dynamic Frameworks
                        // if (type == AdiscopeFrameworkType.FAN && childFileName == "FBAudienceNetwork.framework") {
                        //     project.AddFileToBuildSection(buildTargetGUID, embedSectionID, childFileID);
                        //     PBXProjectExtensions.AddFileToEmbedFrameworks(project, buildTargetGUID, childFileID);
                        // }
                    }
                    continue;
                }

                project.AddFileToBuildSection(buildFrameworkTargetGUID, embedFrameworkSectionID, fileID);
                project.AddFileToBuildSection(buildFrameworkTargetGUID, frameworkLinkPhaseGuid, fileID);
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, buildFrameworkTargetGUID, fileID);

                project.AddFileToBuildSection(buildTargetGUID, embedSectionID, fileID);
                PBXProjectExtensions.AddFileToEmbedFrameworks(project, buildTargetGUID, fileID);
            }

            File.WriteAllText(projectPath, project.WriteToString());

            UpdateDuplicateFrameworkSourceReference(projectPath, adiscopeUnityPath);
        }

        private static void UpdateDuplicateFrameworkSourceReference(string projectPath, string adiscopeUnityPath) {
            string contents = File.ReadAllText(projectPath);

            string[] contentList = contents.Split(new char[] {'\n'});
            List<string> result = new List<string>();

            foreach (string line in contentList) {
                bool isMatch = Regex.IsMatch(line, ".*Frameworks/" + adiscopeUnityPath + "/; sourceTree = SOURCE_ROOT;");
                if (!isMatch) { result.Add(line); }
            }

            contents = String.Join("\n", result);

            PBXProject project = new PBXProject();
            project.ReadFromString(contents);
            File.WriteAllText(projectPath, project.WriteToString());
        }

        private static void UpdateBuildSetting(string path)
        {
            string projectPath = PBXProject.GetPBXProjectPath(path);     
            string contents = File.ReadAllText(projectPath);

            PBXProject project = new PBXProject();
            project.ReadFromString(contents); 

            var defaultTarget = GetDefaultTarget(project);

            // Add `-ObjC` to "Other Linker Flags".
            project.AddBuildProperty(defaultTarget, "OTHER_LDFLAGS", "-ObjC");
            // Update 'NO' to "ENABLE_BITCODE"
            project.UpdateBuildProperty(defaultTarget, "ENABLE_BITCODE", new  string [] { "NO" }, new  string [] { "YES" });

            File.WriteAllText(projectPath, project.WriteToString());
        }
        
        private static void UpdateInfoPlist(string path) {
            string plistPath = Path.Combine (path, "Info.plist" );
            PlistDocument root = new PlistDocument();
            root.ReadFromFile(plistPath);
            
            Dictionary<string, object> injectPlistInfo = new Dictionary<string, object>{
                // Admob, AdManager
                { "GADIsAdManagerApp", true },

                // Permissions
                { "NSCalendarsUsageDescription" , "Some ad content may create a calendar event." },
                { "NSCameraUsageDescription" , "Some ad content may access camera to take picture." },
                { "NSMotionUsageDescription" , "Some ad content may require access to accelerometer for interactive ad experience." },
                { "NSPhotoLibraryUsageDescription" , "Some ad content may require access to the photo library." },
                { "NSUserTrackingUsageDescription", "Some ad content may require access to the user tracking." },

                // Scheme
                { "LSApplicationQueriesSchemes", new List<string>{
                    "fb", "instagram", "tumblr", "twitter"
                }},
            };

            foreach(KeyValuePair<string, object> item in injectPlistInfo) {
                insertInfoPlist(root.root, item.Key, item.Value);
            }

            // SKAdNetwork
            PlistElementArray targetArray = root.root.CreateArray("SKAdNetworkItems");
            DownloadSkAdNetworkPlistFile(path);
            
            string downloadedPath = Path.Combine(path, "AdiscopeSkAdNetworks.plist");
            PlistDocument skAdNetwork = new PlistDocument();
            skAdNetwork.ReadFromFile(downloadedPath);

            PlistElementArray array = (PlistElementArray)skAdNetwork.root["SKAdNetworkItems"];
            foreach(PlistElementDict item in array.values) {
                PlistElementDict dict = targetArray.AddDict();
                dict.SetString("SKAdNetworkIdentifier", item["SKAdNetworkIdentifier"].AsString());
            }

            File.Delete(downloadedPath);


            // Media Properties (Admob, Admanager)
            if (root.root.values.ContainsKey("GADApplicationIdentifier")) {
                root.root.values.Remove("GADApplicationIdentifier");
            }
            
            if (adiscopeTargetMediaID == null) { root.WriteToFile(plistPath); return; }
            downloadedPath = Path.Combine(path, "AdiscopeProperties.json");
            DownloadMediaProperties(path, adiscopeTargetMediaID);

            if (false == File.Exists(downloadedPath)) { root.WriteToFile(plistPath); return; }
            string jsonString = File.ReadAllText(downloadedPath);
            JSONNode node = JSON.Parse(jsonString);
            JSONNode networkNode = node["network"];

            string googleAppID = null;
            string applovinAppID = null;
            if (networkNode.HasKey("admob")) {
                googleAppID = node["network"]["admob"]["settings"]["com.google.android.gms.ads.APPLICATION_ID"];
            }
            else if (networkNode.HasKey("admanager")) {
                googleAppID = node["network"]["admanager"]["settings"]["com.google.android.gms.ads.APPLICATION_ID"];
            }
            if (networkNode.HasKey("applovin")) {
                applovinAppID = node["network"]["applovin"]["settings"]["applovin.sdk.key"];
            }

            if (googleAppID != null) {
                insertInfoPlist(root.root, "GADApplicationIdentifier", googleAppID);
            }
            if (applovinAppID != null) {
                insertInfoPlist(root.root, "AppLovinSdkKey", applovinAppID);
            }

            File.Delete(downloadedPath);


            root.WriteToFile(plistPath);
        }

        private static void DownloadSkAdNetworkPlistFile(string path) {
            string fileName = "AdiscopeSkAdNetworks.plist";
            string uriString = prefixURI;
            uriString += "/" + targetVersion;
            uriString += "/" + fileName;
            (new WebClient()).DownloadFile(new Uri(uriString), Path.Combine(path, fileName));
        }

        private static void DownloadMediaProperties(string path, string mediaID) {
            string fileName = "AdiscopeProperties.json";
            string uriString = "https://admin.adiscope.com/getJson/" + mediaID;
            (new WebClient()).DownloadFile(new Uri(uriString), Path.Combine(path, fileName));
        }

        private static void insertInfoPlist(object plist, string key, object value) {

            if (value is string) {
                if (plist is PlistElementDict) {
                    PlistElementDict plistElementDict = (PlistElementDict)plist;
                    plistElementDict.SetString(key, (string)value);
                } else if (plist is PlistElementArray) {
                    PlistElementArray plistElementArray = (PlistElementArray)plist;
                    if (plistElementArray.IsExistPlist((string)value)) { return; }
                    plistElementArray.AddString((string)value);
                }
                return;  

            } else if  (value is bool) {
                    PlistElementDict plistElementDict = (PlistElementDict)plist;
                    plistElementDict.SetBoolean(key, (bool)value);
            } 

            if (value is List<string>) {
                PlistElementDict rootPlistElementDict = (PlistElementDict)plist;
                PlistElementArray targetPlistElementArray = (PlistElementArray)rootPlistElementDict[key];
                if (targetPlistElementArray == null) { targetPlistElementArray = rootPlistElementDict.CreateArray(key); }

                foreach (string item in (List<string>)value) {
                    insertInfoPlist(targetPlistElementArray, null, item);
                }
            } else if (value is Dictionary<string, object>) {
                PlistElementDict rootPlistElementDict = (PlistElementDict)plist;
                PlistElementDict targetPlistElementDict = (PlistElementDict)rootPlistElementDict[key];
                if (targetPlistElementDict == null) { rootPlistElementDict.CreateDict(key); }

                foreach (KeyValuePair<string, object> valueItem in (Dictionary<string, object>)value) {
                    insertInfoPlist(targetPlistElementDict, valueItem.Key, valueItem.Value);
                }
            }
        }
    }

    public enum AdiscopeFrameworkType {
        Core,
        Admanager,
        Admob,
        Vungle,
        ChartBoost,
        FAN,
        MobVista,
        Tapjoy,
        Ironsource,
        UnityAds,
        AppLovin
    }

    static class AdiscopeFrameworkTypeExtension {

        public static bool IsEmbedFramework(this AdiscopeFrameworkType type) {
            return type == AdiscopeFrameworkType.Core;
        }

        public static string GetFileName(this AdiscopeFrameworkType type) {
            switch (type) {
                case AdiscopeFrameworkType.Core:       return "Adiscope.framework";
                case AdiscopeFrameworkType.Admob:      return "AdiscopeMediaAdMob.framework";
                case AdiscopeFrameworkType.Vungle:     return "AdiscopeMediaVungle.framework";
                case AdiscopeFrameworkType.ChartBoost: return "AdiscopeMediaChartBoost.framework";
                case AdiscopeFrameworkType.FAN:        return "AdiscopeMediaFAN.framework";
                case AdiscopeFrameworkType.MobVista:   return "AdiscopeMediaMobVista.framework";
                case AdiscopeFrameworkType.Tapjoy:     return "AdiscopeMediaTapjoy.framework";
                case AdiscopeFrameworkType.Ironsource: return "AdiscopeMediaIronsource.framework";
                case AdiscopeFrameworkType.UnityAds:   return "AdiscopeMediaUnityAds.framework";
                case AdiscopeFrameworkType.AppLovin:   return "AdiscopeMediaAppLovin.framework";
                default: return null;
            }
        }

        public static List<string> GetChildFrameworkName(this AdiscopeFrameworkType type) {
            switch (type) {
                case AdiscopeFrameworkType.Core:
                    return null;
                case AdiscopeFrameworkType.Admanager:
                    return new List<string>() {
                        "AdiscopeMediaAdManager.framework",
                        "GoogleMobileAds.framework",
                        "UserMessagingPlatform.framework"
                    };
                case AdiscopeFrameworkType.Admob:
                    return new List<string>() {
                        "AdiscopeMediaAdMob.framework",
                        "GoogleMobileAds.framework",
                        "UserMessagingPlatform.framework"
                    };
                case AdiscopeFrameworkType.Vungle:
                    return new List<string>() {
                        "AdiscopeMediaVungle.framework",
                        "VungleSDK.framework"
                    };
                case AdiscopeFrameworkType.ChartBoost:
                    return new List<string>() {
                        "AdiscopeMediaChartBoost.framework",
                        "Chartboost.framework"
                    };
                case AdiscopeFrameworkType.FAN:
                    return new List<string>() {
                        "AdiscopeMediaFAN.framework",
                        "FBSDKCoreKit_Basics.framework",
                        "FBAudienceNetwork.framework",
                    };
                case AdiscopeFrameworkType.MobVista:
                    return new List<string>() {
                        "AdiscopeMediaMobVista.framework",
                        "MTGSDK.framework",
                        "MTGSDKReward.framework"
                    };
                case AdiscopeFrameworkType.Tapjoy:
                    return new List<string>() {
                        "AdiscopeMediaTapjoy.framework",
                        "Tapjoy.framework"
                    };
                case AdiscopeFrameworkType.Ironsource:
                    return new List<string>() {
                        "AdiscopeMediaIronsource.framework",
                        "IronSource.framework"
                    };
                case AdiscopeFrameworkType.UnityAds:
                    return new List<string>() {
                        "AdiscopeMediaUnityAds.framework",
                        "UnityAds.framework"
                    };
                case AdiscopeFrameworkType.AppLovin:
                    return new List<string>() {
                        "AdiscopeMediaAppLovin.framework",
                        "AppLovinSDK.framework"
                    };
                default:
                    return null;
            }
        }
    }
}

#endif
