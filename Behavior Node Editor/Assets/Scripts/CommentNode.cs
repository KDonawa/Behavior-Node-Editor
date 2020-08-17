using UnityEngine;

namespace KD.BehaviorEditor
{   
    public class CommentNode : BaseNode
    {
        string comment = "";

        public override void DrawWindow()
        {
            comment = GUILayout.TextArea(comment, 200);
        }
    }
}

