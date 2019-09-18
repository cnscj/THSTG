using System;
namespace XLibrary.MVC
{
    public class Controller
    {

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
