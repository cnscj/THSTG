using System;
namespace THGame.Package.MVC
{
    public class Controller
    {

        public bool Initialize()
        {
            OnOpen();
            return true;
        }

        public void Dispost()
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
