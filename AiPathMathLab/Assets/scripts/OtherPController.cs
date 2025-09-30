
using System.Collections.Generic;
using UnityEngine;

public class OtherPController : MonoBehaviour
{
    [SerializeField]
    public pathManager pathManager;

    List<wayPoint> thePath;
    wayPoint target;

    public float MoveSpeed, RotateSpeed;

    public Animator animator;
    [SerializeField] bool isWalking;
    [SerializeField] bool isBlocked = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        isWalking = false;
        animator.SetBool("IsWalking", isWalking);

        thePath = pathManager.GetPath();
        if (thePath != null && thePath.Count > 0)
        {
            //set starting target for first waypoint
            target = thePath[0];

        }
    }

    void rotateTowardsTarget()
    {
        float stepSize = RotateSpeed * Time.deltaTime;

        Vector3 targetDir = target.pos - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, stepSize, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    void moveForward()
    {
        float stepSize = Time.deltaTime * MoveSpeed;
        float distanceToTarget = Vector3.Distance(transform.position, target.pos);
        if (distanceToTarget < stepSize)
        {
            //we will overshoot the target
            //so do something smarter here
            return;
        }
        // take a step forward
        Vector3 moveDir = Vector3.forward;
        transform.Translate(moveDir * stepSize);
    }

    private void Update()
    {

        if (Input.anyKeyDown && isBlocked == false)
        {
            //toggle if any key is pressed
            isWalking = !isWalking;
            animator.SetBool("IsWalking", isWalking);
        }
        if (isWalking && isBlocked == false)
        {
            rotateTowardsTarget();
            moveForward();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //switch to next target
        target = pathManager.GetNextTarget();
        Debug.Log(other);

        if (other.gameObject.CompareTag("object"))
        {
            Debug.Log("collision");
            isBlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("object"))
        {
            Debug.Log("left collision");
            isBlocked = false;
        }
    }
}
