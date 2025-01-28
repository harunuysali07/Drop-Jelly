using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Plugins.Tools.Editor
{
    public class AndroidKeystoreAuthenticator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private const string KeystoreName = "Level4.keystore";
        private const string KeystorePassword = "rHlK9M202%dQ";
        private const string KeystoreAliasName = "level4";
        private const string KeystoreAliasPassword = "!T3lK%50T2m^";
        
        public void OnPreprocessBuild(BuildReport report)
        {
            PlayerSettings.Android.useCustomKeystore = true;
            
            PlayerSettings.Android.keystoreName = $"{Directory.GetCurrentDirectory()}/{KeystoreName}";
            PlayerSettings.Android.keystorePass = KeystorePassword;
            PlayerSettings.Android.keyaliasName = KeystoreAliasName;
            PlayerSettings.Android.keyaliasPass = KeystoreAliasPassword;
        }
    }
}