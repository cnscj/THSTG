using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public static class GraphicsSettings
    {
        //画质质量(1~6)
        public static void SetQualityLevel(int level)
        {
            QualitySettings.SetQualityLevel(level,true);
        }

        public static void SetvSync(bool val)
        {
            if (val)
                QualitySettings.vSyncCount = 2;
            else
                QualitySettings.vSyncCount = 0;
        }

        //抗锯齿
    }
}

