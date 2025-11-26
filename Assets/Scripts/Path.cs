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

    public int GetClosestWaypointIndex(Vector3 pos)
    {
        int closestIndex = 0;
        float bestDist = Mathf.Infinity;

        for (int i = 0; i < Waypoints.Length; i++)
        {
            float d = Vector3.Distance(pos, Waypoints[i].transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
