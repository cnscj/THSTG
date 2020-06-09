using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASGame
{
    //运行时图集打包机,打包大小是2的次幂
    public class TexturePackager
    {
        //
        public TextureAtlas Package(Texture[] textures)
        {
            return null;
        }

        public TextureAtlas Package(Dictionary<string, Texture> textures)
        {
            return null;
        }

        public Texture[] Unpackage(TextureAtlas atlas)
        {
            return null;
        }

        public void AddSprite(TextureAtlas atlas, Texture texture, string name)
        {

        }

        public void RemoveSprite(TextureAtlas atlas, string name)
        {

        }

        public void SaveToFile(TextureAtlas atlas)
        {

        }

        public TextureAtlas LoadFromFile(string path)
        {
            return null;
        }
    }

}
