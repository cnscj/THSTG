using System.Collections.Generic;
using Entitas;

namespace STGU3D
{
    public class InputSystem : ReactiveSystem<InputEntity>
    {
        readonly IGroup<GameEntity> _movementers;

        private InputContext __inputContext;
        public InputSystem(Contexts context) : base(context.input)
        {
            __inputContext = context.input;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.Input);
        }

        protected override bool Filter(InputEntity entity)
        {
            return entity.hasInput;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (var e in entities)
            {

            }
        }

    }

}
