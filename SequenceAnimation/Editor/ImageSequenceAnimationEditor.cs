using UnityEditor;
using UnityEngine;

namespace UBW.SequenceAnimation
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ImageSequenceAnimation))]
    public class ImageSequenceAnimationEditor : SequenceAnimationEditor
    {
        protected override void SetInspector()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as ImageSequenceAnimation;
                    sequenceAnimation.Play();
                }

            if (GUILayout.Button("Pause"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as ImageSequenceAnimation;
                    sequenceAnimation.Pause();
                }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Resume"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as ImageSequenceAnimation;
                    sequenceAnimation.Resume();
                }

            if (GUILayout.Button("Stop"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as ImageSequenceAnimation;
                    sequenceAnimation.Stop();
                }

            EditorGUILayout.EndHorizontal();
        }
    }
}
