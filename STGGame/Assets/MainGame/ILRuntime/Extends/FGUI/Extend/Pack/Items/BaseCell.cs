using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGService.UI
{
    public abstract class BaseCell : FWidget
    {
        public BaseCell(string packageName, string componentName) : base(packageName, componentName)
        {

        }
        public void SetItemData(ItemData data)
        {

        }

        public void GetItemData()
        {

        }
    }
}

