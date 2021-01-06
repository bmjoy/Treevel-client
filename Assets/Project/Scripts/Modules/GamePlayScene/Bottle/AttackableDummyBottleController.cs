using Cysharp.Threading.Tasks;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    /// <summary>
    /// ライフ付き、成功判定なし、フリック可能なボトル
    /// </summary>
    public class AttackableDummyBottleController : DynamicBottleController
    {
        public override async UniTask Initialize(BottleData bottleData)
        {
            await base.Initialize(bottleData);

            #if UNITY_EDITOR
            name = Constants.BottleName.ATTACKABLE_DUMMY_BOTTLE;
            #endif

            // set handler
            var lifeEffect = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_EFFECT_PREFAB).ToUniTask();
            lifeEffect.GetComponent<LifeEffectController>().Initialize(this, bottleData.life);
        }
    }
}
