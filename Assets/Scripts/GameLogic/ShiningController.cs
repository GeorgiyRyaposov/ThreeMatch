namespace Assets.Scripts.GameLogic
{
  using System.Collections;
  using Messaging;
  using UnityEngine;
  using UnityEngine.UI;

  public class ShiningController : MonoBehaviour
  {
    public Image Shining;
    public Image Background;
    public Sprite[] ShiningSprites;
    public Sprite[] BackgroundSprites;  
  
    public int ShiningHours = 24;
    public float MoveTime = 0.8f;

    private RectTransform _shining;
    private int _shiningIndex = 0;
    private float _moveStep;
    private int _hours = 0;
    private Vector3 _startPosition;
    private Animation _animation;

    protected void Awake()
    {
      _shining = Shining.GetComponent<RectTransform>();
      _startPosition = _shining.localPosition;

      Shining.GetComponent<Image>().sprite = ShiningSprites[_shiningIndex];
      Background.sprite = BackgroundSprites[_shiningIndex];

      _moveStep = (GetComponent<RectTransform>().rect.width - _shining.rect.width) / ShiningHours;

      _animation = GetComponent<Animation>();

      Messenger.Instance.AddHandler("AddHour", AddHour);
    }

    private void AddHour()
    {
      _hours++;

      if (_hours <= ShiningHours)
      {
        StartCoroutine(MoveShining());
      }
      else
      {
        _animation.Play();

        _hours = 0;
      }
    }

//    protected void Update()
//    {  
//      if (Input.GetKeyUp(KeyCode.Space))
//      {
//        AddHour();
//      }
//    }

    private IEnumerator MoveShining()
    {
      var start = _shining.localPosition;
      var target = _shining.localPosition + Vector3.right * _moveStep;

      var elapsedTime = 0.0f;
      while (elapsedTime < MoveTime)
      {
        _shining.localPosition = Vector3.Lerp(start, target, elapsedTime / MoveTime);
        elapsedTime += Time.deltaTime;
      
        yield return null;
      }
    }

    public void FlipShining()
    {
      _shiningIndex++;
      if (_shiningIndex >= ShiningSprites.Length)
      {
        _shiningIndex = 0;
      }

      Shining.GetComponent<Image>().sprite = ShiningSprites[_shiningIndex];
      Background.sprite = BackgroundSprites[_shiningIndex];

      _shining.localPosition = _startPosition;
    }
  }
}
