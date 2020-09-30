using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public enum EnemyState { Patrolling, Following, Chasing, Attacking, Waiting}

public class EnemyAI : MonoBehaviour
{

    public Transform target;

    public EnemyState state = EnemyState.Waiting;
    public delegate void OnStateChange();
    public OnStateChange AIStateChange;

    public float view = 3f;
    public LayerMask hostileMask;

    public float speed = 2f;

    [SerializeField] GameObject GFX;
    [SerializeField] bool canFly = false;
    public bool AIEnabled = true;

    private Vector3 m_Velocity = Vector3.zero;

    Rigidbody2D rigid;
    [SerializeField] private Animator animator;

    [Header("Behaviour")]
    [SerializeField, Tooltip("Will the AI chase hostiles within range?")]
    private bool isAggressive = false;
    [SerializeField, Tooltip("A speed multiplier when chasing")]
    private float chaseSpeedModifier = 1;
    
    [Header("Patrol")]
    public int currentWaypoint = 0;
    public bool reachedEndOfPath = false;
    public float nextWaypointDistance = 3f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolDelay = 0.1f;
    private bool updatingPatrol = false;

    void Start()
    {
        rigid = GFX.GetComponent<Rigidbody2D>();

        FlightCheck();       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (patrolPoints == null || !AIEnabled)
            return;

        if (state != EnemyState.Chasing)
        {
            Collider2D other = Physics2D.OverlapCircle(GFX.transform.position, view, hostileMask);
            if (other != null)
            {
                Debug.Log("Found hostile " + other.gameObject.name);
                target = other.gameObject.transform;
                if (isAggressive)
                    SwitchState(EnemyState.Chasing);
            }
        }

        if(state == EnemyState.Chasing)
        {
            Collider2D other = Physics2D.OverlapCircle(GFX.transform.position, view + 1, hostileMask);
            if(other == null)
            {
                Debug.Log("Hostile escaped going back to patrol");
                target = null;
                SwitchState(EnemyState.Patrolling);
            }
        }

        Move();

        float distance = Vector2.Distance(rigid.position, patrolPoints[currentWaypoint].position);

        if(distance < nextWaypointDistance)
        {
            if (state == EnemyState.Patrolling)
            {
                StartCoroutine("NextPatrol");
            }
        }
    }

    private void SwitchState(EnemyState state)
    {
        this.state = state;
        AIStateChange();
    }

    /// <summary>
    /// Used to find the next index in the patrol. Automatically pingpongs the values,
    /// so the character will backtrack when reaching the last waypoint
    /// </summary>
    /// <returns></returns>
    private void FindNextPoint()
    {
        if (currentWaypoint >= patrolPoints.Length - 1 && !reachedEndOfPath)
        {
            reachedEndOfPath = true;
        }


        if (currentWaypoint <= 0 && reachedEndOfPath)
        {
            reachedEndOfPath = false;
        }

        if (!reachedEndOfPath)
        {
            currentWaypoint++;
        }
        else
        {
            currentWaypoint--;
        }
    }

    private void Move()
    {
        if (!AIEnabled)
            return;
            

        Vector2 waypoint = ((Vector2)patrolPoints[currentWaypoint].position - rigid.position).normalized;
        
        if(state == EnemyState.Chasing)
            waypoint = ((Vector2)target.position - rigid.position).normalized * chaseSpeedModifier;
        
        if (!canFly)
        {
            waypoint.y = 0;
            //waypoint.Normalize();
        }

        Vector3 force = waypoint * speed * 100f * Time.deltaTime;
        // And then smoothing it out and applying it to the character
        rigid.velocity = Vector3.SmoothDamp(rigid.velocity, force, ref m_Velocity, 0.05f);

        animator.SetFloat("Speed", force.x);

        if (force.x >= 0.01f)
        {
            GFX.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            GFX.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private IEnumerator NextPatrol()
    {
        if (!updatingPatrol)
        {

            updatingPatrol = true;

            SwitchState(EnemyState.Waiting);
            FindNextPoint();

            yield return new WaitForSeconds(patrolDelay);
            SwitchState(EnemyState.Patrolling);

           // target = patrolPoints[currentWaypoint];
            updatingPatrol = false;
        }
    }

    private void FlightCheck()
    {
        if (canFly)
        {
            rigid.gravityScale = 0;
        }
        else
        {
            rigid.gravityScale = 1;
        }
    }

    public void Stop()
    {
        AIEnabled = false;
        StopAllCoroutines();
        //rigid.velocity = Vector3.zero;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GFX.transform.position, nextWaypointDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GFX.transform.position, view);

        Gizmos.color = Color.yellow;
        foreach (Transform wp in patrolPoints)
        {
            Gizmos.DrawWireSphere(wp.transform.position, 0.2f);
        }
    }
}
