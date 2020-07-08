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
        public const string START_UP_SCENE = "StartUpScene";
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
        public const string ROAD = "Road";
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
        public const string TREE = "Tree";
        public const string ROAD = "Road";
        public const string STAGE = "Stage";
        public const string BRANCH = "Branch";
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
        public const string ATTACKABLE_DUMMY_BOTTLE_PREFAB = "AttackableDummyBottlePrefab";
        public const string DYNAMIC_DUMMY_BOTTLE_SPRITE = "dynamicDummyBottle";
        public const string STATIC_DUMMY_BOTTLE_SPRITE = "staticDummyBottle";
        public const string NORMAL_BOTTLE_SPRITE_PREFIX = "normalBottle";
        public const string LIFE_BOTTLE_SPRITE_PREFIX = "lifeBottle";

        // タイル関連
        public const string NORMAL_TILE_PREFAB = "NormalTilePrefab";
        public const string WARP_TILE_PREFAB = "WarpTilePrefab";
        public const string HOLY_TILE_PREFAB = "HolyTilePrefab";
        public const string SPIDERWEB_TILE_PREFAB = "SpiderwebTilePrefab";
        public const string ICE_TILE_PREFAB = "IceTilePrefab";
        public const string NUMBER_TILE_SPRITE_PREFIX = "numberTile";

        // 銃弾関連
        public const string TURN_CARTRIDGE_WARNING_SPRITE = "turnCartridgeWarning";
        public const string TURN_WARNING_LEFT_SPRITE = "turnLeft";
        public const string TURN_WARNING_RIGHT_SPRITE = "turnRight";
        public const string TURN_WARNING_UP_SPRITE = "turnUp";
        public const string TURN_WARNING_BOTTOM_SPRITE = "turnBottom";
        public const string NORMAL_HOLE_GENERATOR_PREFAB = "NormalHoleGeneratorPrefab";
        public const string AIMING_HOLE_GENERATOR_PREFAB = "AimingHoleGeneratorPrefab";

        // ギミック
        public const string TORNADO_PREFAB = "TornadoPrefab";
    }
}
