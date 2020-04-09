using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class ScaleContent : MonoBehaviour
{
    private TransformGesture _transformGesture;
    [SerializeField] private GameObject _content;

    private Vector2 _point1;
    private Vector2 _point2;

    private float _scaleMin = 0.50f;
    private float _scaleMax = 1.5f;
    private float _scale = 1.0f;

    /// <summary>
    /// 直前の2点間の距離
    /// </summary>
    private float _backDist = 0.0f;

    private void Awake()
    {
        _transformGesture = GetComponent<TransformGesture>();
    }

    private void OnEnable()
    {
        _transformGesture.TransformStarted   += OnTransformStarted;
        _transformGesture.Transformed        += OnTransformed;
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
        Debug.Log("変形を開始した");
        _point1 = _transformGesture.ActivePointers[0].Position;
        _point2 = _transformGesture.ActivePointers[1].Position;
        _backDist = Vector2.Distance(_point1, _point2);
    }

    private void OnTransformed( object sender, EventArgs e )
    {
        Debug.Log("変形中");
        _point1 = _transformGesture.ActivePointers[0].Position;
        _point2 = _transformGesture.ActivePointers[1].Position;
        var newDist = Vector2.Distance(_point1, _point2);
        _scale += (newDist - _backDist) / 10000.0f;
        _scale = Math.Min(_scale, _scaleMax);
        _scale = Math.Max(_scale, _scaleMin);
        _content.GetComponent<RectTransform>().localScale = new Vector2(_scale, _scale);
    }

    private void OnTransformCompleted( object sender, EventArgs e )
    {
        Debug.Log("変形を完了した");
    }
}
