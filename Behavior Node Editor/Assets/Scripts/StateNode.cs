using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using KD.BehaviorEditor.States;

namespace KD.BehaviorEditor.Nodes
{
    public class StateNode : BaseNode
    {
        public State currentState; // make this into a list of states
        public List<TransitionNode> transitionNodes = new List<TransitionNode>();
        bool collapse;
        public override void DrawWindow()
        {
            if (currentState == null)
            {
                EditorGUILayout.LabelField("Add state:");
            }
            else
            {
                if (collapse) windowRect.height = 100f;
                else windowRect.height = 300f;
                collapse = EditorGUILayout.Toggle("Collapse Node", collapse);
                
            }
            currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);
        }

        public override void DrawCurve()
        {
            
        }

        public void AddTransitionNode(TransitionNode transitionNode)
        {
            transitionNodes.Add(transitionNode);
        }

        public override void DeleteNode()
        {
            base.DeleteNode();
            foreach (var node in transitionNodes)
            {
                BehaviorEditor.RemoveNodeWindow(node);
            }
            transitionNodes.Clear();
        }
    }
}
