using UnityEngine;

namespace Assets.Scripts.GameLogic
{
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
//            var gameControllerObject = GameObject.FindWithTag("GridController");
//            if (gameControllerObject == null)
//            {
//                Debug.Log("Cannot find 'GridController' script");
//                return;
//            }

//            var gameController = gameControllerObject.GetComponent<GridController>();
//            if (gameController == null)
//            {
//                Debug.Log("Cannot find 'GridController' script");
//                return;
//            }

            var v3 = Input.mousePosition - _startMousePosition;
            v3.Normalize();
            var f = Vector3.Dot(v3, Vector3.up);
            if (Vector3.Distance(_startMousePosition, Input.mousePosition) < Threshold)
            {
                //Debug.Log("No movement");
                return;
            }
            
            var transf = GetComponent<Transform>();
            var currX = (int) Mathf.Round(transf.position.x);
            var currY = (int) Mathf.Round(transf.position.y);
            var newX = currX;
            var newY = currY;

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
            _startMousePosition = Input.mousePosition;

            FindObjectOfType<GridController>().DragBlock(currX, currY, newX, newY);
        }
    }
}
