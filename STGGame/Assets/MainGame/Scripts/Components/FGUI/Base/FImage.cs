using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FImage : FComponent
    {
        public void SetPrecent(float value)
        {
            _obj.asImage.fillAmount = value;
        }
    }

}
