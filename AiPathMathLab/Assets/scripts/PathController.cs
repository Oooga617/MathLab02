
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [SerializeField]
    public pathManager pathManager;

    List<wayPoint> thePath;
    wayPoint target;

    public float MoveSpeed, RotateSpeed;

    public Animator animator;
    bool isWalking;

    private void Start()
    {
        //animator = GetComponent<Animator>();

        isWalking = false;
        animator.SetBool("IsWalking", isWalking);

        thePath = pathManager.GetPath();
        if (thePath != null && thePath.Count >0)
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
      
        if (Input.anyKeyDown)
        {
            //toggle if any key is pressed
            isWalking = !isWalking;
            animator.SetBool("IsWalking", isWalking);
        }
        if (isWalking)
        {
            rotateTowardsTarget();
            moveForward();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("point"))
        {
            //switch to next target
            target = pathManager.GetNextTarget();
            Debug.Log("knight hits point");
        }
        
    }
}
