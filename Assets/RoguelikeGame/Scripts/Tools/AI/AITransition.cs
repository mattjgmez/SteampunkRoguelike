using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools.AI
{
    [Serializable]
    public class AITransition : MonoBehaviour
    {
        /// this transition's decision
        public AIDecision Decision;
        /// the state to transition to if this Decision returns true
        public string TrueState;
        /// the state to transition to if this Decision returns false
        public string FalseState;
    }
}
