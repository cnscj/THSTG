
using System;
using FairyGUI;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    //TODO:
    public class UIManager : MonoSingleton<UIManager>
    {
        public FRoot Root => FRoot.GetInstance();


        private void Awake()
        {
            SetLoaderExtension(typeof(XLoader));
        }

        // FontManager
        public void RegisterFont(BaseFont font)
        {
            FontManager.RegisterFont(font);
        }

        public void UnregisterFont(BaseFont font)
        {
            FontManager.UnregisterFont(font);
        }

        // ViewManager
        public void OpenView<T>(object args = null) where T : FView, new()
        {
            ViewManager.GetInstance().Open<T>(args);
        }

        public void CloseView<T>() where T : FView, new()
        {
            ViewManager.GetInstance().Close<T>();
        }

        public void IsViewOpened<T>() where T : FView, new()
        {
            ViewManager.GetInstance().IsOpened<T>();
        }

        // Stage
        public void AddStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Add(callback1);
        }
        public void RemoveStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Remove(callback1);
        }

        public void AddStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.AddCapture(callback1);
        }

        public void RemoveStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.RemoveCapture(callback1);
        }

        public void AddStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.AddCapture(callback1);
        }

        public void RemoveStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.RemoveCapture(callback1);
        }

        public void SetStageOnKeyDown(EventCallback1 callback1)
        {
            Stage.inst.onKeyDown.Set(callback1);
        }

        public void ClearStageOnKeyDown()
        {
            Stage.inst.onKeyDown.Clear();
        }

        public Vector2 GetStageSize()
        {
            return Stage.inst.size;
        }

        //GRoot
        public void SetRootContentScaleFactor(int width, int height)
        {
            GRoot.inst.SetContentScaleFactor(width, height);
        }

        public void AddRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Add(callback0);
        }

        public void RemoveRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Remove(callback0);
        }

        public void ShowPopup(FObject obj)
        {
            Root.ShowPopup(obj);
        }

        public void HidePopup(FObject obj)
        {
            Root.HidePopup(obj);
        }

        //UIObjectFactory
        public void SetLoaderExtension(Type type)
        {
            UIObjectFactory.SetLoaderExtension(type);
        }
        public void SetPackageItemExtension(string url,Type type)
        {
            UIObjectFactory.SetPackageItemExtension(url,type);
        }

        //
        public string GetUIUrl(string package,string component)
        {
            return UIPackage.GetItemURL(package, component);
        }
    }
}