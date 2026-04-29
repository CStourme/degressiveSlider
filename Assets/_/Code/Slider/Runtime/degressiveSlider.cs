using UnityEngine;
using UnityEngine.UI;

public class degressiveSlider : MonoBehaviour
{
    private Slider UISlider;
    private float total = 1f;
    public float vitesseProgression = 0.1f;

    void Start()
    {
        // Je récupère le composant Slider attaché à ce GameObject
        UISlider = GetComponent<Slider>();
        // Je défini que la valeur maximum du slider est égal à total (1)
        UISlider.maxValue = total;
        // Je force le slider à commencer au maximum
        UISlider.value = total;
        
    }

    void Update()
    {
        // -= pour soustraire de la valeur
        UISlider.value -= vitesseProgression * Time.deltaTime;
    }
    
    // Fonction à choisir dans le menu déroulant (No Function) pour faire le pont
    // Dans l'UnityEvent, chercher le script "degressiveSlider" puis AjouterValeur
    public void AjouterValeur(float bonus)
    {
        UISlider.value += bonus;
    }
}