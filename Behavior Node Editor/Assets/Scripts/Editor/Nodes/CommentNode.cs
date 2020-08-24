using UnityEngine;

namespace KD.StateMachine.BehaviorEditor.Graph.Nodes
{   
    [System.Serializable]
    public class CommentNodeData
    {
        [HideInInspector] public GraphNodeData graphNodeData;
        [HideInInspector] public string comment = string.Empty;
    }
    public class CommentNode : GraphNode
    {
        public readonly CommentNodeData nodeData;

        public CommentNode(BehaviorGraph graph, CommentNodeData nodeData, Vector2 position, string title = "Comment") 
            : base(graph, nodeData.graphNodeData, new Rect(position, new Vector2(200f, 60f)), title)
        {
            this.nodeData = nodeData;
        }

        public override void DrawNodeWindow()
        {
            nodeData.comment = GUILayout.TextArea(nodeData.comment, 200);
        }

        public override bool CanAcceptTransition() => false;
    }
}

