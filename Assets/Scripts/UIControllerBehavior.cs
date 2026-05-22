using UnityEngine;
using UnityEngine.InputSystem;

public class UIControllerBehavior : MonoBehaviour
{
    public GameObject[] UIElementsToToggle;

    public bool m_show = true;

    private InputSystem_Actions inputActions;

    void Awake() => inputActions = new();

    void OnEnable()
    {
        inputActions.UI.ShowHide.performed += OnShowHidePerformed;
        inputActions.UI.Enable();
    }

    void OnDisable()
    {
        inputActions.UI.ShowHide.performed -= OnShowHidePerformed;
        inputActions.UI.Disable();
    }

    void OnDestroy() => inputActions.Dispose();

    private void OnShowHidePerformed(InputAction.CallbackContext context) => ToggleUIElements();

    private void ToggleUIElements()
    {
        m_show = !m_show;

        if(UIElementsToToggle != null)
            foreach (GameObject element in UIElementsToToggle)
                if (element) element.SetActive(m_show);
    }
}