using UnityEngine;

namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// CanvasScalerによる拡大縮小後のキャンバスサイズ
    /// </summary>
    public static class ScaledCanvasSize
    {
        public static readonly Vector2 SIZE_DELTA = GameObject.Find("UIManager/Canvas").GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// ゲーム画面のウィンドウサイズ
    /// </summary>
    public static class WindowSize
    {
        public const float WIDTH = 9;
        public const float HEIGHT = 16;
    }

    /// <summary>
    /// タイルの大きさ
    /// </summary>
    public static class TileSize
    {
        public const float WIDTH = WindowSize.WIDTH * 0.22f;
        public const float HEIGHT = WIDTH;
    }

    /// <summary>
    /// ボトルの大きさ
    /// </summary>
    public static class BottleSize
    {
        public const float WIDTH = TileSize.WIDTH * 0.95f;
        public const float HEIGHT = WIDTH;
    }

    /// <summary>
    /// Cartridge タイプの銃弾の大きさ
    /// </summary>
    public static class CartridgeSize
    {
        public const float WIDTH = WindowSize.WIDTH * 0.15f;
        public const float HEIGHT = WIDTH / 3.0f;
    }

    /// <summary>
    /// Cartridge タイプの銃弾警告の大きさ
    /// </summary>
    public static class CartridgeWarningSize
    {
        public const float WIDTH = WindowSize.WIDTH * 0.15f;
        public const float HEIGHT = WIDTH;
        public const float POSITION_X = WindowSize.WIDTH * 0.16f;
        public const float POSITION_Y = POSITION_X;
    }

    /// <summary>
    /// Hole タイプの銃弾警告の大きさ
    /// </summary>
    public static class HoleWarningSize
    {
        public const float WIDTH = WindowSize.WIDTH * 0.15f;
        public const float HEIGHT = WIDTH;
    }
}
