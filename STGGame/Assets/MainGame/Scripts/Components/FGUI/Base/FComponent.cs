using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame
{

    public class FComponent : GComponent
    {
        protected GObject _obj;
        protected float _interval = 0f;
        private int __scheduler = -1;
        private List<KeyValuePair<int, XLibGame.EventListener>> __listener;

        public float interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
                _RemoveSchedule();
                _InitTick();
            }
        }
    

        public void AddEventListener(int eventId, XLibGame.EventListener listener)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<KeyValuePair<int, XLibGame.EventListener>>();
            __listener.Add(new KeyValuePair<int, XLibGame.EventListener>(eventId, listener));
        }

        public FComponent()
        {

            
        }

        public void Init(GObject obj = null)
        {
            _obj = (obj != null) ? obj : this;
            _obj.onAddedToStage.Add(_OnAddedToStage);
            _obj.onRemovedFromStage.Add(_OnEemovedFromStage);
        }
        //
        protected virtual void OnInitUI()
        {
          
        }

        protected virtual void OnInitEvent()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }

        protected virtual void OnTick()
        {

        }

        //
        private void _OnAddedToStage()
        {
            OnInitUI();
            OnInitEvent();
            _InitTick();

            OnEnter();
        }

        private void _OnEemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();
            OnExit();
        }

        private void _InitTick()
        {
            if (_interval > 0f)
            {
                __scheduler = SchedulerManager.GetInstance().Schedule(OnTick, _interval);
            }

        }

        private void _RemoveEvent()
        {
            if (__listener != null)
            {
                foreach (var pair in __listener)
                {
                    DispatcherManager.GetInstance().RemoveListener(pair.Key, pair.Value);
                }
            }
        }

        private void _RemoveSchedule()
        {
            if (__scheduler != -1)
            {
                SchedulerManager.GetInstance().Unschedule(__scheduler);
            }
        }
    }

}
