using System.Collections;
using UnityEngine;


public enum MovementType{
    Grounded, 
    Flying
}
public class AIMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Collider2D parentCollider;
    public float startingY;
    public Rigidbody2D m_Rigidbody;
    public float horizontalSpeed = 5f;
    public float verticalSpeed = 5f;
    public float currentDirection = 1f;
    public float platformEdgeDistance = 2;
    public Enemy enemy;
    public bool randomizeDirection = true;
    public float randomizeDirectionTime = 1f;
    public MovementType movementType;
    public float verticleSearchRadius = 3f; 
    public float currentDirectionY ; 
    public float verticleCheckTime = 0.1f;

    void Start()

    {
        startingY = transform.position.y; 
        parentCollider = transform.parent.GetComponent<Collider2D>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        enemy = GetComponentInParent<Enemy>();

        if (randomizeDirection)
        {
            StartCoroutine(RandomlyChangeDirection());
        }

        if( movementType == MovementType.Flying){
           
           StartCoroutine(CheckForVerticalMovement());
            //VerticalMovement();
        }
    }
    IEnumerator CheckForVerticalMovement()
    {
        yield return new WaitForSeconds(verticleCheckTime);
        VerticalMovement();
        StartCoroutine(CheckForVerticalMovement());
    }

    IEnumerator RandomlyChangeDirection()
    {
        if (Random.Range(0, 100) > 95)
        {
            currentDirection *= -1;
        }
        yield return new WaitForSeconds(randomizeDirectionTime);
        StartCoroutine(RandomlyChangeDirection());
    }
    // Update is called once per frame
    
    void FixedUpdate()
    {
        HorizontalMovement();

    }

    private void VerticalMovement() {

       var  hitColliders = Physics2D.OverlapCircleAll(transform.position,verticleSearchRadius); 
       bool foundPlayer = false;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Player")
            {
                foundPlayer = true;
                 
                 if (hitCollider.bounds.center.y > startingY)
                {
                    canGoHigher();
                }
                else if (hitCollider.bounds.center.y < startingY)
                {
                    currentDirectionY = -1f;
                }
            }
        }
        if (!foundPlayer)
        {
            canGoHigher(); 
        }
    }

    private void canGoHigher()
    {
        if (transform.position.y > startingY)
        {
            currentDirectionY = 0f;
        }
        else
        {
            currentDirectionY = 1f;
        }
    }

    private void HorizontalMovement()
    {
        if (parentCollider.bounds.extents.x + parentCollider.bounds.center.x - platformEdgeDistance < transform.position.x)
        {
            currentDirection = -1f;
        }
        else if (parentCollider.bounds.center.x - parentCollider.bounds.extents.x + platformEdgeDistance > transform.position.x)
        {
            currentDirection = 1f;
        }
        //Store user input as a movement vector

        Vector3 m_Input = new Vector2(0,0) ;
        if( movementType == MovementType.Flying){

            m_Input = new Vector2(currentDirection, currentDirectionY);
        }
        else if( movementType == MovementType.Grounded){
        m_Input = new Vector2(currentDirection, 0);
        }
        m_Input.x = m_Input.x * horizontalSpeed;
        m_Input.y = m_Input.y * verticalSpeed;
        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        //
        //For NPCs , you should always move, and allow for no enemy component on gameobject
        if (enemy == null)
        {
            m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime );

        }
        else if (!enemy.inKnockback)
        {
            m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime );
        }
    }
}
