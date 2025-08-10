using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class WoodCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.WoodCube;


        public override CubeType PlaceCubeType => CubeType.Wood;
    }
}