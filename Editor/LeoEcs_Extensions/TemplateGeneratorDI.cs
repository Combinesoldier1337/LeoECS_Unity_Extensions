// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Leopotam.EcsLite.UnityEditor.Templates
{
    sealed class TemplateGeneratorDI : ScriptableObject
    {
        const string Title = "LeoECS Lite template generator";
        const string RunSystemDiTemplate = "RunSystemDi.cs.txt";
        const string InitSystemDiTemplate = "InitSystemDi.cs.txt";
        const string StartupTemplate = "StartupEx.cs.txt";

        const string GameStateTemplate = "GameState.cs.txt";

        [MenuItem("Assets/Create/LeoECS Lite/Create [EcsStartup.cs]", false, -201)]
        static void CreateStartupTpl()
        {
            var assetPath = GetAssetPath();
            CreateAndRenameAsset($"{assetPath}/EcsStartup.cs", GetIcon(), (name) => {
                if (CreateTemplateInternal(GetTemplateContent(StartupTemplate), name) == null)
                {
                    if (EditorUtility.DisplayDialog(Title, "Create data folders?", "Yes", "No"))
                    {
                        CreateEmptyFolder($"{assetPath}/Components");
                        CreateEmptyFolder($"{assetPath}/Systems");
                        CreateEmptyFolder($"{assetPath}/Views");
                        CreateEmptyFolder($"{assetPath}/Services");
                        AssetDatabase.Refresh();
                    }
                }
            });
        }

        static void CreateEmptyFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                    File.Create($"{folderPath}/.gitkeep");
                }
                catch
                {
                    // ignored
                }
            }
        }
        [MenuItem("Assets/Create/LeoECS Lite/Systems/Create [InitSystemDi] from template", false, -196)]
        static void CreateInitSystemDiTpl()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/InitSystem.cs", GetIcon(),
                (name) => CreateTemplateInternal(GetTemplateContent(InitSystemDiTemplate), name));
        }

        [MenuItem("Assets/Create/LeoECS Lite/Systems/Create [RunSystemDi] from template", false, -195)]
        static void CreateRunSystemDiTpl()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/RunSystem.cs", GetIcon(),
                (name) => CreateTemplateInternal(GetTemplateContent(RunSystemDiTemplate), name));
        }

        [MenuItem("Assets/Create/LeoECS Lite/Create GameState from template", false, -192)]
        static void CreateGameStateExTpl()
        {
            CreateAndRenameAsset($"{GetAssetPath()}/GameState.cs", GetIcon(),
                (name) => CreateTemplateInternal(GetTemplateContent(GameStateTemplate), name));
        }
        public static string CreateTemplate(string proto, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "Invalid filename";
            }
            var ns = EditorSettings.projectGenerationRootNamespace.Trim();
            if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
            {
                ns = "Client";
            }
            proto = proto.Replace("#NS#", ns);
            proto = proto.Replace("#SCRIPTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(fileName)));
            try
            {
                File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName), proto);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            AssetDatabase.Refresh();
            return null;
        }

        static string SanitizeClassName(string className)
        {
            var sb = new StringBuilder();
            var needUp = true;
            foreach (var c in className)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                    needUp = false;
                }
                else
                {
                    needUp = true;
                }
            }
            return sb.ToString();
        }

        static string CreateTemplateInternal(string proto, string fileName)
        {
            var res = CreateTemplate(proto, fileName);
            if (res != null)
            {
                EditorUtility.DisplayDialog(Title, res, "Close");
            }
            return res;
        }

        static string GetTemplateContent(string proto)
        {
            // hack: its only one way to get current editor script path. :(
            var pathHelper = CreateInstance<TemplateGeneratorDI>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(pathHelper)));
            DestroyImmediate(pathHelper);
            try
            {
                return File.ReadAllText(Path.Combine(path ?? "", proto));
            }
            catch
            {
                return null;
            }
        }

        static string GetAssetPath()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.Contains(Selection.activeObject))
            {
                if (!AssetDatabase.IsValidFolder(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }
            else
            {
                path = "Assets";
            }
            return path;
        }

        static Texture2D GetIcon()
        {
            return EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
        }

        static void CreateAndRenameAsset(string fileName, Texture2D icon, Action<string> onSuccess)
        {
            var action = CreateInstance<CustomEndNameAction>();
            action.Callback = onSuccess;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, fileName, icon, null);
        }

        sealed class CustomEndNameAction : EndNameEditAction
        {
            [NonSerialized] public Action<string> Callback;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Callback?.Invoke(pathName);
            }
        }
    }
}