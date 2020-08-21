using UnityEngine;
using UnityEditor;

namespace KD.BehaviorEditor.Nodes
{
    public abstract class BaseNode
    {
        public Rect windowRect;
        public string windowTitle;

        public BaseNode(Vector2 position, Vector2 size, string title)
        {
            windowRect = new Rect(position, size);
            windowTitle = title;
        }

        public abstract void DrawNodeWindow();
        public virtual void DrawCurve() {  }
        public virtual void DeleteNode() => BehaviorEditor.DeleteNode(this);
        public virtual void ModifyNode(GenericMenu menu) => menu.AddItem(new GUIContent("Delete"), false, DeleteNode);
    }
}
