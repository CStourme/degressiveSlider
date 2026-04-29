using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReactor : MonoBehaviour
{
    public InputActionReference m_inputActionReference;
    public float valeurActive = 0.3f;
    
    // Cet événement sert à transmettre le "bonus" à ajouter
    public UnityEvent<float> OnInputPressed;
    
    private void OnEnable()
    {
        // J' active l'écoute de la touche
        m_inputActionReference.action.Enable();
        // Je m'abonne uniquement au déclenchement de l'action (started)
        m_inputActionReference.action.started += Action_started;
    }

    private void Action_started(InputAction.CallbackContext context)
    {
        // J'envoie la valeur 0.3 une seule fois par pression
        OnInputPressed.Invoke(valeurActive);
    }

    private void OnDisable()
    {
        // Je me désabonne pour éviter des erreurs
        m_inputActionReference.action.started -= Action_started;
        m_inputActionReference.action.Disable();
    }
}