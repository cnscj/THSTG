
using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;
using THGame.UI;

namespace THEditor.UI
{
    [CustomEditor(typeof(EList))]
    public class EListInspector : ScriptGUI<EList>
    {
        protected override void OnProps()
        {
            AddProperty("itemTmpl", "Group1", "预制");
        }

        protected override void OnShow()
        {
            ShowPropertys("Group1");

        }
    }
}
