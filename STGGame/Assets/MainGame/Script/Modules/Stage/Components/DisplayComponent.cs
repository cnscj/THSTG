
using UnityEngine;
namespace STGGame
{
    public class DisplayComponent : MonoBehaviour
    {
        public GameObject displayBody;
        public int displayCode;

        //全部只保留对数据操作的方法,不然写起来太难受了
        private void Start()
        {
            //var entity = gameObject;
            //var displayComp = entity.GetComponent<DisplayCompnent>();
            //var bodyNode = entity.transform.Find("Body");
            //if (bodyNode == null)
            //{
            //    bodyNode = new GameObject("Body").transform;
            //    bodyNode.SetParent(entity.transform);
            //}
            ////实例化
        }
    }
}
