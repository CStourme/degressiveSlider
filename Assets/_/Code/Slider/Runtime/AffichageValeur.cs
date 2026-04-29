using UnityEngine;
using TMPro; // Nécessaire pour TextMeshPro

public class AffichageValeur : MonoBehaviour
{
    private TextMeshProUGUI texte;

    void Awake()
    {
        texte = GetComponent<TextMeshProUGUI>();
    }

    // Cette fonction sera appelée par le Slider
    public void MettreAJourTexte(float valeur)
    {
        // On affiche la valeur avec 1 seule décimale (ex: 0.7)
        // Multiplie par 100 si tu veux afficher un pourcentage
        texte.text = (valeur * 100f).ToString("F0") + "%";
    }
}