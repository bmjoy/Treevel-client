using UnityEngine;
using UnityEngine.UI;
using System;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// 他言語に対応するテキスト
    /// </summary>
    public class MultiLanguageText : Text
    {
        [SerializeField] TextIndex _textIndex;
        public TextIndex TextIndex
        {
            get { return _textIndex; }
            set
            {
                if (_textIndex != value)
                {
                    _textIndex = value;
                    // text = TextUtility.GetText(_textIndex);
                }
            }
        }

        /// <summary>
        /// Override setter to perform special text "index:" for quick transform
        /// </summary>
        /// <value>The text.</value>
        public override string text
        {
            get
            {
                return base.text;
            }

            set
            {
                if ((value != null) && value.StartsWith("index:", StringComparison.Ordinal))
                {
                    string eText = value.Substring("index:".Length);
                    try
                    {
                        TextIndex eTextIndex = (TextIndex)Enum.Parse(typeof(TextIndex), eText);
                        TextIndex = eTextIndex;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
                else
                    base.text = value;
            }
        }

        /// <summary>
        /// 言語が変更されたときに発火するイベント
        /// </summary>
        public void OnLanguageChanged()
        {
            // text = TextUtility.GetText(TextIndex);
        }

        protected override void Start()
        {
            base.Start();
            // Configuration.ChangeLanguageEventHandler += OnLanguageChanged;
            // text = TextUtility.GetText(TextIndex);
        }
    }
}