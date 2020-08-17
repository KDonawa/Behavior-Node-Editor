using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using KD.BehaviorEditor.Nodes;
using KD.BehaviorEditor.States;

/*
 * bug: right click not working at times
 * 
*/
namespace KD.BehaviorEditor
{
    public enum UserActions
    {
        AddState,
        AddTransition,
        DeleteNode,
        CommentNode,
    }
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        static List<BaseNode> nodes = new List<BaseNode>();
        Vector3 mousePosition;
        bool makeTransition;
        bool clickedOnWindow;
        BaseNode selectedNode;
        #endregion

        #region Init

        /*
        The MenuItem attribute allows you to add menu items to the main menu and inspector context menus.
        The MenuItem attribute turns any static function into a menu command. Only static functions can 
        use the MenuItem attribute.
         */
        [MenuItem("Custom Editor Window/Behavior Editor Window")]
        static void Init()
        {
            BehaviorEditor editor = GetWindow<BehaviorEditor>();
            //editor.minSize = new Vector2(800, 600);
        }
        #endregion

        #region GUI Methods
        //called every time the screen refreshes
        void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;
            UserInput(e); // is there a way to check for user input in an event system?
            DrawWindows();
        }
        void DrawWindows() // consider doing this whenever node,etc. is added/removed
        {
            BeginWindows();

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].windowRect = GUI.Window(i, nodes[i].windowRect, DrawNodeWindow, nodes[i].windowTitle);
                //nodes[i].DrawWindow();
                nodes[i].DrawCurve();
            }
            EndWindows();
        }
        void DrawNodeWindow(int index)
        {
            nodes[index].DrawWindow();
            GUI.DragWindow(); // make a window draggable.
        }
        void UserInput(Event e)
        {
            // right click
            if (!makeTransition && e.button == 1 && e.type == EventType.MouseDown) 
            {
                RightClick(e);
            }
            // left click
            if (e.button == 0 && !makeTransition) 
            {
                if (e.type == EventType.MouseDown)
                {
                    LeftClick(e);
                }
            }
        }
        void RightClick(Event e)
        {
            selectedNode = null;
            for (int i = 0; i < nodes.Count; i++)
            {                
                if (nodes[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true; // remove for modifynode()
                    selectedNode = nodes[i];
                    break;
                }
            }
            // put this in the if/else above
            
            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e); // pass the node in
            }
        }
        void LeftClick(Event e)
        {

        }
        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add State"), false, AddStateCallback);
            menu.AddItem(new GUIContent("Add Comment"), false, AddCommentCallback);
            menu.ShowAsContext();
            e.Use();
        }
        void ModifyNode(Event e)
        {
            if (selectedNode == null) return;

            GenericMenu menu = new GenericMenu();
            if (selectedNode is StateNode) 
            {
                if((selectedNode as StateNode).currentState != null)
                {
                    menu.AddItem(new GUIContent("Add Transition"), false, AddTransitionCallBack);                    
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Add Transition"));
                }
                menu.AddItem(new GUIContent("Delete"), false, DeleteNodeCallback);
            }
            else if (selectedNode is CommentNode)
            {
                menu.AddItem(new GUIContent("Delete"), false, DeleteNodeCallback);
            }
            menu.ShowAsContext();
            e.Use();
        }
        void AddStateCallback() => AddNode(new StateNode(), "State", new Rect(mousePosition, new Vector2(200f, 100f)));
        void AddCommentCallback() => AddNode(new CommentNode(), "Comment", new Rect(mousePosition, new Vector2(200f, 100f)));
        void AddTransitionCallBack()
        {            
            StateNode stateNode = (StateNode)selectedNode;
            AddNode(new TransitionNode((StateNode)selectedNode), "Transition", new Rect(stateNode.windowRect.x + 200f, stateNode.windowRect.y, 200f, 100f));
        }
        // helper
        void AddNode(BaseNode node, string nodeName, Rect rect)
        {
            node.windowRect = rect;
            node.windowTitle = nodeName;
            nodes.Add(node);
        }
        void DeleteNodeCallback()
        {
            selectedNode.DeleteNode();
        }
        #endregion

        #region Helpers

        public static void RemoveNodeWindow(BaseNode node)
        {
            nodes.Remove(node);
        }
        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                left ? start.x + start.width : start.x,
                start.y + start.height * 0.5f,
                0f);
            Vector3 endPos = new Vector3(
                end.x + end.width * 0.5f,
                end.y + end.height * 0.5f,
                0);
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

