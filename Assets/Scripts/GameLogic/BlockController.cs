using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlockController : MonoBehaviour
{
  [HideInInspector]
  public Sprite ReplacementSprite;

  private Image _image;

  protected void Awake()
  {
    _image = GetComponent<Image>();
  }

  public void ReplaceSprite()
  {
    if (ReplacementSprite)
    {
      _image.sprite = ReplacementSprite;

      ReplacementSprite = null;
    }    
  }
}
