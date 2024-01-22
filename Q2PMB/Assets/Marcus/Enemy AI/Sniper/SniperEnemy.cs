using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : HealthController
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
    private Animator anim;
    public Transform head;
    public Transform ragdoll;
    public Vector3 lastPlayerPosition;
    public Transform player;

    public Transform muzzleFlash;

    public Transform gun;
    public Transform bullet;
    public LayerMask ignore;
    bool hasDetectedPlayer = false;

    public MeshRenderer eyeRight;
    public MeshRenderer eyeLeft;
    public Material eyeDetected;
    void Start()
    {
        playerCheckInterval = Random.Range(15, 45);
        anim = GetComponent<Animator>();
        Random.InitState(System.DateTime.Now.Millisecond);
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
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
                hasDetectedPlayer = canSeePlayer(40);
                if (hasDetectedPlayer)
                {
                    globalTimer = 0;
                    currentState = State.Combat;
                    init = true;
                    return;
                }
                tracker = 0;
            }

         
            if (globalTimer > idleTime && !StayStill)
            {
                globalTimer = 0;
                currentState = State.Wander;
                init = true;
            }
        }
    
        if (currentState == State.Combat)
        {
            if (tracker == playerCheckInterval)
            {
                hasDetectedPlayer = canSeePlayer(40);
                if (hasDetectedPlayer)
                {

                }
                tracker = 0;
            }

            if (globalTimer > shotTimer)
            {
                anim.Play("Shot");
                Transform spawnedBullet = Instantiate(bullet);
                Transform muzzleflash = Instantiate(muzzleFlash, gun);
                Destroy(muzzleflash.gameObject, .1f);

                spawnedBullet.position = gun.position;
                spawnedBullet.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - gun.position) * 300);
                Destroy(spawnedBullet.gameObject, 2);
                globalTimer = 0;

                shotTimer = Random.Range(4, 15);
            }


            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4.5f);

        

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

       

        if (player)
        {
            RaycastHit hit;
            if (Physics.Raycast(head.position, colliderHit.position - head.position, out hit, range, ~ignore))
            {
                print(hit.transform.name);
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
