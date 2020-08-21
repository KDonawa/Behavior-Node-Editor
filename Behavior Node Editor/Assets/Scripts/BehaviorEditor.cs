using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using KD.BehaviorEditor.Nodes;

/*
 * consider making a transition an array of conditions
*/
namespace KD.BehaviorEditor
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        [SerializeField] BehaviorGraph graph = null;
        public static BehaviorEditor Instance => GetWindow<BehaviorEditor>();
        public BehaviorGraph CurrentGraph { get; private set; }        

        #endregion

        #region Init
        /* The MenuItem attribute turns any static function into a menu command. Only static functions can 
        use the MenuItem attribute. */
        [MenuItem("Window/Custom/Behavior Editor Window")]
        private static void Init() => GetWindow<BehaviorEditor>();

        private void Awake()
        {
            CurrentGraph = graph;
            if (CurrentGraph != null) CurrentGraph.Load();
        }

        #endregion

        #region GUI Methods

        void OnGUI()
        {
            if (graph == null) EditorGUILayout.LabelField("Add a graph");
            graph = (BehaviorGraph)EditorGUILayout.ObjectField(graph, typeof(BehaviorGraph), false);
            
            if (CurrentGraph != graph)
            {
                CurrentGraph = graph;
                if (CurrentGraph != null) CurrentGraph.Load();
            }
            else
            {
                if (CurrentGraph != null)
                {
                    CheckUserInput(Event.current);                  
                    DrawWindows();
                    if (CurrentGraph.IsChanged) CurrentGraph.Save();
                }
            }           
        }
        void DrawWindows() 
        {
            BeginWindows();
            for (int i = 0; i < CurrentGraph.nodes.Count; i++)
            {
                CurrentGraph.nodes[i].windowRect = 
                    GUI.Window(i, CurrentGraph.nodes[i].windowRect, DrawDraggableNodeWindow, CurrentGraph.nodes[i].windowTitle);

                CurrentGraph.nodes[i].DrawCurve();
            }
            EndWindows();
        }
        void DrawDraggableNodeWindow(int index)
        {
            CurrentGraph.nodes[index].DrawNodeWindow();
            GUI.DragWindow(); // make a window draggable.
        }
        void CheckUserInput(Event e)
        {
            if (/*!makeTransition &&*/ e.button == 1 && e.type == EventType.MouseDown) ShowRightClickOptions(e);
            if (/*!makeTransition &&*/ e.button == 0)
            {
                if(e.type == EventType.MouseDown) LeftClickDown(e);
                if (e.type == EventType.MouseUp) LeftClickUp();
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
            menu.AddItem(new GUIContent("Add State"), false, () => CurrentGraph.AddNewStateNode(e.mousePosition));
            menu.AddItem(new GUIContent("Add Comment"), false, () => CurrentGraph.AddNewCommentNode(e.mousePosition));
            menu.ShowAsContext();
            e.Use();
        }

        void LeftClickDown(Event e)
        {

        }
        void LeftClickUp()
        {
            CurrentGraph.IsChanged = true;
        }

        #endregion

        #region Helpers
        public static void DeleteNode(BaseNode node) => Instance.CurrentGraph.DeleteNode(node);
        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                left ? start.x + start.width : start.x,
                start.y + start.height * 0.5f,
                0f);
            Vector3 endPos = new Vector3(
                end.x,
                end.y + end.height * 0.5f,
                0f);
            Vector3 startTangent = startPos + Vector3.right * 50f;
            Vector3 endTangent = endPos + Vector3.left * 50f;

            Color shadow = new Color(0f, 0f, 0f, 0.1f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, shadow, null, (i + 1) * .5f);
            }
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, curveColor, null, 1f);
        }
        #endregion
    }
}

