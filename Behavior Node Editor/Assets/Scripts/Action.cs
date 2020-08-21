using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.BehaviorEditor
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute(StateManager stateManager);
    }
}

