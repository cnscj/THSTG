using System;
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
            OnOpen();
            return true;
        }

        public void Dispose()
        {
            OnClose();
        }

        protected virtual void OnOpen()
        {

        }

        protected virtual void OnClose()
        {

        }

        ////



    }
}
