using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.StateMachine.BehaviorEditor.Graph.Nodes;
using UnityEditor;

namespace KD.StateMachine.BehaviorEditor.Graph
{
    [CreateAssetMenu]
    public class BehaviorGraph : ScriptableObject
    {
        public readonly List<GraphNode> nodes = new List<GraphNode>();
        [HideInInspector] public bool isChanged;

        public List<StateNodeData> stateNodeDataList = new List<StateNodeData>();
        public List<TransitionNodeData> transitionNodeDataList = new List<TransitionNodeData>();
        public List<PortalNodeData> portalNodeDataList = new List<PortalNodeData>();
        public List<CommentNodeData> commentNodeDataList = new List<CommentNodeData>();             
        
        public void AddNewStateNode(Vector2 position)
        {
            StateNodeData nodeData = new StateNodeData { graphNodeData = new GraphNodeData { id = nodes.Count} };
            stateNodeDataList.Add(nodeData);
            AddNode(new StateNode(this, nodeData, position), true);
        }
        public void AddNewTransitionNode(GraphNode startNode, Vector2 position)
        {
            TransitionNodeData nodeData = new TransitionNodeData
            {
                graphNodeData = new GraphNodeData { id = nodes.Count },
                startNodeData = startNode.graphNodeData,
                endNodeData = new GraphNodeData()
            };
            transitionNodeDataList.Add(nodeData);
            AddNode(new TransitionNode(this, nodeData, position), true);
        }
        public void AddNewPortalNode(Vector2 position)
        {
            PortalNodeData nodeData = new PortalNodeData { graphNodeData = new GraphNodeData { id = nodes.Count } };
            portalNodeDataList.Add(nodeData);
            AddNode(new PortalNode(this, nodeData, position), true);
        }
        public void AddNewCommentNode(Vector2 position)
        {
            CommentNodeData nodeData = new CommentNodeData { graphNodeData = new GraphNodeData { id = nodes.Count } };
            commentNodeDataList.Add(nodeData);
            AddNode(new CommentNode(this, nodeData, position), true);
        }        
        public void DeleteNode(GraphNode node)
        {
            if (node is StateNode stateNode) stateNodeDataList.Remove(stateNode.nodeData);
            else if (node is PortalNode portalNode) portalNodeDataList.Remove(portalNode.nodeData);
            else if (node is TransitionNode transitionNode) transitionNodeDataList.Remove(transitionNode.nodeData);
            else if (node is CommentNode commentNode) commentNodeDataList.Remove(commentNode.nodeData);
            nodes.Remove(node);
            isChanged = true;         
        }        
        public void Save()
        {
            for (int i = 0; i < nodes.Count; i++) nodes[i].Save(i);
            ForceSerialization();
            isChanged = false;
        }
        public void Load()
        {
            nodes.Clear();

            foreach (var stateNodeData in stateNodeDataList)
                AddNode(new StateNode(this, stateNodeData, stateNodeData.graphNodeData.position));

            foreach (var portalNodeData in portalNodeDataList)
                AddNode(new PortalNode(this, portalNodeData, portalNodeData.graphNodeData.position));

            foreach (var transitionNodeData in transitionNodeDataList)
                AddNode(new TransitionNode(this, transitionNodeData, transitionNodeData.graphNodeData.position));

            foreach (var commenNodeData in commentNodeDataList)
                AddNode(new CommentNode(this, commenNodeData, commenNodeData.graphNodeData.position));
        }
        public bool StateExists(State state)
        {
            if (state == null) return false;
            foreach (var node in nodes)
            {
                if (node is StateNode stateNode && stateNode.CurrentState == state) return true;
            }
            return false;
        }

        #region Helpers
        void AddNode(GraphNode node, bool requiresUpdate = false)
        {
            nodes.Add(node);
            isChanged = requiresUpdate;
        }
        void ForceSerialization()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        #endregion
    }
}

