using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASGame
{
    //运行时图集打包机
    public class TextureAtlas
    {
        public class SpriteFrame
        {
            public string name;
            public Vector2 size;
            public Vector2 position;
        }

        public Vector2 size;
        public int count;

        public Texture texture;           //主图集
        public Dictionary<string, SpriteFrame> sprites;

        public Texture GetSprite(string name)
        {
            return null;
        }

        public Texture GetSrpte(Vector2 size, Vector2 position)
        {
            return null;
        }
    }

}
