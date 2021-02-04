using UnityEngine;

namespace THGame
{
    public class SkillPrintTrigger : AbstractSkillTrigger
    {
        public string outStr;
        protected override void OnPares(string[] info,string[] args)
        {
            outStr = args[0];
        }
        protected override void OnStart(object sender)
        {
            Debug.Log(outStr);
        }
    }
}
