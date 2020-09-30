using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    public HealthSO health;
    [SerializeField] private bool isInvincible;

    [Header("Attack")]
    [SerializeField] private Vector2 attackOffset;
    [SerializeField] private Rigidbody2D Projectile;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private float attackSpeed = 1f;
    private float _attackTimer;
    private bool canAttack = true;


    [SerializeField] private float speed = 2;
    private float velocity;
    

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Light2D light;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CharacterController2D charController;
    private bool isJumping;
    private bool isCrouching;
    private bool isClimbing;
    [SerializeField]
    private float climbCD = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
           animator = GetComponent<Animator>();

        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();

       
    }

    private void Update()
    {
        velocity = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetKeyDown("t"))
            Hurt(1);

        if(Input.GetButtonDown("Jump"))
        {
            //Makes sure we can't keep jumping on ladders.
            if (climbCD > 0)
                return;

            if (isClimbing)
            {
                climbCD = 0.5f;
                isClimbing = false;
            }

            if (charController.Jump(isCrouching))
            {
                isJumping = true;
                animator.SetTrigger("Jump");
            }
        }
       
        if (Input.GetButton("Crouch"))
        {
            isCrouching = charController.CanCrouch();
            
        }

        if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
        }

        if (Input.GetButtonDown("Fire1")){
            Attack();
        }

        if (!canAttack)
        {
            //staminaSlider.value = _attackTimer;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                canAttack = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (climbCD > 0)
            climbCD -= Time.deltaTime;

        if (isClimbing)
        {
            charController.Climb(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.deltaTime);
            return;
        }

        charController.Move(velocity * Time.fixedDeltaTime, isCrouching, isClimbing);
        animator.SetFloat("Velocity", Mathf.Abs(velocity));
        if (rigidbody.velocity.y < -0.5f)
            animator.SetBool("IsFalling", true);
        else
            animator.SetBool("IsFalling", false);
    }

    private void Attack()
    {
        if (!canAttack)
            return;
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(Vector3.Angle(transform.position, new Vector3(mousepos.x, mousepos.y, 0)));

        Rigidbody2D _projectile = Instantiate(Projectile, GetAttackPoint(), Quaternion.LookRotation(mousepos));

        canAttack = false;
        _attackTimer = attackSpeed;
    }

    public Vector3 GetAttackPoint()
    {
        Vector3 attackPoint = transform.position;
        attackPoint.y += attackOffset.y;
        attackPoint.x += charController.GetDirection() * attackOffset.x;
        return attackPoint;
    }

    public void Hurt(int damage)
    {
        if (isInvincible)
            return;

        health.Hurt(damage);
        //GameManager.instance.healthbar.UpdateHealth();
        StartCoroutine("IFrames", 1f);
        animator.SetTrigger("Hurt");
       
        if (health.IsDead())
        {
            Debug.Log("<color=red> You died </color> ");
            //Create menu pop up, or at least fade screen to black
            GameManager.instance.Respawn();
        }
    }

    private IEnumerator IFrames(float iFrame)
    {
        isInvincible = true;
        renderer.color = new Color(220, 220, 220, 200);
        light.intensity = 0.25f;
        //light.color = new Color(255, 5, 5);
        yield return new WaitForSeconds(iFrame);
        renderer.color = new Color(255, 255, 255, 255);
        isInvincible = false;
        //light.color = new Color(255,174,175);
        light.intensity = 1f;
    }

    public bool Heal(int healing)
    {
        if (health.IsFull())
            return false;

        health.Heal(healing);
        return true;
    }

    public void OnLand()
    {
        animator.SetBool("IsFalling", false);
        isJumping = false;
    }

    public void OnCrouch()
    {
        if (isJumping || isClimbing)
            return;

        animator.SetBool("IsCrouching", isCrouching);
    }

    public void Climb(bool isClimbing)
    {
        if (climbCD > 0)
        {
            this.isClimbing = false;
            animator.SetBool("IsClimbing", false);
            return;
        }
        
        this.isClimbing = isClimbing;
        
        animator.SetBool("IsClimbing", isClimbing);
        

    }

    public void Teleport(Transform location)
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = location.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetAttackPoint(), 0.1f);
    }
}
