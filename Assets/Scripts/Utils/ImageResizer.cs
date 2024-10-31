using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Image))]
public class ImageResizer : MonoBehaviour
{
    private Image image;

    [PreviewField(100)] // Odin attribute to show a sprite preview in the inspector
    [SerializeField] private Sprite newSprite;

    [Button("Set Sprite & Resize")] // This adds a button in the inspector
    private void SetSpriteAndResize()
    {
        image = GetComponent<Image>();

        if (newSprite == null)
        {
            Debug.LogWarning("Sprite is not assigned!");
            return;
        }

        Debug.Log(image);
        // Set the new sprite
        image.sprite = newSprite;

        // Adjust the size of the image to match the sprite's size
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newSprite.rect.width, newSprite.rect.height);
    }
}
