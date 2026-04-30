using UnityEngine;
using UnityEngine.UI;

public class HandleSlide : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private RectTransform handleA;
    [SerializeField] private RectTransform handleB;
    [SerializeField] private Slider monSlider;
    [SerializeField] private AffichageValeur scriptAffichage; // On lie le script de texte ici

    [Header("Paramètres de Jeu")]
    [SerializeField] private float limitesX = 150f;
    [SerializeField] private float écartDeBase = 30f;
    [SerializeField] private float vitesseGlobale = 0.5f;

    private float score = 0f;

    void Update()
    {
        if (handleA == null || handleB == null || monSlider == null) return;

        // --- MOUVEMENT DES HANDLES (Inchangé) ---
        float noiseMouvement = Mathf.PerlinNoise(Time.time * vitesseGlobale, 0f) - 0.5f;
        float centreActuelX = noiseMouvement * (limitesX * 2f);
        float posX_A = centreActuelX - (écartDeBase / 2f);
        float posX_B = centreActuelX + (écartDeBase / 2f);

        handleA.anchoredPosition = new Vector2(posX_A, handleA.anchoredPosition.y);
        handleB.anchoredPosition = new Vector2(posX_B, handleB.anchoredPosition.y);

        // --- CALCUL DU SCORE ---
        float posXSlider = Mathf.Lerp(-limitesX, limitesX, monSlider.value);

        // Si le curseur est entre les poignées
        if (posXSlider >= posX_A && posXSlider <= posX_B)
        {
            // On ajoute 1 point par seconde
            score += 1f * Time.deltaTime;
        }
        else
        {
            // On perd 1 point par seconde (on utilise Mathf.Max pour ne pas descendre sous 0)
            score = Mathf.Max(0, score - 1f * Time.deltaTime);
        }

        // --- ENVOI DU SCORE À L'AFFICHAGE ---
        if (scriptAffichage != null)
        {
            scriptAffichage.MettreAJourTexte(score);
        }
    }
}