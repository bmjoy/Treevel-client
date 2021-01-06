using Treevel.Common.Entities;
using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Common.Components.UIs
{
    /// <summary>
    /// 多言語に対応するテキスト
    /// </summary>
    public class MultiLanguageText : Text
    {
        private ETextIndex _textIndex;

        // 列挙値に対応する文字列
        [SerializeField] private string _indexStr;
        public string IndexStr => _indexStr;

        /// <summary>
        /// TextIndex が設定される同時にテキストを取得して設定する
        /// </summary>
        /// <value></value>
        public ETextIndex TextIndex {
            get =>
                _textIndex.CompareTo(ETextIndex.Error) == 0
                    ? (ETextIndex) System.Enum.Parse(typeof(ETextIndex), _indexStr)
                    : _textIndex;
            set {
                if (_textIndex == value) return;

                _textIndex = value;
                _indexStr = System.Enum.GetName(typeof(ETextIndex), _textIndex);
                text = LanguageUtility.GetText(_textIndex);
            }
        }

        /// <summary>
        /// 言語が変更されたときに発火するイベント
        /// </summary>
        private void OnLanguageChanged()
        {
            text = LanguageUtility.GetText(TextIndex);
        }

        protected override void Awake()
        {
            base.Awake();

            // 言語変更するイベントを登録する
            LanguageUtility.OnLanguageChange += OnLanguageChanged;
            text = LanguageUtility.GetText(TextIndex);
        }

        protected override void OnDestroy()
        {
            LanguageUtility.OnLanguageChange -= OnLanguageChanged;
            base.OnDestroy();
        }
    }
}
