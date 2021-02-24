using UnityEngine;

namespace THGame
{
    public class SkillPrintTrigger : AbstractSkillTrigger
    {
        public string outStr;
        public override void OnCreate(string[] info,string[] args)
        {
            outStr = args.Length > 0 ? args[0] : "";
        }
        public override void OnStart(object sender)
        {
            Debug.Log(outStr);
        }
    }
}
