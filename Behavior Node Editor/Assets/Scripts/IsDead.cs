using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.StateMachine
{
    public class IsDead : Condition
    {
        public override bool CheckCondition(StateManager state)
        {
            return false;
        }
    }
}

