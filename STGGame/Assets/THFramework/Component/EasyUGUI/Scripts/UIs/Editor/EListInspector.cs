
using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;
using THGame.UI;

namespace THEditor.UI
{
    [CustomEditor(typeof(EList))]
    public class EListInspector : EBaseInspector<EList>
    {
        protected override void OnProps()
        {
            AddPropertys("Group1", "预制", "itemTmpl");
        }

        protected override void OnIGUI()
        {
            ShowPropertys("Group1");

        }
    }
}
