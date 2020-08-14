using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public class SolarBeamController : AbstractGimmickController
    {
        private Animator _animator;

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            var direction = gimmickData.solarBeamDirection;
            switch (direction) {
                case ESolarBeamDirection.Horizontal:
                    var row = gimmickData.targetRow;

                    // 親オブジェクトの位置設定
                    var initialPos = BoardManager.Instance.GetTilePos(row, EColumn.Center);
                    transform.position = initialPos;

                    // 
                    break;
                case ESolarBeamDirection.Vertical:
                    break;
            }
        }

        public override IEnumerator Trigger()
        {
            throw new System.NotImplementedException();
        }

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.SetTrigger("Warning");
            }
        }
    }
}
