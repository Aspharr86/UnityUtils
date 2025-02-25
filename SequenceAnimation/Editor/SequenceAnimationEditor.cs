using UnityEditor;

namespace Bubu.UnityUtils.SequenceAnimation
{
    public abstract class SequenceAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
                return;

            SetInspector();
        }

        protected abstract void SetInspector();
    }
}
