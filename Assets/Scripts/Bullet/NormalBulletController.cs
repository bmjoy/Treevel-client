using UnityEngine;

public class NormalBulletController : CartridgeController {

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

    // コンストラクタがわりのメソッド
    public override void initialize(Vector2 motion_vector)
    {
        base.initialize(motion_vector);
        this.speed = 1.0f;
    }
}
