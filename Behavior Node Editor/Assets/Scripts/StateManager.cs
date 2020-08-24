using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.StateMachine
{
    public class StateManager : MonoBehaviour
    {
        public static StateManager _instance;
        
        State _currentState;


        private void Awake()
        {
            
        }
    }
}

