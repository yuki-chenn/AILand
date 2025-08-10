using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class DirtCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.DirtCube;


        public override CubeType PlaceCubeType => CubeType.Dirt;
    }
}