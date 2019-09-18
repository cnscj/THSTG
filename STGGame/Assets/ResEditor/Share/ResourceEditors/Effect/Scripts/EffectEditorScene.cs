using UnityEngine;

namespace STGGame.Editor
{
    public class EffectEditorScene : MonoBehaviour
    {
        private static EffectEditorScene s_instance;
        public GameObject skillFxNode;
        public GameObject modelFxNode;
        public GameObject sceneFxNode;
        public GameObject uiFxNode;
        public static EffectEditorScene GetInstance()
        {
            return s_instance;
        }

        private void Awake()
        {
            s_instance = this;
        }
        void Start()
        {

        }

        
        void Update()
        {

        }

        private void OnValidate()
        {
            s_instance = this;
            skillFxNode = skillFxNode ? skillFxNode : GameObject.Find("SkillFx");
            modelFxNode = modelFxNode ? modelFxNode : GameObject.Find("ModelFx");
            sceneFxNode = sceneFxNode ? sceneFxNode : GameObject.Find("SceneFx");
            uiFxNode = uiFxNode ? uiFxNode : GameObject.Find("UIFx");
        }
    }

}
