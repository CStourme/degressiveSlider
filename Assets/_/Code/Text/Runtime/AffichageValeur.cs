using UnityEngine;
using TMPro;

public class AffichageValeur : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI champTexte;

    // Cette fonction reçoit maintenant le score total
    public void MettreAJourTexte(float valeur)
    {
        if (champTexte != null)
        {
            // "F0" permet d'afficher le score sans décimales
            // On enlève le * 100 et le % pour n'afficher que les points
            champTexte.text = valeur.ToString("F0") + " PTS";
        }
    }
}