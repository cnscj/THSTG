﻿using System;
using THGame.UI;
using UnityEngine;

namespace STGRuntime.UI
{
    public class TestView : FView
    {
        FButton containerBtn;
        TestContainerComp containerComp;
        public TestView():base("Test","TestView")
        {
            
        }
        protected override void OnInitUI()
        {
            containerBtn = GetChild<FButton>("containerBtn");
            containerComp = GetChild<TestContainerComp>("containerComp");

            containerBtn.OnClick((context) =>
            {
                containerComp.SetVisible(!containerComp.IsVisible());
            });

            containerComp.DebugUI();
        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            
        }

        protected override void OnExit()
        {

        }
    }

}
