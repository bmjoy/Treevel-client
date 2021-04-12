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
        public override async UniTask InitializeAsync(BottleData bottleData)
        {
            await base.InitializeAsync(bottleData);

            #if UNITY_EDITOR
            name = Constants.BottleName.ATTACKABLE_DUMMY_BOTTLE;
            #endif

            // set handler
            var lifeAttribute = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_ATTRIBUTE_PREFAB);
            lifeAttribute.GetComponent<LifeAttributeController>().Initialize(this, 1);
        }
    }
}
