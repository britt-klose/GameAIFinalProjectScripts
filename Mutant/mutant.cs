// Brittany Klose
// Game AI Mutant Script
// Final Project 5/4/2024

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D;
using System;

public class mutantAI : MonoBehaviour
{
    public Transform startLocation;
    public Transform hideLocation; // Location where Mutant will hide if needed
    public float moveSpeed = 5f;
    public MeleeWeapon weapon;
    public float rotationSpeed = 5f;
    public float fleeDistance = 20f; // Distance from player where mutant will flee
    public float chaseDistance = 12f; // Distance from player where mutant will enter chase state
    public float RoarDistance = 18f; // Distance from player where mutant will Roar at Ellen
    public float attackDistance = 2f;// Distance from player where mutant will begin attacking player
    public float stopRadius= 1f; // Stop flee movement when Mutant is this close from hiding location  
    public float stuckThreshold = 0.5f; // Threshold to consider the AI stuck
    public float stuckTime = 2f; // Time before resetting if the AI is stuck
    public float stopDistance= 1f; // Distance from player where mutant will stop chasing Ellen and return to idle
    public float distance;
    private int attackChoice;
    public Vector3 destPos;
    public AudioSource roarAudio; 
    public AudioSource punchAudio;
    public AudioSource swipeAudio;
    public AudioSource jumpAudio;

    // Health Damage items if time for death
    public int damage = 10;
    private int health= 30;
    private Damageable damageable;
    private bool isDead = false;

    private NavMeshAgent agent;
    private Animator animator; // Reference to Animator component
    private GameObject ellenPlayer; // Reference to Player 'Ellen'
    private Transform ellenTransform; // Ref to Ellen's Animator
    private float timeStuck = 0f; // Timer for stuck detection
    private Vector3 lastPosition;
    private bool isFirstFrame = true; // Flag to check if it's the first frame


    // FSM 
    private enum MutantState
    {
        Idle,
        Chase,
        Attack, // Combat logic and attacking other NPCs extra credit
        Roar,
        FleetoHide, // Flee to safe spot and hide in idle state
        FleetoHide2,
        Dead 
    }

    private MutantState currentState; // Present state of Mutant
   

    
    private void Start()
    {   weapon.SetOwner(gameObject); 
        roarAudio = GetComponent<AudioSource>();
        punchAudio = GetComponent<AudioSource>();
        swipeAudio= GetComponent<AudioSource>();
        jumpAudio= GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = moveSpeed;
        damageable = GetComponent<Damageable>();
        damageable.OnReceiveDamage.AddListener(onMutantHit);
        damageable.OnDeath.AddListener(OnDeath);
        lastPosition = transform.position;
        animator = GetComponent<Animator>();
        ellenPlayer = GameObject.FindGameObjectWithTag("Player"); 
        ellenTransform = ellenPlayer.transform;
        currentState= MutantState.Idle;

       
    }

