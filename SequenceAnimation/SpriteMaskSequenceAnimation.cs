using UnityEngine;

namespace Bubu.UnityUtils.SequenceAnimation
{
    [RequireComponent(typeof(SpriteMask))]
    public class SpriteMaskSequenceAnimation : SequenceAnimation<SpriteMask>
    {
        private SpriteMask spriteMask;
        protected override SpriteMask component => spriteMask ?? (spriteMask = transform.GetComponent<SpriteMask>());

        protected override void SetSprite(Sprite sprite)
        {
            component.sprite = sprite;
        }
    }
}
