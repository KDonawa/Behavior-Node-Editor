using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.StateMachine
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute(StateManager stateManager);
    }
}

