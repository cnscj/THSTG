using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FRichText : FLabel
    {

        public void SetAutoSize(AutoSizeType type)
        {
            _obj.asRichTextField.autoSize = type;
        }


    }

}
