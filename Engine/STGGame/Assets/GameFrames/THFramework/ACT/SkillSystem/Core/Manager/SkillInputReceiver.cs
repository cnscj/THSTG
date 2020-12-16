using System;
using System.Collections;
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

        public class StateInfo
        {
            public IComparable keyCode;
            public int state;
            public float timeStamp;

            public Action onKeyUp;
            public Action onKeyDown;
            public Action onShotAction;
            public Action onLongAction;
            public bool callbackEnabled;
        }

        public Dictionary<IComparable, StateInfo> _keyStateDict;      //指令状态
        public HashSet<StateInfo> _pressingSet = new HashSet<StateInfo>();

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

        public StateInfo GetStateInfo(IComparable keyCode)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return default;

            if (_keyStateDict.TryGetValue(keyCode, out var stateInfo))
                return stateInfo;

            return default;
        }

        //TODO:执行多次会出BUG
        public void PressKey(IComparable keyCode)
        {
            if (!enabled) return;

            var stateInfo = GetOrCreateStateInfo(keyCode);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state |= KEYSTATE_PRESS;
            stateInfo.onKeyDown?.Invoke();
            stateInfo.callbackEnabled = true;

            _pressingSet.Add(stateInfo);

        }

        public void ReleaseKey(IComparable keyCode)
        {
            if (!enabled) return;

            DealCallbackTime(keyCode, false);
            var stateInfo = GetOrCreateStateInfo(keyCode);

            stateInfo.timeStamp = GetTimeStamp();
            stateInfo.state = KEYSTATE_NONE;
            stateInfo.onKeyUp?.Invoke();
            stateInfo.callbackEnabled = false;

            _pressingSet.Remove(stateInfo);

        }

        public void SetStateCallback(IComparable keyCode, Action onKeyDownCall, Action onKeyUpCall)
        {
            var stateInfo = GetOrCreateStateInfo(keyCode);
            stateInfo.onKeyUp = onKeyUpCall;
            stateInfo.onKeyDown = onKeyDownCall;
        }

        public void SetPressCallback(IComparable keyCode,Action onShotCall,Action onLongCall)
        {
            var stateInfo = GetOrCreateStateInfo(keyCode);
            stateInfo.onShotAction = onShotCall;
            stateInfo.onLongAction = onLongCall;
        }

        private void Update()
        {
            UpdateStateInfo();
        }

        private void DealCallbackTime(IComparable keyCode, bool isSustain)
        {
            if (_keyStateDict == null || _keyStateDict.Count <= 0)
                return;

            if (!_keyStateDict.TryGetValue(keyCode, out StateInfo stateInfo))
                return;

            if (!stateInfo.callbackEnabled)
                return;

            var durationTime = GetTimeStamp() - stateInfo.timeStamp;
            if (durationTime > INTERVAL_SHOT_PRESS)
            {
                stateInfo.onLongAction?.Invoke();
            }
            else
            {
                if (isSustain) return;
                stateInfo.onShotAction?.Invoke();
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

        private StateInfo GetOrCreateStateInfo(IComparable keyCode)
        {
            var dict = GetStateDict();
            StateInfo stateInfo;
            if (!dict.TryGetValue(keyCode, out stateInfo))
            {
                stateInfo = new StateInfo();
                stateInfo.keyCode = keyCode;

                dict[keyCode] = stateInfo;
            }
            return stateInfo;
        }

        private Dictionary<IComparable, StateInfo> GetStateDict()
        {
            _keyStateDict = _keyStateDict ?? new Dictionary<IComparable, StateInfo>();
            return _keyStateDict;
        }

        private float GetTimeStamp()
        {
            return Time.fixedTime;
        }
    }

}