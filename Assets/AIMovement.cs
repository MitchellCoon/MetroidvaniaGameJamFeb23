using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Collider2D parentCollider;
    public float startingY; 
   public Rigidbody2D m_Rigidbody;
    public float m_Speed = 5f;
    public float currentDirection = 1f; 
    public float platformEdgeDistance = 2;
    void Start()

    {
       parentCollider =  transform.parent.GetComponent<Collider2D>();  
       m_Rigidbody = GetComponent<Rigidbody2D>();
  
       // parentCollider.bounds.extents.x; 
    }

    // Update is called once per frame
 void FixedUpdate()
    {
        if( parentCollider.bounds.extents.x + parentCollider.bounds.center.x - platformEdgeDistance < transform.position.x)
        {
            currentDirection = -1f; 
        }
        else if(parentCollider.bounds.center.x - parentCollider.bounds.extents.x + platformEdgeDistance > transform.position.x)
        {
            currentDirection = 1f; 
        }
        //Store user input as a movement vector
        Vector3 m_Input = new Vector2(currentDirection, 0); 

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * m_Speed);
    }
}
