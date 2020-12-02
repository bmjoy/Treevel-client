using System.Linq;
using Treevel.Common.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Treevel.Editor
{
    public class MultiClickBlockerAdder : EditorWindow
    {
        /// <summary>
        /// 全 Scene / Prefab の Button に MultiClickBlocker を追加する
        /// </summary>
        [MenuItem("Tools/Add Multi Click Blockers")]
        private static void AddMultiClickBlockers()
        {
            // Prefab への変更
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/Project"});

            foreach (var prefabGuid in prefabGuids) {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var contentsRoot = PrefabUtility.LoadPrefabContents(path);

                if (AddMultiClickBlocker()) { PrefabUtility.SaveAsPrefabAsset(contentsRoot, path); }
                PrefabUtility.UnloadPrefabContents(contentsRoot);
            }

            // Scene への変更
            var currentScene = SceneManager.GetActiveScene().path;
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Project" });

            foreach (var sceneGuid in sceneGuids) {
                var path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                EditorSceneManager.OpenScene(path);

                if (AddMultiClickBlocker()) { EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(path)); }
            }

            if (!string.IsNullOrEmpty(currentScene)) EditorSceneManager.OpenScene(currentScene);
        }

        /// <summary>
        /// Button に MultiClickBlocker を追加する
        /// </summary>
        /// <returns> 変更されたかどうか </returns>
        private static bool AddMultiClickBlocker()
        {
            var hasChanged = false;

            Resources.FindObjectsOfTypeAll(typeof(Button))
                .Select(button => button as Button)
                .Where(button => button != null)
                .Where(button => button.gameObject.GetComponent<MultiClickBlocker>() == null)
                .Where(button => button.hideFlags != HideFlags.HideAndDontSave)
                .ToList()
                .ForEach(button => {
                    button.gameObject.AddComponent<MultiClickBlocker>();
                    hasChanged = true;
                });

            return hasChanged;
        }
    }
}
