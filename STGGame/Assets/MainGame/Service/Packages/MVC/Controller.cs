﻿using System;
namespace STGService.MVC
{
    public class Controller
    {
        public static T Get<T>() where T : Controller, new()
        {
            return MVCManager.GetInstance().GetController<T>();
        }

        public bool Initialize()
        {
            OnAdded();
            return true;
        }

        public void Dispose()
        {
            OnRemoved();
        }

        protected virtual void OnAdded()
        {

        }

        protected virtual void OnRemoved()
        {

        }

        ////



    }
}