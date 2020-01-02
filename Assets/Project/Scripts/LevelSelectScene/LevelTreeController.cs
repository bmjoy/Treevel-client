using Project.Scripts.MenuSelectScene;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.LevelSelectScene
{
    [RequireComponent(typeof(Button))]
    public class LevelTreeController : MonoBehaviour
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ELevelName _levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] private ETreeName _treeId;

        private void Awake() {
            gameObject.GetComponent<Button>().onClick.AddListener(TreeButtonDown);
        }

        private void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = _treeId;
            // StageSelect Toggle に結びつけるSceneを変更する
            var nowToggle = GameObject.Find("StageSelect").GetComponent<TransitionSelfToggle>();
            SceneManager.UnloadSceneAsync(nowToggle.GetSceneName());
            nowToggle.IsTransition = true;
            StartCoroutine(MenuSelectDirector.Instance.ChangeScene());
        }
    }
}
