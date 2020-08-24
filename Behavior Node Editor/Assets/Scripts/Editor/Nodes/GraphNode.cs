using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace KD.StateMachine.BehaviorEditor.Graph.Nodes
{
    [System.Serializable]
    public class GraphNodeData
    {
        public int id = -1;
        [HideInInspector] public Vector2 position;
    }
    public abstract class GraphNode
    {
        public event System.Action NodeDeletedEvent;
        public event System.Action RemoveTransitionsEvent;
        //public event System.Action ExecuteTransitionsEvent;

        [HideInInspector] public Rect windowRect;
        [HideInInspector] public string windowTitle;

        protected BehaviorGraph _parentGraph;
        public readonly GraphNodeData graphNodeData;
        public int numTransitions;
        //public readonly List<GraphNode> transitionToNodes;
        public GraphNode(BehaviorGraph graph, GraphNodeData nodeData, Rect rect, string title)
        {
            //transitionToNodes = new List<GraphNode>();
            numTransitions = 0;
            graphNodeData = nodeData;
            _parentGraph = graph;
            windowRect = rect;
            windowTitle = title;
        }

        public abstract void DrawNodeWindow();
        public virtual void DrawConnections() {}
        public virtual void DeleteNode()
        {
            NodeDeletedEvent?.Invoke();
            _parentGraph.DeleteNode(this);            
        }
        public void Save(int id)
        {
            graphNodeData.id = id;
            graphNodeData.position = windowRect.position;
        }
        public abstract bool CanAcceptTransition();
        public virtual void ModifyNode(GenericMenu menu)
        {
            if (numTransitions > 0) menu.AddItem(new GUIContent("Remove Transitions"), false, () => RemoveTransitionsEvent?.Invoke());
            menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);        
        }

        protected void AddNewTransition()
        {
            float xOffset = windowRect.width + 50f;
            //float yOffset = (windowRect.height - 50f) * 0.5f;
            float yOffset = numTransitions * 50f + 5f;
            Vector2 position = new Vector2(windowRect.x + xOffset, windowRect.y + yOffset);
            _parentGraph.AddNewTransitionNode(this, position);
        }
    }
}
