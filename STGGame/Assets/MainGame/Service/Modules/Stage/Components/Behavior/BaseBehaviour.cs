using UnityEngine;
using System.Collections;
using THGame;
namespace STGService
{
    public class BaseBehavior
    {
        public virtual void Enter(Object sender)
        {

        }
        public virtual BaseBehavior HandleInput(Object sender, BehaviourMapper inpput)
        {

            return this;
        }
        public virtual void Exit(Object sender)
        {

        }
    }

}
