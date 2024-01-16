using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEditor.VersionControl.Asset;

public class GroundEnemy : HealthController
{
    public bool StayStill;

    public float wanderRadius;

    public float walkSpeed;
    public float strafeSpeed;
    public float runSpeed;

    public float idleTimeMin;
    public float idleTimeMax;

    public Difficulty difficulty;


    [SerializeField] private State currentState;
    [SerializeField] Transform hitParticle;

    private float globalTimer = 0;
    private float idleTime;
    private bool init = true;
    private NavMeshAgent agent;
    private Animator anim;
    public Transform head;
    public Transform ragdoll;
    public Vector3 lastPlayerPosition;

    public MeshRenderer eyeRight;
    public MeshRenderer eyeLeft;
    public Material eyeDetected;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        Random.InitState(System.DateTime.Now.Millisecond);
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
    }

    void Update()
    {
        globalTimer += Time.deltaTime;
        stateMachine();
        deathCheck();
    }

    void stateMachine()
    {
        if (currentState == State.Idle)
        {
            canSeePlayer(15);
            if (init)
            {
                agent.isStopped = true;
                anim.SetBool("Walking", false);
                init = false;
            }

            if (globalTimer > idleTime && !StayStill)
            {
                globalTimer = 0;
                currentState = State.Wander;
                init = true;
                return;
            }
        }

        if (currentState == State.Wander)
        {
            if (init)
            {
                agent.SetDestination(getWanderPosition(wanderRadius));
                agent.isStopped = false;
                init = false;
                anim.SetBool("Walking", true);

            }


            if (pathComplete())
            {
                globalTimer = 0;
                currentState = State.Idle;
                init = true;
                return;
            }
        }


    }

    public void deathCheck()
    {
        if(CurrentHealth <= 0)
        {
            var rag = Instantiate(ragdoll);
            rag.position = transform.position;
            rag.rotation = transform.rotation;
            Destroy(gameObject);
        }
    }

    public override void OnHit(Vector3 pos)
    {
        var particle = Instantiate(hitParticle);
        particle.localScale = new Vector3(.23f, .23f, .23f);
        particle.position = pos;

        Destroy(particle.gameObject, 2);
    }



    public Vector3 getWanderPosition(float distance)
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * distance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 99999, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }

    private bool pathComplete()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public enum Difficulty
    {
        Easy, Normal, Hard
    }

    public enum State
    {
        Idle, Wander, Combat, TakeCover
    }

    bool canSeePlayer(float range)
    {
        Collider[] localTransforms = Physics.OverlapSphere(transform.position, range);
        PlayerMovement player = null;
        Transform colliderHit = null;

        foreach (Collider coll in localTransforms)
        {
            if (coll.transform.name == "AIDetection")
            {
                player = coll.transform.root.GetComponent<PlayerMovement>();
                colliderHit = coll.transform; 
            }
        }

        if (player)
        {
            RaycastHit hit;
            if (Physics.Raycast(head.position, colliderHit.position - head.position, out hit, range))
            {
                if (hit.transform.name == "AIDetection")
                {
                    float angle = Vector3.Angle(colliderHit.position - transform.position, transform.forward);

                    if (angle <= 65)
                    {
                        Material[] oldMats = eyeRight.materials;
                        oldMats[0] = eyeDetected;

                        eyeRight.materials = oldMats;
                        eyeLeft.materials = oldMats;
                        lastPlayerPosition = colliderHit.position;
                        return true;
                    }
                }
            }
        }

        return false;
    }


}
