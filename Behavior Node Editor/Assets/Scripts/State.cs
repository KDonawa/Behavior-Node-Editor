using KD.BehaviorEditor.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.BehaviorEditor
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        [SerializeField] Action[] onStateUpdate = null;
        [SerializeField] Action[] onStateEnter = null;
        [SerializeField] Action[] onStateExit = null;

        [SerializeField] List<Transition> transitions = null;

        public List<Transition> Transitions => transitions;

        public void Tick()
        {
            //OnStateUpdate();
            //CheckTransitions
        }
        public void OnStateEnter(StateManager manager)
        {
            foreach (var action in onStateEnter)
            {
                if (action != null) action.Execute(manager);
            }
        }
        public void OnStateUpdate(StateManager manager)
        {
            foreach (var action in onStateUpdate)
            {
                if (action != null) action.Execute(manager);
            }
            //CheckTransitions
        }
        public void OnStateExit(StateManager manager)
        {
            foreach (var action in onStateExit)
            {
                if (action != null) action.Execute(manager);
            }
        }

    }
}

