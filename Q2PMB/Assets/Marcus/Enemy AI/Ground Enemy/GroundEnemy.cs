using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEditor.VersionControl.Asset;

public class GroundEnemy : HealthController
{
    public bool StayStill;

    public float wanderRadius;

    float tracker;
    public float playerCheckInterval;
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
    public Transform player;

    public Transform gun;
    public Transform bullet;

    bool hasDetectedPlayer = false;

    public MeshRenderer eyeRight;
    public MeshRenderer eyeLeft;
    public Material eyeDetected;


    private float bulletMinDamage;
    private float bulletMaxDamage;
    private float bulletSpeed;

    float rotationSpeed;
    void Start()
    {
        playerCheckInterval = Random.Range(15, 45);
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        Random.InitState(System.DateTime.Now.Millisecond);
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
        setDifficulity();
    }


    public void setDifficulity()
    {
        string difficulty = PlayerPrefs.GetString("difficulty");

        if (difficulty == "low")
        {
            shotTimer = 2;
            rotationSpeed = 3;
            CurrentHealth = 40;
            MaxHealth = 40;
            bulletMinDamage = 10;
            bulletMaxDamage = 16;
            bulletSpeed = 150;
        }
        if (difficulty == "medium")
        {
            shotTimer = 1;
            rotationSpeed = 4.5f;
            CurrentHealth = 50;
            MaxHealth = 50;
            bulletMinDamage = 16;
            bulletMaxDamage = 25;
            bulletSpeed = 225;
        }
        if (difficulty == "high")
        {
            shotTimer = .5f;
            rotationSpeed = 7;
            CurrentHealth = 70;
            MaxHealth = 70;
            bulletMinDamage = 22;
            bulletMaxDamage = 32;
            bulletSpeed = 310;
        }

    }
    float shotTimer = 2;
    void Update()
    {
        globalTimer += Time.deltaTime;
        stateMachine();
        deathCheck();
    }

    void stateMachine()
    {
        tracker++;
        if (currentState == State.Idle)
        {
            if (tracker == playerCheckInterval)
            {
                hasDetectedPlayer = canSeePlayer(15);
                if (hasDetectedPlayer)
                {
                    globalTimer = 0;
                    currentState = State.Combat;
                    init = true;
                    return;
                }
                tracker = 0;
            }

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
            }
        }

