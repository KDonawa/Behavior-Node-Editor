using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KD.StateMachine.BehaviorEditor.Graph.Nodes
{
    [System.Serializable]
    public class TransitionNodeData
    {
        [HideInInspector] public GraphNodeData graphNodeData;
        public Condition condition;
        [HideInInspector] public GraphNodeData startNodeData;
        [HideInInspector] public GraphNodeData endNodeData;
    }
    public class TransitionNode : GraphNode
    {
        public readonly TransitionNodeData nodeData;

        readonly GraphNode _startNode;
        GraphNode _endNode;
        bool _makingTransition;

        public TransitionNode(BehaviorGraph graph, TransitionNodeData nodeData,
            Vector2 position, string title = "Transition Node") : base(graph, nodeData.graphNodeData, new Rect(position, new Vector2(100f, 50f)), title)
        {
            this.nodeData = nodeData;

            _startNode = _parentGraph.nodes.Find(x => x.graphNodeData.id == nodeData.startNodeData.id);
            //_startNode.transitionToNodes.Add(this);
            _startNode.numTransitions++;
            _startNode.NodeDeletedEvent += DeleteNode;
            _startNode.RemoveTransitionsEvent += DeleteNode;
            //_startNode.ExecuteTransitionsEvent += Execute;

            _endNode = _parentGraph.nodes.Find(x => x.graphNodeData.id == nodeData.endNodeData.id);
            if(_endNode != null) _endNode.NodeDeletedEvent += ClearEndNodeReference;
        }
        //void Execute() { }
        public override void DrawNodeWindow()
        {
            nodeData.condition = (Condition)EditorGUILayout.ObjectField(nodeData.condition, typeof(Condition), false);
        }        
        public override void ModifyNode(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Make Transition"), false, StartNewTransition);
            if(_endNode != null) menu.AddItem(new GUIContent("Break Transition"), false, ClearEndNodeReference);
            base.ModifyNode(menu);
        }
        public override void DeleteNode()
        {
            base.DeleteNode();
            //_startNode.transitionToNodes.Remove(this);
            _startNode.numTransitions--;
            ClearEndNodeReference();          
            _startNode.RemoveTransitionsEvent -= DeleteNode;
            _startNode.NodeDeletedEvent -= DeleteNode;
            //_startNode.ExecuteTransitionsEvent -= Execute;
        }
        public override bool CanAcceptTransition() => false;
        public override void DrawConnections()
        {
            DrawTransitionConnection(_startNode.windowRect, windowRect, Color.white);
            if (_makingTransition) DrawMakingTransitionCurve(windowRect, Color.red);
            if (_endNode != null) DrawTransitionConnection(windowRect, _endNode.windowRect, Color.white);
        }

        #region helpers
        void ClearEndNodeReference()
        {
            if (_endNode != null)
            {
                _endNode.NodeDeletedEvent -= ClearEndNodeReference;
                _endNode = null;
                nodeData.endNodeData = new GraphNodeData();
            }            
        }
        void StartNewTransition()
        {
            ClearEndNodeReference();
            BehaviorEditor.AttemptTransitionEvent += TransitionTo;
            BehaviorEditor.Instance.attemptingTransition = true;
            _makingTransition = true;            
        }
        void TransitionTo(GraphNode node)
        {
            if (node != null && node.graphNodeData.id != nodeData.startNodeData.id)
            {
                _endNode = node;
                nodeData.endNodeData = node.graphNodeData;
                _endNode.NodeDeletedEvent += ClearEndNodeReference;             
            }
            BehaviorEditor.AttemptTransitionEvent -= TransitionTo;
            BehaviorEditor.Instance.attemptingTransition = false;
            _makingTransition = false;
        }
        
        void DrawMakingTransitionCurve(Rect start, Color curveColor)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height * 0.5f, 0f);
            Vector3 endPos = Event.current.mousePosition;
            DrawCurves(startPos, endPos, curveColor);
            BehaviorEditor.RepaintWindow();
        }
        void DrawTransitionConnection(Rect start, Rect end, Color curveColor)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height * 0.5f, 0f);
            Vector3 endPos = new Vector3(end.x, end.y + end.height * 0.5f, 0f);
            DrawCurves(startPos, endPos, curveColor);
        }
        void DrawCurves(Vector3 startPos, Vector3 endPos, Color curveColor)
        {
            Vector3 startTangent = startPos + Vector3.right * 50f;
            Vector3 endTangent = endPos + Vector3.left * 50f;

            Color shadow = new Color(0f, 0f, 0f, 0.1f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, shadow, null, (i + 1) * 1f);
            }
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, curveColor, null, 2f);
        }
        #endregion
    }
}

