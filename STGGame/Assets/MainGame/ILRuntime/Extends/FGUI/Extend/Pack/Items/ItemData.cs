using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGService.UI
{
    public class ItemData
    {
        ItemMetadata __itemMetadata;
        PackType __packType;

        public ItemData(string code)
        {

        }

        public ItemCategory GetCategory()
        {
            return ItemCategory.Props;
        }

        public new ItemType GetType()
        {
            return ItemType.Stone;
        }
    }

}
