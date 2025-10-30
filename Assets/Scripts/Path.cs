using UnityEngine;

public class Path : MonoBehaviour
{
    public GameObject[] Waypoints;

    public Vector3 GetPosition(int index)
    {
        return Waypoints[index].transform.position;
    }

    void Update()
    {
        
    }
}
