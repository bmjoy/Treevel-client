using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.UI;

public class ScaleContent : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private TransformGesture _transformGesture;
    [SerializeField] private GameObject _content;

    /// <summary>
    /// タッチした2点
    /// </summary>
    private Vector2 _prePoint1;
    private Vector2 _prePoint2;

    /// <summary>
    /// タッチした2点間の距離
    /// </summary>
    private float _preScreenDist = 0.0f;

    /// <summary>
    /// タッチした2点の中点
    /// </summary>
    private Vector3 _preMeanPoint;

    /// <summary>
    /// 拡大率 (初期値 : 1.0f)
    /// </summary>
    private float _preScale = 1.0f;
    /// <summary>
    /// 拡大率の上限
    /// </summary>
    private static float _SCALE_MIN = 0.50f;
    /// <summary>
    /// 拡大率の下限
    /// </summary>
    private static float _SCALE_MAX = 1.5f;

    /// <summary>
    /// Contentの余白(Screen何個分の余白があるか)
    /// </summary>
    private static float _LEFT_OFFSET = 1.5f;
    private static float _RIGHT_OFFSET = 1.5f;
    private static float _TOP_OFFSET = 1.0f;
    private static float _BOTTOM_OFFSET = 1.0f;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _transformGesture = GetComponent<TransformGesture>();
        _content.GetComponent<RectTransform>().transform.localPosition += new Vector3(0, Screen.height/2, 0);
    }

    private void OnEnable()
    {
        // 2点のタッチ開始時
        _transformGesture.TransformStarted   += OnTransformStarted;
        // 2点のタッチ中
        _transformGesture.Transformed        += OnTransformed;
        // 2点のタッチ終了時
        _transformGesture.TransformCompleted += OnTransformCompleted;
    }

    private void OnDisable()
    {
        _transformGesture.TransformStarted   -= OnTransformStarted;
        _transformGesture.Transformed        -= OnTransformed;
        _transformGesture.TransformCompleted -= OnTransformCompleted;
    }

    private void OnTransformStarted( object sender, EventArgs e )
    {
        // タッチ位置を取得
        _prePoint1 = _transformGesture.ActivePointers[0].Position;
        _prePoint2 = _transformGesture.ActivePointers[1].Position;
        // スクリーン上の距離を計算
        _preScreenDist = Vector2.Distance(_prePoint1, _prePoint2);
        // スクリーン上の中点座標を計算
        _preMeanPoint = (_prePoint1 + _prePoint2) / 2;
        // 2点タッチしている時はスクロールしない
        _scrollRect.enabled = false;
    }

    private void OnTransformed( object sender, EventArgs e )
    {
        // タッチ位置等の計算
        var _newPoint1 = _transformGesture.ActivePointers[0].Position;
        var _newPoint2 = _transformGesture.ActivePointers[1].Position;
        var _newScreenDist = Vector2.Distance(_newPoint1, _newPoint2);
        var _newMeanPoint = (Vector3)((_newPoint1 + _newPoint2) / 2);

        // 2点のタッチ開始時の距離と現在の距離の比から拡大率を求める
        var _newScale = _preScale * _newScreenDist / _preScreenDist;
        // 拡大率を閾値内に抑える
        _newScale = Math.Min(_newScale, _SCALE_MAX);
        _newScale = Math.Max(_newScale, _SCALE_MIN);
        // Content空間での拡大縮小を行う
        _content.GetComponent<RectTransform>().localScale = new Vector2(_newScale, _newScale);

        // _preObjectPointを常に、_newObjectPointに合わせる
        // スクリーン上の中点座標をContentのオブジェクト座標に変換する
        var _preObjectPoint = ((-1)*_content.GetComponent<RectTransform>().transform.localPosition + (_preMeanPoint - new Vector3(Screen.width/2, Screen.height/2))) / _preScale;
        var _newObjectPoint = ((-1)*_content.GetComponent<RectTransform>().transform.localPosition + (_newMeanPoint - new Vector3(Screen.width/2, Screen.height/2))) / _preScale;
        // 拡大縮小後の_preObjectPointの座標を求める
        var _scaledPreObjectPoint = _preObjectPoint * _newScale / _preScale;
        // _preObjectPointの座標と_newObjectPointの座標の差
        var moveAmount = _newObjectPoint - _scaledPreObjectPoint;

        var _preLocalPosition = _content.GetComponent<RectTransform>().transform.localPosition;
        // 拡大縮小後Contentの外を表示しないようにする(左右方向)
        if(_preLocalPosition.x + moveAmount.x >= (_LEFT_OFFSET * _newScale - 0.5f) * Screen.width)
        {
            moveAmount.x = (_LEFT_OFFSET * _newScale - 0.5f) * Screen.width - _preLocalPosition.x;
        }
        else if(_preLocalPosition.x + moveAmount.x <= (-1) * ((_RIGHT_OFFSET * _newScale - 0.5f) * Screen.width))
        {
            moveAmount.x = (-1) * ((_RIGHT_OFFSET * _newScale - 0.5f) * Screen.width) - _preLocalPosition.x;
        }
        // 拡大縮小後Contentの外を表示しないようにする(上下方向)
        if(_preLocalPosition.y + moveAmount.y <= (-1) * ((_TOP_OFFSET * _newScale - 0.5f) * Screen.height))
        {
            moveAmount.y = (-1) * ((_TOP_OFFSET * _newScale - 0.5f) * Screen.height) - _preLocalPosition.y;
        }
        else if(_preLocalPosition.y + moveAmount.y >= (_BOTTOM_OFFSET * _newScale - 0.5f) * Screen.height)
        {
            moveAmount.y = (_BOTTOM_OFFSET * _newScale - 0.5f) * Screen.height - _preLocalPosition.y;
        }
        _content.GetComponent<RectTransform>().transform.localPosition += new Vector3(moveAmount.x, moveAmount.y, 0);

        // 値の更新
        _prePoint1 = _newPoint1;
        _prePoint2 = _newPoint2;
        _preScreenDist = _newScreenDist;
        _preMeanPoint = _newMeanPoint;
        _preScale = _newScale;
    }

    private void OnTransformCompleted( object sender, EventArgs e )
    {
        // スクロールを解除する
        _scrollRect.enabled = true;
    }
}
