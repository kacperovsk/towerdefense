using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConfirmationMenu : MonoBehaviour
{
    public static ConfirmationMenu Instance;

    [Header("UI References")]
    public GameObject panel;         // Panel confirmation
    public Button yesButton;
    public Button noButton;

    private System.Action onConfirmAction; // akcja do wywo³ania po potwierdzeniu

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);

        panel.SetActive(false);
        // Podpiêcie przycisków
        if (yesButton != null) yesButton.onClick.AddListener(OnConfirm);
        if (noButton != null) noButton.onClick.AddListener(OnCancel);
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnCancel();
    }
    // Wywo³anie menu z akcj¹ potwierdzenia
    public void Show(System.Action confirmAction)
    {
        onConfirmAction = confirmAction;
        panel.SetActive(true);

        if (yesButton != null)
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
    }

    private void OnConfirm()
    {
        onConfirmAction?.Invoke();
        panel.SetActive(false);
    }

    private void OnCancel()
    {
        panel.SetActive(false);
    }
}
