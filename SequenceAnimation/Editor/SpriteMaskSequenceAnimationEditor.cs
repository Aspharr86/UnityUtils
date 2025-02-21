using UnityEditor;
using UnityEngine;

namespace UBW.SequenceAnimation
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpriteMaskSequenceAnimation))]
    public class SpriteMaskSequenceAnimationEditor : SequenceAnimationEditor
    {
        protected override void SetInspector()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteMaskSequenceAnimation;
                    sequenceAnimation.Play();
                }

            if (GUILayout.Button("Pause"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteMaskSequenceAnimation;
                    sequenceAnimation.Pause();
                }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Resume"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteMaskSequenceAnimation;
                    sequenceAnimation.Resume();
                }

            if (GUILayout.Button("Stop"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteMaskSequenceAnimation;
                    sequenceAnimation.Stop();
                }

            EditorGUILayout.EndHorizontal();
        }
    }
}
