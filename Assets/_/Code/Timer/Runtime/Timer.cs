using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private float tempsTotal = 30f;
    
    private TextMeshProUGUI timerText;
    private float tempsEcoule = 0f;
    private bool estFige = false;

    void Awake()
    {
        // On récupère automatiquement le composant texte sur ce GameObject
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (estFige) return;

        // On augmente notre propre chronomètre interne (Time.deltaTime est le temps entre 2 images)
        tempsEcoule = tempsEcoule + Time.deltaTime;

        // Calcul du temps restant pour l'affichage
        float tempsRestant = GetTempsRestant();

        // On injecte le texte dans le composant
        if (timerText != null)
        {
            timerText.text = "Timer : " + tempsRestant.ToString("F0");
        }
    }

    // Permet aux autres scripts de lire le temps restant
    public float GetTempsRestant()
    {
        float restant = tempsTotal - tempsEcoule;
        return (restant < 0) ? 0 : restant;
    }

    // Permet de stopper le décompte visuel si besoin
    public void FigerTimer()
    {
        estFige = true;
    }
}