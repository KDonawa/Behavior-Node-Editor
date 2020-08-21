using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace KD.BehaviorEditor.Nodes
{
    public class StateNode : BaseNode
    {
        [SerializeField] State state = null;
        public State CurrentState { get; private set; }
        public List<TransitionNode> TransitionNodes { get; private set; } = new List<TransitionNode>();

        readonly BehaviorGraph _parentGraph;
        SerializedObject _serializedState;
        ReorderableList _onStateUpdate;
        ReorderableList _onStateEnter;
        ReorderableList _onStateExit;
        bool _collapseWindow;
        bool _isDuplicateState;

        public StateNode(BehaviorGraph graph, Vector2 position, State initialState = null, string title = "State") 
            : base(position, new Vector2(200f, 100f), title) 
        {
            _parentGraph = graph;
            state = initialState;
            CurrentState = state;            
            ResetReorderableLists();
        }

        public override void DrawNodeWindow()
        {
            if (CurrentState == null)
            {
                windowRect.height = 90f;
                if (_isDuplicateState) EditorGUILayout.LabelField(" Cannot add duplicate states!");
                else EditorGUILayout.LabelField(" Add a state");
            }
            else
            {
                _collapseWindow = EditorGUILayout.ToggleLeft(" Collapse", _collapseWindow);
                if (_collapseWindow) windowRect.height = 90f;
            }

            EditorGUILayout.Space();
            state = (State)EditorGUILayout.ObjectField(state, typeof(State), false);
            EditorGUILayout.Space();
                       
            if (CurrentState != state)
            { 
                _isDuplicateState = false;                

                if (!_parentGraph.StateExists(state))
                {
                    CurrentState = state;                    
                    LoadNewTransitionNodes();
                }
                else
                {
                    _isDuplicateState = true;
                    state = null;
                    CurrentState = null;
                }
                ResetReorderableLists();
            }

            if (_serializedState == null || _collapseWindow) return;

            //_serializedState.Update();
            UpdateReorderableList(_onStateEnter, "On State Enter");
            UpdateReorderableList(_onStateUpdate, "On State Update");
            UpdateReorderableList(_onStateExit, "On State Exit");
            _serializedState.ApplyModifiedProperties();

            windowRect.height = 300f + (_onStateEnter.count + _onStateUpdate.count + _onStateExit.count) * EditorGUIUtility.singleLineHeight;
        }
        
        public override void ModifyNode(GenericMenu menu)
        {
            if (CurrentState != null) menu.AddItem(new GUIContent("Add Transition"), false, AddNewTransition);
            else menu.AddDisabledItem(new GUIContent("Add Transition"));

            base.ModifyNode(menu);
        }
        public override void DeleteNode()
        {
            base.DeleteNode();
            ClearTransitionNodes();
        }

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
        
        void AddNewTransition()
        {
            Transition transition = new Transition();
            CurrentState.Transitions.Add(transition);
            CreateNewTransitionNode(transition);
        }
        void CreateNewTransitionNode(Transition transition)
        {
            float xOffset = windowRect.width + 30f;
            float yOffset = TransitionNodes.Count * (50f + 5f);
            Vector2 position = new Vector2(windowRect.x + xOffset, windowRect.y + yOffset);

            _parentGraph.AddNewTransitionNode(this, transition, position);
        }
        void LoadNewTransitionNodes()
        {
            ClearTransitionNodes();
            if (CurrentState == null) return;
            foreach (var transiton in CurrentState.Transitions) CreateNewTransitionNode(transiton);
        }
        void ClearTransitionNodes()
        {
            foreach (var node in TransitionNodes) _parentGraph.DeleteNode(node);
            TransitionNodes.Clear();
        }
        #endregion

    }    
}
