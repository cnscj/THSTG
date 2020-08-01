using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
namespace THGame.UI
{
    public interface IGoUpdate
    {
        void Update(GoUpdateContext context);
    }

}