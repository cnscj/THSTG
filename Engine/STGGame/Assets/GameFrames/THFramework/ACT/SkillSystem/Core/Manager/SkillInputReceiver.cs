using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //长按,短按,按下,弹起
    public class SkillInputReceiver : MonoBehaviour
    {
        public static readonly float INTERVAL_SHOT_PRESS = 0.65f;    //这个数以下判定为短按,以上判断为长按

        public static readonly int KEYSTATE_NONE = 0x0;
        public static readonly int KEYSTATE_PRESS = 0x1;

        public float pressResponseTime = INTERVAL_SHOT_PRESS;      //短按响应时间
        public event Action<SkillInputStateInfo> OnKeyDown;
        public event Action<SkillInputStateInfo> OnKeyUp;
        public event Action<SkillInputStateInfo> OnShotPress;
        public event Action<SkillInputStateInfo> OnLongPress;

        private Dictionary<IComparable, SkillInputStateInfo> _keyStateDict;       //指令状态
        private HashSet<SkillInputStateInfo> _pressingSet = new HashSet<SkillInputStateInfo>();

        public int GetKeyState(IComparable keyCode)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return KEYSTATE_NONE;

            if (_keyStateDict.TryGetValue(keyCode, out var stateInfo))
            {
                return stateInfo.state;
            }

            return KEYSTATE_NONE;
        }

        public SkillInputStateInfo GetStateInfo(IComparable keyCode)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return default;

            if (_keyStateDict.TryGetValue(keyCode, out var stateInfo))
                return stateInfo;

            return default;
        }

        public void PressKey(IComparable keyCode)
        {
            if (!enabled) return;

            var stateInfo = GetOrCreateStateInfo(keyCode);

            OnKeyDown?.Invoke(stateInfo);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state |= KEYSTATE_PRESS;
            stateInfo.callbackEnabled = true;

            if (_pressingSet.Contains(stateInfo)) return;
            _pressingSet.Add(stateInfo);

        }

        public void ReleaseKey(IComparable keyCode)
        {
            if (!enabled) return;

            var stateInfo = GetOrCreateStateInfo(keyCode);
            if (!_pressingSet.Contains(stateInfo)) return;

            DealCallbackTime(keyCode, false);
            OnKeyUp?.Invoke(stateInfo);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state = KEYSTATE_NONE;
            stateInfo.callbackEnabled = false;

            _pressingSet.Remove(stateInfo);

        }

        private void Update()
        {
            UpdateStateInfo();
        }

        private void DealCallbackTime(IComparable keyCode, bool isSustain)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return;

            if (!_keyStateDict.TryGetValue(keyCode, out SkillInputStateInfo stateInfo))
                return;

            if (!stateInfo.callbackEnabled)
                return;

            var durationTime = GetTimeStamp() - stateInfo.timeStamp;
            if (durationTime > pressResponseTime)
            {
                OnLongPress?.Invoke(stateInfo);
            }
            else
            {
                if (isSustain) return;
                OnShotPress?.Invoke(stateInfo);
            }
            stateInfo.callbackEnabled = false;
        }

        private void UpdateStateInfo()
        {
            if (_pressingSet == null || _pressingSet.Count <= 0)
                return;

            foreach(var stateInfo in _pressingSet)
            {
                DealCallbackTime(stateInfo.keyCode,true);
            }
        }

        private SkillInputStateInfo GetOrCreateStateInfo(IComparable keyCode)
        {
            var dict = GetStateDict();
            SkillInputStateInfo stateInfo;
            if (!dict.TryGetValue(keyCode, out stateInfo))
            {
                stateInfo = new SkillInputStateInfo();
                stateInfo.keyCode = keyCode;

                dict[keyCode] = stateInfo;
            }
            return stateInfo;
        }

        private Dictionary<IComparable, SkillInputStateInfo> GetStateDict()
        {
            _keyStateDict = _keyStateDict ?? new Dictionary<IComparable, SkillInputStateInfo>();
            return _keyStateDict;
        }

        private float GetTimeStamp()
        {
            return Time.fixedTime;
        }
    }

}