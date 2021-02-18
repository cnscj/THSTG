using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{

    public interface ISkillTimelinePlayable
    {
        object Owner { get; set; }
        int StartFrame { get; }
        int EndFrame { get; }

        void Seek(int startFrame);

        void Start(object owner);

        void Update(int tickFrame);

        void End();

        void Reset();
    }

}
