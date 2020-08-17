using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KD.BehaviorEditor.States;

namespace KD.BehaviorEditor.Nodes
{
    public class TransitionNode : BaseNode
    {
        public Transition transition;
        public StateNode enterStateNode;
        public StateNode exitStateNode;

        public TransitionNode(StateNode stateNode)
        {
            enterStateNode = stateNode;
            transition = new Transition();

            stateNode.AddTransitionNode(this);
        }

        public override void DrawWindow()
        {
            //EditorGUILayout.LabelField(""); // idk about this
            transition.condition = (Condition)EditorGUILayout.ObjectField(transition.condition, typeof(Condition), false);

            if(transition.condition == null)
            {
                EditorGUILayout.LabelField("No Condition!");
            }
            else
            {
                transition.disable = EditorGUILayout.Toggle("Disable", transition.disable);
            }
        }

        public override void DrawCurve()
        {
            //Rect rect = new Rect(windowRect.position, windowRect.size*.5f);
            Rect rect = windowRect;
            rect.y += windowRect.height * .5f;
            rect.width = 1f;
            rect.height = 1f;

            BehaviorEditor.DrawNodeCurve(enterStateNode.windowRect, rect, true, Color.black);

        }
    }
}

