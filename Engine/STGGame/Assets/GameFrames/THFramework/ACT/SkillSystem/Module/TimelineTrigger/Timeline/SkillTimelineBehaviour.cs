namespace THGame
{
    public interface SkillTimelineBehaviour
    {
        void OnStart(object owner);
        void OnUpdate(int tickFrame);
        void OnEnd();

        void OnCreate(string[] info, string[] args);
        void OnDestroy();
    }
}