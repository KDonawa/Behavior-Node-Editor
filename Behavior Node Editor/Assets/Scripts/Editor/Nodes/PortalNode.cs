using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KD.StateMachine.BehaviorEditor.Graph.Nodes
{
    [System.Serializable]
    public class PortalNodeData
    {
        [HideInInspector] public GraphNodeData graphNodeData;
        public State state;
    }

    public class PortalNode : GraphNode
    {
        public readonly PortalNodeData nodeData;

        public PortalNode(BehaviorGraph graph, PortalNodeData nodeData, Vector2 position, string title = "Portal Node") 
            : base(graph, nodeData.graphNodeData, new Rect(position, new Vector2(90f,50f)), title)
        {
            this.nodeData = nodeData;
        }

        public override void DrawNodeWindow()
        {
            nodeData.state = (State)EditorGUILayout.ObjectField(nodeData.state, typeof(State), false);
        }

        public override bool CanAcceptTransition() => true;
    }
}