using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConroller : MonoBehaviour
{
    private Rigidbody2D characterRigidbody;
    private float horizontalInput;
    private bool jumpInput;
    [SerializeField]private float characterSpeed = 4.5f;
    [SerializeField] private float jumpForce = 5;
    public static Animator characterAnimator;
    [SerializeField] private int healthPoints = 5;
    private bool isAttacking;
    [SerializeField] private Transform attackHitBox;
    [SerializeField] private float attackRadius = 1;

    void Awake()
    {
        characterRigidbody = GetComponent<Rigidbody2D>();
        characterAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //characterRigidbody.AddForce(Vector2.up * jumpForce);
    }

    void Update()
    {
        Moviment();

        if(Input.GetButtonDown("Jump") && GroundSensor.isGrounded && !isAttacking)
       {
         Jump();
        }
      
       if(Input.GetButtonDown("Fire1") && GroundSensor.isGrounded && !isAttacking)
       {
         Attack();
       }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isAttacking)
        {
            if(horizontalInput == 0)
            {
                characterRigidbody.velocity = new Vector2(0, characterRigidbody.velocity.y);
            }
        }
        else 
        {
            characterRigidbody.velocity = new Vector2(horizontalInput  * characterSpeed, characterRigidbody.velocity.y);
        }
        
    }


    void Moviment()
    {
    
     horizontalInput = Input.GetAxis("Horizontal");
        

       if(horizontalInput < 0)
       {

        if(!isAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); //sirve para girar al personaje de una manera compleja
        }
            
            characterAnimator.SetBool("IsRunning", true);
       }

       else if(horizontalInput > 0)
       {

        if(!isAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        characterAnimator.SetBool("IsRunning", true);
       }

       else
       {
        characterAnimator.SetBool("IsRunning", false);
       }
    }

    void Attack()
    {
        if(horizontalInput == 0)
        {
         characterAnimator.SetTrigger("Attack"); 
        }

        else
        {
            characterAnimator.SetTrigger("RunAttack");
           StartCoroutine(AttackAnimation()); 
        }
        
         
    }

    IEnumerator AttackAnimation()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.15f);

        Collider2D[] collider = Physics2D.OverlapCircleAll(attackHitBox.position, attackRadius); 
        foreach(Collider2D enemy in collider)
        {
            if(enemy.gameObject.CompareTag("Mimico"))
            {
                //Destroy(enemy.gameObject);
                Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
                enemyRigidbody.AddForce(transform.right + transform.up * 2, ForceMode2D.Impulse);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.TakeDamage();
            }
        }

        yield return new WaitForSeconds(0.16f);

        isAttacking = false;
    }

    void Jump()
    {
        
     characterRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); 
     characterAnimator.SetBool("IsJumping", true);
        
    }

    void TakeDamage()
        {
            healthPoints--;
                
            if(healthPoints <= 0)
            {
                Die();
            }

            else
            {
                characterAnimator.SetTrigger("IsHurt");
            }
        }
    
    void Die()
    {
        characterAnimator.SetTrigger("IsDeath");
        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            //characterAnimator.SetTrigger("IsHurt");
            //Destroy(gameObject, 1f);
            TakeDamage();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackHitBox.position, attackRadius);
    }
}   