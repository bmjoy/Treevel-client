using System.Collections;
using System.Collections.Generic;
using Project.Scripts.MenuSelectScene.LevelSelect;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene
{
    public class StageTreeController : TreeController
    {
        [SerializeField] private Image _treeImage;
        [SerializeField] private Material _material;
        [SerializeField] private Image _mask;

        protected override void ReflectUnreleasedState()
        {
            // グレースケール
            _treeImage.material = _material;
            // GetComponent<Image>().material = _material;
        }

        protected override void ReflectReleasedState()
        {
            _mask.enabled = false;
            // _treeImage.material = null;
        }

        protected override void ReflectClearedState()
        {
            _mask.enabled = false;
            // _treeImage.material = null;
            // アニメーション
            Debug.Log($"{treeId} is cleared.");
        }

        protected override void ReflectAllClearedState()
        {
            _mask.enabled = false;
            // _treeImage.material = null;
            // アニメーション
            Debug.Log($"{treeId} is all cleared.");
        }
    }
}
