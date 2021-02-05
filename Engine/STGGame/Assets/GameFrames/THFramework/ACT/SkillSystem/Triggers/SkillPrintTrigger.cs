﻿using UnityEngine;

namespace THGame
{
    public class SkillPrintTrigger : AbstractSkillTrigger
    {
        public string outStr;
        protected override void OnCreate(string[] info,string[] args)
        {
            outStr = args.Length > 0 ? args[0] : "";
        }
        protected override void OnStart(object sender)
        {
            Debug.Log(outStr);
        }
    }
}
