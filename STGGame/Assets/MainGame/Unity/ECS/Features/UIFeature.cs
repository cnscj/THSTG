using Entitas;

namespace STGU3D
{
    public class UIFeature : Feature
    {
        public UIFeature(Contexts contexts)
        {
            Add(new TestEventSystem(contexts));
        }
    }
}

