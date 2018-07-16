using UnityEngine;

public class CartridgeController : BulletController {

    Vector2 motion_vector;
    protected float speed;

	// Use this for initialization
	protected override void Start () {
		
	}

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        transform.Translate(motion_vector * speed);
    }

    // コンストラクタがわりのメソッド
    public virtual void initialize(Vector2 motion_vector)
    {
        this.motion_vector = motion_vector;
    }

}
