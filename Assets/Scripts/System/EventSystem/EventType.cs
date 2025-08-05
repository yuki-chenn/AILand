
namespace AILand.System.EventSystem
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventType
    {
        #region Base

        OnShowUIPanel, // 显示UI面板
        OnHideUIPanel, // 隐藏UI面板

        #endregion



        #region MenuScene

        // LoadScene
        StartGame, // 开始游戏

        #endregion
        
        
        
        ShowDrawIslandShapePanelUI, // 显示绘制岛屿面板
        ShowDrawIslandCellTypePanelUI, // 显示绘制岛屿格子类型面板
        PlayerCreateIsland, // 玩家创建岛屿
        
        
        
        OpenBag, // 打开背包
        OpenChest, // 打开箱子
        SwitchItemInInventoryData, // 更换数据背包中的item
        RefreshBagInventory, // 更新了背包数据
        SelectInventoryItemChange, // 选择的背包物品改变
    }
}

