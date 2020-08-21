using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.BehaviorEditor.Nodes;
using UnityEditor;

namespace KD.BehaviorEditor
{
    [CreateAssetMenu]
    public class BehaviorGraph : ScriptableObject
    {
        public readonly List<BaseNode> nodes = new List<BaseNode>();
        public bool IsChanged { get; set; }

        [SerializeField] List<StateNodeData> stateNodeDataList = null;
        [SerializeField] List<CommentNodeData> commentNodeDataList = null;
        readonly List<StateNode> _stateNodes = new List<StateNode>();
        readonly List<CommentNode> _commentNodes = new List<CommentNode>();

        private void OnEnable() => Load();        

        public void AddNewStateNode(Vector2 position, State state = null)
        {
            StateNode node = new StateNode(this, position, state);
            _stateNodes.Add(node);
            nodes.Add(node);
            IsChanged = true;
        }
        public void AddNewCommentNode(Vector2 position)
        {
            CommentNode node = new CommentNode(position);
            _commentNodes.Add(node);
            nodes.Add(node);
            IsChanged = true;
        }
        public void AddNewTransitionNode(StateNode stateNode, Transition transition, Vector2 position)
        {
            TransitionNode node = new TransitionNode(stateNode, transition, position);
            nodes.Add(node);
            IsChanged = true;
        }
        public void DeleteNode(BaseNode node)
        {
            if (node == null) return;

            if (node is StateNode stateNode) _stateNodes.Remove(stateNode);
            else if (node is CommentNode commentNode) _commentNodes.Remove(commentNode);
            nodes.Remove(node);

            IsChanged = true;
        }
        public bool StateExists(State state)
        {
            if (state == null) return false;
            return _stateNodes.Find(x => x.CurrentState == state) != null;
        }


        public void Save()
        {
            // Save StateNodeData
            stateNodeDataList.Clear();
            foreach (var node in _stateNodes)
            {
                StateNodeData stateNodeData = new StateNodeData();
                stateNodeData.stateNodePosition = node.windowRect.position;
                if(node.CurrentState != null)
                {
                    stateNodeData.state = node.CurrentState;
                    stateNodeData.transitionNodePositions = new Vector2[node.TransitionNodes.Count];
                    for (int i = 0; i < node.TransitionNodes.Count; i++)
                    {
                        stateNodeData.transitionNodePositions[i] = node.TransitionNodes[i].windowRect.position;
                    }
                }
                stateNodeDataList.Add(stateNodeData);
            }

            // Save CommentNodes
            commentNodeDataList.Clear();
            foreach (var node in _commentNodes)
            {
                commentNodeDataList.Add(new CommentNodeData { position = node.windowRect.position });
            }
            
            ForceSerialization();
            IsChanged = false;
        }
        public void Load()
        {
            nodes.Clear();
            _stateNodes.Clear();
            _commentNodes.Clear();

            // load State Nodes
            foreach (var stateNodeData in stateNodeDataList)
            {
                StateNode stateNode = new StateNode(this, stateNodeData.stateNodePosition, stateNodeData.state);
                nodes.Add(stateNode);
                _stateNodes.Add(stateNode);
                if(stateNodeData.state != null)
                {
                    for (int i = 0; i < stateNodeData.transitionNodePositions.Length; i++)
                    {
                        nodes.Add(new TransitionNode(stateNode, stateNodeData.state.Transitions[i], stateNodeData.transitionNodePositions[i]));
                    }
                } 
            }

            // load comment nodes
            foreach (var commenNodeData in commentNodeDataList)
            {
                nodes.Add(new CommentNode(commenNodeData.position));
            }
        }
        public void ForceSerialization()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }

    [System.Serializable]
    public class StateNodeData
    {
        public Vector2 stateNodePosition;
        public State state;
        public Vector2[] transitionNodePositions;
    }

    [System.Serializable]
    public class CommentNodeData
    {
        public Vector2 position;
    }
}

