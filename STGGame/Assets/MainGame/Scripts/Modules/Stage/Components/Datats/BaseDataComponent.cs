using UnityEngine;
namespace STGGame
{
    public class BaseDataCompnent : MonoBehaviour
    {
        public int code;            //代表code
        [HideInInspector]
        public EEntityType type;    //类型
    }

}
