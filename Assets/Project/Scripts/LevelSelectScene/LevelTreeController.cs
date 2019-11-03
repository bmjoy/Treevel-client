
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTreeController : MonoBehaviour, IPointerClickHandler
{
    private BoxCollider2D _collider;
    private float _originalWidth;
    private float _originalHeight;

    void Awake() {
        _collider = GetComponent<BoxCollider2D>();
        // 元々の画像サイズの取得
        _originalWidth = GetComponent<SpriteRenderer>().size.x;
        _originalHeight = GetComponent<SpriteRenderer>().size.y;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(_originalHeight, _originalWidth);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("yobaretayo");
        print(this.name);
    }
}
