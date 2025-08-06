using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class StoneCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.StoneCube;


        public override CubeType PlaceCubeType => CubeType.Stone;
    }
}