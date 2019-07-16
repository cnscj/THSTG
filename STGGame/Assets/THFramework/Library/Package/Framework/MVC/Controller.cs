using System;
namespace THGame.Package.MVC
{
    public class Controller
    {
        private View m_view;


        public View GetView()
        {
            return m_view;
        }

        public bool Initialize()
        {
            m_view = OnView();
            return true;
        }

        ////
        public virtual void Clear()
        {


        }

        protected virtual View OnView()
        {
            return null;
        }

    }
}
