using UnityEngine;

namespace Assets.Scripts.GameLogic
{
<<<<<<< HEAD
  public class Block : MonoBehaviour
  {
    public BlockTypes BlockType;
    private Vector3 _startMousePosition;
    private const int Threshold = 9;

    void OnMouseDown()
    {
      _startMousePosition = Input.mousePosition;
    }

    void OnMouseDrag()
    {
      var v3 = Input.mousePosition - _startMousePosition;
      v3.Normalize();
      var f = Vector3.Dot(v3, Vector3.up);
      if (Vector3.Distance(_startMousePosition, Input.mousePosition) < Threshold)
      {
        //Debug.Log("No movement");
        return;
      }

      var transf = GetComponent<Transform>();
      var currX = (int)Mathf.Round(transf.position.x);
      var currY = (int)Mathf.Round(transf.position.y);
      var newX = currX;
      var newY = currY;

=======
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
>>>>>>> bf2445ff612b246e0307904cffec2e4e23e21012
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
<<<<<<< HEAD
        }
        else
        {
          newX--;
        }
      }
      _startMousePosition = Input.mousePosition;

      FindObjectOfType<GridController>().DragBlock(currX, currY, newX, newY);
=======
        }
        else
        {
          newX--;
        }
      }

      _transform.position = _startBlockPosition;
      gameController.DragBlock(this, oldX, oldY, newX, newY);
>>>>>>> bf2445ff612b246e0307904cffec2e4e23e21012
    }
  }
}
