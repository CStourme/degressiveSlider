using UnityEngine;
using UnityEngine.SceneManagement; // Requis pour charger des scènes

public class BoutonRetry : MonoBehaviour
{
    // Cette fonction doit être 'public' pour être visible par le composant Button
    public void RelancerLeJeu()
    {
        // IMPORTANT : On remet le temps à 1 !
        // Puisqu'on l'a figé à 0 dans TerminerPartie, si on ne le remet pas à 1,
        // le jeu restera bloqué sur la première image après le rechargement.
        Time.timeScale = 1f;

        // On demande à Unity de recharger la scène actuellement active
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}