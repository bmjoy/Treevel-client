using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.MenuSelectScene
{
    public class SaveScrollRect : ScrollRect
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            // 初期位置の調整
            content.transform.localPosition = UserSettings.LevelSelectScrollPosition;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UserSettings.LevelSelectScrollPosition = content.transform.localPosition;
        }
    }
}
