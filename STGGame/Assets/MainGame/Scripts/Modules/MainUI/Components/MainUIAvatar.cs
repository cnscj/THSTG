using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
    public class MainUIAvatar : FWidget
    {
        FLabel lvLabel;
        FRichText nameText;
        public MainUIAvatar() : base("MainUI", "MainUIAvatar")
        {

        }
        public void SetText(string text)
        {
            lvLabel.SetText("123");
            nameText.SetText(text);
        }

        protected override void OnInitUI()
        {
            lvLabel = GetChild<FLabel>("lvLabel");
            nameText = GetChild<FRichText>("nameText");
        }
    }

}
