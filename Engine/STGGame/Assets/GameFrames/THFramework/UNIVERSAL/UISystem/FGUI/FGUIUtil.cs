using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    /// <summary>
    /// 这个类的作用是封装一些Fgui的方法，
    /// </summary>
    public static class FGUIUtil
    {
        public class BaseArgs
        {
            public Vector2 xy = new Vector2(0,0);
            public Vector2 pivot = new Vector2(0,0);
            public bool pivotAsAnchor = false;
            public Vector2 scale = new Vector2(1, 1);
            public float alpha = 1f;
            public bool center = false;

            public FComponent parent = UIManager.GetInstance().Root;
        }

        public class LabelArgs : BaseArgs
        {
            public class Style
            {
                public Color color = Color.black;
                public int size = 20;
                public string font = "";
                public bool underline = false;
            }
            public Style style = new Style();
        }

        public class GraphArgs : BaseArgs
        {

        }

        public class ButtonArgs : BaseArgs
        {

        }

        public class RichTextArgs : BaseArgs
        {

        }
        ///////////////////////////
        
        public static FComponent NewComponent(BaseArgs baseArgs)
        {
            var fComponent = NewT<FComponent, GComponent>(baseArgs);
            return fComponent;
        }

        public static FButton NewButton(ButtonArgs buttonArgs)
        {
            var fButton = NewT<FButton, GButton>(buttonArgs);
            return fButton;
        }

        public static FGraph NewGraph(GraphArgs graphArgs)
        {
            var fGraph = NewT<FGraph, GGraph>(graphArgs);
            return fGraph;
        }

        public static FLabel NewLabel(LabelArgs labelArgs)
        {
            var fLabel = NewT<FLabel, GLabel>(labelArgs);
            fLabel.SetColor(labelArgs.style.color);


            return fLabel;
        }

        public static FRichText NewRichText(RichTextArgs richTextArgs)
        {
            var fRichText = NewT<FRichText, GRichTextField>(richTextArgs);
           
            return fRichText;
        }


        private static T1 NewT<T1, T2>(BaseArgs baseArgs) where T1 : FComponent, new() where T2 : GObject, new()
        {
            var fCompomnet = FComponent.Create<T1>(new T2());
            InitBaseArgs(fCompomnet, baseArgs);

            return fCompomnet;
        }

        /// <summary>
        /// 初始化基准组件
        /// </summary>
        /// <param name="fComponent"></param>
        /// <param name="baseArgs"></param>
        private static void InitBaseArgs(FComponent fComponent, BaseArgs baseArgs)
        {
            if (fComponent == null)
                return;

            if (baseArgs == null)
                return;

            fComponent.SetXY(baseArgs.xy);
            fComponent.SetPivot(baseArgs.pivot.x, baseArgs.pivot.y, baseArgs.pivotAsAnchor);
            fComponent.SetScale(baseArgs.scale.x, baseArgs.scale.y);
            fComponent.SetAlpha(baseArgs.alpha);

            if (baseArgs.parent != null)
            {
                baseArgs.parent.AddChild(fComponent);
            }
        }
    }
}
