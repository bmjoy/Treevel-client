﻿using System;
using UnityEngine;

namespace Bullet
{
    public class CartridgeController : BulletController
    {
        private Vector2 motionVector;
        protected float speed;
        protected float width;
        protected float height;

        // Use this for initialization
        protected override void Start()
        {
        }

        // Update is called once per frame
        protected override void Update()
        {
            // Check if bullet goes out of window
            if (this.transform.position.x < -WindowSize.WIDTH - width / 2 ||
                this.transform.position.x > WindowSize.WIDTH + width / 2 ||
                this.transform.position.y < -WindowSize.HEIGHT - height / 2 ||
                this.transform.position.y > WindowSize.HEIGHT + height / 2)
            {
                Destroy(gameObject);
            }
        }

        protected override void FixedUpdate()
        {
            transform.Translate(motionVector * speed, Space.World);
        }

        // コンストラクタがわりのメソッド
        public virtual void Initialize(Vector2 motionVector)
        {
            this.motionVector = motionVector;
            var angle = Vector2.Dot(motionVector, Vector2.left) / motionVector.magnitude;
            angle = (float) (Mathf.Acos(angle) * 180 / Math.PI);
            angle *= (-1)*Mathf.Sign(motionVector.y);

            if (motionVector.x > 0)
            {
                var temp = transform.localScale;
                temp.y *= (-1);
                transform.localScale = temp;
            }

            transform.Rotate(new Vector3(0, 0, angle), Space.World);
        }
    }
}
