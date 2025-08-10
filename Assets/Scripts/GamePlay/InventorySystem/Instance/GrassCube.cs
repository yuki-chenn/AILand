using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;

namespace GamePlay.InventorySystem.Instance
{
    public class GrassCube : BaseItem
    {
        public override ItemEnum itemEnum => ItemEnum.GrassCube;


        public override CubeType PlaceCubeType => CubeType.Grass;
    }
}