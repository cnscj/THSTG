using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
namespace STGGame
{
    public class ControllerAdaptor : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(XLibrary.MVC.Controller);//这是你想继承的那个类
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adaptor);//这是实际的适配器类
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);//创建一个新的实例
        }

        //实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
        public class Adaptor : XLibrary.MVC.Controller, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod m_OnOpenMethod;
            bool m_OnOpenMethodGot;

            IMethod m_OnCloseMethod;
            bool m_OnCloseMethodGot;

            public Adaptor()
            {

            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            //你需要重写所有你希望在热更脚本里面重写的方法，并且将控制权转到脚本里去
            protected override void OnOpen()
            {
                if (!m_OnOpenMethodGot)
                {
                    m_OnOpenMethod = instance.Type.GetMethod("OnOpen", 0);
                    m_OnOpenMethodGot = true;
                }
                if (m_OnOpenMethod != null)
                {
                    appdomain.Invoke(m_OnOpenMethod, instance, null);//没有参数建议显式传递null为参数列表，否则会自动new object[0]导致GC Alloc
                }
            }

            protected override void OnClose()
            {
                if (!m_OnCloseMethodGot)
                {
                    m_OnCloseMethod = instance.Type.GetMethod("OnClose", 0);
                    m_OnCloseMethodGot = true;
                }
                if (m_OnCloseMethod != null)
                {
                    appdomain.Invoke(m_OnCloseMethod, instance, null);
                }
            }
        }
    }
}
