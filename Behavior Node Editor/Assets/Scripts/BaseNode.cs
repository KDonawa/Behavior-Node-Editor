using UnityEngine;

namespace KD.BehaviorEditor
{
    public abstract class BaseNode
    {
        public Rect windowRect;
        public string windowTitle;

        public virtual void DrawWindow()
        {

        }
        public virtual void DrawCurve()
        {

        }
        public virtual void DeleteNode()
        {
            BehaviorEditor.RemoveNodeWindow(this);
        }
    }
}
