﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Parameters")]
    public string Name;
    public float stepResolution;
    public Vector2 velocity = new Vector2();
    public new CircleCollider2D collider;
    public float radiusDelta;
    public float effectValue;
    public bool isDamage;
    public List<Sprite> sprites = new List<Sprite>();


    private const bool collideWithProjectiles = false; // To turn off and on collision between projectiles that use this script


    private float step = 0;
    private float stepSize = 0f;
    private bool hasHit = false;

    private Vector2 currPos = new Vector2(0, 0);
    private Vector2 nextPos = new Vector2();
    private Vector2 origin = new Vector2();

    private SpriteRenderer spriteRen;

    private float minDistance = float.PositiveInfinity;
    private int minDistanceIndex = 0;
    private int i = 0;
    private float upwardsVelocity;
    private float sideVelocity;
    private float pullVelocity;
    private Transform parent;
    private XPLevelController xpControl;
    private bool hasValues;

    public void SetValues(float upwardsVelocity, float sideVelocity, float pullVelocity, Transform parent)
    {
        this.upwardsVelocity = upwardsVelocity;
        this.sideVelocity = sideVelocity;
        this.pullVelocity = pullVelocity;
        this.parent = parent;
        this.hasValues = true;
    }

    public void SetValues(Vector2 velocity)
    {
        this.velocity = velocity;
    }


    public void SetPlayerXpController(XPLevelController xpControl)
    {
        this.xpControl = xpControl;
    }


    void CalculateVelocity()
    {
        if (hasValues && parent != null)
        {
            velocity = (transform.position - parent.position).normalized * upwardsVelocity + 
                Vector3.Cross((parent.transform.position - transform.position), transform.forward).normalized * sideVelocity +
                (transform.position - parent.position).normalized * -pullVelocity;
        } 
    }


    private void Start()
    {
        spriteRen = GetComponent<SpriteRenderer>();
        if (spriteRen && sprites.Count > 0)
        {
            int index = Random.Range(0, sprites.Count);
            spriteRen.sprite = sprites[index];
        }
    }


    void Awake()
    {
        currPos = transform.position;
        this.gameObject.SetActive(true);
    }


    void HitEvent(Collider2D hit)
    {
        if(hit.CompareTag("Damagable") || hit.CompareTag("Player"))
        {
            HealthManager healthMangCollider = hit.GetComponent<HealthManager>();
            if (isDamage)
            {
                healthMangCollider.Damage(effectValue);
            }
            else
            {
                healthMangCollider.Heal(effectValue);
            }
            KillSelf();

        } else if ( hit.CompareTag("Projectiles"))
        {
            if (collideWithProjectiles) {
                Destroy(hit.gameObject);
                KillSelf();
            }
        } else
        {

            KillSelf();

        }
    }


    public void KillSelf()
    {
        Destroy(this.gameObject);
    }


    void MinDistanceHit(Collider2D[] hits)
    {
        minDistance = float.PositiveInfinity;
        minDistanceIndex = 0;

        for (i=0; i< hits.Length; i++)
        {

            if (hits[i] != collider)
            {

                origin = new Vector2(transform.position.x, transform.position.y);
                if(Vector2.Distance(hits[i].ClosestPoint(transform.position), origin) < minDistance)
                {
                    minDistanceIndex = i;
                }
            }
        }
    }


    void FixedUpdate()
    {
        stepSize = 1f / stepResolution;
        for (step=0; step<1f; step += stepSize)
        {
            if (!hasHit)
            {
                if (hasValues)
                {
                    CalculateVelocity();
                    hasValues = false;
                }

                nextPos = currPos + velocity * stepSize * Time.deltaTime;
                Collider2D[] results = Physics2D.OverlapCircleAll(currPos, collider.radius);

                if (results.Length > 1)
                {
                    hasHit = true;
                    MinDistanceHit(results);
                    HitEvent(results[minDistanceIndex]);
                }

                currPos = nextPos;
                transform.position = currPos;
            }
        }
    }
}
