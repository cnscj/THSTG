using UnityEngine;
using XLibrary.Package;

namespace STGGame.Editor
{
    public class LevelEditor : MonoSingleton<LevelEditor>
    {
        public GameObject root = null;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnValidate()
        {
            root = GameObject.Find("Root");
        }
    }

}
