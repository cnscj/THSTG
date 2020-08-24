using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
namespace THGame.UI
{
    public abstract class GoBaseUpdater
    {
        public GoUpdateContext context;

        public virtual void OnAdded() { }
        public virtual void OnRemove() { }
        public virtual void OnReplace(GameObject oldGameObject, GameObject newGameObject) { }
        public virtual void OnUpdate() { }
        public virtual void OnReset() { }
    }

}