
using THGame;
using UnityEngine;

namespace STGGame
{
    public class SpiralCustom : ObjectEmitCustom
    {
        public int range;

        public override ObjectEmitCalculateResult Calculate(ObjectEmitCalculateParams args)
        {
            ObjectEmitCalculateResult result = new ObjectEmitCalculateResult();

            return result;
        }
        public override void DrawGizmos(ObjectEmitter emitter)
        {

        }
    }
}

