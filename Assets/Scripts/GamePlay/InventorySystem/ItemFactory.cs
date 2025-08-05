using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AILand.GamePlay.InventorySystem
{
    public static class ItemFactory
    {
        private static Dictionary<int, Type> itemTypeMap = new Dictionary<int, Type>();
        private static Dictionary<int, BaseItem> itemInstanceMap = new Dictionary<int, BaseItem>();

        static ItemFactory()
        {
            AutoRegisterItems();
        }

        public static void RegisterItem(BaseItem instance, Type type)
        {
            int id = instance.config.itemID;
            if (!itemTypeMap.TryAdd(id, type))
            {
                Debug.LogError($"itemTypeMap already contains ID {id} for type {itemTypeMap[id]}. Cannot register {type.Name} again.");
                return;
            }

            itemInstanceMap.TryAdd(id, instance);
            Debug.Log($"register item: {type.Name} with ID {id}");
        }

        public static BaseItem GetItemByID(int id)
        {

            if (!itemInstanceMap.ContainsKey(id))
            {
                if (itemTypeMap.ContainsKey(id))
                {
                    Type type = itemTypeMap[id];
                    itemInstanceMap[id] = (BaseItem)Activator.CreateInstance(type);
                }
                else
                {
                    // 没有找到对应的道具
                    Debug.LogError("未找到ID为 " + id + " 的道具类型");
                    return null;
                }
            }

            return itemInstanceMap[id];


        }

        // 通过反射自动注册道具类型
        public static void AutoRegisterItems()
        {
            var itemType = typeof(BaseItem);
            var assembly = Assembly.GetAssembly(itemType);

            var itemTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(itemType));

            foreach (var type in itemTypes)
            {
                var instance = (BaseItem)Activator.CreateInstance(type);
                RegisterItem(instance, type);
            }
        }
    }
}