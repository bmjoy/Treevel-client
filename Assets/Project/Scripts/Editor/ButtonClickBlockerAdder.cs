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
            var currentScene = SceneManager.GetActiveScene().path;
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Project" });

            foreach (var sceneGuid in sceneGuids) {
                var path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                EditorSceneManager.OpenScene(path);

                Resources.FindObjectsOfTypeAll(typeof(Button))
                    .Select(button => button as Button)
                    .Where(button => button != null)
                    .Where(button => button.gameObject.GetComponent<ButtonClickBlocker>() != null)
                    .ToList()
                    .ForEach(button => {
                        button.gameObject.AddComponent<ButtonClickBlocker>();
                    });

                EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(path));
            }

            if (!string.IsNullOrEmpty(currentScene)) EditorSceneManager.OpenScene(currentScene);
        }
    }
}
