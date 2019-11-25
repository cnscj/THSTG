using Entitas;

namespace STGU3D
{
    public class GameFeature : Feature
    {
        public GameFeature(Contexts contexts)
        {
            Add(new MovementSystem(contexts));
            Add(new TransformSystem(contexts));

            Add(new ViewReactiveSystem(contexts));
            Add(new ViewExecuteSystem(contexts));

            Add(new ShotExecuteSystem(contexts));
            Add(new BombReactiveSystem(contexts));
            Add(new EliminateExecuteSystem(contexts));
            //Add(new DecelerateSystem(contexts));

            Add(new HealthReactiveSystem(contexts));
            Add(new HealthExecuteSystem(contexts));

            Add(new RecycleSystem(contexts));
            Add(new CageSystem(contexts));

            Add(new DestroyedSystem(contexts));
        }
    }
}

