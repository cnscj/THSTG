
using UnityEngine;

namespace THGame
{
    public abstract class ObjectEmitCustom : MonoBehaviour
    {
        public abstract ObjectEmitCalculateResult Calculate(ObjectEmitCalculateParams args);
        public virtual void DrawGizmos(ObjectEmitter emitter) { }
    }
}

