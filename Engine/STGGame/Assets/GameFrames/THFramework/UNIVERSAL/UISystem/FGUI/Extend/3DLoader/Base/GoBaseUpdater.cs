using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
namespace THGame.UI
{
    public abstract class GoBaseUpdater
    {
       public abstract void Update(GoUpdateContext context);
    }

}