    // Update is called once per frame
    private void Update()
        {
            health = damageable.currentHitPoints;
           distance = Vector3.Distance(transform.position, ellenTransform.position);

            switch(currentState)
            {
                    case MutantState.Idle:
                        UpdateIdleState();
                        break;

                    case MutantState.Roar:
                        UpdateRoarState();
                        break; 

                    case MutantState.Chase:
                        UpdateChaseState();
                        break;   
                        
                    case MutantState.Attack:
                        UpdateAttackState();
                        break;  
                    
                    case MutantState.FleetoHide:
                        UpdateFleeToHideState();
                        break;
                    case MutantState.FleetoHide2:
                        UpdateFleeToHideState2();
                        break;
                        
                    case MutantState.Dead:
                        OnDeath();
                        break;  
                    
                }

            if (health <= 0){
                isDead = true;
                currentState = MutantState.Dead;
            }
            // Update animator parameters (Speed and IsChasing)
            UpdateAnimation();
        
        }

    
    private void UpdateIdleState(){
            animator.SetBool("isIdle", true);
            animator.SetBool("isChasing", false);
            animator.SetBool("isRoaring", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isPatrolling", false);
            
        if (distance <= RoarDistance && distance > chaseDistance)
        {
            currentState = MutantState.Roar;
            print("Mutant Roaring");
        }
        
    }


     private void UpdateRoarState(){
        animator.SetBool("isRoaring", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isIdle", false);
        roarAudio.Play();
        if (distance <= chaseDistance && distance >attackDistance)
        {
            Debug.Log("Transitioning to Chase state");
            currentState = MutantState.Chase;
        }
        else if (distance > RoarDistance)
        {
            Debug.Log("Transitioning back to idle state");
            currentState = MutantState.Idle;
        }

    }

    private void UpdateChaseState(){
        if (ellenPlayer!=null){
            agent.SetDestination(ellenTransform.position);
            Debug.Log("Entered chase state");
            animator.SetBool("isChasing", true);
            animator.SetBool("isRoaring", false);
            animator.SetBool("attackJump", false);
            animator.SetBool("attackPunch", false);
            animator.SetBool("attackSwipe", false);
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isIdle", false);
        }
        if(distance <= attackDistance && agent.hasPath)
        {
            Debug.Log("Transitioning to attack state");
            currentState = MutantState.Attack;
        }
        else if (distance > RoarDistance && agent.hasPath)
        {
            Debug.Log("Transitioning back to idle state");
            currentState = MutantState.Idle;
        }
    }


    private void UpdateAttackState(){
        animator.SetBool("isRoaring", false);
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isIdle", false);
        System.Random randomNumber = new System.Random();
        attackChoice = randomNumber.Next(0,3);
        if (attackChoice == 0)
        {
            animator.SetBool("attackPunch", true);
            animator.SetBool("attackSwipe", false);
            animator.SetBool("attackJump", false);
            Debug.Log("Random attack: 0");
            punchAudio.Play();
        }
        else if (attackChoice == 1)
        {
            animator.SetBool("attackSwipe", true);
            animator.SetBool("attackJump", false);
            animator.SetBool("attackPunch", false);
            Debug.Log("Random attack: 1");
            swipeAudio.Play();
        }
        else if (attackChoice == 2)
        {
            animator.SetBool("attackJump", true);
            animator.SetBool("attackPunch", false);
            animator.SetBool("attackSwipe", false);
            Debug.Log("Random attack: 2");
            jumpAudio.Play();
        } else 
            {
                Debug.Log("No attack method was chosen");
            }
        

        if(distance > attackDistance && distance <= RoarDistance)
        {
            Debug.Log("Transitioning back to chasing state");
            currentState = MutantState.Chase;
        }
        else if (distance >= fleeDistance)
        {
            Debug.Log("Flee to safety then idle");
            animator.SetBool("isFleeing", true);
            if (Vector3.Distance(transform.position, hideLocation.position) > Vector3.Distance(transform.position, startLocation.position))
            {
               currentState = MutantState.FleetoHide; 
            }
            else
            {
                currentState = MutantState.FleetoHide2;
            }
            
        }
    }

    private void onMutantHit()
    { 
        Debug.Log("Got hit!!!");
        animator.SetBool("isFleeing", true);
    }

    private void OnDeath(){
        isDead = true;
        animator.SetTrigger("DeathTrigger");
        weapon.EndAttack();
    }

    private IEnumerator UpdateFleeToHideState()
    {
         animator.SetBool("IsFleeing", true); // Trigger fleeing animation
         while (Vector3.Distance(transform.position, hideLocation.position) > stopRadius)
         {
            Vector3 direction = (hideLocation.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
             // Move towards the target location
            transform.position = Vector3.MoveTowards(transform.position, hideLocation.position, moveSpeed * Time.deltaTime);

            yield return null;
         }

        // Once close enough, stop at the target location
        transform.position = hideLocation.position;
        animator.SetBool("IsFleeing", false); // Stop fleeing animation
        currentState = MutantState.Idle; // Transition back to idle state
        animator.SetBool("isIdle", true); // Set to idle state
    }

    private IEnumerator UpdateFleeToHideState2()
    {
        animator.SetBool("IsFleeing", true); // Trigger fleeing animation
         while (Vector3.Distance(transform.position, startLocation.position) > stopRadius)
         {
            Vector3 direction = (startLocation.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
             // Move towards the target location
            transform.position = Vector3.MoveTowards(transform.position, startLocation.position, moveSpeed * Time.deltaTime);

            yield return null;
         }

           // Once close enough, stop at the target location
        transform.position = startLocation.position;
        animator.SetBool("IsFleeing", false); // Stop fleeing animation
        currentState = MutantState.Idle; // Transition back to idle state
        animator.SetBool("isIdle", true); // Set to idle state
    }

// Check if mutant is stuck
    private void CheckIfStuck()
        {
            // Skip stuck detection on the first frame
            if (isFirstFrame)
            {
                isFirstFrame = false; // Mark that the first frame has passed
                return;
            }

            // Check if Mutant hasn't moved significantly in the last frame
            if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
            {
                timeStuck += Time.deltaTime; // Increment stuck time
            }
            else
            {
                timeStuck = 0f; // Reset stuck time if Mutant moved
            }

            lastPosition = transform.position;

            // If Mutant has been stuck for too long, reset its path
            if (timeStuck >= stuckTime)
            {
                HandleStuckState();
            }
        }

        // Handle the stuck state by resetting the AI's path
        private void HandleStuckState()
        {
            Debug.Log("Mutant is stuck. Resetting path...");
            agent.ResetPath();  // Reset the NavMeshAgent path to force it to find a new one
            timeStuck = 0f; // Reset the stuck timer
        }

     // Update animation parameters
        private void UpdateAnimation()
        {
            // Get the current speed of Mutant using the NavMeshAgent's velocity
            float speed = agent.velocity.magnitude;

            // Update the Speed parameter in the animator
            animator.SetFloat("Speed", speed);

            // Update the IsChasing boolean to control the chase animation
            animator.SetBool("isChasing", ellenTransform != null);
        }

        
}