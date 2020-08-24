using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace KD.StateMachine.BehaviorEditor.Graph.Nodes
{
    [System.Serializable]
    public class StateNodeData
    {
        [HideInInspector] public GraphNodeData graphNodeData;
        public State state;
        [HideInInspector] public bool isCollapsed;        
    }

    public class StateNode : GraphNode
    {
        public readonly StateNodeData nodeData;
        public State CurrentState { get; private set; }

        SerializedObject _serializedState;
        ReorderableList _onStateUpdate;
        ReorderableList _onStateEnter;
        ReorderableList _onStateExit;        
        bool _isDuplicateState;

        public StateNode(BehaviorGraph graph,StateNodeData nodeData, Vector2 position, string title = "State Node") 
            : base(graph, nodeData.graphNodeData, new Rect(position, new Vector2(180f, 100f)), title) 
        {
            this.nodeData = nodeData;
            CurrentState = nodeData.state;
            ResetReorderableLists();
        }

        public override void DrawNodeWindow()
        {
            if (CurrentState == null)
            {           
                if (_isDuplicateState)
                {
                    EditorGUILayout.LabelField("Cannot add duplicate states!");
                    windowRect.height = 75f;
                }
                else windowRect.height = 55f;
            }
            else
            {
                nodeData.isCollapsed = EditorGUILayout.ToggleLeft(" collapse", nodeData.isCollapsed);
                if (nodeData.isCollapsed) windowRect.height = 75f;
            }

            nodeData.state = (State)EditorGUILayout.ObjectField(nodeData.state, typeof(State), false);
                       
            if (CurrentState != nodeData.state)
            { 
                _isDuplicateState = false;                

                if (!_parentGraph.StateExists(nodeData.state)) CurrentState = nodeData.state;
                else
                {
                    _isDuplicateState = true;
                    nodeData.state = null;
                    CurrentState = null;
                }
                ResetReorderableLists();
            }

            if (_serializedState == null || nodeData.isCollapsed) return;

            //_serializedState.Update();
            UpdateReorderableList(_onStateEnter, "On State Enter");
            UpdateReorderableList(_onStateUpdate, "On State Update");
            UpdateReorderableList(_onStateExit, "On State Exit");
            _serializedState.ApplyModifiedProperties();

            windowRect.height = 280f + (_onStateEnter.count + _onStateUpdate.count + _onStateExit.count) * EditorGUIUtility.singleLineHeight;
        }        
        public override void ModifyNode(GenericMenu menu)
        {
            if (CurrentState != null) menu.AddItem(new GUIContent("Add Transition"), false, AddNewTransition);
            else menu.AddDisabledItem(new GUIContent("Add Transition"));
            base.ModifyNode(menu);
        }

        public override bool CanAcceptTransition() => true;

        #region Helpers
        void UpdateReorderableList(ReorderableList list, string targetName)
        {
            if (list == null) return;

            list.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, targetName);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };

            list.DoLayoutList();
        }

        void ResetReorderableLists()
        {
            if (CurrentState == null)
            {
                _serializedState = null;
                _onStateUpdate = null; _onStateEnter = null; _onStateExit = null;
                return;
            }
            _serializedState = new SerializedObject(CurrentState);
            _onStateUpdate = new ReorderableList(_serializedState, _serializedState.FindProperty("onStateUpdate"), true, true, true, true);
            _onStateEnter = new ReorderableList(_serializedState, _serializedState.FindProperty("onStateEnter"), true, true, true, true);
            _onStateExit = new ReorderableList(_serializedState, _serializedState.FindProperty("onStateExit"), true, true, true, true);
        }
        
        

        #endregion

    }    
}
