using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalCancel : MonoBehaviour
{
    [Header("cancel")]
    public InputActionReference cancelAction;
    [Header("back destination")]
    public string backScene;
    void OnEnable()
    {
        cancelAction.action.performed += OnCancel;
    }

    void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
    }

    void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (!string.IsNullOrEmpty(backScene))
        {
            SceneManager.LoadScene(backScene);
        }
    }
}
