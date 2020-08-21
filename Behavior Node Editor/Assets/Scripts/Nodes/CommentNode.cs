using UnityEngine;

namespace KD.BehaviorEditor.Nodes
{   
    public class CommentNode : BaseNode
    {
        [SerializeField] string comment = "";

        public CommentNode(Vector2 position, string title = "Comment") 
            : base(position, new Vector2(150f, 100f), title) { }

        public override void DrawNodeWindow()
        {
            comment = GUILayout.TextArea(comment, 200);
        }
    }
}

