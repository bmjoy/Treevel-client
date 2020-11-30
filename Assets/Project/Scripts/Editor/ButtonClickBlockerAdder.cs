using System.Linq;
using Treevel.Common.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Treevel.Editor
{
    public class ButtonClickBlockerAdder : EditorWindow
    {
        [MenuItem("Tools/Add Button Click Blocker")]
        private static void AddButtonClickBlocker()
        {
            // Prefab への変更
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/Project"});

            foreach (var prefabGuid in prefabGuids) {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var contentsRoot = PrefabUtility.LoadPrefabContents(path);

                if (ChangeButtons()) { PrefabUtility.SaveAsPrefabAsset(contentsRoot, path); }
                PrefabUtility.UnloadPrefabContents(contentsRoot);
            }

            // Scene への変更
            var currentScene = SceneManager.GetActiveScene().path;
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Project" });

            foreach (var sceneGuid in sceneGuids) {
                var path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                EditorSceneManager.OpenScene(path);

                if (ChangeButtons()) { EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(path)); }
            }

            if (!string.IsNullOrEmpty(currentScene)) EditorSceneManager.OpenScene(currentScene);
        }

        /// <summary>
        /// Button に変更を加える
        /// </summary>
        /// <returns> 変更されたかどうか </returns>
        private static bool ChangeButtons()
        {
            var hasChanged = false;

            Resources.FindObjectsOfTypeAll(typeof(Button))
                .Select(button => button as Button)
                .Where(button => button != null)
                .Where(button => button.gameObject.GetComponent<ButtonClickBlocker>() == null)
                .Where(button => button.hideFlags != HideFlags.HideAndDontSave)
                .ToList()
                .ForEach(button => {
                    button.gameObject.AddComponent<ButtonClickBlocker>();
                    hasChanged = true;
                });

            return hasChanged;
        }
    }
}
