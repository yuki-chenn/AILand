using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class SandCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.SandCube;


        public override CubeType PlaceCubeType => CubeType.Sand;
    }
}