using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FWidget : FComponent
    {
        private string __package;
        private string __component;
        public string package { get { return __package; } }
        public string component { get { return __component; } }

        protected float _interval = 0f;
        private int __scheduler = -1;
        private List<KeyValuePair<int, XLibGame.EventListener>> __listener;

        public void AddEventListener(int eventId, XLibGame.EventListener listener)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<KeyValuePair<int, XLibGame.EventListener>>();
            __listener.Add(new KeyValuePair<int, XLibGame.EventListener>(eventId, listener));
        }

        public FWidget(string package, string component)
        {
            __package = package;
            __component = component;
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

        ///
        public override FObject InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);
            if (obj != null)
            {
                obj.onAddedToStage.Add(_OnAddedToStage);
                obj.onRemovedFromStage.Add(_OnEemovedFromStage);

                OnInitUI();
            }
            return this;
        }
    }

}
