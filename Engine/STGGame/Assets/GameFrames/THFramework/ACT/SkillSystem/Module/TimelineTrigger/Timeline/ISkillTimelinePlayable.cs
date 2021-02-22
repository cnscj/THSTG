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

        /// <summary>
        /// 跳转到指定帧
        /// </summary>
        /// <param name="startFrame">跳转帧</param>
        void Seek(int startFrame);

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="data">输入参数</param>
        void Start(object data);

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="tickFrame">当前帧</param>
        void Update(int tickFrame);

        /// <summary>
        /// 结束执行
        /// </summary>
        void End();

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();
    }

}
