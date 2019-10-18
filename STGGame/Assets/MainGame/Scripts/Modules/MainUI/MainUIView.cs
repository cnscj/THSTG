using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame
{
    public class MainUIView : FView
    {
        GGraph n1; 
        public MainUIView() : base("MainUI", "MainUIView")
        {

        }

        protected override void OnInitUI()
        {
            n1 = GetChild("n1") as GGraph;
        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            Debug.Log("MainUI-enter");
        }

        protected override void OnExit()
        {

        }
    }
}


