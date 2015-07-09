namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BlockMover : MonoBehaviour
    {
        private bool _moving;
        private float distanceToNearBottom = 40f;
        private float distanceToBorder = 200f;
        private float inverseMoveTime;          //Used to make movement more efficient.
        private float moveTime = 0.1f;           //Time it will take object to move, in seconds.
        private CircleCollider2D boxCollider;
        private Rigidbody2D rb2D;
        private bool _disabled = true;

        void Start()
        {
            boxCollider = GetComponent<CircleCollider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
            StartCoroutine(WaitAndEnableScript());
        }

        private IEnumerator WaitAndEnableScript()
        {
            yield return new WaitForSeconds(1);
            _disabled = false;
        }
        
        void FixedUpdate()
        {
            if (_disabled)
                return;

            if(_moving)
                return;

            boxCollider.enabled = false;
            var bottomHit = Physics2D.Raycast(transform.position, -Vector2.up, 3.5f);
            boxCollider.enabled = true;

            if (bottomHit.transform == null)
            {
                var end = transform.position + new Vector3(0, -40, 0);
                _moving = true;
                StartCoroutine(SmoothMovement(end));
                
                return;
            }

            var currentSprite = GetComponent<Image>().sprite;
            if (currentSprite == null)
            {
                return;
            }

            var counter = new List<RaycastHit2D>();

            boxCollider.enabled = false;
            var leftHits = Physics2D.RaycastAll(transform.position, -Vector2.right, distanceToBorder);
            var rightHits = Physics2D.RaycastAll(transform.position, Vector2.right, distanceToBorder);
            
            boxCollider.enabled = true;

            counter.AddRange(CheckNearBlocks(leftHits, currentSprite));
            counter.AddRange(CheckNearBlocks(rightHits, currentSprite));

            if (counter.Count >= 2)
            {
                foreach (var hit in counter)
                {
                    Destroy(hit.transform.gameObject);
                }

                Destroy(gameObject);
            }

            counter.Clear();

            boxCollider.enabled = false;
            var topHits = Physics2D.RaycastAll(transform.position, Vector2.up, distanceToBorder);
            var bottomHits = Physics2D.RaycastAll(transform.position, -Vector2.up, distanceToBorder);
            boxCollider.enabled = true;

            counter.AddRange(CheckNearBlocks(topHits, currentSprite));
            counter.AddRange(CheckNearBlocks(bottomHits, currentSprite));

            if (counter.Count >= 2)
            {
                foreach (var hit in counter)
                {
                    Destroy(hit.transform.gameObject);
                }

                Destroy(gameObject);
            }
        }
        
        private static IEnumerable<RaycastHit2D> CheckNearBlocks(IEnumerable<RaycastHit2D> hits, Sprite currentSprite)
        {
            foreach (var hit in hits)
            {
                var img = hit.collider.gameObject.GetComponent<Image>();
                if(img == null)
                    continue;

                if (currentSprite == img.sprite)
                    yield return hit;
                else
                {
                    break;
                }
            }
        }
        
        protected IEnumerator SmoothMovement(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                rb2D.MovePosition(newPostion);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
            _moving = false;
        }
    }
}
