using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{

    public abstract class ToolTipBase : FWidget
    {
        ToolTipData _toolTipData;
        public ToolTipBase(string package,string component):base(package, component)
        {
            
        }

        public virtual void SetToolTipData(ToolTipData data)
        {
            _toolTipData = data;
            OnState(data);
        }

        public ToolTipData GetToolTipData()
        {
            return _toolTipData;
        }

        ///
        protected virtual void OnState(ToolTipData data)
        {

        }
    }
}

