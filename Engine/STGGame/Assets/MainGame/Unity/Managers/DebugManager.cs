
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary.Package;
using XLibGame.Debugger;

using Logger = XLibGame.Debugger.Logger;
namespace STGU3D
{
    public class DebugManager : MonoSingleton<SpriteManager>
    {
        [Header("日志收集")]
        public bool isOpenLog = true;        //是否开启日志收集
        public string logName = "Log.txt";
        public float logMaxKb = -1;

        [Header("Bug上报")]
        public bool isOpenBugUpload = false;
        public string bugUploadUrl = "";
        public string bugUploadKey = "";

        private DebugManager()
        {
            

        }

        private void Awake()
        {
            if (isOpenLog)
            {
                Logger.GetInstance().logPath = Path.Combine(Application.persistentDataPath, string.Format(logName));
                Logger.GetInstance().maxKb = logMaxKb;
                Logger.GetInstance().Open();
            }
            if (isOpenBugUpload)
            {

            }
        }

        private void OnDestroy()
        {
            Logger.GetInstance().Close();
        }
    }
}
