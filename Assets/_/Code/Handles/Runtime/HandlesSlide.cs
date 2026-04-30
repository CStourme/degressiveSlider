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
    
    // Communication : ce script envoie le score au script "AffichageValeur"
    [SerializeField] public AffichageValeur scoreText;
    
    // Communication : on a besoin de l'objet texte de TextMeshPro pour le temps
    [SerializeField] public TextMeshProUGUI timerText;
    
    // Référence vers l'objet qui contient le message de fin
    [SerializeField] public GameObject elapsedTimeText;
    
    // --- LES RÉGLAGES ---
    [Header("Gameplay Settings")]
    [SerializeField] private float tempsTotal = 15f;
    
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

    // --- LES VARIABLES CACHÉES (Mémoire interne) ---
    private float monScore = 0f;
    private float tempsEcoule = 0f;
    private float franchissement = 4f;
    private Image PoigneeGaucheColor;
    private Image PoigneeDroiteColor;
    
    // Les interrupteurs (booléens) pour mémoriser des états
    private bool etaitDansLaZone = false;
    private bool jeuTermine = false; 

    void Awake()
    {
        // On initialise le score de départ
        monScore = 50f;
        
        // On va chercher le pot de peinture (Image) sur les poignées pour changer leur couleur plus tard
        PoigneeGaucheColor = poigneeGauche.GetComponent<Image>();
        PoigneeDroiteColor = poigneeDroite.GetComponent<Image>();
    }

    void Update()
    {
        // SECURITÉ : Si l'interrupteur 'jeuTermine' est allumé, on ne fait plus rien
        if (jeuTermine == true)
        {
            return; // On arrête l'Update ici
        }

        // On augmente notre propre chronomètre interne (Time.deltaTime est le temps entre 2 images)
        tempsEcoule = tempsEcoule + Time.deltaTime;

        // --- ETAPE 1 : CALCULER LA POSITION DU CENTRE ---
        // Le PerlinNoise donne un chiffre entre 0 et 1 qui bouge doucement.
        float calculBruitPos = Mathf.PerlinNoise(tempsEcoule * vitesseDeDeplacement, 0f);
        // On transforme le 0 à 1 en -0.5 à 0.5 pour avoir un centre
        float positionZeroUn = calculBruitPos - 0.5f;
        // On multiplie par la largeur totale (300 car le slider va de -150 à 150)
        float centreX = positionZeroUn * (largeurDuSlider * 2f);


        // --- ETAPE 2 : CALCULER LA LARGEUR DE LA ZONE ---
        // On fait varier la taille de l'écart avec un autre bruit Perlin
        float calculBruitLargeur = Mathf.PerlinNoise(tempsEcoule * vitesseVariationZone, 500f);
        float largeurVariable = calculBruitLargeur - 0.5f; 
        float largeurFinale = largeurZoneCible + (largeurVariable * (forceVariationZone * 2f));


        // --- ETAPE 3 : PLACER LES POIGNÉES ---
        // On calcule les positions X par rapport au centre et à la largeur
        float posX_Gauche = centreX - (largeurFinale / 2f);
        float posX_Droite = centreX + (largeurFinale / 2f);

        // On applique physiquement les positions sur les objets UI dans Unity
        poigneeGauche.anchoredPosition = new Vector2(posX_Gauche, 0f);
        poigneeDroite.anchoredPosition = new Vector2(posX_Droite, 0f);


        // --- ETAPE 4 : CALCULER LA POSITION DU CURSEUR DU SLIDER ---
        // On regarde la valeur du Slider (0 à 1) et on la transforme en position X (-150 à 150)
        float positionXDuCurseur = Mathf.Lerp(-largeurDuSlider, largeurDuSlider, barreSlider.value);


        // --- ETAPE 5 : VÉRIFIER SI ON GAGNE OU ON PERD ---
        // On crée une condition pour savoir si on est au milieu des deux poignées
        bool estAuMilieu = (positionXDuCurseur > posX_Gauche && positionXDuCurseur < posX_Droite);
        
        if (estAuMilieu)
        {
            // --- CAS A : ON EST DANS LA ZONE ---
            monScore = monScore + Time.deltaTime;
            
            // On met les deux poignées en vert
            PoigneeGaucheColor.color = Color.green;
            PoigneeDroiteColor.color = Color.green;
            
            // On "arme" le piège : on mémorise qu'on est à l'intérieur
            etaitDansLaZone = true;
        }
        else
        {
            // --- CAS B : ON EST À L'EXTÉRIEUR ---
            
            // On vérifie si on vient juste de franchir la limite
            if (etaitDansLaZone == true)
            {
                monScore = monScore - franchissement; // Perte de 4 points d'un coup
                etaitDansLaZone = false; // On désarme le piège pour ne pas perdre 4pts en boucle
            }

            // On perd aussi un petit peu de score chaque seconde
            monScore = monScore - Time.deltaTime;

            // Logique des couleurs : on allume en rouge seulement la poignée qui a été dépassée
            if (positionXDuCurseur <= posX_Gauche)
            {
                PoigneeGaucheColor.color = Color.red;
                PoigneeDroiteColor.color = Color.green; 
            }
            else if (positionXDuCurseur >= posX_Droite)
            {
                PoigneeDroiteColor.color = Color.red;
                PoigneeGaucheColor.color = Color.green; 
            }
        }
        
        // --- ETAPE 6 : ENVOYER LES DONNÉES À L'INTERFACE (UI) ---
        
        // On envoie le score au script de texte externe s'il existe
        if (scoreText != null)
        {
            scoreText.MettreAJourTexte(monScore);
        }

        // Gestion du Timer
        if (timerText != null)
        {
            float tempsRestant = tempsTotal - tempsEcoule;
            
            // Si le temps arrive à zéro
            if (tempsRestant <= 0 && jeuTermine == false)
            {
                tempsRestant = 0;
                TerminerLeJeu(); // On appelle les instructions de fin
            }

            // On affiche le mot "Timer" suivi du chiffre arrondi à zéro virgule
            timerText.text = "Timer : " + tempsRestant.ToString("F0");
        }
    }

    // --- FONCTION DE FIN DE JEU (Les ordres de clôture) ---
    void TerminerLeJeu()
    {
        // On allume l'interrupteur pour bloquer l'Update
        jeuTermine = true; 

        // On rend visible l'objet "Elapsed Time" (texte, bouton, etc.)
        if (elapsedTimeText != null)
        {
            // 1. On crée l'instance
            GameObject instance = Instantiate(elapsedTimeText, transform.parent);
            
            // 2. CORRECTION : On force l'échelle à 1 (sinon il est souvent à 0 ou minuscule)
            instance.transform.localScale = Vector3.one;

            // 3. CORRECTION : On centre l'objet dans l'écran
            RectTransform rt = instance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero; 
            }
            instance.SetActive(true);
        }

        // On fait disparaître le Slider de l'écran (on accède au .gameObject pour l'éteindre)
        if (barreSlider != null)
        {
            barreSlider.gameObject.SetActive(false);
        }

        // On fait disparaître le texte du Score
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        // On demande au moteur Unity de figer le temps (plus rien ne bouge)
        Time.timeScale = 0; 
    }
}