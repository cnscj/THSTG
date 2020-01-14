using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:
namespace ASGame
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MapUVSprite : MonoBehaviour
    {
        public int uSpeed;
        public int vSpeed;

        public GameObject nextSprite;
        public SpriteRenderer m_spriteRenderer;

        void Start()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            if (nextSprite == null)
            {
                nextSprite = Object.Instantiate(gameObject, gameObject.transform.parent?.transform, false);

                var uvCom = nextSprite.GetComponent<MapUVSprite>();
                if (uvCom != null) Object.Destroy(uvCom);
            }
            
        }

        void Update()
        {
            UpdatePos();
        }

        void UpdatePos()
        {
            //动态根据速度方向,算出next的方向
            //运动轨迹是个椭圆
            Vector2 speed = new Vector2(uSpeed, vSpeed);
            var direction = speed.normalized;
        }


    }


}
