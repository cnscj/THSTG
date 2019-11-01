
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace STGGame
{
    public class ILRuntimeManager : MonoBehaviour
    {
        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
        AppDomain appdomain;

        System.IO.MemoryStream fs;
        System.IO.MemoryStream p;
        void Start()
        {
            StartCoroutine(LoadHotFixAssembly());
        }

        IEnumerator LoadHotFixAssembly()
        {
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            
            byte[] dll = AssetManager.GetInstance().LoadScript("THProject");
            fs = new MemoryStream(dll);

            appdomain.LoadAssembly(fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

            InitializeILRuntime();
            OnHotFixLoaded();

            yield return null;
        }

        void InitializeILRuntime()
        {
            //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
        }

        void OnHotFixLoaded()
        {
            //HelloWorld，第一次方法调用
            appdomain.Invoke("THHFGmae.Main", "Init", null, null);

        }

        private void OnDestroy()
        {
            if (fs != null)
                fs.Close();
            if (p != null)
                p.Close();
            fs = null;
            p = null;
        }

        void Update()
        {

        }
    }
}
