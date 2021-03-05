
namespace THGame
{
    [System.Serializable]
    public class SkillTimelineAsset
    {
        public string name;
        public string type;
        public string[] args;

        public float startTime = 0f;
        public float durationTime = -1f;

    }
}
