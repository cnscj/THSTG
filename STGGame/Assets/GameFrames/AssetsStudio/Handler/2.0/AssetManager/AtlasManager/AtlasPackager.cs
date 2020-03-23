using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASGame
{
    //运行时图集打包机
    public class AtlasPackager
    {
        public class AtlasInfo
        {
            public string name;
            public Vector2 size;
            public int count;

            public Texture main;           //主图集
            public Dictionary<string, SpriteInfo> sprites;
        }

        public class SpriteInfo
        {
            public string name;
            public Vector2 size;
            public Vector2 position;
        }

        public void RemovePackage(string atlasKey)
        {

        }

        public void GetSprite(string atlasKey, string spriteKey)
        {

        }

        public void RemoveSprite(string atlasKey, string spriteKey)
        {

        }

        public void AddSprite(string atlasKey, string spriteKey, Texture texture)
        {

        }
    }

}