        if (currentState == State.Wander)
        {
            if (tracker == playerCheckInterval)
            {
                hasDetectedPlayer = canSeePlayer(15);
                if (hasDetectedPlayer)
                {
                    globalTimer = 0;
                    currentState = State.Combat;
                    init = true;
                }
                tracker = 0;
            }

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

        if (currentState == State.Combat)
        {
            anim.SetBool("Combat", true);
            if (tracker == playerCheckInterval)
            {
                hasDetectedPlayer = canSeePlayer(20);
                if (hasDetectedPlayer)
                {

                }
                tracker = 0;
            }
            agent.updateRotation = false;
            agent.isStopped = false;

            if (init)
            {
                getCombatPosition();

                init = false;
            }

            if (globalTimer > shotTimer)
            {
                anim.CrossFade("Fire", .1f);
                Transform spawnedBullet = Instantiate(bullet);
                spawnedBullet.GetComponent<Bullet>().maxDamage = bulletMaxDamage;
                spawnedBullet.GetComponent<Bullet>().minDamage = bulletMinDamage;
                spawnedBullet.position = gun.position;
                spawnedBullet.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - gun.position) * bulletSpeed);
                Destroy(spawnedBullet.gameObject, 2);
                globalTimer = 0;
            }


            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            if (pathComplete())
            {
                init = true;
            }

        }
    }

    public void deathCheck()
    {
        if (CurrentHealth <= 0)
        {
            var rag = Instantiate(ragdoll);
            rag.position = transform.position;
            rag.rotation = transform.rotation;
            Destroy(rag.gameObject, 30);
            Destroy(gameObject);
        }
    }

    public override void OnHit(Vector3 pos)
    {
        hasDetectedPlayer = canSeePlayerOnHit(40);
        if (hasDetectedPlayer)
        {
            globalTimer = 0;
            currentState = State.Combat;
            init = true;
        }

        var particle = Instantiate(hitParticle);
        particle.localScale = new Vector3(.23f, .23f, .23f);
        particle.position = pos;

        Destroy(particle.gameObject, 2);
    }

    public Vector3 getCombatPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Vector3.Distance(transform.position, player.position) > 10)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position + transform.forward * Random.Range(5, 15), out hit, 99999, NavMesh.AllAreas))
                {
                    anim.SetBool("StrafeR", false);
                    anim.SetBool("StrafeL", false);
                    anim.SetBool("Run", true);
                    agent.speed = 2;
                    agent.destination = hit.position;

                    return hit.position;
                }
            }
            else
            {
                bool random = Random.Range(0, 2) == 1;
                if (random)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.position + transform.right * Random.Range(5, 15), out hit, 99999, NavMesh.AllAreas))
                    {
                        anim.SetBool("StrafeR", true);
                        anim.SetBool("StrafeL", false);
                        anim.SetBool("Run", false);

                        anim.CrossFade("StrafeRAim", .1f);
                        agent.speed = 1.2f;
                        agent.destination = hit.position;
                    }
                    return hit.position;

                }
                else
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.position + -transform.right * Random.Range(5, 15), out hit, 99999, NavMesh.AllAreas))
                    {
                        anim.SetBool("StrafeR", false);
                        anim.SetBool("StrafeL", true);
                        anim.SetBool("Run", false);

                        agent.speed = 1.2f;

                        agent.destination = hit.position;
                    }
                    return hit.position;

                }
            }
        }

        return transform.position;
    }
    public Vector3 getWanderPosition(float distance)
    {
        for (int i = 0; i < 20; i++)
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

        List<GroundEnemy> localCompananions = new List<GroundEnemy>();

        foreach (Collider coll in localTransforms)
        {
            if (coll.transform.name == "AIDetection")
            {
                player = coll.transform.root.GetComponent<PlayerMovement>();
                colliderHit = coll.transform;
            }

            if (coll.transform.root.GetComponent<GroundEnemy>())
            {
                localCompananions.Add(coll.transform.root.GetComponent<GroundEnemy>());
            }
        }

        if (!hasDetectedPlayer)
        {


            foreach (GroundEnemy enemy in localCompananions)
            {
                RaycastHit hit;

                if (Physics.Raycast(head.position, enemy.head.position - head.position, out hit, range))
                {
                    if (hit.transform.GetComponent<Hitbox>())
                    {
                        float angle = Vector3.Angle(enemy.head.position - transform.position, transform.forward);

                        if (angle <= 65)
                        {
                            if (enemy.hasDetectedPlayer)
                            {
                                Material[] oldMats = eyeRight.materials;
                                oldMats[0] = eyeDetected;
                                this.player = enemy.player;
                                eyeRight.materials = oldMats;
                                eyeLeft.materials = oldMats;
                                lastPlayerPosition = enemy.lastPlayerPosition;
                                globalTimer = 0;
                                currentState = State.Combat;
                                init = true;
                                continue;
                            }
                        }
                    }
                }
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
                        this.player = player.transform;

                        Material[] oldMats = eyeRight.materials;
                        oldMats[0] = eyeDetected;

                        eyeRight.materials = oldMats;
                        eyeLeft.materials = oldMats;
                        lastPlayerPosition = colliderHit.position;
                        hasDetectedPlayer = true;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    bool canSeePlayerOnHit(float range)
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
            this.player = player.transform;

            Material[] oldMats = eyeRight.materials;
            oldMats[0] = eyeDetected;

            eyeRight.materials = oldMats;
            eyeLeft.materials = oldMats;
            lastPlayerPosition = colliderHit.position;
            return true;

        }

        return false;
    }


}
