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

        void Awake() {
            gameObject.GetComponent<Button>().onClick.AddListener(TreeButtonDown);
        }

        public void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = _treeId;
            // StageSelect Toggle に結びつけるSceneを変更する
            GameObject.Find("StageSelect").GetComponent<TransitionSelfToggle>().SetSceneName(SceneName.STAGE_SELECT_SCENE);
            SceneManager.UnloadSceneAsync(SceneName.LEVEL_SELECT_SCENE);
            StartCoroutine(MenuSelectDirector.Instance.ChangeScene(SceneName.STAGE_SELECT_SCENE));
        }
    }
}
