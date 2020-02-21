﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpookyBalloonScript : MonoBehaviour
{
    [Header("Designer Friendly Options")]
    [SerializeField] bool fixedWanderer;
    [SerializeField] Transform[] fixedWanderSpots;
    [SerializeField] float freeWanderRadius, playerDetectionRadius, maxSpeed, idleTime, turningSpeed = 0.08f;

    [SerializeField] AudioClip hoveringSound, chasingSound, explosionSound;

    #region Internal stuff
    private static UnityStandardAssets.Characters.FirstPerson.FirstPersonController player; //static ref to player - all hail nested namespaces
    AudioSource aud;
    private float idleTimer, chaseTimer;
    private Vector3 wanderSpot;

    private Vector3 startPos;
    public int fixedWandererIterator = 1; //determines the current node the fixed patroller is supposed to visit
    bool forwardWanderOrder = true; //for fixed patrolling logic

    private enum BalloonState { IDLE, WANDERING, CHASING };
    private BalloonState state = BalloonState.IDLE;
    #endregion

    private void OnEnable()
    {
        if (player == null)
            player = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

        aud = GetComponent<AudioSource>();
        idleTimer = idleTime;
        startPos = transform.position;
    }

    private void Update()
    {
        HandleState();
    }

    void HandleState()
    {
        switch (state)
        {
            case BalloonState.IDLE:
                {
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0.0f)
                        ChangeState(BalloonState.WANDERING);

                    if (IsPlayerNear())
                        ChangeState(BalloonState.CHASING);
                    break;
                }
            case BalloonState.WANDERING:
                {
                    Vector3 dir = (wanderSpot - transform.position).normalized;
                    transform.position += dir * (maxSpeed / 3.0f) * Time.deltaTime;

                    BalloonLookAt(wanderSpot);

                    if (ReachedWanderSpot())
                        ChangeState(BalloonState.IDLE);

                    if (IsPlayerNear())
                        ChangeState(BalloonState.CHASING);
                    break;
                }
            case BalloonState.CHASING:
                {
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    float chaseSpeed = Mathf.Lerp(0.1f, maxSpeed, Mathf.Clamp(chaseTimer / maxSpeed, 0, 1)); //the balloon accelerates base on how much time passed since the chase began

                    transform.position += dir * chaseSpeed * Time.deltaTime;
                    chaseTimer += Time.deltaTime;

                    BalloonLookAt(player.transform);

                    if (!IsPlayerNear()) //sends badoom to its first spawnpoint
                        ResetActivity();
                    break;
                }
        }
    }

    void ChangeState(BalloonState state)
    {
        this.state = state;

        switch (state)
        {
            case BalloonState.IDLE:
                {
                    idleTimer = idleTime;
                    break;
                }
            case BalloonState.WANDERING:
                {
                    PickWanderSpot();
                    break;
                }
            case BalloonState.CHASING:
                {
                    //TODO: play "chasing" clip
                    chaseTimer = 0.0f;
                    break;
                }
        }
    }

    private bool IsPlayerNear()
    {
        float dist = (player.transform.position - transform.position).sqrMagnitude;
        return (dist < playerDetectionRadius * playerDetectionRadius);
    }

    private void PickWanderSpot()
    {
        Vector3 spot;
        if (fixedWanderer)
        {
            spot = (fixedWandererIterator == 0) ? startPos : fixedWanderSpots[fixedWandererIterator].position;
            fixedWandererIterator += forwardWanderOrder ? 1 : -1;

            if (fixedWandererIterator > fixedWanderSpots.Length - 1)
            {
                fixedWandererIterator = fixedWanderSpots.Length - 1;
                forwardWanderOrder = !forwardWanderOrder;
            }
            else if (fixedWandererIterator < 0)
            {
                fixedWandererIterator = 1;
                forwardWanderOrder = !forwardWanderOrder;
            }

            wanderSpot = spot;
        }
        else
        {
            spot = startPos + (Random.insideUnitSphere * freeWanderRadius);

            wanderSpot.x = spot.x;
            wanderSpot.y = transform.position.y;
            wanderSpot.z = spot.y;
        }
    }

    private bool ReachedWanderSpot()
    {
        float f = Mathf.Floor(transform.position.x);
        float g = Mathf.Floor(transform.position.y);
        float h = Mathf.Floor(transform.position.z);

        float a = Mathf.Floor(wanderSpot.x);
        float b = Mathf.Floor(wanderSpot.y);
        float c = Mathf.Floor(wanderSpot.z);

        return (f == a) && (g == b) && (h == c);
    }

    private void BalloonLookAt(Transform target)
    {
        Vector3 prev = transform.rotation.eulerAngles;
        transform.LookAt(target);
        Vector3 fut = transform.localRotation.eulerAngles;

        Vector3 v = Vector3.Lerp(prev, fut, turningSpeed);
        transform.rotation = Quaternion.Euler(prev.x, v.y, prev.z);
    }

    private void BalloonLookAt(Vector3 target)
    {
        Vector3 prev = transform.rotation.eulerAngles;
        transform.LookAt(target);
        Vector3 fut = transform.localRotation.eulerAngles;

        Vector3 v = Vector3.Lerp(prev, fut, turningSpeed);
        transform.rotation = Quaternion.Euler(prev.x, v.y, prev.z);
    }

    private void ResetActivity() //back to square one and in wandering state
    {
        ChangeState(BalloonState.WANDERING);
        wanderSpot = startPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.tag)
        {
            case "BalloonStopper":
                {
                    ResetActivity();
                    print("stopped");
                }
                break;

            case "Player":
                {
                    //explode on player contact
                    //this means screen fx, sound, maybe camera shake, maybe slowed speed
                    print("player got badoomed");
                }
                break;
        }
    }
}