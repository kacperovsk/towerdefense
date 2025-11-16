using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    static float maxHealth = 10f;
    [SerializeField] private float health = maxHealth;
    [SerializeField] private Path currentPath;
    [Header("Player Damage")]
    [SerializeField] private int playerDamage = 1; //Ile obrażeń zada przeciwnik gdy dojdzie do końca. Ustawiam na 1 bo edytuje w prefabie anyway
    private Vector3 targetPosition;
    private int currentPosition;
    private bool facingRight = true;

    //Wartości kasy dla wrogów, nie jestem pewien czy to potrzebne ale buja
    [Header("Reward")]
    [SerializeField] private int moneyValue = 10;
    void Flip(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f) Die();
    }

    private void Die()
    {
        //Teraz przed usunięciem dodajemy kasę do gracza
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(moneyValue);
        }
        Destroy(gameObject);
    }

    private void Awake()
    {
        currentPath = GameObject.Find("Path1").GetComponent<Path>();
    }
    private void OnEnable()
    {
        // USTAWIENIE WAYPOINTA NA SPAWN
        currentPosition = 0; 
        // USTAWIANIE HP NA MAX NA SPAWN
        health = maxHealth; 
        // OBRACANIE W PRAWO NA SPAWN
        facingRight = true;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale; 

        targetPosition = currentPath.GetPosition(currentPosition);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        float relativeDistance = (transform.position - targetPosition).magnitude;

        // Jak blisko waypointa to idzie do kolejnego.
        if (relativeDistance < 0.05f)
        {

            if(currentPosition < currentPath.Waypoints.Length-1)
            {
                currentPosition++;
                targetPosition = currentPath.GetPosition(currentPosition);
                float dirX = targetPosition.x - transform.position.x;
                if (dirX > 0 && !facingRight)
                {
                    Flip(true); //prawo
                }
                else if (dirX < 0 && facingRight)
                {
                    Flip(false); //lewo
                }
            }
            //Z tego co wiem to jest ten skrypt usuwania wroga na końcu ścieżki
            else
            {
                // więc jak dotrze do końca
                if (GameManager.Instance != null)
                {
                    // To gracz traci HP
                    GameManager.Instance.LoseLife(playerDamage); 
                }
                //i tutaj tak jak wcześniej wróg znika z puli
                gameObject.SetActive(false);
            }
        }

    }

    public float GetProgress() // nie mam pojecia co tu sie dzieje ale buja i dzia��
    {
        if (currentPath == null || currentPath.Waypoints == null || currentPath.Waypoints.Length < 2)
            return 0f;

        int prevIndex = Mathf.Clamp(currentPosition - 1, 0, currentPath.Waypoints.Length - 1);
        int nextIndex = Mathf.Clamp(currentPosition, 0, currentPath.Waypoints.Length - 1);

        Vector3 prevPos = currentPath.Waypoints[prevIndex].transform.position;
        Vector3 nextPos = currentPath.Waypoints[nextIndex].transform.position;

        float segmentLength = Vector3.Distance(prevPos, nextPos);
        float distanceFromPrev = Vector3.Distance(transform.position, prevPos);

        // ile procent trasy mi�dzy tymi waypointami wr�g pokona�
        float segmentProgress = (segmentLength > 0f) ? Mathf.Clamp01(distanceFromPrev / segmentLength) : 0f;

        // pe�ny progres = numer poprzedniego waypointa + u�amek segmentu
        float totalProgress = (prevIndex + segmentProgress) / (currentPath.Waypoints.Length - 1);

        return Mathf.Clamp01(totalProgress);
    }
}
