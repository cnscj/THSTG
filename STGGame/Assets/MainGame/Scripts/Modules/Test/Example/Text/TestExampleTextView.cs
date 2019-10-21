using System;
using UnityEngine;

namespace STGGame.UI
{
    public class TestExampleTextView : FView
    {
        FRichText t1;
        FRichText t2;
        FRichText t3;
        public TestExampleTextView():base("Test", "TestExampleTextView")
        {
            
        }
        protected override void OnInitUI()
        {
            t1 = GetChild<FRichText>("t1");
            t2 = GetChild<FRichText>("t2");
            t3 = GetChild<FRichText>("t3");
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
