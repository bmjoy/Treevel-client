using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class BulletData
    {
        public EBulletType type;
        public int ratio;

        public int line;

        public ECartridgeDirection direction;

        public List<ECartridgeDirection> turnDirections;

        public List<int> turnLines;

        public List<int> randomCartridgeDirection;
        public List<int> randomRow;
        public List<int> randomColumn;

        public List<int> randomTurnDirection;
        public List<int> randomTurnRow;
        public List<int> randomTurnColumn;
    }
}
