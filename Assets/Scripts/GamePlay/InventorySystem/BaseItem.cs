using AILand.GamePlay.World;
using AILand.System.SOManager;

namespace AILand.GamePlay.InventorySystem
{
    public abstract class BaseItem
    {
        public virtual ItemEnum itemEnum { get; set; }

        public ItemConfigSO config { get { return SOManager.Instance.itemConfigDict[itemEnum]; } }
    }
}