using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask solidObjectsLayer;

    public bool canMove = true;
    private bool isMoving;
    private Vector2 input;

    private Animator animator;
    private Transform playerTransform;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (!canMove) 
        {
            animator.SetBool("isMoving", false);
            return;
        }

        if (!isMoving)
        {
            // get input
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // prevents diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                if (input.x < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
                else if (input.x > 0)
                    transform.localScale = new Vector3(1, 1, 1);

                Vector3 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        input = Vector2.zero;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void ResetMovement()
    {
        isMoving = false;
        input = Vector2.zero;
    }

    public void FaceDirection(Vector2 dir)
    {
        animator.SetFloat("moveX", dir.x);
        animator.SetFloat("moveY", dir.y);

        if (dir.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dir.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
