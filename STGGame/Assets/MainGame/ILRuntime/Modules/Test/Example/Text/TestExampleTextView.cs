using System;
using UnityEngine;

namespace STGService.UI
{
    public class TestExampleTextView : FView
    {
        FRichText t1;
        FRichText t2;
        FRichText t6;
        FTextInput t3;
        FTextInput t4;
        FGraph n7;
        public TestExampleTextView():base("Test", "TestExampleTextView")
        {
            
        }
        protected override void OnInitUI()
        {
            t1 = GetChild<FRichText>("t1");
            t2 = GetChild<FRichText>("t2");
            t3 = GetChild<FTextInput>("t3");
            t4 = GetChild<FTextInput>("t4");
            t6 = GetChild<FRichText>("t6");
            n7 = GetChild<FGraph>("n7");

            t4.OnChanged((context) =>
            {
                t3.SetText(t4.GetText());
            });

            n7.OnClick((context) =>
            {
                System.Random random = new System.Random();
                t6.SetVar("num", random.Next(1, 100).ToString());
            });
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
