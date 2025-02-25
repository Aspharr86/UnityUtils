using UnityEngine;
using UnityEngine.UI;

namespace Bubu.UnityUtils.SequenceAnimation
{
    [RequireComponent(typeof(Image))]
    public class ImageSequenceAnimation : SequenceAnimation<Image>
    {
        private Image image;
        protected override Image component => image ?? (image = transform.GetComponent<Image>());

        protected override void SetSprite(Sprite sprite)
        {
            component.sprite = sprite;
        }
    }
}
