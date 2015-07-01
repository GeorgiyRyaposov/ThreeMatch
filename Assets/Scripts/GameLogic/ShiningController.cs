using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class ShiningController : MonoBehaviour
{
  public RectTransform Sun;
  public RectTransform Moon;
  public int ShiningHours = 24;
  public float MoveTime = 5.0f;

  private RectTransform _shining;
  private int _moveStep;
  private int _hours = 0;
  private Vector3 _startPosition;

  protected void Awake()
  {
    _startPosition = Sun.localPosition;
    _shining = Sun;

    _moveStep = (int)(GetComponent<RectTransform>().rect.width - _shining.rect.width) / ShiningHours;
  }

  protected void Update()
  {  
    if (Input.GetKeyUp(KeyCode.Space))
    {
      _hours++;

      if (_hours <= ShiningHours)
      {
        StartCoroutine(MoveShining());
      }
      else
      {
        FlipShining();

        _hours = 0;
      }
    }
  }

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

  private void FlipShining()
  {
    if (Sun.gameObject.activeSelf)
    {
      Sun.gameObject.SetActive(false);
      Moon.gameObject.SetActive(true);
      _shining = Moon;
    }
    else
    {
      Sun.gameObject.SetActive(true);
      Moon.gameObject.SetActive(false);
      _shining = Sun;
    }

    _shining.localPosition = _startPosition;
  }
}
