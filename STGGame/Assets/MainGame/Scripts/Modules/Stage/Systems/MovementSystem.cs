using Unity.Entities;
using Unity.Jobs;

namespace STGGame
{

    public class MovementSystem : JobComponentSystem
    {

        struct MovementJob : IJobProcessComponentData<Position, Rotation, Movement>
        {
            public void Execute(ref Position position, ref Rotation rotation, ref Movement movenent)
            {
                
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob moveJob = new MovementJob
            {

            };
            JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

            return moveHandle;
        }
    }

}