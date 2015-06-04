using UnityEngine;

namespace Assets.Scripts.GameLogic
{
  public class Block : MonoBehaviour 
  {
    public BlockTypes BlockType;

    private const int Threshold = 9;
    private Vector3 _startMousePosition;
    private Vector3 _startBlockPosition;
    private Vector3 _targetPosition;
    private Vector3 _previousPosition;
    private Transform _transform;
    private float speed = 10.0F;
    private float journeyLength;
    private float mouseX;
    private float mouseY;

    void Start()
    {
      _transform = GetComponent<Transform>();
      _previousPosition = _transform.position;
      _targetPosition = _transform.position;
      _startBlockPosition = _transform.position;
    }

    void Update()
    {
      mouseX = Input.mousePosition.x;
      mouseY = Input.mousePosition.y;

      if (_transform.position != _targetPosition)
      {
        _transform.position = Vector3.Lerp(_transform.position, _targetPosition, Time.deltaTime * speed);
      }
    }

    public void MoveToNewPoint(int x, int y)
    {
      _targetPosition = new Vector3(x, y, 1.0f);
    }

    public void MoveToNewPoint(Vector3 newPosition)
    {
      _targetPosition = newPosition;
    }

    public void MoveToPreviousPoint()
    {
      _targetPosition = _previousPosition;
    }

    void OnMouseDown()
    {
      _previousPosition = _transform.position;
      _startBlockPosition = _transform.position;
      _startMousePosition = Input.mousePosition;
    }

    void OnMouseDrag()
    {
      _transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouseX, mouseY, 10.0f)); 
    }

    void OnMouseUp()
    {
      var gameControllerObject = GameObject.FindWithTag("GameController");
      if (gameControllerObject == null)
      {
        Debug.Log("Cannot find 'GameController' script");
        return;
      }

      var gameController = gameControllerObject.GetComponent<GameController>();
      if (gameController == null)
      {
        Debug.Log("Cannot find 'GameController' script");
        return;
      }

      var v3 = Input.mousePosition - _startMousePosition;
      v3.Normalize();
      var f = Vector3.Dot(v3, Vector3.up);
      if (Vector3.Distance(_startMousePosition, Input.mousePosition) < Threshold)
      {
        //Debug.Log("No movement");
        return;
      }

      var oldX = (int)Mathf.Round(_startBlockPosition.x);
      var oldY = (int)Mathf.Round(_startBlockPosition.y);
      var newX = oldX;
      var newY = oldY;
      if (f >= 0.5)
      {
        newY++;
      }
      else if (f <= -0.5)
      {
        newY--;
      }
      else
      {
        f = Vector3.Dot(v3, Vector3.right);
        if (f >= 0.5)
        {
          newX++;
        }
        else
        {
          newX--;
        }
      }

      _transform.position = _startBlockPosition;
      gameController.DragBlock(this, oldX, oldY, newX, newY);
    }
  }
}
