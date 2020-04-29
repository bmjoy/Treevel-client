using System.Collections;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

public class DisplayUnifier : MonoBehaviour
{
    /// <summary>
    /// ゲーム画面以外を埋める背景
    /// </summary>
    [SerializeField] private GameObject _backgroundPrefab;

    private void Awake()
    {
        StartCoroutine(UnifyDisplay());
    }

    /// <summary>
    /// ゲーム画面のアスペクト比を統一する
    /// </summary>
    /// Bug: ゲーム画面遷移時にカメラ範囲が狭くなることがある
    private IEnumerator UnifyDisplay()
    {
        // 想定するデバイスのアスペクト比
        const float targetRatio = WindowSize.WIDTH / WindowSize.HEIGHT;
        // 実際のデバイスのアスペクト比
        var currentRatio = (float)Screen.width / Screen.height;
        // 許容するアスペクト比の誤差
        const float aspectRatioError = 0.001f;

        if ((targetRatio - aspectRatioError <= currentRatio) && (currentRatio <= (targetRatio + aspectRatioError))) yield break;

        // ゲーム盤面以外を埋める背景画像を表示する
        var background = Instantiate(_backgroundPrefab);
        background.transform.position = new Vector2(0f, 0f);
        var originalWidth = background.GetComponent<SpriteRenderer>().size.x;
        var originalHeight = background.GetComponent<SpriteRenderer>().size.y;
        var ratio = 0f;
        if (currentRatio > targetRatio + aspectRatioError) {
            // 横長のデバイスの場合
            ratio = targetRatio / currentRatio;
            var rectX = (1 - ratio) / 2f;
            background.transform.localScale = new Vector2(WindowSize.WIDTH / originalWidth / ratio, WindowSize.HEIGHT / originalHeight);
            // 背景を描画するために1フレーム待つ
            yield return null;
            Destroy(background);
            if (Camera.main != null) Camera.main.rect = new Rect(rectX, 0f, ratio, 1f);
            // カメラの描画範囲を縮小させ、縮小させた範囲の背景を取り除くために1フレーム待つ
            yield return null;
        } else if (currentRatio < targetRatio - aspectRatioError) {
            // 縦長のデバイスの場合
            ratio = currentRatio / targetRatio;
            var rectY = (1 - ratio) / 2f;
            background.transform.localScale = new Vector2(WindowSize.WIDTH / originalWidth / ratio, WindowSize.HEIGHT / originalHeight / ratio);
            yield return null;
            Destroy(background);
            if (Camera.main != null) Camera.main.rect = new Rect(0f, rectY, 1f, ratio);
            yield return null;
        }

        // このオブジェクトを破壊
        Destroy(gameObject);
    }
}
