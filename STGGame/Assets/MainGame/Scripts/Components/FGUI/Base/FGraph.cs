using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FGraph : FComponent
    {
        public void DrawRect(float aWidth, float aHeight, int lineSize, Color lineColor, Color fillColor)
        {
            _obj.asGraph.DrawRect(aWidth, aHeight, lineSize, lineColor, fillColor);
        }

        public void SetNativeObject(DisplayObject displayObject)
        {
            _obj.asGraph.SetNativeObject(displayObject);
        }
    }

}
