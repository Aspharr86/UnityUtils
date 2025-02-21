using UnityEngine;

namespace UBW.SequenceAnimation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererSequenceAnimation : SequenceAnimation<SpriteRenderer>
    {
        private SpriteRenderer spriteRenderer;
        protected override SpriteRenderer component => spriteRenderer ?? (spriteRenderer = transform.GetComponent<SpriteRenderer>());

        protected override void SetSprite(Sprite sprite)
        {
            component.sprite = sprite;
        }
    }
}
