using UnityEngine;
using UnityEngine.UI; // Ajouté pour manipuler le Slider

public class HandleSlide : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private RectTransform handleA;
    [SerializeField] private RectTransform handleB;
    [SerializeField] private Slider monSlider; // Glisse ton Slider ici

    [Header("Paramètres")]
    [SerializeField] private float vitesseGlobale = 0.5f;
    [SerializeField] private float limitesX = 150f;
    [SerializeField] private float écartDeBase = 30f;

    void Update()
    {
        if (handleA == null || handleB == null || monSlider == null) return;

        // --- LOGIQUE DE MOUVEMENT (Inchangée) ---
        float noiseMouvement = Mathf.PerlinNoise(Time.time * vitesseGlobale, 0f) - 0.5f;
        float centreActuelX = noiseMouvement * (limitesX * 2f);
        float posX_A = centreActuelX - (écartDeBase / 2f);
        float posX_B = centreActuelX + (écartDeBase / 2f);

        handleA.anchoredPosition = new Vector2(posX_A, handleA.anchoredPosition.y);
        handleB.anchoredPosition = new Vector2(posX_B, handleB.anchoredPosition.y);

        // --- NOUVELLE LOGIQUE DE GAMEPLAY ---

        // 1. On convertit la valeur du Slider (0 à 1) en position X (-150 à 150)
        // La fonction Lerp fait la conversion mathématique pour nous
        float posXSlider = Mathf.Lerp(-limitesX, limitesX, monSlider.value);

        // 2. Vérification : Est-ce que le Slider est entre HandleA et HandleB ?
        if (posXSlider >= posX_A && posXSlider <= posX_B)
        {
            // LE JOUEUR GAGNE : On peut changer la couleur en vert par exemple
            handleA.GetComponent<Image>().color = Color.green;
            handleB.GetComponent<Image>().color = Color.green;
        }
        else
        {
            // LE JOUEUR PERD : On remet en rouge ou blanc
            handleA.GetComponent<Image>().color = Color.red;
            handleB.GetComponent<Image>().color = Color.red;
        }
    }
}