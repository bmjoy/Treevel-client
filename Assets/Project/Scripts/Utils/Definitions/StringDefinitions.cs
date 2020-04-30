namespace Project.Scripts.Utils.Definitions
{
    /// <summary>
    /// シーンの名前
    /// </summary>
    public static class SceneName
    {
        public const string MENU_SELECT_SCENE = "MenuSelectScene";
        public const string SPRING_STAGE_SELECT_SCENE = "SpringStageSelectScene";
        public const string SUMMER_STAGE_SELECT_SCENE = "SummerStageSelectScene";
        public const string AUTOMN_STAGE_SELECT_SCENE = "AutomnStageSelectScene";
        public const string WINTER_STAGE_SELECT_SCENE = "WinterStageSelectScene";
        public const string GAME_PLAY_SCENE = "GamePlayScene";
    }

    /// <summary>
    /// Sorting Layer の名前
    /// </summary>
    public static class SortingLayerName
    {
        public const string TILE = "Tile";
        public const string HOLE = "Hole";
        public const string BOTTLE = "Bottle";
        public const string BULLET = "Bullet";
        public const string BULLET_WARNING = "BulletWarning";
    }

    /// <summary>
    /// タグの名前
    /// </summary>
    public static class TagName
    {
        public const string TILE = "Tile";
        public const string NORMAL_BOTTLE = "NormalBottle";
        public const string DUMMY_BOTTLE = "DummyBottle";
        public const string BULLET = "Bullet";
        public const string BULLET_WARNING = "BulletWarning";
        public const string GRAPH_UI = "GraphUi";
    }

    /// <summary>
    /// Addressable Asset System で使うアドレス、
    /// Addressables Groups Windowsの「Addressable Name」と一致する必要がある
    /// </summary>
    public static class Address
    {
        // ボトル関連
        public const string NORMAL_BOTTLE_PREFAB = "NormalBottlePrefab";
        public const string LIFE_BOTTLE_PREFAB = "LifeBottlePrefab";
        public const string DYNAMIC_DUMMY_BOTTLE_PREFAB  = "DynamicDummyBottlePrefab";
        public const string STATIC_DUMMY_BOTTLE_PREFAB = "StaticDummyBottlePrefab";
        public const string LIFE_DUMMY_BOTTLE_PREFAB = "LifeDummyBottlePrefab";
        public const string DYNAMIC_DUMMY_BOTTLE_SPRITE = "dynamicDummyBottle";
        public const string STATIC_DUMMY_BOTTLE_SPRITE = "staticDummyBottle";
        public const string NORMAL_BOTTLE_SPRITE_PREFIX = "normalBottle";
        public const string LIFE_BOTTLE_SPRITE_PREFIX = "lifeBottle";

        // タイル関連
        public const string NORMAL_TILE_PREFAB = "normalTilePrefab";
        public const string WARP_TILE_PREFAB = "warpTilePrefab";
        public const string NUMBER_TILE_SPRITE_PREFIX = "numberTile";

        // 銃弾関連
        public const string NORMAL_CARTRIDGE_GENERATOR_PREFAB = "NormalCartridgeGeneratorPrefab";
        public const string TURN_CARTRIDGE_GENERATOR_PREFAB = "TurnCartridgeGeneratorPrefab";
        public const string TURN_CARTRIDGE_WARNING_SPRITE = "turnCartridgeWarning";
        public const string TURN_WARNING_LEFT_SPRITE = "turnLeft";
        public const string TURN_WARNING_RIGHT_SPRITE = "turnRight";
        public const string TURN_WARNING_TOP_SPRITE = "turnTop";
        public const string TURN_WARNING_BOTTOM_SPRITE = "turnBottom";
        public const string NORMAL_HOLE_GENERATOR_PREFAB = "NormalHoleGeneratorPrefab";
        public const string AIMING_HOLE_GENERATOR_PREFAB = "AimingHoleGeneratorPrefab";
    }
}
