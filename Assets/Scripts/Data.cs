public static class WindowSize
{
    public const float WIDTH = 9;
    public const float HEIGHT = 16;

    public const float BRANK_WIDTH = 0;
    public const float BRANK_HEIGHT = 0;
}

public static class TileSize
{
    public const float WIDTH = WindowSize.WIDTH * 0.24f;

    public const float HEIGHT = WIDTH;

    // タイル群の上端位置を指定
    public const float MARGIN_TOP = WindowSize.HEIGHT * 0.15f;
}
