using System.Collections;
using System.Collections.Generic;
using Project.Scripts.GameDatas;
using Project.Scripts.GamePlayScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns.StateMachine;
using UnityEngine;

public abstract class AbstractGimmickController : MonoBehaviour
{
    [SerializeField] protected float _warningDisplayTime = BulletWarningParameter.WARNING_DISPLAYED_TIME;

    public virtual void Initialize(GimmickData gimmickData)
    {
        GamePlayDirector.OnSucceed -= OnSucceed;
        GamePlayDirector.OnFail -= OnFail;
    }

    /// <summary>
    /// ギミック発動（初期化の後に呼ぶ）
    /// </summary>
    public abstract IEnumerator Trigger();

    private void OnEnable()
    {
        GamePlayDirector.OnSucceed += OnSucceed;
        GamePlayDirector.OnFail += OnFail;
    }

    private void OnDisable()
    {
        GamePlayDirector.OnSucceed -= OnSucceed;
        GamePlayDirector.OnFail -= OnFail;
    }

    protected virtual void OnSucceed()
    {
        GameEnd();
    }

    protected virtual void OnFail()
    {
        GameEnd();
    }

    protected virtual void GameEnd()
    {
    }
}
