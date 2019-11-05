
using UnityEngine;
namespace STGService
{
    public class DisplayComponent : MonoBehaviour
    {
        public string spriteCode = "";

        [HideInInspector] public GameObject displayGO = null;
        [HideInInspector] public Animator animator = null;
		//全部只保留对数据操作的方法,不然写起来太难受了
		private void Start()
        {
            var entity = gameObject;
            var bodyNode = entity.transform.Find("Body");
            if (bodyNode == null)
            {
                bodyNode = new GameObject("Body").transform;
                bodyNode.SetParent(entity.transform);
            }
            //
            if (spriteCode != "")
            {
                var sprite = AssetManager.GetInstance().LoadSprite(spriteCode);
                if (sprite)
                {
                    displayGO = Instantiate(sprite, bodyNode.transform);
                    animator = displayGO.GetComponent<Animator>();
                }
            }

        }
    }
}
