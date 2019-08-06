using UnityEngine;
using UnityEngine.UI;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.TextUtils;

namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// 多言語に対応するテキスト
    /// </summary>
    public class MultiLanguageText : Text
    {
        [SerializeField] ETextIndex _textIndex;

        /// <summary>
        /// TextIndex が設定される同時にテキストを取得して設定する
        /// </summary>
        /// <value></value>
        public ETextIndex TextIndex
        {
            get {
                return _textIndex;
            }
            set {
                if (_textIndex != value) {
                    _textIndex = value;
                    text = LanguageUtility.GetText(_textIndex);
                }
            }
        }

        /// <summary>
        /// 言語が変更されたときに発火するイベント
        /// </summary>
        public void OnLanguageChanged()
        {
            text = LanguageUtility.GetText(TextIndex);
        }

        protected override void Start()
        {
            base.Start();

            // 言語変更するイベントを登録する
            LanguageUtility.OnLanguageChange += OnLanguageChanged;
            text = LanguageUtility.GetText(TextIndex);
        }

        protected override void OnDestroy()
        {
            LanguageUtility.OnLanguageChange -= OnLanguageChanged;
        }
    }
}
