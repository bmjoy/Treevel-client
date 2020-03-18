using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Patterns;
using UnityEngine;

namespace Project.Scripts.MenuSelectScene
{
    /// <summary>
    /// 全体ゲームに出現するUI（プログレスバー、メッセージダイアログ等）
    /// を制御するマネージャークラス
    /// </summary>
    public class UIManager : SingletonObject<UIManager>
    {
        /// <summary>
        /// プログレスバーのインスタンス
        /// </summary>
        public ProgressBar ProgressBar { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            ProgressBar = FindObjectOfType<ProgressBar>();
        }

        /// <summary>
        /// プログレスバーを表示／非表示
        /// </summary>
        /// <param name="show"></param>
        public void ShowProgressBar(bool show)
        {
            ProgressBar.gameObject.SetActive(show);
        }
    }
}
