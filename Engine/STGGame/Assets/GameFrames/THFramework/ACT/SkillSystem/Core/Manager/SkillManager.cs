using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SkillManager : MonoSingleton<SkillManager>
    {
        private SkillCountdownCache _skillCountdownCache;
        private SkillCommandCache _skillCommandCache;

        public SkillCountdownCache GetCountdownCache(){return _skillCountdownCache = _skillCountdownCache ?? CreateManager<SkillCountdownCache>("CountdownCache");}
        public SkillCommandCache GetCommandCache() {return _skillCommandCache = _skillCommandCache ?? CreateManager<SkillCommandCache>("CommandCache");}


        protected T CreateManager<T>(string name) where T : MonoBehaviour
        {
            GameObject managerGObj = new GameObject(name);
            managerGObj.transform.SetParent(transform);
            T manager = managerGObj.AddComponent<T>();

            return manager;
        }
    }
}
