using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool caught = false;
    public bool isDead = false;
    public float lookRadius = 50.0f;
    Transform target;
    NavMeshAgent agent;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        target = player.transform;
        agent = GetComponent<NavMeshAgent>();
    }
    public LayerMask targetPlayer;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance)
                {
                    //Attack Target
                    FaceTarget();
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0));

            caught = true;
            isDead = true;
            GetComponent<Animator>().speed = 0;
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            GetComponent<Animator>().speed = 0;
            caught = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetComponent<Animator>().speed = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
