using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools.AI
{
    public abstract class AIAction : MonoBehaviour
    {
        public string Label;
        public abstract void PerformAction();
        public bool ActionInProgress { get; set; }
        
        protected AIBrain _brain;

        protected virtual void Start()
        {
            _brain = this.gameObject.GetComponent<AIBrain>();
            Initialization();
        }

        protected virtual void Initialization() { }

        public virtual void OnEnterState()
        {
            ActionInProgress = true;
        }

        public virtual void OnExitState()
        {
            ActionInProgress = false;
        }
    }
}
