using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame.UI
{
    public static class ItemUtil
    {
        public static bool IsStone(ItemData data)
        {
            return data != null ? (data.GetCategory() == ItemCategory.Props && data.GetType() == ItemType.Stone) : false ;
        }
    }

}
