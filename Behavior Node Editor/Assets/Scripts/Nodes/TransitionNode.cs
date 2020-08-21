using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KD.BehaviorEditor.Nodes
{
    public class TransitionNode : BaseNode
    {
        [SerializeField] Transition transition = null;

        readonly StateNode _startNode;
        //readonly StateNode _endNode;

        public TransitionNode(StateNode node, Transition transition, Vector2 position, string title = "Transition") 
            : base(position, new Vector2(150f, 50f), title)
        {
            _startNode = node;
            this.transition = transition;
            _startNode.TransitionNodes.Add(this);
        }

        public override void DrawNodeWindow()
        {
            transition.condition = (Condition)EditorGUILayout.ObjectField(transition.condition, typeof(Condition), false);
        }

        public override void DrawCurve()
        {
            BehaviorEditor.DrawNodeCurve(_startNode.windowRect, windowRect, true, Color.black);
        }

        public override void DeleteNode()
        {
            base.DeleteNode();
            _startNode.CurrentState.Transitions.Remove(transition);
            _startNode.TransitionNodes.Remove(this);
            //_startNode.RemoveTransitionNode(this);
        }
    }
}

