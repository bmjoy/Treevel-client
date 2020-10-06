using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付き、成功判定なし、フリック可能なボトル
    /// </summary>
    public class AttackableDummyBottleController : DynamicBottleController
    {
        public override async void Initialize(GameDatas.BottleData bottleData)
        {
            base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = Utils.Definitions.BottleName.ATTACKABLE_DUMMY_BOTTLE;
            #endif

            // set handler
            var lifeEffect = await AddressableAssetManager.Instantiate(Address.LIFE_EFFECT_PREFAB).Task;
            lifeEffect.GetComponent<LifeEffectController>().Initialize(this, bottleData.life);
        }
    }
}
