using Entitas;

namespace STGU3D
{
    public class GameFeature : Feature
    {
        public GameFeature(Contexts contexts)
        {
            Add(new MovementSystem(contexts));
        }
    }
}

