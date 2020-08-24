using UnityEngine;
using KD.StateMachine.BehaviorEditor.Graph;
using KD.StateMachine.BehaviorEditor.Graph.Nodes;
using UnityEditor;

/*
 * consider making a transition an array of conditions
*/
namespace KD.StateMachine.BehaviorEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        [SerializeField] BehaviorGraph graph = null;
        public static BehaviorEditor Instance => GetWindow<BehaviorEditor>();
        public BehaviorGraph CurrentGraph { get; private set; }

        public bool attemptingTransition = false;
        #endregion

        #region Init
        /* The MenuItem attribute turns any static function into a menu command. Only static functions can 
        use the MenuItem attribute. */
        [MenuItem("Window/Custom/Behavior Editor Window")]
        private static void Init()
        {
            GetWindow<BehaviorEditor>("Behavior Editor");
        }
        
        private void OnEnable()
        {
            attemptingTransition = false;
            
            CurrentGraph = graph;
            if (CurrentGraph != null) CurrentGraph.Load();
        }
        #endregion

        #region Drawing GUI
        private void OnGUI()
        {
            if (graph == null) EditorGUILayout.LabelField("Add a graph");
            graph = (BehaviorGraph)EditorGUILayout.ObjectField(graph, typeof(BehaviorGraph), false);

            if (CurrentGraph != graph)
            {
                CurrentGraph = graph;
                if (CurrentGraph != null) CurrentGraph.Load();
            }
            if (CurrentGraph != null)
            {
                if (CurrentGraph.isChanged) CurrentGraph.Save();
                CheckUserInput(Event.current);
                Draw();
            }
        }

        void Draw() 
        {
            BeginWindows();
            for (int i = 0; i < CurrentGraph.nodes.Count; i++)
            {
                CurrentGraph.nodes[i].windowRect = 
                    GUI.Window(i, CurrentGraph.nodes[i].windowRect, DrawDraggableNodeWindow, CurrentGraph.nodes[i].windowTitle);
                CurrentGraph.nodes[i].DrawConnections();
            }            
            EndWindows();
        }
        void DrawDraggableNodeWindow(int index)
        {
            CurrentGraph.nodes[index].DrawNodeWindow();
            GUI.DragWindow();
        }
        #endregion

        #region User Input

        public static event System.Action<GraphNode> AttemptTransitionEvent;
        void CheckUserInput(Event e)
        {
            if (e.button == 1 && e.type == EventType.MouseDown)
            {
                if(attemptingTransition) AttemptTransition(e);
                else ShowRightClickOptions(e);                 
            }                
            else if (e.button == 0)
            {
                if(attemptingTransition && e.type == EventType.MouseDown) AttemptTransition(e);
                else if (e.type == EventType.MouseUp) CurrentGraph.isChanged = true;
            }
        }
        void ShowRightClickOptions(Event e)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var node in CurrentGraph.nodes)
            {
                if (node.windowRect.Contains(e.mousePosition))
                {                    
                    node.ModifyNode(menu);
                    menu.ShowAsContext();
                    e.Use();
                    return;
                }
            }
            // add a new node option if none were right clicked on
            menu.AddItem(new GUIContent("Add State Node"), false, () => CurrentGraph.AddNewStateNode(e.mousePosition));
            menu.AddItem(new GUIContent("Add Portal Node"), false, () => CurrentGraph.AddNewPortalNode(e.mousePosition));
            menu.AddItem(new GUIContent("Add Comment Node"), false, () => CurrentGraph.AddNewCommentNode(e.mousePosition));
            menu.ShowAsContext();
            e.Use();
        }        
        void AttemptTransition(Event e)
        {
            foreach (var node in CurrentGraph.nodes)
            {
                if(node.CanAcceptTransition() && node.windowRect.Contains(e.mousePosition))
                {
                    AttemptTransitionEvent?.Invoke(node);
                    e.Use();
                    return;
                }
            }
            AttemptTransitionEvent?.Invoke(null);
            e.Use();
        }

        #endregion

        #region Helpers
        public static void RepaintWindow() => Instance.Repaint();
        #endregion
    }
}

