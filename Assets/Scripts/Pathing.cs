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

    void Start()
    {
        pointIndex = 0;
        Debug.Log("point0: " + points[pointIndex]);
        movepoint = points[pointIndex];
    }

    void Update()
    {
        if (pointIndex >= MaxNum) return;

        transform.position = Vector2.MoveTowards(transform.position, movepoint.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, movepoint.position) <= 0)
        {
            if (pointIndex > MaxNum)
            {
                pointIndex = 0;
            }
            pointIndex++;
            movepoint = points[pointIndex];
        }
        Vector2 pos = movepoint.position - transform.position;
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
    }

    
}
