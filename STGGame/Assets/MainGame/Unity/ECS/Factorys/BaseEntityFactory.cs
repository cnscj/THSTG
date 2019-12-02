using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public abstract class BaseEntityFactor
    {
        public abstract GameEntity CreateEntity(string code); 
    }
}
