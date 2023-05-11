/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Adiscope.Editor
{
    /// <summary>
    /// Manager class to handle multiple direcoty information and file operation
    /// </summary>
    public class FileManager
    {
        #region CONST VARIABLES
        private const string PATH_KEY_TARGET_PATH = "targetPath";
        private const string PATH_KEY_PLUGIN_ANDROID_SOURCE_PATH = "pluginAndroidSourcePath";

        private const string EXTRAS_KEY_ROOT = "actions";
        private const string EXTRAS_KEY_ACTION = "action";
        private const string EXTRAS_KEY_TARGET = "target";
        private const string EXTRAS_KEY_TARGET_TO = "to";
        private const string EXTRAS_ACTION_COPY = "copy";

        private const string PATH_JSON_FILE_NAME = "path.json";
        private const string EXTRAS_JSON_FILE_NAME = "actions.json";
        private const string ANDROID_MANIFEST_XML_FILE_NAME = "AndroidManifest.xml";
        private const string PROJECT_PROPERTIES_FILE_NAME = "project.properties";
        private const string CLASS_JAR_FILE_NAME = "classes.jar";
        private const string VALUES_XML_FILE_NAME = "values.xml";
        private const string INSTALLATION_LIST_FILE_NAME = "AdiscopeInstalledFileList.json";

        private const string ADAPTER_PREFIX = "adapter.";
        private const string EXTRAS_PREFIX = "extras-";
        private const string EXTRAS_POSTFIX = "-extras";

        private const string EXT_JSON_FILE = ".json";
        private const string EXT_AAR_FILE = ".aar";
        private const string EXT_JAR_FILE = ".jar";
        private const string EXT_META_FILE = ".meta";
        private const string EXT_ZIP_FILE = ".zip";
        private const string WILDCARD = "*";

        private const string TEMP_ASSETS_DIR_NAME = "AdiscopeTempFiles";
        private const string EDITOR_DIR_NAME = "Editor";
        private const string PLUGINS_DIR_NAME = "Plugins";
        private const string ANDROID_DIR_NAME = "Android";
        private const string ADDITIONALLIB_PREFIX_NAME = "additionalLib";
        private const string LIBS_DIR_NAME = "libs";
        private const string RES_DIR_NAME = "res";
        private const string VALUES_DIR_NAME = "values";
        private const string JNI_DIR_NAME = "jni";
        private const string ASSETS_DIR_NAME = "assets";

        private const string PROJECT_PROPERTIES_CONTENT = "android.library=true";

        private const string MSG_NOT_INITIALIZED = "need to be initialized - no library zip file";
        #endregion

        #region PROPERTIES
        /// <summary>
        /// C:/Users/[UserName]/AppData/Local/Temp/[CompanyName]/[ProjectName]/AdiscopeTempFiles
        /// </summary>
        private string DirPathTempAssets { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Adiscope
        /// </summary>
        private string DirPathTempAssetsTarget { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android/Adiscope
        /// for AndroidManifest and project.properties
        /// </summary>
        private string DirPathTempAssetsPluginsAndroidTarget { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android/Adiscope/libs
        /// </summary>
        private string DirPathTempAssetsPluginsAndroidTargetLibs { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android/libs (for Unity4)
        /// </summary>
        private string DirPathTempAssetsPluginsAndroidLibs { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android
        /// </summary>
        private string DirPathTempAssetsPluginsAndroid { get; set; }

        /// <summary>
        /// [UnityProject]/Assets
        /// </summary>
        private string DirPathAssets { get; set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android/Adiscope/AndroidManifest.xml
        /// </summary>
        public string FilePathTempAndroidManifestXml { get; private set; }

        /// <summary>
        /// TempAssetsDirPath/Plugins/Android/Adiscope/project.properties
        /// </summary>
        private string FilePathTempProjectProperties { get; set; }

        /// <summary>
        /// [UnityProject]/Assets/Adiscope/Editor/InstalledPluginList.json
        /// </summary>
        private string FilePathInstalledPluginFileList { get; set; }
        #endregion

        private string targetPath, pluginAndroidSrcPath;
        private ZipFile sdkArchiveFile;

        public FileManager()
        {
        }

        #region public methods

        public static void RemoveDirectory(string path) {
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (FileInfo file in directory.EnumerateFiles()) { file.Delete(); }
            foreach (DirectoryInfo dir in directory.EnumerateDirectories()) { dir.Delete(true); }
            Directory.Delete(path);
        }

        public static void ExtractTar(String tarFileName, String destFolder) {
            Stream inStream = File.OpenRead(tarFileName);

            ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(inStream, System.Text.Encoding.UTF8);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            inStream.Close();
        }

        public static void ExtractZip(string filename, string extractDirectory)
            {
                ICSharpCode.SharpZipLib.Zip.FastZip fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                fastZip.ExtractZip(filename, extractDirectory, null);
        }

        /// <summary>
        /// init file manager for installation
        /// check the bundle file is valide zip and
        /// installation path from path.json in the bundle file
        /// </summary>
        /// <param name="zipFilePath">zipped sdk bundle file path</param>
        /// <returns>true on success, false on failure</returns>
        public bool InitializeBundle(string zipFilePath)
        {
            // open zip entry
            try
            {
                this.sdkArchiveFile = ZipFile.Read(zipFilePath);
            }
            catch (Exception e)
            {
                Logger.e("can't initialize file manager");
                Logger.e(e);
                return false;
            }

            if (!ReadPathInfo())
            {
                Logger.e("can't get directory information");
                return false;
            }

            if (!SetPaths())
            {
                Logger.e("can't create internal directory");
                return false;
            }

            ClearTempDirectory();

            return true;
        }

        /// <summary>
        /// copy necessary binary files to temp directory
        /// </summary>
        /// <param name="adName">ad company name</param>
        /// <param name="isNetwork">is this third party or not</param>
        /// <param name="supportUnity4">generate binary files for unity4</param>
        /// <returns>true on success, false on failure</returns>
        public bool CopyBinaryFilesToTempDirectory(string adName, bool isNetwork, bool supportUnity4)
        {
            if (supportUnity4)
            {
                if (!ExtractJARFilesToTempDirectory(adName, isNetwork))
                {
                    Logger.e("failed to copy jar files of {0}", adName);
                    return false;
                }
            }
            else
            {
                if (!ExtractAARFilesToTempDirectory(adName, isNetwork))
                {
                    Logger.e("failed to copy aar files of {0}", adName);
                    return false;
                }
            }

            // do extra files job if exists
            if (!ExtractExtraFilesToTempDirectory(adName, isNetwork))
            {
                Logger.e("failed to copy extra files of {0}", adName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// create and write project.properties file
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool WriteProjectProperties()
        {
            if (!CreateParentDirectory(this.FilePathTempProjectProperties))
            {
                Logger.e("can't create parent directory. file: {0}",
                    this.FilePathTempProjectProperties);
                return false;
            }

            try
            {
                File.WriteAllText(this.FilePathTempProjectProperties, PROJECT_PROPERTIES_CONTENT);
            }
            catch (Exception e)
            {
                Logger.e("failed to write file: {0}", this.FilePathTempProjectProperties);
                Logger.e(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// install files in temp to unity's assets directory
        /// also generates installed file list log file
        /// </summary>
        /// <returns></returns>
        public bool InstallFilesToAssetsDirectory()
        {
            // Uninstall previouse
            Logger.i("try to uninstall previous installation if exists");
            DeleteInstalledFiles();

            Logger.i("copying files to assets...");
            // Copy Files
            if (!CopyDirectory(this.DirPathTempAssets, this.DirPathAssets, true))
            {
                Logger.e("failed to copy temp directory to assets");
                return false;
            }

            // Log Installed files list
            LogInstalledFiles();

            // Delete Temp Files
            ClearTempDirectory();

            Logger.i("installation done");

            return true;
        }

        /// <summary>
        /// read json file as dictionary
        /// </summary>
        /// <param name="filePath">json file path</param>
        /// <returns>json data map</returns>
        public Dictionary<string, object> ReadJsonFile(string filePath)
        {
            // check file path
            if (filePath == null || filePath.Length == 0)
            {
                Logger.e("invalid file path");
                return null;
            }

            // Read json string from jsonFilePath
            string jsonString = string.Empty;
            try
            {
                jsonString = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Logger.e("can't read file: {0}", filePath);
                Logger.e(e);
                return null;
            }

            if (jsonString == null || jsonString.Length == 0)
            {
                Logger.e("content not exists");
                return null;
            }

            // Parse the contents
            Dictionary<string, object> settings = null;
            try
            {
                settings = MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
            }
            catch (Exception e)
            {
                Logger.e("parsing json string error: {0}", jsonString);
                Logger.e(e);
                return null;
            }

            if (settings == null)
            {
                Logger.e("parsing failed, returned null");
                return null;
            }

            return settings;
        }

        /// <summary>
        /// read manifeset setting json in zip as dictionary
        /// </summary>
        /// <param name="adName">ad company name</param>
        /// <param name="isNetwork">is this third party or not</param>
        /// <returns>json data map</returns>
        public Dictionary<string, object> ReadManifestSettingJsonFromBundleFile(string adName, bool isNetwork)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return null;
            }

            // adName: adiscope, adapter-adcolony, adpater-adpopcorn etc.
            string filePathInZip;
            if (!isNetwork)
            {   // adiscope.json
                filePathInZip = GetCombinedPath(
                    this.pluginAndroidSrcPath, adName + EXT_JSON_FILE);
            }
            else
            {   // adapter-xxxx.json
                filePathInZip = GetCombinedPath(
                    this.pluginAndroidSrcPath, ADAPTER_PREFIX + adName + EXT_JSON_FILE);
            }

            return ReadJsonFileFromZip(filePathInZip, this.sdkArchiveFile);
        }

        /// <summary>
        /// close zip file. call this method if InitializeBundle is called
        /// </summary>
        public void Close()
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return;
            }

            this.sdkArchiveFile.Dispose();
        }

        /// <summary>
        /// delete files in installed plugins file list
        /// then delete empty dir in assets
        /// </summary>
        public void DeleteInstalledFiles()
        {
            // if installed file list exists, delete files in it
            // or list not exists, don't delete files
            string listFilePath = string.Empty;
            string[] listFiles = Directory.GetFiles(
                Application.dataPath, INSTALLATION_LIST_FILE_NAME,
                SearchOption.AllDirectories);

            if (listFiles.Length == 0)
            {
                Logger.e("can't find log file {0}", INSTALLATION_LIST_FILE_NAME);
                Logger.e("uninstall canceled");
                return;
            }
            else if (listFiles.Length >= 2)
            {
                Logger.e("multiple {0} files found", INSTALLATION_LIST_FILE_NAME);
                Logger.e("uninstall canceled. please check the current settings");
                return;
            }
            else
            {
                listFilePath = listFiles[0];
            }

            if (File.Exists(listFilePath))
            {
                List<object> files;

                try
                {
                    string jsonStr = File.ReadAllText(listFilePath);
                    files = MiniJSON.Json.Deserialize(jsonStr) as List<object>;
                }
                catch (Exception e)
                {
                    Logger.e("can't read installed file list from: {0}",
                        this.FilePathInstalledPluginFileList);
                    Logger.e(e);
                    return;
                }

                foreach (object file in files)
                {
                    if (file != null && file is string)
                    {
                        try
                        {
                            string fullFilePath =
                                GetCombinedPath(Application.dataPath, file.ToString());
                            File.Delete(fullFilePath);
                            File.Delete(fullFilePath + EXT_META_FILE);
                        }
                        catch (Exception e)
                        {
                            if (e is DirectoryNotFoundException)
                            {
                                Logger.w("file not exists: {0}", file);
                                Logger.w(e);
                            }
                            else
                            {
                                Logger.e("can't delete installed file: {0}", file);
                                Logger.e(e);
                                return;
                            }
                        }
                    }
                    else
                    {
                        Logger.w("file list info has invalid entry: {0}", file);
                    }
                }

                // Check empty directory and delete it
                DeleteEmptyDirectories(Application.dataPath);
            }
            else
            {
                // if not, delete only Assets/Plugins/Android/AdiscopeSdk
                Logger.i("install log file not exists: {0}", listFilePath);
            }
        }

        /// <summary>
        /// print all entries in bundle file
        /// </summary>
        public void PrintZipAll()
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return;
            }

            foreach (ZipEntry entry in this.sdkArchiveFile)
            {
                Logger.d("zip entry file name: {0}", entry.FileName);
            }
        }

        /// <summary>
        /// print entires in a directory inside of bundle file
        /// </summary>
        /// <param name="directoryEntryName"></param>
        public void PrintZipDirectory(string directoryEntryName)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return;
            }

            foreach (ZipEntry entry in
                this.sdkArchiveFile.SelectEntries(WILDCARD, directoryEntryName))
            {
                Logger.d("zip entry file name: {0}", entry.FileName);
            }
        }

        /// <summary>
        /// get open source library information from bundle zip file
        /// </summary>
        /// <returns>open source library information</returns>
        public Dictionary<string, object> ReadAdditionalLibFromZip()
        {

            List<string> additionalLibNetwork = new List<string>();
            string pathTemp = GetCombinedPath(
                PLUGINS_DIR_NAME, ANDROID_DIR_NAME, ADDITIONALLIB_PREFIX_NAME + WILDCARD);

            foreach (ZipEntry entry in this.sdkArchiveFile.SelectEntries(pathTemp))
            {
                if (entry.IsDirectory)
                {
                    String nameTemp = entry.FileName.TrimEnd('/');
                    int idxTemp = nameTemp.LastIndexOf('.') + 1;
                    nameTemp = nameTemp.Substring(idxTemp, nameTemp.Length - idxTemp);
                    additionalLibNetwork.Add(nameTemp);
                }
            }

            Dictionary<string, object> DicAdditionalLib = new Dictionary<string, object>();
            foreach (string networkName in additionalLibNetwork)
            {

                pathTemp = GetCombinedPath(
                    PLUGINS_DIR_NAME, ANDROID_DIR_NAME, ADDITIONALLIB_PREFIX_NAME + "." + networkName);

                List<string> libraryNames = new List<string>();
                pathTemp += WILDCARD;
                foreach (ZipEntry entry in this.sdkArchiveFile.SelectEntries(pathTemp))
                {
                    if (!entry.IsDirectory)
                    {
                        String nameTemp = entry.FileName;
                        int idxTemp = nameTemp.LastIndexOf('/') + 1;
                        nameTemp = nameTemp.Substring(idxTemp, nameTemp.Length - idxTemp);
                        libraryNames.Add(nameTemp);
                    }
                }
                libraryNames.Sort();
                DicAdditionalLib[networkName] = libraryNames;
            }

            return DicAdditionalLib;
        }

        /// <summary>
        /// ToString override method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retStr = string.Empty;
            System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties(
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static);

            retStr += "FileManager{";

            int cnt = 0;
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                string name = property.Name;
                object value = property.GetValue(this, null);

                if (cnt > 0)
                {
                    retStr += ", ";
                }

                retStr += string.Format("{0}=\"{1}\"", name, value);
                cnt++;
            }

            retStr += "}";

            return retStr;
        }
        #endregion

        #region private methods
        private bool ReadPathInfo()
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return false;
            }

            // check path
            Dictionary<string, object> paths = ReadJsonFileFromZip(PATH_JSON_FILE_NAME, this.sdkArchiveFile);
            if (paths == null)
            {
                Logger.e("can't get path information from bundle file: {0}", PATH_JSON_FILE_NAME);
                return false;
            }

            /* 
                path.json example
                {
                    "targetPath": "Adiscope",
                    "sdkSourcePath": "Sdk",
                    "pluginAndroidSourcePath": "Plugins/Android",
                    "pluginIOSSourcePath": "Plugins/iOS",
                }
             */

            if (!paths.ContainsKey(PATH_KEY_TARGET_PATH))
            {
                Logger.e("can't get paths information: {0}", PATH_KEY_TARGET_PATH);
                return false;
            }
            else if (!paths.ContainsKey(PATH_KEY_PLUGIN_ANDROID_SOURCE_PATH))
            {
                Logger.e("can't get paths information: {0}", PATH_KEY_PLUGIN_ANDROID_SOURCE_PATH);
                return false;

            }

            this.targetPath = paths[PATH_KEY_TARGET_PATH] as string;
            this.pluginAndroidSrcPath = paths[PATH_KEY_PLUGIN_ANDROID_SOURCE_PATH] as string;

            return true;
        }

        private bool SetPaths()
        {
            this.DirPathTempAssets = GetCombinedPath(
                 Application.temporaryCachePath, TEMP_ASSETS_DIR_NAME);

            this.DirPathTempAssetsTarget = GetCombinedPath(this.DirPathTempAssets, this.targetPath);

            this.DirPathTempAssetsPluginsAndroidTarget = GetCombinedPath(
                this.DirPathTempAssets, PLUGINS_DIR_NAME, ANDROID_DIR_NAME, string.Format("{0}.{1}", this.targetPath, "androidlib"));

            this.DirPathTempAssetsPluginsAndroidTargetLibs = GetCombinedPath(
                this.DirPathTempAssetsPluginsAndroidTarget, LIBS_DIR_NAME);

            this.DirPathAssets = Application.dataPath;

            this.FilePathTempAndroidManifestXml = GetCombinedPath(
                this.DirPathTempAssetsPluginsAndroidTarget, ANDROID_MANIFEST_XML_FILE_NAME);

            this.FilePathTempProjectProperties = GetCombinedPath(
                this.DirPathTempAssetsPluginsAndroidTarget, PROJECT_PROPERTIES_FILE_NAME);

            this.FilePathInstalledPluginFileList = GetCombinedPath(Application.dataPath,
                this.targetPath, EDITOR_DIR_NAME, INSTALLATION_LIST_FILE_NAME);

            this.DirPathTempAssetsPluginsAndroidLibs = GetCombinedPath(
                this.DirPathTempAssets, PLUGINS_DIR_NAME, ANDROID_DIR_NAME, LIBS_DIR_NAME);

            this.DirPathTempAssetsPluginsAndroid = GetCombinedPath(
                this.DirPathTempAssets, PLUGINS_DIR_NAME, ANDROID_DIR_NAME);
            Logger.d("SetPaths: {0}", ToString());

            return true;
        }

        private bool ClearTempDirectory()
        {
            // clear temporary folder
            string dirToDelete = this.DirPathTempAssets;
            try
            {
                Directory.Delete(dirToDelete, true);
            }
            catch (Exception e)
            {
                if (e is DirectoryNotFoundException)
                {
                    Logger.w("temp root directory not exists: {0}", dirToDelete);
                    Logger.w(e);
                }
                else
                {
                    Logger.e("can't delete temp root directory: {0}", dirToDelete);
                    Logger.e(e);
                    return false;
                }
            }

            return true;
        }

        private bool CreateParentDirectory(string filePath)
        {
            string parentDirectory;

            try
            {
                parentDirectory = Path.GetDirectoryName(filePath);
            }
            catch (Exception e)
            {
                Logger.e("can't get parent directory name. filePath: {0}", filePath);
                Logger.e(e);
                return false;
            }

            try
            {
                Directory.CreateDirectory(parentDirectory);
            }
            catch (Exception exception)
            {
                Logger.e("can't create directory: {0}", parentDirectory);
                Logger.e(exception);
                return false;
            }

            return true;
        }

        private bool ExtractAARFilesToTempDirectory(string adName, bool isNetwork)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return false;
            }

            if (adName == null)
            {
                Logger.e("invalid ad network name");
                return false;
            }

            string fileNamePrefix;
            if (isNetwork)
            {
                fileNamePrefix = ADAPTER_PREFIX + adName;
            }
            else
            {
                fileNamePrefix = adName;
            }
            string filePathPrefixInZip = GetCombinedPath(this.pluginAndroidSrcPath, fileNamePrefix);

            int cnt = 0;
            foreach (ZipEntry entry in
                this.sdkArchiveFile.SelectEntries(filePathPrefixInZip + WILDCARD + EXT_AAR_FILE))
            {
                if (entry.IsDirectory)
                {
                    continue;
                }

                if (entry.FileName.Contains("crossPromotion"))
                {
                    // 크로스프로모션 파일은 제외한다.
                    continue;
                }

                string extractedFilePath = GetCombinedPath(
                    this.DirPathTempAssetsTarget, entry.FileName);

                // admob 이나 admanager는 동일한 aar 이기 때문에 이 두케이스는 이미 압축이 되었더라도 덮어 쓰기가 가능해야한다.
                bool extractResult = false;
                if (adName == "admob" || adName == "admanager")
                {
                    extractResult = ExtractToSpecificFile(entry, extractedFilePath, true);
                }
                else
                {
                    extractResult = ExtractToSpecificFile(entry, extractedFilePath, false);
                }
            
                if (!extractResult)
                {
                    Logger.e("can't extract file: {0}", extractedFilePath);
                    return false;
                }
                cnt++;
            }

            if (cnt == 0)
            {
                Logger.e("can't find any aar file for {0}", adName);
                return false;
            }

            return true;
        }
        
        public bool CopyAdditionalLibFilesToTempDirectory(BundleImporter bi, string adName, Dictionary<string, object> libInfo)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return false;
            }

            if (adName == null)
            {
                Logger.e("invalid ad network name");
                return false;
            }

            string fileNamePrefix = ADDITIONALLIB_PREFIX_NAME + "." + adName;
            string filePathPrefixInZip = GetCombinedPath(this.pluginAndroidSrcPath, fileNamePrefix);

            foreach (ZipEntry entry in
                this.sdkArchiveFile.SelectEntries(filePathPrefixInZip + WILDCARD + EXT_JAR_FILE))
            {
                String libraryName = entry.FileName;
                int idxTemp = libraryName.LastIndexOf('/') + 1;
                libraryName = libraryName.Substring(idxTemp, libraryName.Length - idxTemp);

                string keyName = bi.makeDNKey(adName, libraryName);
                if (EditorPrefs.GetBool(keyName, false))
                {
                    string targetPath = DirPathTempAssetsPluginsAndroid + "/" + libraryName;
                    if (!ExtractToSpecificFile(entry, targetPath, true))
                    {
                        Logger.e("can't extract file: {0}", targetPath);
                    }
                }
            }

            foreach (ZipEntry entry in this.sdkArchiveFile.SelectEntries(filePathPrefixInZip + WILDCARD + EXT_AAR_FILE))
            {
                String libraryName = entry.FileName;
                int idxTemp = libraryName.LastIndexOf('/') + 1;
                libraryName = libraryName.Substring(idxTemp, libraryName.Length - idxTemp);

                string keyName = bi.makeDNKey(adName, libraryName);
                if (EditorPrefs.GetBool(keyName, false))
                {
                    string targetPath = DirPathTempAssetsPluginsAndroid + "/" + libraryName;
                    if (!ExtractToSpecificFile(entry, targetPath, true))
                    {
                        Logger.e("can't extract file: {0}", targetPath);
                    }
                }
            }
            return true;
        }

        private bool ExtractJARFilesToTempDirectory(string adName, bool isNetwork)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return false;
            }

            if (adName == null)
            {
                Logger.e("invalid ad network name");
                return false;
            }

            string fileNamePrefix;
            if (isNetwork)
            {
                fileNamePrefix = ADAPTER_PREFIX + adName;
            }
            else
            {
                fileNamePrefix = adName;
            }
            string filePathPrefixInZip = GetCombinedPath(this.pluginAndroidSrcPath, fileNamePrefix);

            int cnt = 0;
            foreach (ZipEntry entry in
                this.sdkArchiveFile.SelectEntries(filePathPrefixInZip + WILDCARD + EXT_AAR_FILE))
            {
                Logger.d("extracting aar to memory: {0}", entry.FileName);

                using (MemoryStream ms = ReadFileFromZip(entry.FileName, this.sdkArchiveFile))
                {
                    if (ms == null)
                    {
                        Logger.e("can't read file from zip: {0}", entry.FileName);
                        return false;
                    }

                    using (MemoryStream aarInMem = new MemoryStream(ms.ToArray()))
                    {
                        if (aarInMem == null)
                        {
                            Logger.e("can't read file from memory stream: {0}", entry.FileName);
                            return false;
                        }

                        ZipFile aar = ZipFile.Read(aarInMem);
                        if (!ExplodeAARFile(aar, entry.FileName))
                        {
                            Logger.e("failed to explode aar file: {0}", entry.FileName);
                            return false;
                        }
                    }
                }
                cnt++;
            }

            if (cnt == 0)
            {
                Logger.e("can't find any jar file for {0}", adName);
                return false;
            }

            return true;
        }

        private bool ExtractExtraFilesToTempDirectory(string adName, bool isNetwork)
        {
            if (this.sdkArchiveFile == null)
            {
                Logger.e(MSG_NOT_INITIALIZED);
                return false;
            }

            if (adName == null)
            {
                Logger.e("invalid ad network name");
                return false;
            }

            string fileNamePrefix;
            if (isNetwork)
            {
                fileNamePrefix = EXTRAS_PREFIX + adName;
            }
            else
            {
                fileNamePrefix = adName + EXTRAS_POSTFIX;
            }
            string filePathPrefixInZip = GetCombinedPath(this.pluginAndroidSrcPath, fileNamePrefix);

            Logger.d("looking for the extra file: {0}{1}{2}", filePathPrefixInZip, WILDCARD, EXT_ZIP_FILE);

            int cnt = 0;
            foreach (ZipEntry entry in
                this.sdkArchiveFile.SelectEntries(filePathPrefixInZip + WILDCARD + EXT_ZIP_FILE))
            {
                Logger.d("extracting zip to memory: {0}", entry.FileName);

                using (MemoryStream ms = ReadFileFromZip(entry.FileName, this.sdkArchiveFile))
                {
                    if (ms == null)
                    {
                        Logger.e("can't read file from zip: {0}", entry.FileName);
                        return false;
                    }

                    using (MemoryStream zipInMem = new MemoryStream(ms.ToArray()))
                    {
                        if (zipInMem == null)
                        {
                            Logger.e("can't read file from memory stream: {0}", entry.FileName);
                            return false;
                        }

                        ZipFile zip = ZipFile.Read(zipInMem);
                        if (!ExplodeExtrasZipFile(zip, entry.FileName))
                        {
                            Logger.e("failed to explode extras zip file: {0}", entry.FileName);
                            return false;
                        }
                    }
                }
                cnt++;
            }

            if (cnt == 0)
            {
                Logger.d("no extras file for {0}", adName);
            }

            return true;
        }

        private bool ExplodeAARFile(ZipFile aar, string aarFileName)
        {
            if (aar == null)
            {
                Logger.e("can't explode aar, file is null");
                return false;
            }

            Logger.d("exploding aar file: {0}", aarFileName);

            foreach (ZipEntry entry in aar)
            {
                if (entry.IsDirectory)
                {
                    continue;
                }

                string fileName;
                if (entry.FileName.Equals(CLASS_JAR_FILE_NAME))
                {
                    // classes.jar
                    fileName = GetCombinedPath(
                        this.DirPathTempAssetsPluginsAndroidTargetLibs,
                        Path.GetFileName(aarFileName).Replace(EXT_AAR_FILE, EXT_JAR_FILE));
                }
                else if (entry.FileName.StartsWith(LIBS_DIR_NAME) &&
                    entry.FileName.EndsWith(EXT_JAR_FILE))
                {
                    // libs/jar file
                    fileName = GetCombinedPath(
                        this.DirPathTempAssetsPluginsAndroidTargetLibs,
                        Path.GetFileName(entry.FileName));
                }
                else if (entry.FileName.StartsWith(RES_DIR_NAME))
                {
                    if (entry.FileName.StartsWith(GetCombinedPath(RES_DIR_NAME, VALUES_DIR_NAME)))
                    {
                        // if the name is start with res/values*
                        // e.g.
                        // res/values/values.xml
                        // res/values-ko/values-ko.xml
                        // res/values-xx/values-xx.xml
                        fileName = GetCombinedPath(
                            this.DirPathTempAssetsPluginsAndroidTarget,
                            Path.GetDirectoryName(entry.FileName),
                            Path.GetFileNameWithoutExtension(aarFileName) + "-" +
                            Path.GetFileName(entry.FileName));
                    }
                    else
                    {
                        // res/* except values
                        fileName = GetCombinedPath(
                            this.DirPathTempAssetsPluginsAndroidTarget, entry.FileName);
                    }
                }
                else if (entry.FileName.StartsWith(JNI_DIR_NAME))
                {
                    // jni/*
                    fileName = GetCombinedPath(
                        this.DirPathTempAssetsPluginsAndroidLibs,
                        entry.FileName.Replace(JNI_DIR_NAME + "/", ""));
                }
                else if (entry.FileName.StartsWith(ASSETS_DIR_NAME))
                {
                        fileName = GetCombinedPath(
                            this.DirPathTempAssetsPluginsAndroidTarget, entry.FileName);
                }
                else
                {
                    continue;
                }

                Logger.d("extracting {0} to {1}", entry.FileName, fileName);

                if (!ExtractToSpecificFile(entry, fileName, false))
                {
                    Logger.e("can't extract file: {0}/{1}", aarFileName, entry.FileName);
                    return false;
                }
            }

            return true;
        }

        private bool ExplodeExtrasZipFile(ZipFile zip, string zipFileName)
        {
            if (zip == null)
            {
                Logger.e("can't explode zip, file is null");
                return false;
            }

            Logger.d("exploding zip file: {0}", zipFileName);

            // 1. Read "actions.json"
            // 2. run actions one by one
            // 2.1 if action == copy
            Dictionary<string, object> dic = ReadJsonFileFromZip(EXTRAS_JSON_FILE_NAME, zip);
            if (dic == null)
            {
                Logger.e("can't find {0} file", EXTRAS_JSON_FILE_NAME);
                return false;
            }

            if (!dic.ContainsKey(EXTRAS_KEY_ROOT))
            {
                Logger.e("can't find key: {0} ", EXTRAS_KEY_ROOT);
                return false;
            }
            List<object> actions = dic[EXTRAS_KEY_ROOT] as List<object>;

            Logger.d("number of actions: {0}", actions.Count);

            foreach(object actionEntry in actions)
            {
                Dictionary<string, object> actionSetting = actionEntry as Dictionary<string, object>;
                if (!actionSetting.ContainsKey(EXTRAS_KEY_ACTION))
                {
                    Logger.e("invalid action definition");
                    foreach (string key in actionSetting.Keys)
                    {
                        Logger.e("{0}: {1}", key, actionSetting[key] as string);
                    }

                    return false;
                }

                string action = actionSetting[EXTRAS_KEY_ACTION] as string;

                switch (action)
                {
                    case EXTRAS_ACTION_COPY:
                        if (actionSetting.ContainsKey(EXTRAS_KEY_TARGET)
                            && actionSetting.ContainsKey(EXTRAS_KEY_TARGET_TO))
                        {
                            string target = actionSetting[EXTRAS_KEY_TARGET] as string;
                            string to = actionSetting[EXTRAS_KEY_TARGET_TO] as string;

                            Logger.d("{0} : {1} : {2}", action, target, to);

                            int cnt = 0;
                            foreach (ZipEntry zipEntry in zip)
                            {
                                if (zipEntry.IsDirectory)
                                {
                                    continue;
                                }

                                if (zipEntry.FileName.StartsWith(target))
                                {
                                    Logger.d("extracting file: {0}", zipEntry.FileName);

                                    string extractedFilePath = GetCombinedPath(
                                        this.DirPathTempAssets, to, zipEntry.FileName);

                                    if (!ExtractToSpecificFile(zipEntry, extractedFilePath, false))
                                    {
                                        Logger.e("can't extract file: {0}", extractedFilePath);
                                        return false;
                                    }

                                    cnt++;
                                }
                            }

                            if (cnt == 0)
                            {
                                Logger.e("there's no files to copy: {0} to {1}", target, to);
                                return false;
                            }
                        }
                        else
                        {
                            Logger.w("copy needs [{0}] and [{1}] values",
                                EXTRAS_KEY_TARGET, EXTRAS_KEY_TARGET_TO);
                        }
                        break;
                    default:
                        Logger.w("invalid action to perform: {0}", action);
                        break;
                }
            }

            return true;
        }

        private bool ExtractToSpecificFile(ZipEntry entry, string fileNameToSave, bool overwrite)
        {
            if (!CreateParentDirectory(fileNameToSave))
            {
                Logger.e("can't create parent directory. file: {0}", fileNameToSave);
                return false;
            }

            if (!overwrite && File.Exists(fileNameToSave))
            {
                Logger.e("file already exists {0}", fileNameToSave);
                return false;
            }

            try
            {
                using (FileStream fs = new FileStream(fileNameToSave, FileMode.Create))
                {
                    entry.Extract(fs);
                }
            }
            catch (Exception e)
            {
                Logger.e("failed to extract as specific file: {0} -> {1}",
                    entry.FileName, fileNameToSave);
                Logger.e(e);
                return false;
            }

            return true;
        }

        private bool CopyDirectory(string srcDirPath, string destDirPath, bool recursive)
        {
            // this method is based on the source code from the Microsoft's documentation page
            // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories

            if (!Directory.Exists(srcDirPath))
            {
                Logger.w("directory not found: {0}", srcDirPath);
                return false;
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            // Get the subdirectories for the specified directory.
            string[] dirs = Directory.GetDirectories(srcDirPath);
            string[] files = Directory.GetFiles(srcDirPath);
            foreach (string file in files)
            {
                if (file.EndsWith(EXT_META_FILE))
                {
                    //continue;
                }

                try
                {
                    File.Copy(file, file.Replace(srcDirPath, destDirPath), true);
                }
                catch (Exception e)
                {
                    Logger.e("file copy failed from {0} to {1}",
                        file, file.Replace(srcDirPath, destDirPath));
                    Logger.e(e);
                    return false;
                }
            }

            if (recursive)
            {
                foreach (string subdir in dirs)
                {
                    if (!CopyDirectory(subdir, subdir.Replace(srcDirPath, destDirPath), recursive))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string GetCombinedPath(params string[] paths)
        {
            string fullPath = string.Empty;

            foreach (string path in paths)
            {
                try
                {
                    fullPath = Path.Combine(fullPath, path);
                }
                catch (Exception e)
                {
                    Logger.e("can't combine path arguments: {0}", path);
                    Logger.e(e);
                    return null;
                }
            }

            return fullPath.Replace("\\", "/");
        }

        private Dictionary<string, object> ReadJsonFileFromZip(string filePathInZip, ZipFile zip)
        {
            MemoryStream ms = ReadFileFromZip(filePathInZip, zip);
            if (ms == null)
            {
                Logger.i("failed to read json file from zip file: {0}", filePathInZip);
                return null;
            }

            return ReadJsonFileFromMemoryStream(ms);
        }

        private Dictionary<string, object> ReadJsonFileFromMemoryStream(MemoryStream ms)
        {
            if (ms == null)
            {
                Logger.e("memory stream is null");
                return null;
            }

            // Read json string from jsonFilePath
            string jsonString = string.Empty;
            try
            {
                byte[] jsonBytes = ms.ToArray();
                jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);
            }
            catch (Exception e)
            {
                Logger.e("can't read bytes from memory stream");
                Logger.e(e);
                return null;
            }

            if (jsonString == null || jsonString.Length == 0)
            {
                Logger.e("json file content not exists. file");
                return null;
            }

            // Parse the contents
            Dictionary<string, object> settings = null;
            try
            {
                settings = MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
            }
            catch (Exception e)
            {
                Logger.e("parsing json string error: {0}", jsonString);
                Logger.e(e);
                return null;
            }

            if (settings == null)
            {
                Logger.e("parsing failed, the parser returned null: {0}", jsonString);
                return null;
            }

            return settings;
        }

        private MemoryStream ReadFileFromZip(string filePathInArchive, ZipFile zip)
        {
            if (zip == null)
            {
                Logger.e("zip file is null");
                return null;
            }

            Logger.d("Extractin {0} from {1}", filePathInArchive, zip.Name);

            if (!zip.ContainsEntry(filePathInArchive))
            {
                Logger.i("can't find zip entry: {0} from {1}", filePathInArchive, zip.Name);
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    ZipEntry entry = zip[filePathInArchive];

                    Logger.d("reading zipped entry: {0} from {1}", entry.FileName, zip.Name);

                    entry.Extract(ms);

                    return ms;
                }
                catch (Exception e)
                {
                    Logger.e("failed to get the content: {0} from {1}", filePathInArchive, zip.Name);
                    Logger.e(e);

                    return null;
                }
            }
        }

        private void DeleteEmptyDirectories(string path)
        {
            if (!Directory.Exists(path))
            {
                Logger.w("directory not found: {0}", path);
                return;
            }

            string[] dirs = Directory.GetDirectories(path);
            foreach (string subdir in dirs)
            {
                DeleteEmptyDirectories(subdir);
            }

            dirs = Directory.GetDirectories(path);
            if (dirs.Length == 0)
            {
                // has no child directory
                string[] files = Directory.GetFiles(path);
                if (files.Length == 0)
                {
                    // this is empty directory, delete
                    try
                    {
                        Directory.Delete(path);
                        File.Delete(path + EXT_META_FILE);
                    }
                    catch (Exception e)
                    {
                        Logger.e("failed to delete dir {0}", path);
                        Logger.e(e);
                        return;
                    }
                }
            }
        }

        private void LogInstalledFiles()
        {
            // get file hierachy
            string[] filesInTemp =
                Directory.GetFiles(this.DirPathTempAssets, "*", SearchOption.AllDirectories);
            List<string> filesInAsset = new List<string>();
            foreach (string file in filesInTemp)
            {
                // replace "\" to "/"
                // delete temp dir path to make relative path to Assets
                // delete proceeding "/"
                string newFileName =
                    file.Replace("\\", "/").Replace(this.DirPathTempAssets, "").Substring(1);
                filesInAsset.Add(newFileName);
            }

            try
            {
                string listStr = MiniJSON.Json.Serialize(filesInAsset);
                File.WriteAllText(this.FilePathInstalledPluginFileList, listStr);
            }
            catch (Exception e)
            {
                Logger.e("failed to write file: {0}", this.FilePathInstalledPluginFileList);
                Logger.e(e);
            }

            return;
        }
        #endregion
    }
}
