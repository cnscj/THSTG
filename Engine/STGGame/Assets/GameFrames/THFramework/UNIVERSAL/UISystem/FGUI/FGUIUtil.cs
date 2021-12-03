using System;
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
            public string packageName;
            public string componentName;

            public Vector2 xy = new Vector2(0,0);
            public Vector2 size = new Vector2(100, 100);
            public Vector2 pivot = new Vector2(0,0);
            public bool pivotAsAnchor = false;
            public Vector2 scale = new Vector2(1, 1);
            public float alpha = 1f;
            public bool center = false;
            public string title;

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
            public class Wrapper
            {
                public GameObject target;
                public bool cloneMaterial;
            }
            public Wrapper wrapper;
        }

        public class ButtonArgs : BaseArgs
        {
            public EventCallback1 onClick;
        }

        public class RichTextArgs : BaseArgs
        {

        }

        public class TextInputArgs : BaseArgs
        {
            public string promptText;
        }

        public class ListArgs : BaseArgs
        {
            public FComponent template;
            public FList.ItemStateFuncT0 onState;
            public EventCallback1 onClickItem;
        }

        public class LoaderArgs : BaseArgs
        {
            public string url;
        }
        ///////////////////////////

        public static FComponent NewComponent(BaseArgs baseArgs = null)
        {
            var fComponent = NewT<FComponent, GComponent>(baseArgs);
            return fComponent;
        }

        public static FButton NewButton(ButtonArgs buttonArgs = null)
        {
            var fButton = NewT<FButton, GButton>(buttonArgs);
            if (buttonArgs != null)
            {
                if (buttonArgs.onClick != null) fButton.OnClick(buttonArgs.onClick);
            }

            return fButton;
        }

        public static FGraph NewGraph(GraphArgs graphArgs = null)
        {
            var fGraph = NewT<FGraph, GGraph>(graphArgs);
            if (graphArgs != null)
            {
                if (graphArgs.wrapper != null)
                {
                    var goWrapper = new GoWrapper();
                    goWrapper.SetWrapTarget(graphArgs.wrapper.target, graphArgs.wrapper.cloneMaterial);
                    fGraph.SetNativeObject(goWrapper);
                }
            }
            return fGraph;
        }

        public static FLabel NewLabel(LabelArgs labelArgs = null)
        {
            var fLabel = NewT<FLabel, GLabel>(labelArgs);
            if (labelArgs != null)
            {
                fLabel.SetColor(labelArgs.style.color);
            }

            return fLabel;
        }

        public static FRichText NewRichText(RichTextArgs richTextArgs = null)
        {
            var fRichText = NewT<FRichText, GRichTextField>(richTextArgs);
            if (richTextArgs != null)
            {

            }
            return fRichText;
        }

        public static FTextInput NewTextInput(TextInputArgs textInputArgs = null)
        {
            var fTextInput = NewT<FTextInput, GTextInput>(textInputArgs);
            if (textInputArgs != null)
            {
                fTextInput.SetPlaceHolder(textInputArgs.promptText);
            }
            
            return fTextInput;
        }

        public static FLoader NewLoader(LoaderArgs loaderArgs = null)
        {
            var fLoader = NewT<FLoader>(UIObjectFactory.NewObject(ObjectType.Loader), loaderArgs);
            InitBaseArgs(fLoader, loaderArgs);
            if (loaderArgs != null)
            {
                fLoader.SetUrl(loaderArgs.url);
            }
            
            return fLoader;
        }

        public static FList NewList(ListArgs listArgs)
        {
            var fList = NewT<FList, GList>(listArgs);
            if (listArgs != null)
            {
                if (listArgs.onState != null) fList.SetState(listArgs.onState);
                if (listArgs.onClickItem != null) fList.OnClickItem(listArgs.onClickItem);
            }

            return fList;
        }

        private static T NewT<T>(GObject fguiObj, BaseArgs baseArgs) where T : FComponent, new()
        {
            T fComponent = null;
 
            if (baseArgs != null && !string.IsNullOrEmpty(baseArgs.packageName))
            {
                fComponent = FComponent.Create<T>(baseArgs.packageName, baseArgs.componentName);
            }
            else
            {
                fComponent = FComponent.Create<T>(fguiObj);
            }
            InitBaseArgs(fComponent, baseArgs);
            return fComponent;
        }
        private static T1 NewT<T1, T2>(BaseArgs baseArgs) where T1 : FComponent, new() where T2 : GObject, new()
        {
            var fComponent = NewT<T1>(new T2(), baseArgs);
            InitBaseArgs(fComponent, baseArgs);

            return fComponent;
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
            fComponent.SetSize(baseArgs.size);
            fComponent.SetPivot(baseArgs.pivot.x, baseArgs.pivot.y, baseArgs.pivotAsAnchor);
            fComponent.SetScale(baseArgs.scale.x, baseArgs.scale.y);
            fComponent.SetAlpha(baseArgs.alpha);
            fComponent.SetText(baseArgs.title);

            if (baseArgs.center) fComponent.Center();
            if (baseArgs.parent != null) baseArgs.parent.AddChild(fComponent);
        }

        ///////////////////////////
        ///
        public static GComponent CreateLayerObject(int sortingOrder, string layerName = null)
        {
            var obj = new GComponent();
            obj.sortingOrder = sortingOrder;
            obj.SetSize(GRoot.inst.width, GRoot.inst.height);
            obj.AddRelation(GRoot.inst, RelationType.Size);
            GRoot.inst.AddChild(obj);

            if (!string.IsNullOrEmpty(layerName))
            {
                obj.rootContainer.gameObject.name = layerName;
            }

            return obj;
        }

        public static string GetUIUrl(string package, string component)
        {
            return UIPackage.GetItemURL(package, component);
        }

        ///////////////////////////


        public static Texture2D Texture2Texture2d(Texture texture, Rect source)
        {
            if (texture == null)
                return default;

            Texture2D texture2D = new Texture2D((int)source.width, (int)source.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(source, 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;

        }

        //长变宽,宽边长
        public static Texture2D Texture2DRotate(Texture2D ori)
        {
            Texture2D newTexture = new Texture2D(ori.height, ori.width, ori.format, false);
            for (int i = 0; i <= ori.width - 1; i++)
            {
                for (int j = 0; j <= ori.height - 1; j++)
                {
                    Color color = ori.GetPixel(i, j);
                    newTexture.SetPixel(j, newTexture.height - 1 - i, color);
                }
            }
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTex"></param>
        /// <returns></returns>
        public static Texture2D NTexture2Texture2d(NTexture nTex)
        {
            if (nTex == null)
                return default;

            bool isRotated = nTex.rotated;
        Texture2D oriT2d = (Texture2D)nTex.nativeTexture;

        var uvX = nTex.uvRect.x * oriT2d.width;
        var uvY = 0f;

#if UNITY_STANDALONE_WIN
            var nTexHeight = isRotated ? nTex.width : nTex.height;
            uvY = oriT2d.height - nTex.uvRect.y * oriT2d.height - nTexHeight;
#else
        uvY = nTex.uvRect.y* oriT2d.height;
#endif
        Texture2D newT2d = Texture2Texture2d(oriT2d, new Rect(uvX, uvY, isRotated ? nTex.height : nTex.width, isRotated ? nTex.width : nTex.height));

            //这里需要处理下UV偏移和翻转问题,整个图像向右旋转90
 
            if (isRotated)
            {
                newT2d = Texture2DRotate(newT2d);
            }

            return newT2d;
        }
    }
}
