using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private bool isDoubleSpeed = false; // czy jest w³¹czone x2

    public void ToggleDoubleSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;

        if (isDoubleSpeed)
        {
            Time.timeScale = 2f; // podwójna prêdkoœæ
        }
        else
        {
            Time.timeScale = 1f; // normalna prêdkoœæ
        }
    }
}
