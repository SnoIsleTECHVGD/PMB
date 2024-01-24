using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Boss : HealthController
{
    public Transform player;
    public Transform muzzleFlash;
    public SkinnedMeshRenderer eye;
    public Material eyeDetected;
    public Transform head;
    public LayerMask ignore;
    public GameObject canvas;
    public Image healthBar; 
    int tracker;
    [SerializeField] Transform hitParticle;

    public Transform ragdoll;

    public Transform lGun;
    public Transform rGun;

    public Transform bullet;

    public NavMeshAgent agent;
    public Animator anim;
    bool init = true;

    float gunTimer = 0;
    bool whichSide;

    float shotTimer = .7f;

    float specialAttackCooldown = 3;
    float specialAttackTimer;
    public bool isPerformingSpecialAttack = false;

    private float bulletMinDamage;
    private float bulletMaxDamage;
    private float bulletSpeed;


    private float specialBulletMinDamage;
    private float specialBulletMaxDamage;
    private float specialBulletSpeed;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        setDifficulity();
    }

    public void setDifficulity()
    {
        string difficulty = PlayerPrefs.GetString("difficulty");

        if (difficulty == "low")
        {
            CurrentHealth = 500;
            MaxHealth = 500;

            bulletMinDamage = 12;
            bulletMaxDamage = 18;
            bulletSpeed = 150;

            specialBulletMaxDamage = 30;
            specialBulletMinDamage = 20;
            specialBulletSpeed = 100;
        }
        if (difficulty == "medium")
        {
            CurrentHealth = 850;
            MaxHealth = 850;

            bulletMinDamage = 18;
            bulletMaxDamage = 27;
            bulletSpeed = 250;

            specialBulletMaxDamage = 40;
            specialBulletMinDamage = 30;
            specialBulletSpeed = 200;
        }
        if (difficulty == "high")
        {

            CurrentHealth = 1250;
            MaxHealth = 1250;

            bulletMinDamage = 25;
            bulletMaxDamage = 37;
            bulletSpeed = 300;

            specialBulletMaxDamage = 50;
            specialBulletMinDamage = 40;
            specialBulletSpeed = 300;
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
        deathCheck();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Material[] oldMats = eye.materials;
        oldMats[1] = eyeDetected;

        eye.materials = oldMats;
        var particle = Instantiate(hitParticle);
        particle.localScale = new Vector3(.23f, .23f, .23f);
        particle.position = pos;

        healthBar.fillAmount = CurrentHealth / MaxHealth;
        Destroy(particle.gameObject, 2);
    }


    void Update()
    {
        tracker++;

        if (tracker == 15 && !player)
        {
            canSeePlayer(20);

            tracker = 0;
        }


        if (player)
        {
            canvas.SetActive(true);
            gunTimer += Time.deltaTime;
            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

            if (!isPerformingSpecialAttack)
            {
                specialAttackTimer += Time.deltaTime;
            }
            else
            {
                specialAttackTimer = 0;
            }

            if (Vector3.Distance(head.position, player.position) < 7 && pathComplete() && !isPerformingSpecialAttack && specialAttackTimer > specialAttackCooldown)
            {
                agent.isStopped = true;
                gunTimer = 0;
                isPerformingSpecialAttack = true;
                anim.CrossFade("SpecialAttack", .1f);
                StartCoroutine(specialAttack());
            }

            if (Vector3.Distance(head.position, player.position) > 8 && init || Vector3.Distance(head.position, player.position) > 7 && pathComplete() && !isPerformingSpecialAttack)
            {
                agent.isStopped = false;

                anim.SetBool("Running", true);
                init = false;
                agent.SetDestination(player.position - (transform.forward * 2));
            }


            if (Vector3.Distance(head.position, player.position) < 5.5f && !isPerformingSpecialAttack && specialAttackTimer > specialAttackCooldown)
            {
                gunTimer = 0;
                agent.isStopped = true;
                isPerformingSpecialAttack = true;
                anim.CrossFade("SpecialAttack", .1f);
                StartCoroutine(specialAttack());
            }

            if (Vector3.Distance(head.position, player.position) < 6.9f && pathComplete() && !isPerformingSpecialAttack)
            {
                anim.SetBool("Running", true);

                agent.isStopped = false;
                if (whichSide)
                {
                    agent.SetDestination(player.position - transform.forward + (transform.right));

                }
                else
                {
                    agent.SetDestination(player.position - transform.forward + (-transform.right));
                }
            }


            if (!pathComplete() && !agent.isStopped)
            {
                if (gunTimer > shotTimer)
                {
                    gunTimer = 0;

                    whichSide = !whichSide;

                    if (whichSide)
                    {
                        Transform spawnedBullet = Instantiate(bullet);
                        spawnedBullet.GetComponent<Bullet>().maxDamage = bulletMaxDamage;
                        spawnedBullet.GetComponent<Bullet>().minDamage = bulletMinDamage;
                        Transform muzzleflash = Instantiate(muzzleFlash, rGun);
                        Destroy(muzzleflash.gameObject, .1f);

                        spawnedBullet.position = rGun.position;
                        spawnedBullet.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - rGun.position) * bulletSpeed);
                        Destroy(spawnedBullet.gameObject, 2);

                        anim.CrossFade("FireRight", .03f);
                    }
                    else
                    {
                        Transform spawnedBullet = Instantiate(bullet);
                        spawnedBullet.GetComponent<Bullet>().maxDamage = bulletMaxDamage;
                        spawnedBullet.GetComponent<Bullet>().minDamage = bulletMinDamage;
                        Transform muzzleflash = Instantiate(muzzleFlash, lGun);
                        Destroy(muzzleflash.gameObject, .1f);

                        spawnedBullet.position = lGun.position;
                        spawnedBullet.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - lGun.position) * bulletSpeed);
                        Destroy(spawnedBullet.gameObject, 2);

                        anim.CrossFade("FireLeft", .03f);

                    }
                }
            }

        }

    }


    IEnumerator specialAttack()
    {
        yield return new WaitForSeconds(.374f);



        Transform spawnedBullet = Instantiate(bullet);
        spawnedBullet.GetComponent<Bullet>().maxDamage = specialBulletMaxDamage;
        spawnedBullet.GetComponent<Bullet>().minDamage = specialBulletMinDamage;

        Transform muzzleflash = Instantiate(muzzleFlash, rGun);
        Destroy(muzzleflash.gameObject, .1f);
        spawnedBullet.localScale = new Vector3(.3f, .3f, .3f);

        spawnedBullet.position = rGun.position + transform.forward;
        spawnedBullet.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - rGun.position) * specialBulletSpeed);
        Destroy(spawnedBullet.gameObject, 2);


        yield return new WaitForSeconds(.474f);

        isPerformingSpecialAttack = false;
        agent.isStopped = false;

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
            if (Physics.Raycast(head.position, colliderHit.position - head.position, out hit, range, ~ignore))
            {
                if (hit.transform.name == "AIDetection")
                {
                    float angle = Vector3.Angle(colliderHit.position - transform.position, transform.forward);

                    if (angle <= 65)
                    {
                        this.player = player.transform;

                        Material[] oldMats = eye.materials;
                        oldMats[1] = eyeDetected;

                        eye.materials = oldMats;
                        return true;
                    }
                }
            }
        }

        return false;
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
}
