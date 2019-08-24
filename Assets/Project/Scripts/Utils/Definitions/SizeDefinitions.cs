﻿namespace Project.Scripts.Utils.Definitions
{
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

        // タイル群の上端位置を指定
        public const float MARGIN_TOP = WindowSize.HEIGHT * 0.15f;
    }

    /// <summary>
    /// パネルの大きさ
    /// </summary>
    public static class PanelSize
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
    /// Hole タイプの銃弾の大きさ
    /// </summary>
    public static class HoleSize
    {
        public const float WIDTH = WindowSize.WIDTH * 0.15f;
        public const float HEIGHT = WIDTH;
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
