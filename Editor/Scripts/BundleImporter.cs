/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Adiscope.Editor
{
    /// <summary>
    /// AdiscopeSdk Bundle Importer in Unity Editor Menu Extention
    /// Read "service.json" from admin and each mediation adapter setting from bundle zip file
    /// (eg. adapter-adcolony.json, adapter-unityads.json, etc.)
    /// then automatically create and copy plugin files
    /// (Unity5: AndroidManifest.xml, project.properties, *.aar, etc.)
    /// (Unity4: AndroidManifest.xml, project.properties, res, libs, *.jar, etc.)
    /// </summary>
    public class BundleImporter : EditorWindow
    {
        #region CONST VALUES
        private const string KEY_SERVICE_JSON_FILE_PATH = "ADISCOPE_KEY_SERVICE_JSON_FILE_PATH";
        private const string KEY_BUNDLE_FILE_PATH = "ADISCOPE_KEY_BUNDLE_FILE_PATH";
        private const string KEY_FLAG_VERBOSE = "ADISCOPE_KEY_FLAG_VERBOSE";
        private const string KEY_FLAG_FORCE_UNITY4 = "ADISCOPE_KEY_FLAG_FORCE_UNITY4";
        private const string KEY_OPEN_SOURCE_LIB_PATH = "KEY_OPEN_SOURCE_LIB_PATH";

        private const string MENU_ITEM_BUNDLE_IMPORTER = "Adiscope/Sdk Bundle Importer";

        private const string EDITOR_WINDOW_NAME = "Adiscope Sdk Bundle Importer";
        private const int EDITOR_WINDOW_WIDTH = 600;
        private const int EDITOR_WINDOW_HEIGHT = 500;

        private const string ADISCOPE_CORE_FILE_NAME = "adiscopeCore";

        private const string SERVICE_JSON_KEY_ADISCOPE = "adiscope";
        private const string SERVICE_JSON_KEY_AD_NETWORK = "network";
        private const string SERVICE_JSON_KEY_VERSION = "version";
        private const string SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS = "ads";
        private const string SERVICE_JSON_KEY_AD_NETWORK_INFO_SETTINGS = "settings";
        private const string SERVICE_JSON_KEY_OFFERWALL_AD = "offerwallAd";
        private const string SERVICE_JSON_KEY_REWARDED_VIDEO_AD = "rewardedVideoAd";
        private const string SERVICE_JSON_KEY_INTERSTITIAL_AD = "interstitialAd";

        private const string ADAPTER_MANIFEST_KEY_SECTION_COMMON = "common";
        private const string ADAPTER_MANIFEST_KEY_SECTION_OFFERWALL_AD = "offerwallAd";
        private const string ADAPTER_MANIFEST_KEY_SECTION_REWARDED_VIDEO_AD = "rewardedVideoAd";
        private const string ADAPTER_MANIFEST_KEY_SECTION_INTERSTITIAL_AD = "interstitialAd";

        private const string ADAPTER_MANIFEST_KEY_FIELD_QUERIES = "queries";
        private const string ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS = "permissions";
        private const string ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES = "activities";
        private const string ADAPTER_MANIFEST_KEY_FIELD_META_DATA = "meta-data";
        private const string ADAPTER_MANIFEST_KEY_FIELD_RECEIVERS = "receivers";
        private const string ADAPTER_MANIFEST_KEY_FIELD_SERVICES = "services";
        private const string ADAPTER_MANIFEST_KEY_FIELD_PROVIDERS = "providers";
        private const string ADAPTER_MANIFEST_KEY_FIELD_USES_LIBRARYS = "uses-librarys";

        private const string MSG_PROGRESS_BAR = "Updating plugin settings...";

        private const string MSG_LABEL_SELECT_FILES = "1. Please select AdiscopeSdk files";
        private const string MSG_TEXT_PATH_TO_SERVICE_JSON_FILE = "path to service.json file";
        private const string MSG_BUTTON_SELECT_SERVICE_JSON_FILE = "Select service.json file";
        private const string MSG_TEXT_PATH_TO_BUNDLE_FILE = "path to bundle file";
        private const string MSG_BUTTON_SELECT_BUNDLE_FILE = "Select bundle file";

        private const string MSG_LABEL_DETAILS_NETWORKS = "2. Details of the Ad Networks used";

        private const string MSG_LABEL_SELECT_TASK = "3. Please select task";
        private const string MSG_BUTTON_INSTALL = "Install/Update";
        private const string MSG_BUTTON_UNINSTALL = "Uninstall";
        private const string MSG_ERR_SERVICE_JSON = "Please locate service.json file";
        private const string MSG_ERROR_BUNDLE = "Please locate bundle file";
        private const string MSG_CHECK_VERBOSE = "verbose";
        private const string MSG_CHECK_FORCE_UNITY4 = "force to support unity4 (jar, not aar)";
        private const string MSG_ERROR = "Error";
        private const string MSG_OK = "OK";

        private const string MSG_LABEL_CLOSE_WINDOW = "4. Close Adiscope Sdk Bundle Importer window";
        private const string MSG_BUTTON_CLOSE = "Close";

        private const string FILE_EXTENSION_SERVICE_JSON = "json";
        private const string FILE_EXTENSION_BUNDLE = "zip";

        private const int MIN_SERVICE_JSON_VERSION = 1;
        private const int MAX_SERVICE_JSON_VERSION = 1;
        #endregion

        private int unityVersionMajor;
        private string svcJsonFilePath;
        private string zipFilePath;
        private bool verbose;
        private bool supportUnity4;
        private bool noGui;
        private GUIStyle labelFieldStyle;
        private GUIStyle textFieldStyle;

        private bool isInstalledAdmobMeta;

        //////////////////////////////////////////////////
        // about detailsNetworks
        private List<string> usedNetworks;
        private Dictionary<string, object> additionalLibNetwork;
        private Dictionary<string, string> additionalLibNetworkSettingVer = new Dictionary<string, string>()
        {
            {"vungle", "v2" }
        };
        private Dictionary<string, List<string>> googleDependency = new Dictionary<string, List<string>>()
        {
            {"admob", new List<string> {"play-services-ads" }},
            {"mobvista", new List<string> {"use androidx" }},
            {"vungle", new List<string> {"use androidx" }},
        };

        private string detailNetworkHelp = "" +
            "In this menu, the list of networks included in the game are displayed.\n" +
            "Information about the networks are given too, such as google dependencies, open-source libraries for the network.\n\n" +
            "We recommend you to go through the checklist given below before applying open source libraries to your application.\n\n" +
            "1. If a open-source library is not included in the game, checkbox for that library should be selected so that the library can be copied into the game folder.\n\n" +
            "2. If the list of libraries of the game can't be specified, it's recommended to select all library checkboxs.\n" +
            "   (If there is any duplicated libraries, compiler will throw an error message while build the apk)\n\n" +
            "3. If you are using automated build server, required library files can be manually copied.\n" + 
            "   (select all library checkboxs is not required)\n\n" +
            "4. Default path for open source libraries is '[ProjectPath]/Assets/Plugins/Android', the path is modifiable.\n\n";

        private GUIStyle detailsNetworkHeaderStyle;
        private GUIStyle detailsNetworkRowStyle;
        private const int HEADER1_WIDTH = 90;
        private const int HEADER2_WIDTH = 220;
        private const int BODY1_WIDTH = 84;
        private const int BODY2_WIDTH = 218;
        private string openSourceLibPath;

        public static void Build()
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!start");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Adiscope/Sdk/Examples/AdiscopeExampleScene01.unity", "Assets/Adiscope/Sdk/Examples/AdiscopeExampleScene02.unity" };
            buildPlayerOptions.locationPathName = "/Users/junghee/junghee.apk";
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);


            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!done");
        }

        [MenuItem(MENU_ITEM_BUNDLE_IMPORTER)]
        public static void UpdatePluginSettings()
        {
            // Get existing open window or if none, make a new one:
            BundleImporter importer =
                EditorWindow.GetWindow<BundleImporter>(true, EDITOR_WINDOW_NAME);

            importer.InitVariables(false);

            importer.InitGUI();
            importer.minSize = new Vector2(EDITOR_WINDOW_WIDTH, EDITOR_WINDOW_HEIGHT);

            // Show will lead to OnGUI()
            importer.Show();
        }

        /// <summary>
        /// install plugins from Commandline interface with argument
        /// EX.>
        /// "c:\Program Files\Unity 2017.2.0b11\Editor\Unity.exe" -batchmode -quit
        ///     -executeMethod Adiscope.Editor.BundleImporter.UpdatePluginSettingsFromCLI
        ///     -adiscopeJsonPath "d:\Workspace\Adiscope\service.json"
        ///     -adiscopeBundlePath "d:\Workspace\Adiscope\SdkRelease\1.0.427.171011\AdiscopeUnitySdkBundle-1.0.427.171011.zip"
        ///     -adiscopeForceUnity4 false
        /// </summary>
        public static void UpdatePluginSettingsFromCLI()
        {
            string jsonPath = GetArg("-adiscopeJsonPath");
            string bundlePath = GetArg("-adiscopeBundlePath");
            string forceUnity4 = GetArg("-adiscopeForceUnity4");

            Logger.d("adiscopeJsonPath: {0}", jsonPath);
            Logger.d("adiscopeBundlePath: {0}", bundlePath);
            Logger.d("adiscopeForceUnity4: {0}", forceUnity4);

            bool forceToSupportUnity4 = true;

            if (forceUnity4 != null)
            {
                try
                {
                    forceToSupportUnity4 = Boolean.Parse(forceUnity4);
                }
                catch (Exception e)
                {
                    Logger.w("can't parse adiscopeForceUnity4: {0}", forceUnity4);
                    Logger.w(e);
                }
            }

            Logger.d("forceful support unity 4: {0}", forceToSupportUnity4);

            UpdatePluginSettingsNoGUI(jsonPath, bundlePath, forceToSupportUnity4);
        }

        /// <summary>
        /// install plugins without UI prompt
        /// </summary>
        /// <param name="serviceJsonFilePath">path of service.json</param>
        /// <param name="bundleFilePath">path of bundle zip file</param>
        /// <param name="forceToSupportUnity4">if true, plugins files will be type of JAR instead of AAR</param>
        public static void UpdatePluginSettingsNoGUI(string serviceJsonFilePath, string bundleFilePath, bool forceToSupportUnity4)
        {
            BundleImporter importer = ScriptableObject.CreateInstance<BundleImporter>();

            importer.InitVariables(true);

            Logger.i("serviceJsonFilePath: {0}", serviceJsonFilePath);
            Logger.i("bundleFilePath: {0}", bundleFilePath);
            Logger.i("forceToSupportUnity4: {0}", forceToSupportUnity4);

            if (importer.unityVersionMajor < 5)
            {
                Logger.i(
                    "forceToSupportUnity4 is set to true because unity version is {0}.x",
                    importer.unityVersionMajor);
                forceToSupportUnity4 = true;
            }

            if (!importer.InstallSdk(serviceJsonFilePath, bundleFilePath, forceToSupportUnity4))
            {
                Logger.e("Adiscope plugin import failed");
                return;
            }

            Logger.i("Adiscope plugin import done");
        }

        /// <summary>
        /// uninstall plugins from Commandline interface
        /// EX.
        /// "c:\Program Files\Unity 2017.2.0b11\Editor\Unity.exe" -batchmode -quit
        ///     -executeMethod Adiscope.Editor.BundleImporter.UninstallPluginFromCLI
        /// </summary>
        public static void UninstallPluginFromCLI()
        {
            UninstallPluginsNoGUI();
        }

        /// <summary>
        /// uninstall plugins without UI prompt
        /// </summary>
        public static void UninstallPluginsNoGUI()
        {
            BundleImporter importer = ScriptableObject.CreateInstance<BundleImporter>();

            importer.InitVariables(true);

            importer.UninstallSdk();

            Logger.i("Adiscope plugin uninstall done");
        }

        private void InitVariables(bool noGui)
        {
#if UNITY_2017_2_OR_NEWER || UNITY_2017_1_OR_NEWER
            this.unityVersionMajor = 2017;
#elif UNITY_5_6_OR_NEWER || UNITY_5_5_OR_NEWER || UNITY_5_4_OR_NEWER || UNITY_5_3_OR_NEWER
            this.unityVersionMajor = 5;
#elif UNITY_4_7_2 || UNITY_4_7
            this.unityVersionMajor = 4;
#else
            this.unityVersionMajor = 0;
#endif
            // this member variable is used to suppress UI when invoked from Command line interface
            this.noGui = noGui;

            Logger.d("unity major version: {0}", this.unityVersionMajor);

            this.svcJsonFilePath = EditorPrefs.GetString(KEY_SERVICE_JSON_FILE_PATH);
            this.zipFilePath = EditorPrefs.GetString(KEY_BUNDLE_FILE_PATH);

            string openSourceLibDefaultPath = FileManager.GetCombinedPath(Application.dataPath, "Plugins", "Android");
            this.openSourceLibPath = EditorPrefs.GetString(KEY_OPEN_SOURCE_LIB_PATH, openSourceLibDefaultPath);

            this.verbose = EditorPrefs.GetBool(KEY_FLAG_VERBOSE);
            Logger.verbose = this.verbose;

            this.supportUnity4 = EditorPrefs.GetBool(KEY_FLAG_FORCE_UNITY4, true);

            reMakeDetailsNetworkData();

        }

        private void InitGUI()
        {
            try
            {
                if (EditorStyles.label != null)
                {
                    this.labelFieldStyle = new GUIStyle(EditorStyles.label);
                    this.labelFieldStyle.fontSize = 12;
                    this.labelFieldStyle.fontStyle = FontStyle.Bold;
                }
                else
                {
                    this.labelFieldStyle = new GUIStyle();
                }

                if (EditorStyles.textField != null)
                {
                    this.textFieldStyle = new GUIStyle(EditorStyles.textField);
                    this.textFieldStyle.fontSize = 12;
                    this.textFieldStyle.wordWrap = true;
                    this.textFieldStyle.alignment = TextAnchor.MiddleLeft;
                }
                else
                {
                    this.textFieldStyle = new GUIStyle();
                }
            }
            catch (Exception e)
            {
                Logger.w(e);
                this.labelFieldStyle = new GUIStyle();
                this.textFieldStyle = new GUIStyle();
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(MSG_LABEL_SELECT_FILES,
                this.labelFieldStyle, GUILayout.Height(16));

            string textServiceJsonFile = MSG_TEXT_PATH_TO_SERVICE_JSON_FILE;
            if (this.svcJsonFilePath != null && this.svcJsonFilePath.Length > 0)
            {
                textServiceJsonFile = this.svcJsonFilePath;
            }

            EditorGUILayout.LabelField(textServiceJsonFile, this.textFieldStyle,
                GUILayout.Height(
                    10 + this.textFieldStyle.CalcHeight(
                        new GUIContent(textServiceJsonFile), position.width)));

            if (GUILayout.Button(MSG_BUTTON_SELECT_SERVICE_JSON_FILE, GUILayout.Height(30)))
            {
                string dirPath = null;
                if (this.svcJsonFilePath != null && this.svcJsonFilePath.Length > 0)
                {
                    dirPath = Path.GetDirectoryName(this.svcJsonFilePath);
                }

                this.svcJsonFilePath = EditorUtility.OpenFilePanel(
                    MSG_BUTTON_SELECT_SERVICE_JSON_FILE, dirPath, FILE_EXTENSION_SERVICE_JSON);

                EditorPrefs.SetString(KEY_SERVICE_JSON_FILE_PATH, this.svcJsonFilePath);

                reMakeDetailsNetworkData();
            }

            EditorGUILayout.Space();

            string textZipFile = MSG_TEXT_PATH_TO_BUNDLE_FILE;
            if (this.zipFilePath != null && this.zipFilePath.Length > 0)
            {
                textZipFile = this.zipFilePath;
            }

            EditorGUILayout.LabelField(textZipFile, this.textFieldStyle,
                GUILayout.Height(
                    10 + this.textFieldStyle.CalcHeight(
                        new GUIContent(textZipFile), position.width)));

            if (GUILayout.Button(MSG_BUTTON_SELECT_BUNDLE_FILE, GUILayout.Height(30)))
            {
                string dirPath = null;
                if (this.zipFilePath != null && this.zipFilePath.Length > 0)
                {
                    dirPath = Path.GetDirectoryName(this.zipFilePath);
                }

                this.zipFilePath = EditorUtility.OpenFilePanel(
                    MSG_BUTTON_SELECT_BUNDLE_FILE, dirPath, FILE_EXTENSION_BUNDLE);

                EditorPrefs.SetString(KEY_BUNDLE_FILE_PATH, this.zipFilePath);

                reMakeDetailsNetworkData();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(MSG_LABEL_DETAILS_NETWORKS, this.labelFieldStyle, GUILayout.Height(16), GUILayout.Width(240));
                if (GUILayout.Button("update", GUILayout.Width(50)))
                {
                    reMakeDetailsNetworkData();
                }
                if (GUILayout.Button("help", GUILayout.Width(50)))
                {
                    EditorUtility.DisplayDialog("help", detailNetworkHelp, "Ok");
                }
            GUILayout.EndHorizontal();

            this.detailsNetworkHeaderStyle = new GUIStyle(EditorStyles.helpBox);
            this.detailsNetworkHeaderStyle.fontSize = 12;
            this.detailsNetworkHeaderStyle.fontStyle = FontStyle.Bold;
            this.detailsNetworkHeaderStyle.alignment = TextAnchor.MiddleCenter;

            this.detailsNetworkRowStyle = new GUIStyle(EditorStyles.label);
            this.detailsNetworkRowStyle.fontSize = 11;

            // header UI
            GUILayout.BeginVertical("Box");
                GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                        GUILayout.Label("Network", detailsNetworkHeaderStyle,  GUILayout.Width(HEADER1_WIDTH));
                        GUILayout.Label("Google dependency", detailsNetworkHeaderStyle, GUILayout.Width(HEADER2_WIDTH));
                        GUILayout.Label("open source (check to copy)", detailsNetworkHeaderStyle, GUILayout.MinWidth(HEADER2_WIDTH));
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();

            // body UI
            int cntOpenSourceLib = 0;
            if (usedNetworks != null && usedNetworks.Count > 0)
            {
                foreach (string networkName in usedNetworks)
                {
                    // body area
                    GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                            // column 1
                            GUILayout.Label(networkName, detailsNetworkRowStyle, GUILayout.Width(BODY1_WIDTH));

                            // column 2
                            GUILayout.BeginVertical(GUILayout.Width(BODY2_WIDTH), GUILayout.MaxHeight(1));
                            if (googleDependency.ContainsKey(networkName))
                            {
                                List<string> dependencies = googleDependency[networkName] as List<string>;
                                foreach (string dependency in dependencies)
                                {
                                    GUILayout.Label(dependency, detailsNetworkRowStyle, GUILayout.Width(BODY2_WIDTH));
                                }
                            }
                            else
                            {
                                GUILayout.Label("-", detailsNetworkRowStyle, GUILayout.Width(BODY2_WIDTH));
                            }
                            GUILayout.EndHorizontal();

                            // column 3
                            if (additionalLibNetwork != null && additionalLibNetwork.ContainsKey(networkName))
                            {
                                GUILayout.BeginVertical();
                                List<string> libraries = additionalLibNetwork[networkName] as List<string>;
                                foreach (string library in libraries)
                                {
                                    cntOpenSourceLib++;
                                    string keyName = makeDNKey(networkName, library);
                                    bool check = EditorPrefs.GetBool(keyName, false);
                                    check = GUILayout.Toggle(check, library, GUILayout.Width(BODY2_WIDTH));
                                    EditorPrefs.SetBool(keyName, check);
                                }
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                GUILayout.Label("-", detailsNetworkRowStyle, GUILayout.Width(BODY2_WIDTH));
                            }
                        GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            if (cntOpenSourceLib > 0)
            {
                GUILayout.BeginHorizontal("Box");
                    if (GUILayout.Button("Path to copy", GUILayout.Width(100)))
                    {
                        string dirPath = null;
                        if (this.openSourceLibPath != null && this.openSourceLibPath.Length > 0)
                        {
                            dirPath = Path.GetDirectoryName(this.openSourceLibPath);
                        }

                        this.openSourceLibPath = EditorUtility.OpenFolderPanel(
                            "title", dirPath, "");

                        EditorPrefs.SetString(KEY_OPEN_SOURCE_LIB_PATH, this.openSourceLibPath);
                    }

                    EditorGUILayout.LabelField(openSourceLibPath, this.textFieldStyle,
                        GUILayout.MinWidth(400));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(MSG_LABEL_SELECT_TASK,
                 this.labelFieldStyle, GUILayout.Height(16));

            if (GUILayout.Button(MSG_BUTTON_INSTALL, GUILayout.Height(30)))
            {
                if (this.svcJsonFilePath == null || this.svcJsonFilePath.Length == 0)
                {
                    EditorUtility.DisplayDialog(MSG_ERROR, MSG_ERR_SERVICE_JSON, MSG_OK);
                }
                else if (this.zipFilePath == null || this.zipFilePath.Length == 0)
                {
                    EditorUtility.DisplayDialog(MSG_ERROR, MSG_ERROR_BUNDLE, MSG_OK);
                }
                else
                {
                    InstallSdk(this.svcJsonFilePath, this.zipFilePath, this.supportUnity4);
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(MSG_BUTTON_UNINSTALL, GUILayout.Height(30)))
            {
                UninstallSdk();
            }

            EditorGUILayout.Space();

            this.verbose = GUILayout.Toggle(this.verbose, MSG_CHECK_VERBOSE);
            if (this.verbose != Logger.verbose)
            {
                EditorPrefs.SetBool(KEY_FLAG_VERBOSE, this.verbose);
                Logger.verbose = this.verbose;
            }

            // unity version dependent action
            if (this.unityVersionMajor >= 5)
            {
                // if unity version >= 5
                // then give option to forcefully generate plugins to support unity4
                // "unity4 supports" means plugins based on jar files in Assets/Plugins/
                bool tmp = GUILayout.Toggle(this.supportUnity4, MSG_CHECK_FORCE_UNITY4);
                if (tmp != this.supportUnity4)
                {
                    this.supportUnity4 = tmp;
                    EditorPrefs.SetBool(KEY_FLAG_FORCE_UNITY4, this.supportUnity4);
                }
            }
            else
            {
                // if unity version < 5
                // then automatically set "force unity4" flag to generate plugins to support unity4
                // "unity4 supports" means plugins based on jar files in Assets/Plugins/
                EditorGUI.BeginDisabledGroup(true);
                if (!this.supportUnity4)
                {
                    this.supportUnity4 = true;
                    EditorPrefs.SetBool(KEY_FLAG_FORCE_UNITY4, this.supportUnity4);
                }

                GUILayout.Toggle(this.supportUnity4, MSG_CHECK_FORCE_UNITY4);

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(MSG_LABEL_CLOSE_WINDOW,
                this.labelFieldStyle, GUILayout.Height(16));

            if (GUILayout.Button(MSG_BUTTON_CLOSE, GUILayout.Height(30)))
            {
                Close();
            }
        }

        public void OnDisable()
        {
            //EditorPrefs.DeleteKey(KEY_SERVICE_JSON_FILE_PATH);
            //EditorPrefs.DeleteKey(KEY_BUNDLE_FILE_PATH);
            //EditorPrefs.DeleteKey(KEY_FLAG_VERBOSE);
            //EditorPrefs.DeleteKey(KEY_FLAG_FORCE_UNITY4);
        }

        private bool InstallSdk(string serviceJsonFilePath, string bundleFilePath, bool forceToSupportUnity4)
        {
            SetProgress(0f);
            
            // init modules
            FileManager fm = new FileManager();
            if (!fm.InitializeBundle(bundleFilePath))
            {
                TerminateWithError("selected file is invalid bundle zip file: {0}", bundleFilePath);
                return false;
            }
            // Unity Gradle Build 에서 페키지명이 중복되던 문제를 해결하기 위해 이렇게 수정했습니다.
            string packageName = "com.nps.adiscope";
            /*
#if UNITY_5_6_OR_NEWER
            packageName = UnityEditor.PlayerSettings.applicationIdentifier;
#else
            packageName = UnityEditor.PlayerSettings.bundleIdentifier;
#endif*/
            ManifestHandler manifestHandler = new ManifestHandler(packageName);
            //adNetworkInfo_Settings[attrName].ToString()
            SetProgress(10f);

            // first of all, get settings from "service.json"
            // "service.json" is supposed to be downloaded from admin page
            // and placed as "Assets/AdiscopeSdk/Editor/service.json"
            Dictionary<string, object> serviceSettings = fm.ReadJsonFile(serviceJsonFilePath);
            if (serviceSettings == null)
            {
                TerminateWithError("can't get service setting from: {0}", serviceJsonFilePath);
                fm.Close();
                return false;
            }

            SetProgress(20f);

            // check version of service.json
            if (!CheckServiceJsonVersion(serviceSettings))
            {
                TerminateWithError(
                    "version not supported from: {0}, please use the latest version of AdiscopeSdk",
                    serviceJsonFilePath);
                fm.Close();
                return false;
            }

            // if everything is ready, start the configuration
            // set adiscope sdk core setting 
            if (!ConfigureAdiscope(serviceSettings, fm, manifestHandler, forceToSupportUnity4))
            {
                TerminateWithError("failed to configure adiscope sdk core");
                fm.Close();
                return false;
            }

            SetProgress(30f);

            // then configure ad network 
            if (!ConfigureAdNetwork(serviceSettings, fm, manifestHandler, forceToSupportUnity4))
            {
                TerminateWithError("failed to configure ad network");
                fm.Close();
                return false;
            }

            SetProgress(80f);

            // write AndroidManifest.xml file to temporary taret folder
            if (!manifestHandler.WriteXmlFile(fm.FilePathTempAndroidManifestXml))
            {
                TerminateWithError("can't write android manifest xml file");
                fm.Close();
                return false;
            }

            // replace ${applicationId} to applicationId
#if UNITY_5_6_OR_NEWER
            string applicationId = UnityEditor.PlayerSettings.applicationIdentifier;
#else
            string applicationId = UnityEditor.PlayerSettings.bundleIdentifier;
#endif

            string newManifest = File.ReadAllText(fm.FilePathTempAndroidManifestXml);
            newManifest = newManifest.Replace("${applicationId}", applicationId);
            File.WriteAllText(fm.FilePathTempAndroidManifestXml, newManifest);

            // write project.properties file to temporary target folder
            if (!fm.WriteProjectProperties())
            {
                TerminateWithError("can't write project properties file");
                fm.Close();
                return false;
            }

            SetProgress(90f);

            // if everything succeeded, move temp target folder to Assets/AdiscopeSdk/Plugins
            if (!fm.InstallFilesToAssetsDirectory())
            {
                TerminateWithError("can't install plugin files to assets");
                fm.Close();
                return false;
            }

            fm.Close();

            AssetDatabase.Refresh();

            SetProgress(100f);

            TerminateWithSuccess();

            return true;
        }

        private void UninstallSdk()
        {
            SetProgress(0f);

            FileManager fm = new FileManager();
            fm.DeleteInstalledFiles();

            SetProgress(80f);

            AssetDatabase.Refresh();

            SetProgress(100f);

            TerminateWithSuccess();
        }

        private bool CheckServiceJsonVersion(Dictionary<string, object> settings)
        {
            if (!settings.ContainsKey(SERVICE_JSON_KEY_VERSION) ||
                settings[SERVICE_JSON_KEY_VERSION] == null)
            {
                Logger.e("missing json key [{0}] from service setting", SERVICE_JSON_KEY_VERSION);
                return false;
            }

            int version = Convert.ToInt32(settings[SERVICE_JSON_KEY_VERSION]);

            Logger.d("service setting version: {0}", version);

            if (version < MIN_SERVICE_JSON_VERSION || version > MAX_SERVICE_JSON_VERSION)
            {
                Logger.e("service setting version must be {0} >= version >= {1}",
                    MAX_SERVICE_JSON_VERSION, MIN_SERVICE_JSON_VERSION);
                return false;
            }

            return true;
        }

        private bool ConfigureAdiscope(Dictionary<string, object> settings, FileManager fileManager,
            ManifestHandler manifestHandler, bool forceToSupportUnity4)
        {
            Logger.d("setting {0}", SERVICE_JSON_KEY_ADISCOPE);

            // adiscope example
            // "adiscope": {
            //   "ads": {
            //     "rewardedVideoAd": true,
            //     "offerwallAd": true
            //   },
            //   "settings": {
            //     "adiscope_app_id": "adiscope_ad_app_id_example"
            //   }
            // },
            if (!settings.ContainsKey(SERVICE_JSON_KEY_ADISCOPE) ||
                settings[SERVICE_JSON_KEY_ADISCOPE] == null)
            {
                Logger.e("missing json key [{0}] from service setting", SERVICE_JSON_KEY_ADISCOPE);

                return false;
            }
            Dictionary<string, object> adiscopeInfo =
                settings[SERVICE_JSON_KEY_ADISCOPE] as Dictionary<string, object>;
            if (!ConfigureAdDetail(ADISCOPE_CORE_FILE_NAME, false, adiscopeInfo, fileManager,
                manifestHandler, forceToSupportUnity4))
            {
                Logger.e("failed to configure settings for {0}", SERVICE_JSON_KEY_ADISCOPE);
                return false;
            }

            return true;
        }

        private bool ConfigureAdNetwork(Dictionary<string, object> settings,
            FileManager fileManager, ManifestHandler manifestHandler, bool forceToSupportUnity4)
        {
            // get network setting
            if (!settings.ContainsKey(SERVICE_JSON_KEY_AD_NETWORK) ||
                settings[SERVICE_JSON_KEY_AD_NETWORK] == null)
            {
                Logger.e("missing json key [{0}] from service setting",
                    SERVICE_JSON_KEY_AD_NETWORK);

                return false;
            }
            Dictionary<string, object> networkSettings =
                settings[SERVICE_JSON_KEY_AD_NETWORK] as Dictionary<string, object>;

            // iteration for each AdNetwork except Adiscope
            foreach (string adNetworkName in networkSettings.Keys)
            {
                // adNetworkName = adcolony, adpopcorn, applovin, appang, etc.
                Logger.d("setting AdNetwork: {0}", adNetworkName);

                // adNetworkInfo Example
                // "adcolony": {
                //   "ads": {
                //     "rewardedVideoAd": true,
                //     "offerwallAd": true
                //   },
                //   "settings": {
                //     "adcolony_app_id": "appbb468920e81440c4ae"
                //   }
                // },
                if (!networkSettings.ContainsKey(adNetworkName) ||
                    networkSettings[adNetworkName] == null)
                {
                    Logger.e("AdNetwork: {0} - missing setting info", adNetworkName);
                    return false;
                }
                Dictionary<string, object> adNetworkInfo =
                    networkSettings[adNetworkName] as Dictionary<string, object>;

                if (!ConfigureAdDetail(adNetworkName, true, adNetworkInfo, fileManager,
                    manifestHandler, forceToSupportUnity4))
                {
                    Logger.e("AdNetwork: {0} - failed to configure detailed information",
                        adNetworkName);
                    return false;
                }
            }

            return true;
        }

        private bool ConfigureAdDetail(string adName, bool isNetwork, 
            Dictionary<string, object> adNetworkInfo, FileManager fileManager,
            ManifestHandler manifestHandler, bool forceToSupportUnity4)
        {
            // adNetworkInfo_Ads contains enable/disable of reward video / offer wall ads
            //   "ads": {
            //     "offerwallAd": true,
            //     "rewardedVideoAd": true
            //   },

            // check MANDATORY "ads" field
            if (!adNetworkInfo.ContainsKey(SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS) ||
                adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS] == null)
            {
                Logger.e("missing {0} info", SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS);
                return false;
            }
            Dictionary<string, object> adNetworkInfo_Ads =
                adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS] as Dictionary<string, object>;

            // 20200622 @nmj 로직 변경.
            // AS-IS : service.json 에 필수값이 없으면 수행 중단함.
            // TO-BE : 클라든 서버든 필수값이 없으면 사용안함으로 처리.
            // whether rewarded video ad enabled
            bool rewardedVideoAdEnabled = false;
            if (adNetworkInfo_Ads.ContainsKey(SERVICE_JSON_KEY_REWARDED_VIDEO_AD) &&
                adNetworkInfo_Ads[SERVICE_JSON_KEY_REWARDED_VIDEO_AD] != null)
            {
                rewardedVideoAdEnabled =
                    Boolean.Parse(adNetworkInfo_Ads[SERVICE_JSON_KEY_REWARDED_VIDEO_AD].ToString());
            }

            // whether offer wall ad enabled
            bool offerwallAdEnabled = false;
            if (adNetworkInfo_Ads.ContainsKey(SERVICE_JSON_KEY_OFFERWALL_AD) &&
                adNetworkInfo_Ads[SERVICE_JSON_KEY_OFFERWALL_AD] != null)
            {
                offerwallAdEnabled =
                    Boolean.Parse(adNetworkInfo_Ads[SERVICE_JSON_KEY_OFFERWALL_AD].ToString());
            }

            // whether interstitial ad enabled
            bool interstitialAdEnabled = false;
            if (adNetworkInfo_Ads.ContainsKey(SERVICE_JSON_KEY_INTERSTITIAL_AD) &&
                adNetworkInfo_Ads[SERVICE_JSON_KEY_INTERSTITIAL_AD] != null)
            {
                interstitialAdEnabled =
                    Boolean.Parse(adNetworkInfo_Ads[SERVICE_JSON_KEY_INTERSTITIAL_AD].ToString());
            }


            Logger.d("network : {0}, RewardedVideoAd : {1}, OfferallAd: {2}, InterstitialAd: {3}", adName,
                (rewardedVideoAdEnabled ? "enabled" : "disabled"),
                (offerwallAdEnabled ? "enabled" : "disabled"),
                (interstitialAdEnabled ? "enabled" : "disabled"));

            // adNetworkInfoSettings contains extra key/value information like 
            // app_id, hash_key, etc depend on each ad network company
            //   "settings": {
            //     "adcolony_app_id": "appbb468920e81440c4ae"
            //   }

            // check OPTIONAL "settings" field
            Dictionary<string, object> adNetworkInfo_Settings = null;
            if (!adNetworkInfo.ContainsKey(SERVICE_JSON_KEY_AD_NETWORK_INFO_SETTINGS) ||
                adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_SETTINGS] == null)
            {
                Logger.w("missing {0} info", SERVICE_JSON_KEY_AD_NETWORK_INFO_SETTINGS);
                adNetworkInfo_Settings = new Dictionary<string, object>();
            }
            else
            {
                adNetworkInfo_Settings =
                    adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_SETTINGS]
                    as Dictionary<string, object>;
            }

            if (!rewardedVideoAdEnabled && !offerwallAdEnabled && !interstitialAdEnabled)
            {
                Logger.d("no need to set: {0}", adName);
                return true;
            }

            // read file "[ZippedLibraryFile]/adapter-xxx/ManifestSetting.json"
            Dictionary<string, object> adapterManifestSetting =
                fileManager.ReadManifestSettingJsonFromBundleFile(adName, isNetwork);
            if (adapterManifestSetting == null)
            {
                Logger.i("failed to get manifest setting from sdk bundle zip file: {0}", adName);
                // 20200622 @nmj 로직 변경. 서버든 클라든 둘다 데이타가 없는 경우는 skip 처리함.
                return true;
            }

            // 1. check Common Section
            if (!SetManifestSettingsCommon(
                manifestHandler, adNetworkInfo_Settings, adapterManifestSetting))
            {
                Logger.e("failed to set common configuration");
                return false;
            }

            // 2. check Rewarded Video Ad
            if (rewardedVideoAdEnabled)
            {
                if (!SetManifestSettingsRewardedVideoAd(
                    manifestHandler, adNetworkInfo_Settings, adapterManifestSetting))
                {
                    Logger.e("failed to set rewardedVideoAd configuration");
                    return false;
                }
            }

            // 3. check Offerwall Ad
            if (offerwallAdEnabled)
            {
                if (!SetManifestSettingsOfferwallAd(
                    manifestHandler, adNetworkInfo_Settings, adapterManifestSetting))
                {
                    Logger.e("failed to set offerwallAd configuration");
                    return false;
                }
            }

            // 4. check Interstitial Ad
            if (interstitialAdEnabled)
            {
                if (!SetManifestSettingsInterstitialAd(
                    manifestHandler, adNetworkInfo_Settings, adapterManifestSetting))
                {
                    Logger.e("failed to set interstitialAd configuration");
                    return false;
                }
            }

            // adName 이 admoob이거나 admanager 이면 admanager 는 별도로 aar 이 존재 하지 않으므로 admob 으로 파일을 생성 해주어야함
            if (adName == "admanager" || adName == "admob")
            {
                if (!fileManager.CopyBinaryFilesToTempDirectory(
                "admob", isNetwork, forceToSupportUnity4))
                {
                    Logger.e("failed to copy binary files to temp directory");
                    return false;
                }
                return true;
            }

            if (!fileManager.CopyBinaryFilesToTempDirectory(
                adName, isNetwork, forceToSupportUnity4))
            {
                Logger.e("failed to copy binary files to temp directory");
                return false;
            }

            if (!fileManager.CopyAdditionalLibFilesToTempDirectory(this, adName, additionalLibNetwork))
            {
                Logger.e("failed to copy additionalLib files to temp directory");
                return false;
            }

            return true;
        }

        private bool SetManifestSettingsCommon(
            ManifestHandler manifestHandler,
            Dictionary<string, object> adNetworkInfo_Settings,
            Dictionary<string, object> adapterManifestSetting)
        {
            return SetManifestSettings(
                ADAPTER_MANIFEST_KEY_SECTION_COMMON,
                manifestHandler,
                adNetworkInfo_Settings,
                adapterManifestSetting);
        }

        private bool SetManifestSettingsRewardedVideoAd(
            ManifestHandler manifestHandler,
            Dictionary<string, object> adNetworkInfo_Settings,
            Dictionary<string, object> adapterManifestSetting)
        {
            return SetManifestSettings(
                ADAPTER_MANIFEST_KEY_SECTION_REWARDED_VIDEO_AD,
                manifestHandler,
                adNetworkInfo_Settings,
                adapterManifestSetting);
        }

        private bool SetManifestSettingsOfferwallAd(
            ManifestHandler manifestHandler,
            Dictionary<string, object> adNetworkInfo_Settings,
            Dictionary<string, object> adapterManifestSetting)
        {
            return SetManifestSettings(
                ADAPTER_MANIFEST_KEY_SECTION_OFFERWALL_AD,
                manifestHandler,
                adNetworkInfo_Settings,
                adapterManifestSetting);
        }

        private bool SetManifestSettingsInterstitialAd(
            ManifestHandler manifestHandler,
            Dictionary<string, object> adNetworkInfo_Settings,
            Dictionary<string, object> adapterManifestSetting)
        {
            return SetManifestSettings(
                ADAPTER_MANIFEST_KEY_SECTION_INTERSTITIAL_AD,
                manifestHandler,
                adNetworkInfo_Settings,
                adapterManifestSetting);
        }

        private bool SetManifestSettings(
            string sectionName,
            ManifestHandler manifestHandler,
            Dictionary<string, object> adNetworkInfo_Settings,
            Dictionary<string, object> adapterManifestSetting)
        {
            Logger.d("configuring {0} section", sectionName);

            if (!adapterManifestSetting.ContainsKey(sectionName) ||
                adapterManifestSetting[sectionName] == null)
            {
                // It have to be ignored
                return true;
            }
            
            if (adNetworkInfo_Settings.ContainsKey("adiscope_media_id"))
            {
                manifestHandler.mediaId = adNetworkInfo_Settings["adiscope_media_id"].ToString();
            }
            if (adNetworkInfo_Settings.ContainsKey("adiscope_media_secret"))
            {
                manifestHandler.mediaSecret = adNetworkInfo_Settings["adiscope_media_secret"].ToString();
            }
            if (adNetworkInfo_Settings.ContainsKey("adiscope_sub_domain"))
            {
                manifestHandler.subDomain = adNetworkInfo_Settings["adiscope_sub_domain"].ToString();
            }

            Dictionary<string, object> sectionSettings =
                adapterManifestSetting[sectionName] as Dictionary<string, object>;

            //QUERIES
            if (sectionName == ADAPTER_MANIFEST_KEY_SECTION_COMMON)
            {
                if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_QUERIES) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_QUERIES] == null)
                {
                    Logger.w("missing {0}/{1} part in manifest setting",
                         sectionName, ADAPTER_MANIFEST_KEY_FIELD_QUERIES);
                } else
                {
                    List<object> queries = sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_QUERIES] as List<object>;
                    foreach (object query in queries)
                    {
                        Logger.d("{0}/{1}: {2}",
                            sectionName, ADAPTER_MANIFEST_KEY_FIELD_QUERIES, query);

                        if (query != null && query.ToString().Length > 0)
                        {
                            if (!manifestHandler.AddAction(query.ToString()))
                            {
                                Logger.e("failed to add permission: {0}", query);
                                return false;
                            }
                        }
                    }
                }
            }


            // PERMISSIONS
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS] == null)
            {
                Logger.e("missing {0}/{1} part in manifest setting",
                     sectionName, ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS);
                return false;
            }

            List<object> permissions =
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS] as List<object>;
            foreach (object permission in permissions)
            {
                Logger.d("{0}/{1}: {2}",
                    sectionName, ADAPTER_MANIFEST_KEY_FIELD_PERMISSIONS, permission);

                if (permission != null && permission.ToString().Length > 0)
                {
                    if (!manifestHandler.AddPermission(permission.ToString()))
                    {
                        Logger.e("failed to add permission: {0}", permission);
                        return false;
                    }
                }
            }

            // ACTIVITY
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES] == null)
            {
                Logger.e("missing {0}/{1} part in manifest setting",
                    sectionName, ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES);
                return false;
            }

            

            List<object> activities =
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES] as List<object>;
            foreach (object activity in activities)
            {
                Logger.d("----------------{0}/{1}: {2}",
                    sectionName, ADAPTER_MANIFEST_KEY_FIELD_ACTIVITIES, activity);

                if (activity != null && activity.ToString().Length > 0)
                {
                    if (!manifestHandler.AddActivity(activity.ToString()))
                    {
                        Logger.e("failed to add activity: {0}", activity);
                        return false;
                    }
                }
            }

            // METADATA
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_META_DATA) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_META_DATA] == null)
            {
                Logger.e("missing {0}/{1} part in manifest setting",
                    sectionName, ADAPTER_MANIFEST_KEY_FIELD_META_DATA);
                return false;
            }

            List<object> metaDatas =
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_META_DATA] as List<object>;
            foreach (object metadata in metaDatas)
            {
                Logger.d("{0}/{1}: {2}",
                    sectionName, ADAPTER_MANIFEST_KEY_FIELD_META_DATA, metadata);

                if (metadata != null && metadata.ToString().Length > 0)
                {
                    // if meta-data has place_holder_* in its android:value,
                    // replace place_holder_* with value from service.json
                    // if there's no matched value in service.json, return error.
                    string attrName;
                    string attrValue;

                    if (!manifestHandler.GetXmlMetadataAttrNameAndValue(
                        metadata.ToString(), out attrName, out attrValue))
                    {
                        Logger.e("failed to get name/value from meta-data: {0}", metadata);
                        return false;
                    }

                    string metaDataToAdd = null;

                    // value has to be set by parameter in service.json
                    if (attrValue.Contains("place_holder_"))
                    {
                        Logger.d("meta-data android:name={0} android:value={1}",
                            attrName, attrValue);

                        if (adNetworkInfo_Settings.ContainsKey(attrName) &&
                            adNetworkInfo_Settings[attrName].ToString().Length > 0)
                        {
                            Logger.d("new meta-data android:value={0}",
                                adNetworkInfo_Settings[attrName].ToString());
                            metaDataToAdd = metadata.ToString().Replace(
                                attrValue,
                                adNetworkInfo_Settings[attrName].ToString());
                        }
                        else
                        {
                            // 이미 meta-data 가 존재 하더라도 그것이 core의 google application key 이면 중복해서 입력 가능하도록 처리한다.
                            if (adapterManifestSetting[SERVICE_JSON_KEY_AD_NETWORK] as string == "core" && attrName == "com.google.android.gms.ads.APPLICATION_ID")
                            {
                                return true;
                            }
                            Logger.e("missing meta-data: [{0}] from service setting", attrName);
                            return false;
                        }
                    }
                    else
                    {
                        metaDataToAdd = metadata.ToString();
                    }

                    // 만약에 현재 세팅하려는 값이 애드메니져의 것이고 이미 애드몹의 키가 세팅 되어 있다면 admob의 키를 따라야 한다.
                    if (adapterManifestSetting[SERVICE_JSON_KEY_AD_NETWORK] as string == "admanager")
                    {
                        if (!isInstalledAdmobMeta)
                        {
                            if (!manifestHandler.AddMetaData(metaDataToAdd))
                            {
                                Logger.e("failed to add meta-data: {0}", metadata);
                                return false;
                            }
                        }
                        return true;
                    }
                    // then add to manifest
                    if (!manifestHandler.AddMetaData(metaDataToAdd))
                    {
                        Debug.Log("metaDataToAdd failed : " + metaDataToAdd);
                        Logger.e("failed to add meta-data: {0}", metadata);
                        return false;
                    }
                    else
                    {
                        if (adapterManifestSetting[SERVICE_JSON_KEY_AD_NETWORK] as string == "admob")
                        {
                            isInstalledAdmobMeta = true;
                        }
                    }
                }
            }

            // RECEIVER
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_RECEIVERS) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_RECEIVERS] == null)
            {
                // nothing. this is optional
            }
            else
            {
                List<object> receivers =
                    sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_RECEIVERS] as List<object>;
                foreach (object receiver in receivers)
                {
                    Logger.d("{0}/{1}: {2}",
                        sectionName, ADAPTER_MANIFEST_KEY_FIELD_RECEIVERS, receiver);

                    if (receiver != null && receiver.ToString().Length > 0)
                    {
                        if (!manifestHandler.AddReceiver(receiver.ToString()))
                        {
                            Logger.e("failed to add receiver: {0}", receiver);
                            return false;
                        }
                    }
                }
            }

            // SERVICE
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_SERVICES) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_SERVICES] == null)
            {
                // nothing. this is optional
            }
            else
            {
                List<object> services =
                    sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_SERVICES] as List<object>;
                foreach (object service in services)
                {
                    Logger.d("{0}/{1}: {2}",
                        sectionName, ADAPTER_MANIFEST_KEY_FIELD_SERVICES, service);

                    if (service != null && service.ToString().Length > 0)
                    {
                        if (!manifestHandler.AddService(service.ToString()))
                        {
                            Logger.e("failed to add service: {0}", service);
                            return false;
                        }
                    }
                }
            }

            // PROVIDER
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_PROVIDERS) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_PROVIDERS] == null)
            {
                // nothing. this is optional
            }
            else
            {
                List<object> providers =
                    sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_PROVIDERS] as List<object>;
                foreach (object provider in providers)
                {
                    Logger.d("{0}/{1}: {2}",
                        sectionName, ADAPTER_MANIFEST_KEY_FIELD_PROVIDERS, provider);

                    if (provider != null && provider.ToString().Length > 0)
                    {
                        if (!manifestHandler.AddProvider(provider.ToString()))
                        {
                            Logger.e("failed to add provider: {0}", provider);
                            return false;
                        }
                    }
                }
            }

            // USES_LIBRARY
            if (!sectionSettings.ContainsKey(ADAPTER_MANIFEST_KEY_FIELD_USES_LIBRARYS) ||
                sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_USES_LIBRARYS] == null)
            {
                // nothing. this is optional
            }
            else
            {
                List<object> usesLibrarys =
                    sectionSettings[ADAPTER_MANIFEST_KEY_FIELD_USES_LIBRARYS] as List<object>;
                foreach (object usesLibrary in usesLibrarys)
                {
                    Logger.d("{0}/{1}: {2}",
                        sectionName, ADAPTER_MANIFEST_KEY_FIELD_USES_LIBRARYS, usesLibrary);

                    if (usesLibrary != null && usesLibrary.ToString().Length > 0)
                    {
                        if (!manifestHandler.AddUsesLibrary(usesLibrary.ToString()))
                        {
                            Logger.e("failed to add usesLibrary: {0}", usesLibrary);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void TerminateWithSuccess()
        {
            if (this.noGui)
            {
                Logger.d("invokded from CLI, suppress UI - TerminateWithSuccess");
                return;
            }

            EditorUtility.DisplayDialog(EDITOR_WINDOW_NAME, "Done", "Close");
            EditorUtility.ClearProgressBar();
        }

        private void TerminateWithError(string format, params object[] args)
        {
            Logger.e(format, args);

            if (this.noGui)
            {
                Logger.d("invokded from CLI, suppress UI - TerminateWithError");
                return;
            }

            EditorUtility.DisplayDialog(EDITOR_WINDOW_NAME,
                "Failed to update settings. Please check the console log.", "Close");

            EditorUtility.ClearProgressBar();

            return;
        }

        private void SetProgress(float progress)
        {
            if (this.noGui)
            {
                Logger.d("invokded from CLI, suppress UI - SetProgress");
                return;
            }

            EditorUtility.DisplayProgressBar(EDITOR_WINDOW_NAME, MSG_PROGRESS_BAR, progress);
        }

        private static string GetArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        private void reMakeDetailsNetworkData()
        {
            usedNetworks = new List<string>();
            additionalLibNetwork = null;

            if (this.svcJsonFilePath == null || this.svcJsonFilePath.Length == 0)
            {
                return;
            }

            if (this.zipFilePath == null || this.zipFilePath.Length == 0)
            {
                return;
            }

            FileManager fm = new FileManager();
            if (!fm.InitializeBundle(zipFilePath))
            {
                fm.Close();
                return;
            }

            additionalLibNetwork = fm.ReadAdditionalLibFromZip();

            Dictionary<string, object> serviceSettings = fm.ReadJsonFile(svcJsonFilePath);
            if (serviceSettings == null)
            {
                fm.Close();
                return;
            }

            if (!serviceSettings.ContainsKey(SERVICE_JSON_KEY_AD_NETWORK) ||
                serviceSettings[SERVICE_JSON_KEY_AD_NETWORK] == null)
            {
                fm.Close();
                return;
            }

            Dictionary<string, object> networkSettings =
                serviceSettings[SERVICE_JSON_KEY_AD_NETWORK] as Dictionary<string, object>;

            foreach (string adNetworkName in networkSettings.Keys)
            {
                if (networkSettings[adNetworkName] == null)
                {
                    continue;
                }

                Dictionary<string, object> adNetworkInfo =
                    networkSettings[adNetworkName] as Dictionary<string, object>;

                if (adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS] == null)
                {
                    continue;
                }

                Dictionary<string, object> adNetworkInfo_Ads =
                    adNetworkInfo[SERVICE_JSON_KEY_AD_NETWORK_INFO_ADS] as Dictionary<string, object>;

                if (adNetworkInfo_Ads[SERVICE_JSON_KEY_REWARDED_VIDEO_AD] == null)
                {
                    continue;
                }

                bool rewardedVideoAdEnabled = Boolean.Parse(adNetworkInfo_Ads[SERVICE_JSON_KEY_REWARDED_VIDEO_AD].ToString());
                if (rewardedVideoAdEnabled)
                {
                    usedNetworks.Add(adNetworkName);
                }
            }

            usedNetworks.Sort();

            fm.Close();

        }

        public string makeDNKey(string networkName, string libraryName)
        {
            string ver = "v1";
            if (additionalLibNetworkSettingVer.ContainsKey(networkName))
            {
                ver = additionalLibNetworkSettingVer[networkName];
            }
            return string.Format("DN_{0}_{1}_{2}", networkName, ver, libraryName);
        }

    }
}
