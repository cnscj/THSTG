using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //长按,短按,按下,弹起
    public class SkillInputReceiver : MonoBehaviour
    {
        public event Action<SkillInputStateInfo> OnKeyDown;
        public event Action<SkillInputStateInfo> OnKeyUp;
        public event Action<SkillInputStateInfo> OnShotPress;
        public event Action<SkillInputStateInfo> OnLongPress;

        private Dictionary<IComparable, SkillInputStateInfo> _keyStateDict;       //指令状态
        private HashSet<SkillInputStateInfo> _pressingSet = new HashSet<SkillInputStateInfo>();
        private Queue<SkillInputStateInfo> _releaseQueue = new Queue<SkillInputStateInfo>();

        public int GetKeyState(IComparable keyCode)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return SkillInputStateInfo.KEYSTATE_NONE;

            if (_keyStateDict.TryGetValue(keyCode, out var stateInfo))
            {
                return stateInfo.state;
            }

            return SkillInputStateInfo.KEYSTATE_NONE;
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
            stateInfo.durationTime = 0;

            OnKeyDown?.Invoke(stateInfo);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state |= SkillInputStateInfo.KEYSTATE_PRESS;
            stateInfo.callbackEnabled = true;

            if (_pressingSet.Contains(stateInfo)) return;
            _pressingSet.Add(stateInfo);

        }

        public void ReleaseKey(IComparable keyCode)
        {
            if (!enabled) return;

            var stateInfo = GetOrCreateStateInfo(keyCode);
            if (!_pressingSet.Contains(stateInfo)) return;
            stateInfo.durationTime = GetTimeStamp() - stateInfo.timeStamp;

            OnKeyUp?.Invoke(stateInfo);
            DealCallbackTime(keyCode, false);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state = SkillInputStateInfo.KEYSTATE_NONE;
            stateInfo.callbackEnabled = false;

            _pressingSet.Remove(stateInfo);

        }

        private void Update()
        {
            UpdateStateInfo();
            PurgeStateInfo();
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
            if (durationTime > stateInfo.responTime)
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
                //超时自动释放
                if (stateInfo.releaseTimeout > 0f)
                {
                    var durationTime = GetTimeStamp() - stateInfo.timeStamp;
                    if (durationTime >= stateInfo.releaseTimeout)
                    {
                        _releaseQueue.Enqueue(stateInfo);
                    }
                }
            }
        }

        private void PurgeStateInfo()
        {
            if (_releaseQueue == null || _releaseQueue.Count <= 0)
                return;

            while(_releaseQueue.Count > 0)
            {
                var stateInfo = _releaseQueue.Dequeue();
                ReleaseKey(stateInfo.keyCode);
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