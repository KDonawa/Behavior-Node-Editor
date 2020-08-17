using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.BehaviorEditor.States
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckCondition(StateManager state);
    }
}

