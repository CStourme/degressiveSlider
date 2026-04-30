using UnityEngine;
using TMPro;
public class AffichageValeur : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI champTexte;

    /// <summary>
    /// Cette méthode est le "récepteur". Elle attend qu'on lui envoie un chiffre
    /// Comme le slider est dégressif, il va envoyer des chiffres de plus en plus petits (ex: 0.9, 0.8...)
    /// </summary>
    public void MettreAJourTexte(float valeur)
    {
        // Avant de manipuler le texte, on vérifie qu'on a bien glissé l'objet dans l'inspecteur
        // Cela évite l'erreur "NullReferenceException"
        if (champTexte != null)
        {
            /*
               LA FORMATION DU TEXTE :
               - (valeur * 100f) : Le slider va de 0 à 1. En multipliant par 100, on obtient un pourcentage (ex: 0.5 devient 50)
               - .ToString("F0") : C'est un formatage. "F" signifie 'Fixed-point' (nombre à virgule)
                                   et le "0" indique qu'on veut ZÉRO chiffre après la virgule (on arrondit à l'entier)
               - + "%" : On fait ce qu'on appelle une 'concaténation' en ajoutant le symbole % à la fin
            */
            champTexte.text = (valeur * 100f).ToString("F0") + "%";
            
            // PETIT PLUS:
            // Ajout changement de couleur si la barre est basse !
            // On vérifie d'abord le seuil le plus bas (CRITIQUE)
            if (valeur < 0.2f) 
            {
                champTexte.color = Color.red; 
            }
            // Si on n'est pas sous 20%, on vérifie si on est sous 50% (ALERTE)
            else if (valeur < 0.5f) 
            {
                // Note : Color.orange est une couleur prédéfinie dans Unity.
                champTexte.color = Color.orange; 
            }
            // Si aucune des conditions ci-dessus n'est vraie (OK)
            else 
            {
                champTexte.color = Color.white;
            }
        }
        else
        {
            // Si on a oublié d'assigner le texte dans Unity, ce message apparaîtra dans la console pour nous aider
            Debug.LogWarning($"[AffichageValeur] Attention : Tu as oublié de glisser ton objet texte sur {gameObject.name} !");
        }
    }
}