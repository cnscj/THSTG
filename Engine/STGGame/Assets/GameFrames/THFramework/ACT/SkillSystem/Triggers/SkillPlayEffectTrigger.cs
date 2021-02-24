using UnityEngine;
using UnityEditor;
namespace THGame
{
    public class SkillPlayEffectTrigger : AbstractSkillTrigger
    {
        public string effectId;
        public override void OnCreate(string[] info,string[] args)
        {
            effectId = args.Length > 0 ? args[0] : "";
        }
        public override void OnStart(object owner)
        {
            var go = owner as GameObject;
            string resPath = string.Format("Assets/ZCustom_Test/Res/Effects/Prefabs/{0}.prefab", effectId);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resPath);

            Object.Instantiate(prefab, go.transform);
        }
    }
}
