using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class SnowCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.SnowCube;


        public override CubeType PlaceCubeType => CubeType.Snow;
    }
}