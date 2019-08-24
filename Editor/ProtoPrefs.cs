using System.IO;
using UnityEditor;
using UnityEngine;

namespace E7.Protobuf
{
    internal static class ProtoPrefs
    {
        private static readonly string PrefProtocEnable = "ProtobufUnity_Enable";
        private static readonly string PrefGrpcCompilationEnabled = "ProtobufUnity_GrpcCompilationEnabled";
        private static readonly string PrefProtocExecutable = "ProtobufUnity_ProtocExecutable";
        private static readonly string PrefProtocGrpcExecutable = "ProtobufUnity_ProtocGrpcExecutable";
        private static readonly string PrefLogError = "ProtobufUnity_LogError";
        private static readonly string PrefLogStandard = "ProtobufUnity_LogStandard";
        
        internal static bool isAutoCompilationEnabled
        {
            get => EditorPrefs.GetBool(PrefProtocEnable, true);
            set => EditorPrefs.SetBool(PrefProtocEnable, value);
        }
        
        internal static bool isGrpcCompilationEnabled
        {
            get => EditorPrefs.GetBool(PrefGrpcCompilationEnabled, true);
            set => EditorPrefs.SetBool(PrefGrpcCompilationEnabled, value);
        }
        
        internal static bool isLoggingErrorEnabled
        {
            get => EditorPrefs.GetBool(PrefLogError, true);
            set => EditorPrefs.SetBool(PrefLogError, value);
        }

        internal static bool isLoggingStandardEnabled
        {
            get => EditorPrefs.GetBool(PrefLogStandard, false);
            set => EditorPrefs.SetBool(PrefLogStandard, value);
        }

        internal static string rawExcPath
        {
            get => EditorPrefs.GetString(PrefProtocExecutable, "");
            set => EditorPrefs.SetString(PrefProtocExecutable, value);
        }
        
        internal static string excPath
        {
            get
            {
                string ret = EditorPrefs.GetString(PrefProtocExecutable, "");
                return ret.StartsWith("..") ? Path.Combine(Application.dataPath, ret) : ret;
            }
            
            set => EditorPrefs.SetString(PrefProtocExecutable, value);
        }
        
        internal static string rawExcGrpcPath
        {
            get => EditorPrefs.GetString(PrefProtocGrpcExecutable, "");
            set => EditorPrefs.SetString(PrefProtocGrpcExecutable, value);
        }
        
        internal static string excGrpcPath
        {
            get
            {
                string ret = EditorPrefs.GetString(PrefProtocGrpcExecutable, "");
                return ret.StartsWith("..") ? Path.Combine(Application.dataPath, ret) : ret;
            }
            set => EditorPrefs.SetString(PrefProtocGrpcExecutable, value);
        }

       

#if UNITY_2018_3_OR_NEWER
        internal class ProtobufUnitySettingsProvider : SettingsProvider
        {
            public ProtobufUnitySettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope)
            { }

            public override void OnGUI(string searchContext)
            {
                ShowProtobufPreferencesWindow();
            }

            [SettingsProvider]
            static SettingsProvider CreateProtobufPreferenceSettingsProvider()
            {
                return new ProtobufUnitySettingsProvider("Preferences/Protobuf");
            }
        }
        
#else
        [PreferenceItem("Protobuf")]
#endif
        private static void ShowProtobufPreferencesWindow()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = 200; 
            isAutoCompilationEnabled = EditorGUILayout.Toggle(new GUIContent("Enable Protobuf Auto Compilation", ""), isAutoCompilationEnabled);

            EditorGUILayout.HelpBox(@"On Windows put the path to protoc.exe (e.g. C:\My Dir\protoc.exe), on macOS and Linux you can use ""which protoc"" to find its location. (e.g. /usr/local/bin/grpc_csharp_plugin)", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path to protoc:");
            rawExcPath = EditorGUILayout.TextField(rawExcPath, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            isGrpcCompilationEnabled = EditorGUILayout.Toggle(new GUIContent("Enable Grpc Services compilation", ""), isGrpcCompilationEnabled);
            EditorGUI.BeginDisabledGroup(!isGrpcCompilationEnabled);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path to Grpc plugin:");
            rawExcGrpcPath = EditorGUILayout.TextField(rawExcGrpcPath, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            isLoggingErrorEnabled = EditorGUILayout.Toggle(new GUIContent("Log Error Output", "Log compilation errors from protoc command."), isLoggingErrorEnabled);
            isLoggingStandardEnabled = EditorGUILayout.Toggle(new GUIContent("Log Standard Output", "Log compilation completion messages."), isLoggingStandardEnabled);

            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Compile .proto files")))
            {
                ProtobufUnityCompiler.CompileAllInProject();
            }

            if (EditorGUI.EndChangeCheck())
            {
            }
        }
    }
}