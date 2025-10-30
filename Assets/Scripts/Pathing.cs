using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class Pathing: MonoBehaviour
{
    public float speed;
    public int MaxNum;
    public int pointIndex;
    public Transform movepoint;
    public Transform[] points;

    private bool facingRight = true; 

    void Start()
    {
        pointIndex = 0;
        //Debug.Log("point0: " + points[pointIndex]);
        movepoint = points[pointIndex];
    }

    void Flip(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Update()
    {
        if (pointIndex >= MaxNum) return;
        
        //ruch w stronê punktu
        transform.position = Vector2.MoveTowards(transform.position, movepoint.position, speed * Time.deltaTime);
        
        //sprawdzenie czy dotar³ do punktu
        if (Vector2.Distance(transform.position, movepoint.position) <= 0)
        {
            if (pointIndex > MaxNum)
            {
                pointIndex = 0;
            }
            pointIndex++;
            movepoint = points[pointIndex];

            float dirX = movepoint.position.x - transform.position.x;

            if (dirX > 0 && !facingRight)
            {
                Flip(true); //prawo
            }
            else if (dirX < 0 && facingRight)
            {
                Flip(false); //lewo
            }
        }
        


        //Vector2 pos = movepoint.position - transform.position;
        //float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
    }

    
}
