using System;
namespace THGame
{
    namespace MVC
    {
        public class Module
        {
            private Controller m_controller;
            private View m_view;

            protected virtual Controller OnController()
            {
                return null;
            }

            protected virtual View OnView()
            {
                return null;
            }

            public bool Init()
            {
                m_controller = OnController();
                m_view = OnView();

                return true;
            }

            public Controller GetController()
            {
                return m_controller;
            }

            public View GetView()
            {
                return m_view;
            }
        }
    }
}
