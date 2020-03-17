using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
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

        private void Awake()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(TreeButtonDown);
        }

        private void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = _treeId;
            AddressableAssetManager.Instance.LoadScene(SceneName.STAGE_SELECT_SCENE);
        }
    }
}
