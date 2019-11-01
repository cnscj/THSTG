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
            [EResType.Entity] = "Entities",
            [EResType.Audio] = "Audios",
            [EResType.Level] = "Levels",
            [EResType.Model] = "Models",
            [EResType.Sprite] = "Sprites",
            [EResType.Effect] = "Effects",

            [EResType.UI] = "UIs",
            [EResType.Shader] = "Shaders",
            [EResType.Config] = "Configs",
            [EResType.Script] = "Scripts",
        };
    }

}
