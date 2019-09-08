using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.StageSelectScene.OverlappedScrollRect
{
    public class OverlappedScrollRect : ScrollRect
    {
        private bool _routeToParent = false;

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            transform.TransmitParentEventSystemHandler<IInitializePotentialDragHandler>((parent) => {
                // イベント情報を親へ伝える
                parent.OnInitializePotentialDrag(eventData);
            });
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_routeToParent)
                transform.TransmitParentEventSystemHandler<IDragHandler>((parent) => {
                    // イベント情報を親へ伝える
                    parent.OnDrag(eventData);
                });
            base.OnDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                // 該当するものがない場合，イベント情報を親へ伝えるフラグをオンにする
                _routeToParent = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                // 該当するものがない場合，イベント情報を親へ伝えるフラグをオンにする
                _routeToParent = true;
            else
                _routeToParent = false;

            if (_routeToParent)
                transform.TransmitParentEventSystemHandler<IBeginDragHandler>((parent) => {
                    // イベント情報を親に伝える
                    parent.OnBeginDrag(eventData);
                });
            else
                base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_routeToParent)
                transform.TransmitParentEventSystemHandler<IEndDragHandler>((parent) => {
                    // イベント情報を親に伝える
                    parent.OnEndDrag(eventData);
                });
            else
                base.OnEndDrag(eventData);

            _routeToParent = false;
        }
    }
}
