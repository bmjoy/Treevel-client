namespace Treevel.Common.Entities
{
    public enum EBottleEffectType
    {
        Dark,
        Reverse,
        Selfish,
        Life,
    }

    public static class BottleEffectTypeExtension
    {
        /// <summary>
        /// レイヤー内の描画順序を取得する
        /// 0番目はBottleの描画順序なので、Effectの順序は1番以降
        /// </summary>
        public static int GetOrderInLayer(this EBottleEffectType type)
        {
            return (int)type + 1;
        }
    }
}
