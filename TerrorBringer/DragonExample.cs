using UnityEngine;
using System.Collections;
using Gamekit3D;



// figure out what the dragon does when he gets to the hiding spot (?)
// when dragon fires it doesnt work maybe try to work on that

// in main: change ellen staff radius to 1
// 

public class DragonExample : MonoBehaviour{

    //public GameObject flame;
    private Animator animator;
    public GameObject Ellan;
    private Transform t;
    private UnityEngine.AI.NavMeshAgent agent;
    private Damageable damageable;
    private AnimatorStateInfo stateInfo;
    public MeleeWeapon weapon;
    private GameObject fireDeath;


    public Renderer  characterRenderer; // for getting hi animation
    private Color hitColor = Color.blue;
    public float flashDuration = 3f;


    AnimatorClipInfo[] clipInfo;
    private string[] animatios = {"Basic Attack", "Flame Attack", "Claw Attack"};

    private bool lowLife = false;
    private bool isFleeing = false;
    private int health;
    private Vector3 original;
    private float distance;
    private bool isChasing = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private float walkSpeed = 2f;
    private float runSpeed = 10f;
    private float rotationSpeed = 5f;
    private string[] chasingModes={"isFlying","isRunning"};
    private string[] attacks = {"basicAttack","flameAttack","clawAttack"};
    private string chase;
    private string currentAttack = "basicAttack";
    private enum State{
        Idle,
        Walking,
        Chasing,
        Attacking,
        Fleeing,
        Dead
    };
    private State state;


    public void Start() {
        //flame.SetActive(false);

        weapon.SetOwner(gameObject); 
        original = transform.position;
        t = Ellan.GetComponent<Transform>();
        distance = Vector3.Distance(transform.position,t.position);
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        damageable = GetComponent<Damageable>();
        damageable.OnReceiveDamage.AddListener(OnDragonHit);
        damageable.OnDeath.AddListener(OnDeath);

        fireDeath = GameObject.FindWithTag("Fire");
        fireDeath.SetActive(false);
        
        state = State.Idle;
    }

    public void Update(){
    health = damageable.currentHitPoints;
   
        for (int i = 0; i < attacks.Length; i++) {
        animator.ResetTrigger(attacks[i]);
    }
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        distance = Vector3.Distance(transform.position, t.position);
        
        if(lowLife && state == State.Idle){
            animator.SetFloat("distance", 1000f);
        }else{
            animator.SetFloat("distance", distance);
        }
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    // Check if any attack animation is currently playing and not finished
        isAttacking = (
            (stateInfo.IsName(animatios[0]) || 
            stateInfo.IsName(animatios[1]) || 
            stateInfo.IsName(animatios[2])||
            stateInfo.IsName("Take Off")) 
            && stateInfo.normalizedTime < 0.99f
        );
        if(!isAttacking && state == State.Attacking){
            //Debug.Log("Stop attack");
            //weapon.EndAttack();
        }

        if(distance >= 50){
            state = State.Idle;
        }else if(distance >= 25){
            state = State.Walking;
            if(isChasing)isChasing = false;
            
        }else if(distance >= 5){
            state = State.Chasing;
            if(!isChasing){
                if(distance <= 15){
                    chase = "isRunning";
                }else{
                int ind = Random.Range(0, chasingModes.Length);
                chase = chasingModes[ind];
                }
                isChasing = true;
                animator.SetBool(chase,isChasing);
            }
        }else{
            if(isChasing)isChasing = false;
            
            state = State.Attacking;
            
        }

            
                

        if(!isChasing){
            foreach (string mode in chasingModes){
                animator.SetBool(mode, false);
            }
            //animator.SetBool(chase,isChasing);
            agent.ResetPath(); // Immediately stops movement
        }
        if(health == 1 && !isFleeing){
            state = State.Fleeing;
        }else if(isFleeing && distance > 5){
            //Debug.Log("Low on life relaxing!!");
            lowLife = true;
            state = State.Idle;
        }else if(isDead){
            state = State.Dead;
        }

        switch(state){
            case State.Idle: idle(); break;
            case State.Walking: move(walkSpeed); break;
            case State.Chasing: move(runSpeed); break;
            case State.Attacking: attack();break;
            case State.Fleeing: flee(runSpeed); break;
            case State.Dead:  break;
        }

    }

    public void idle(){
       if(lowLife){
            //Debug.Log("Calling low on life!!1");
            animator.SetBool("lowLife", true);
            rotate(t.position);
       }
    }

    public void attack(){
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        //Debug.Log(isAttacking);

        if (!isAttacking) {
            int ind = Random.Range(0, attacks.Length);
            currentAttack = attacks[ind];
            rotate(t.position);
            isAttacking = true;
            animator.SetTrigger(currentAttack); // should be currentAttack
            if(currentAttack == "flameAttack"){
                weapon.BeginAttack(true);
                //StartCoroutine(turnOnFire(3f));
            }else{
                weapon.BeginAttack(false); // should be fasle
            }
        }
        if (stateInfo.normalizedTime >= 1f) {
        weapon.EndAttack(); // Call a method to end the attack
    }
    }
    public void move(float speed){
        rotate(t.position);
        
        
        if(!isAttacking){
            agent.speed = speed;
            agent.SetDestination(t.position);
        }
    }

    public void flee(float speed){
        rotate(original);
        if(!isAttacking){    
            agent.speed = speed;
            agent.SetDestination(original);
        }
        //Debug.Log("Distance from fleeing spot- " + Vector3.Distance(original,transform.position));
        if(Vector3.Distance(original,transform.position) < 6f){
                state = State.Idle;
                isFleeing = true;
                animator.SetFloat("distance", 100f);  
                animator.SetBool("isRunning", false);    
                animator.SetBool("lowLife", true);
        }
    }
    
    
    public void rotate(Vector3 target){
       
        //Vector3 directionToPlayer = t.position - transform.position;
        Vector3 directionToPlayer = target - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer); // directionToPlayer

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void OnDragonHit(){
        //dragonDamageable.currentHitPoints

        //Debug.Log("Got hit!!!");
        animator.SetTrigger("gotHit");
        StartCoroutine(FlashRed());

        
    }

    
    private IEnumerator FlashRed(){
        Color originalColor = characterRenderer.material.color;
        characterRenderer.material.color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        characterRenderer.material.color = originalColor;
    }
    
    public void OnDeath(){
        for(int i = 0; i < attacks.Length; i++){
            animator.ResetTrigger(attacks[i]);
       }
        isDead = true;
        animator.SetTrigger("isDead");
        weapon.EndAttack();
        fireDeath.transform.position = transform.position;
        fireDeath.SetActive(true);
        Destroy(gameObject);
        Destroy(fireDeath, 4f);
    }

/**
    private IEnumerator turnOnFire(float duration){
        
        //flame.SetActive(true);
        //Debug.Log("YESFlamessssssssssssss for " + duration);

        //yield return new WaitForSeconds(duration);
        flame.SetActive(false);

    }
*/
}