using FairyGUI;
using System;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FWidget : FComponent
    {
        public string package { get; protected set; }
        public string component { get; protected set; }

        protected float _interval = -1.0f;
        private int __scheduler = -1;
        private List<KeyValuePair<int, EventListener2>> __listener;

        public void AddEventListener(int eventId, EventListener2 listener)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<KeyValuePair<int, EventListener2>>();
            __listener.Add(new KeyValuePair<int, EventListener2>(eventId, listener));
        }

        public FWidget(string packageName, string componentName)
        {
            package = packageName;
            component = componentName;
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
            OnInitEvent();
            _InitScheduler();

            OnEnter();
        }

        private void _OnEemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();
            OnExit();
        }

        private void _InitScheduler()
        {
            if (_interval >= 0f)
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

        ///
        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            if (obj != null && obj != _obj)
            {
                base.InitWithObj(obj);
                OnInitUI();

                obj.onAddedToStage.Add(_OnAddedToStage);
                obj.onRemovedFromStage.Add(_OnEemovedFromStage);
            }
            
            return this;
        }
    }

}
