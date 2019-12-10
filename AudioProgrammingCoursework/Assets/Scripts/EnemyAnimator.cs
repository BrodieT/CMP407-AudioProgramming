using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    //Debug
    GameObject me;

    Animator animator;
    NavMeshAgent agent;
    const float animSmoothTime = 0.1f;
    private float speedPercent = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        me = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, animSmoothTime, Time.deltaTime);
    }
}
