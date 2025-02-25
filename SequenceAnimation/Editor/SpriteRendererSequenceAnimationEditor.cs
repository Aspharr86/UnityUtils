using UnityEditor;
using UnityEngine;

namespace Bubu.UnityUtils.SequenceAnimation
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpriteRendererSequenceAnimation))]
    public class SpriteRendererSequenceAnimationEditor : SequenceAnimationEditor
    {
        protected override void SetInspector()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteRendererSequenceAnimation;
                    sequenceAnimation.Play();
                }

            if (GUILayout.Button("Pause"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteRendererSequenceAnimation;
                    sequenceAnimation.Pause();
                }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Resume"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteRendererSequenceAnimation;
                    sequenceAnimation.Resume();
                }

            if (GUILayout.Button("Stop"))
                for (int i = 0; i < targets.Length; i++)
                {
                    var sequenceAnimation = targets[i] as SpriteRendererSequenceAnimation;
                    sequenceAnimation.Stop();
                }

            EditorGUILayout.EndHorizontal();
        }
    }
}
