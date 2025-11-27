using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    [SerializeField] Direction direction;
    [SerializeField] Facing facing;
    [SerializeField] Transform teleportTargetLocation;
    [SerializeField] private FadeScreen fadeScreen;
    CinemachineConfiner2D confiner;

    enum Direction {Up, Down, Left, Right, Teleport }
    enum Facing {Up, Down}

    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private IEnumerator TeleportSequence(GameObject player)
    {
        var controller = player.GetComponent<player_controller>();

        controller.canMove = false;
        controller.StopAllCoroutines();
        controller.ResetMovement();

        // Play Sound Effect Here
        yield return StartCoroutine(fadeScreen.FadeOut(1f));

        switch(facing)
        {
            case Facing.Up:
                controller.FaceDirection(new Vector2(0, 1));
                break;
            case Facing.Down:
                controller.FaceDirection(new Vector2(0, -1));
                break;
        }

        confiner.BoundingShape2D = mapBoundary;
        player.transform.position = teleportTargetLocation.position;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(fadeScreen.FadeIn(1f));

        controller.canMove = true;
        
    }


    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            StartCoroutine(TeleportSequence(player));
        }
        else
        {
            // normal up/down/left/right transitions
            Vector3 newPos = player.transform.position;

            switch (direction)
            {
                case Direction.Up:
                    newPos.y += 2; 
                    break;
                case Direction.Down:
                    newPos.y -= 2;
                    break;
                case Direction.Left:
                    newPos.x -= 2;
                    break;
                case Direction.Right:
                    newPos.x += 2;
                    break;
            }

            player.transform.position = newPos;
        }

    }

    private IEnumerator ReenableMovement(player_controller controller)
    {
        yield return new WaitForSeconds(0.5f);
        controller.canMove = true;
    }

}
