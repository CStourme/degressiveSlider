using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleSlide : MonoBehaviour
{
    // --- LES "BOITES" (VARIABLES) ---
    // On prépare les cases pour glisser nos objets depuis Unity
    [Header("Références")]
    [SerializeField] public RectTransform poigneeGauche; 
    [SerializeField] public RectTransform poigneeDroite;
    [SerializeField] public Slider barreSlider;
    [SerializeField] public AffichageValeur scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    // --- LES RÉGLAGES ---
    [Header("Mouvement Global (Position)")]
    [SerializeField] public float vitesseDeDeplacement = 0.5f;
    [SerializeField] public float largeurDuSlider = 150f;
    
    [Header("Variation de l'Écart (Largeur)")]
    [Tooltip("L'écart moyen entre les deux poignées")]
    [SerializeField] public float largeurZoneCible = 40f;
    
    [Tooltip("De combien l'écart peut-il s'agrandir ou se réduire")]
    [SerializeField] public float forceVariationZone = 20f;
   
    [Tooltip("Vitesse à laquelle la zone change de taille")]
    [SerializeField] public float vitesseVariationZone = 1.0f;

    // --- LES VARIABLES CACHÉES ---
    public float monScore = 0f;
    private float tempsEcoule = 0f;
    public float tempsTotal;
    private float franchissement = 4f;
    private Image PoigneeGaucheColor;
    private Image PoigneeDroiteColor;
    private bool etaitDansLaZone = false;

    void Awake()
    {
        monScore = 50f;
        tempsTotal = 15f;
        PoigneeGaucheColor = poigneeGauche.GetComponent<Image>();
        PoigneeDroiteColor = poigneeDroite.GetComponent<Image>();
    }
    void Update()
    {
        // On augmente notre propre chronomètre interne
        tempsEcoule = tempsEcoule + Time.deltaTime;

        // --- ETAPE 1 : CALCULER LA POSITION DU CENTRE ---
        // Le PerlinNoise donne un chiffre entre 0 et 1 qui bouge doucement.
        float calculBruitPos = Mathf.PerlinNoise(tempsEcoule * vitesseDeDeplacement, 0f);
        // On transforme le 0 à 1 en -0.5 à 0.5
        float positionZeroUn = calculBruitPos - 0.5f;
        // On multiplie par la largeur totale (300 car -150 à 150)
        float centreX = positionZeroUn * (largeurDuSlider * 2f);


        // --- ETAPE 2 : CALCULER LA LARGEUR DE LA ZONE ---
        // On refait un bruit pour que la zone change de taille toute seule
        float calculBruitLargeur = Mathf.PerlinNoise(tempsEcoule * vitesseVariationZone, 500f);
        float largeurVariable = calculBruitLargeur - 0.5f; // Donne entre -0.5 et 0.5
        
        // On part de la largeur de base et on ajoute/retire la variation
        float largeurFinale = largeurZoneCible + (largeurVariable * (forceVariationZone * 2f));


        // --- ETAPE 3 : PLACER LES POIGNÉES ---
        // La poignée gauche est au centre MOINS la moitié de la largeur
        float posX_Gauche = centreX - (largeurFinale / 2f);
        // La poignée droite est au centre PLUS la moitié de la largeur
        float posX_Droite = centreX + (largeurFinale / 2f);

        // On applique les positions sur les objets dans Unity
        poigneeGauche.anchoredPosition = new Vector2(posX_Gauche, 0f);
        poigneeDroite.anchoredPosition = new Vector2(posX_Droite, 0f);


        // --- ETAPE 4 : CALCULER LA POSITION DU CURSEUR DU SLIDER ---
        // Le script "MonControleurSlider" fait bouger la 'value' de 0 à 1.
        // Ici on transforme ce 0 à 1 en position X (-150 à 150)
        float positionXDuCurseur = Mathf.Lerp(-largeurDuSlider, largeurDuSlider, barreSlider.value);


        // --- ETAPE 5 : VÉRIFIER SI ON GAGNE OU ON PERD ---
        
        // On crée une variable pour savoir si on est au milieu à cet instant précis
        bool estAuMilieu = (positionXDuCurseur > posX_Gauche && positionXDuCurseur < posX_Droite);
        
        if (estAuMilieu)
        {
            // --- CAS A - ON GAGNE : on ajoute du temps au score
            monScore = monScore + Time.deltaTime;
            // LE JOUEUR GAGNE : On peut changer la couleur en vert
            PoigneeGaucheColor.color = Color.green;
            PoigneeDroiteColor.color = Color.green;
            // On mémorise qu'on est à l'intérieur
            etaitDansLaZone = true;
        }
        else
        {
            // --- CAS B - ON PERD : LE JOUEUR EST À L'EXTÉRIEUR ---
    
            // A - SI on était dans la zone juste avant, ça veut dire qu'on vient de SORTIR
            if (etaitDansLaZone == true)
            {
                monScore = monScore - franchissement; // On retire les 5 points UNE SEULE FOIS
                // On mémorise qu'on est dehors pour ne pas reperdre 5pts au prochain Update
                // On "désarme le piège" pour ne pas perdre 5pts en boucle
                etaitDansLaZone = false;
            }

            // B. LA PERTE CONTINUE
            monScore = monScore - Time.deltaTime;

            // C. LA LOGIQUE DES COULEURS DIFFÉRENCIÉES
            if (positionXDuCurseur <= posX_Gauche)
            {
                // On est sorti par la GAUCHE
                PoigneeGaucheColor.color = Color.red;
                PoigneeDroiteColor.color = Color.green; // La droite reste vert
            }
            else if (positionXDuCurseur >= posX_Droite)
            {
                // On est sorti par la DROITE
                PoigneeDroiteColor.color = Color.red;
                PoigneeGaucheColor.color = Color.green; // La gauche reste vert
            }
        }
        
        // --- ETAPE 6 : ENVOYER LE SCORE AU TEXTE ---
        // On demande au script "AffichageValeur" d'afficher notre score
        if (scoreText != null)
        {
            scoreText.MettreAJourTexte(monScore);
        }

        if (timerText != null)
        {
            // On calcule le temps restant : le total moins le temps qui a défilé
            float tempsRestant = tempsTotal - tempsEcoule;

            // On s'assure que le timer ne devienne pas négatif
            if (tempsRestant < 0) tempsRestant = 0;

            // On l'affiche avec 0 chiffre après la virgule
            timerText.text = "Timer : " + tempsRestant.ToString ("F0");
        }
    }
}