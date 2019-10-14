using UnityEngine;
using System.Collections;
using THGame;
using System.Collections.Generic;

namespace STGGame
{
	public static class GameConfig
    {
        public static readonly Dictionary<EResType, string> resTypeMap = new Dictionary<EResType, string>()
        {
            [EResType.Entity] = "entities",
            [EResType.Audio] = "audios",
            [EResType.Level] = "levels",
            [EResType.Model] = "models",
            [EResType.Sprite] = "sprites",
            [EResType.Effect] = "effects",

            [EResType.UI] = "uis",
            [EResType.Shader] = "shaders",
            [EResType.Config] = "configs",
        };
    }

}
