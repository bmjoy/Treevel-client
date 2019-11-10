using System;
using Project.Scripts.MenuSelectScene;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils.Definitions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TouchScript.Gestures;

namespace Project.Scripts.LevelSelectScene
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(TapGesture))]
    public class LevelTreeController : MonoBehaviour
    {
        // 木のレベル
        [SerializeField] private int _level;
        // 木のid
        [SerializeField] private int _treeId;

        private BoxCollider2D _collider;
        private float _originalWidth;
        private float _originalHeight;

        void Awake() {
            _collider = GetComponent<BoxCollider2D>();
            // 元々の画像サイズの取得
            _originalWidth = GetComponent<SpriteRenderer>().size.x;
            _originalHeight = GetComponent<SpriteRenderer>().size.y;
            // colliderの大きさを画像サイズに合わせる
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(_originalHeight, _originalWidth);
        }

        public void HandleTap(object sender, EventArgs e)
        {
            StageSelectDirector.levelName = _level-1;
            // StageSelect Toggle に結びつけるSceneを変更する
            GameObject.Find("StageSelect").GetComponent<TransitionSelfToggle>().SetSceneName(SceneName.STAGE_SELECT_SCENE);
            SceneManager.UnloadSceneAsync(SceneName.LEVEL_SELECT_SCENE);
            StartCoroutine(MenuSelectDirector.Instance.ChangeScene(SceneName.STAGE_SELECT_SCENE));
        }

        private void OnEnable()
        {
            GetComponent<TapGesture>().Tapped += HandleTap;
        }

        private void OnDisable()
        {
            GetComponent<TapGesture>().Tapped -= HandleTap;
        }
    }
}
