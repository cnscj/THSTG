using Entitas;

namespace STGU3D
{
    public class GameFeature : Feature
    {
        public GameFeature(Contexts contexts)
        {
            Add(new MovementSystem(contexts));
            //Add(new TransformSystem(contexts));
            Add(new ViewInitializeSystem(contexts));
            Add(new ViewReactiveSystem(contexts));
            Add(new ViewExecuteSystem(contexts));

            Add(new ShotExecuteSystem(contexts));
            Add(new BombReactiveSystem(contexts));
            Add(new EliminateExecuteSystem(contexts));

            Add(new DestroyedSystem(contexts));
        }
    }
}

