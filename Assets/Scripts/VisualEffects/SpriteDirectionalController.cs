using UnityEngine;

public class SpriteDirectionalController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Animator animator;

    private Vector3 myPosition;
    private Vector3 playerPosition;

    private Vector3 currentPosition;
    private bool isMoving;

    private float angle;
    [SerializeField] private float stopFollowingValue;

    private void Start()
    {
        currentPosition = transform.position;
    }
    void LateUpdate()
    {
        if (currentPosition != transform.position) { isMoving = true; currentPosition = transform.position; }
        else { isMoving = false; currentPosition = transform.position; }

        myPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        playerPosition = new Vector3(player.transform.position.x, 0f, (player.transform.position.z+stopFollowingValue));

        if (myPosition.x > playerPosition.x && myPosition.z > playerPosition.z)
        {
            angle = -90f;
        }
        else if (myPosition.x < playerPosition.x && myPosition.z > playerPosition.z)
        {
            angle = -45f;
        }
        else if (myPosition.x > playerPosition.x && myPosition.z < playerPosition.z)
        {
            angle = 45f;
        }
        else if (myPosition.x < playerPosition.x && myPosition.z < playerPosition.z)
        {
            angle = 90f;
        }
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("angle", angle);
    }
}
