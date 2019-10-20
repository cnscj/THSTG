using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FTransition
    {
        protected Transition _obj;
        public FTransition(Transition trans)
        {
            _obj = trans;
        }

        public Transition GetObject()
        {
            return _obj;
        }

        public void Play()
        {
            
        }

        public void Play(PlayCompleteCallback onComplete)
        {
           
        }
    }

}
