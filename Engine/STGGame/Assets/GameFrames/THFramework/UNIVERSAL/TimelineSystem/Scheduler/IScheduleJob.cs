using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface IScheduleJob
    {
        int EndTime { get; }

        void Start();

        void Update(int tickTime);

        void End();

    }
}
