using Entitas;

namespace STGU3D
{
    public class GameFeature : Feature
    {
        public GameFeature(Contexts contexts)
        {
            Add(new MovementSystem(contexts));
            Add(new TransformSystem(contexts));

            Add(new ViewAnimationSystem(contexts));
            Add(new ViewRuntimeTransformSystem(contexts));
            Add(new ViewEditorTransformSystem(contexts));

            Add(new ShotExecuteSystem(contexts));
            Add(new BombReactiveSystem(contexts));
            Add(new EliminateExecuteSystem(contexts));

            Add(new InvincibleExecuteSystem(contexts));
            Add(new FollowSystem(contexts));

            Add(new HealthReactiveSystem(contexts));
            Add(new HealthExecuteSystem(contexts));
            Add(new LifeReactiveSystem(contexts));

            Add(new RecycleSystem(contexts));
            Add(new CageSystem(contexts));

            Add(new ColliderSystem(contexts));

            Add(new CommandSystem(contexts));

            Add(new DestroyedSystem(contexts));
        }
    }
}

