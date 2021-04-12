namespace Treevel.Common.Entities
{
    public enum EBottleAttributeType
    {
        Dark,
        Reverse,
        Selfish,
        Life,
    }

    public static class BottleAttributeTypeExtension
    {
        /// <summary>
        /// レイヤー内の描画順序を取得する
        /// 0番目はBottleの描画順序なので、Attributeの順序は1番以降
        /// </summary>
        public static int GetOrderInLayer(this EBottleAttributeType type)
        {
            return (int)type + 1;
        }
    }
}
