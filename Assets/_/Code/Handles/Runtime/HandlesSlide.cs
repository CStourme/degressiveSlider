using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleSlide : MonoBehaviour
{
    // --- LES "BOITES" (VARIABLES) ---
    // On prépare les cases pour glisser nos objets depuis Unity
    [Header("Références")]
    [SerializeField] private RectTransform canvasParent; // Glisser l'objet Canvas ici pour servir de parent aux prefabs de fin
    [SerializeField] public RectTransform poigneeGauche; 
    [SerializeField] public RectTransform poigneeDroite;
    [SerializeField] public Slider barreSlider;
    
    // Communication : ce script envoie les points au nouveau système de score et record
    [SerializeField] public Total scoreSystem; 
    
    // Communication : référence vers le script Timer externe pour synchroniser la fin de partie
    [SerializeField] public Timer timerSystem;
    
    // Référence vers le prefab qui contient le message ELAPSED TIME (Temps écoulé)
    [SerializeField] public GameObject elapsedTimeText;
    
    // Référence vers le prefab qui contient le message GAME OVER (Score à zéro)
    [SerializeField] public GameObject gameOverText;
    
    // --- LES RÉGLAGES ---
    [Header("Gameplay Settings")]
    // Temps de protection pour ne pas perdre 5 points trop souvent (anti-spam de faute)
    [SerializeField] private float penaliteDeFranchissement = 5f;
    [SerializeField] private float tempsDeGraceFranchissement = 1.0f; 

    // Paramètres pour varier le motif de mouvement à chaque lancement
    [Header("Réglages du Seed (Aléatoire)")]
    [Tooltip("Si coché, le jeu choisira un motif différent à chaque démarrage")]
    [SerializeField] private bool utiliserSeedAleatoire = true;
    [Tooltip("La valeur fixe du motif (si le mode aléatoire est décoché)")]
    [SerializeField] private float seedFixe = 0f;
    
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
    private float tempsEcouleLocal = 0f; // Chrono interne utilisé uniquement pour le calcul du bruit de Perlin
    private Image PoigneeGaucheColor;
    private Image PoigneeDroiteColor;
    
    // Valeur de décalage pour le Perlin Noise (permet de changer de "chemin" à chaque partie)
    private float seedActuelle;
    // Le chrono de protection actuel (décompte après une faute)
    private float timerGraceActuel = 0f; 
    // Les interrupteurs (booléens) pour mémoriser des états
    private bool etaitDansLaZone = false;
    private bool jeuTermine = false; 

    void Awake()
    {
        // On initialise le score de départ à 50
        monScore = 50f;

        // Gestion du seed : on définit le point de départ sur la carte infinie du Perlin Noise
        if (utiliserSeedAleatoire)
        {
            // On choisit un nombre au hasard très grand pour un motif unique
            seedActuelle = Random.Range(0f, 99999f);
        }
        else
        {
            seedActuelle = seedFixe;
        }
        
        // On récupère les composants Image pour pouvoir changer la couleur (Vert/Rouge) dynamiquement
        PoigneeGaucheColor = poigneeGauche.GetComponent<Image>();
        PoigneeDroiteColor = poigneeDroite.GetComponent<Image>();
    }

    void Update()
    {
        // SECURITÉ : Si l'interrupteur 'jeuTermine' est allumé, on stoppe toute la logique
        if (jeuTermine == true)
        {
            return; 
        }

        // On fait descendre le chrono de protection s'il est actif (Time.deltaTime = temps depuis l'image précédente)
        if (timerGraceActuel > 0) 
        {
            timerGraceActuel -= Time.deltaTime;
        }

        // On augmente le temps local pour que le mouvement de Perlin continue d'avancer
        tempsEcouleLocal = tempsEcouleLocal + Time.deltaTime;

        // --- ETAPE 1 : CALCULER LA POSITION DU CENTRE ---
        // Le PerlinNoise renvoie une valeur entre 0 et 1 de façon fluide
        float calculBruitPos = Mathf.PerlinNoise((tempsEcouleLocal * vitesseDeDeplacement) + seedActuelle, 0f);
        // On recentre la valeur (-0.5 à 0.5) et on l'adapte à la largeur du slider
        float centreX = (calculBruitPos - 0.5f) * (largeurDuSlider * 2f);


        // --- ETAPE 2 : CALCULER LA LARGEUR DE LA ZONE ---
        // On utilise un deuxième échantillon de bruit (décalé de 500) pour que la taille varie indépendamment de la position
        float calculBruitLargeur = Mathf.PerlinNoise((tempsEcouleLocal * vitesseVariationZone) + seedActuelle, 500f);
        float largeurFinale = largeurZoneCible + ((calculBruitLargeur - 0.5f) * (forceVariationZone * 2f));


        // --- ETAPE 3 : PLACER LES POIGNÉES ---
        // On positionne les poignées à gauche et à droite du centre calculé
        poigneeGauche.anchoredPosition = new Vector2(centreX - (largeurFinale / 2f), 0f);
        poigneeDroite.anchoredPosition = new Vector2(centreX + (largeurFinale / 2f), 0f);


        // --- ETAPE 4 : POSITION DU CURSEUR ---
        // On convertit la valeur du Slider (0 à 1) en position X réelle dans l'UI
        float positionXDuCurseur = Mathf.Lerp(-largeurDuSlider, largeurDuSlider, barreSlider.value);


        // --- ETAPE 5 : LOGIQUE DE COLLISION ET SCORE ---
        // On vérifie si le curseur est coincé entre les deux poignées
        bool estAuMilieu = (positionXDuCurseur > poigneeGauche.anchoredPosition.x && positionXDuCurseur < poigneeDroite.anchoredPosition.x);
        
        if (estAuMilieu)
        {
            // Gain de points progressif quand on est dans la zone
            monScore = monScore + Time.deltaTime;
            PoigneeGaucheColor.color = Color.green;
            PoigneeDroiteColor.color = Color.green;
            etaitDansLaZone = true;
        }
        else
        {
            // Si on sort de la zone (Franchissement de limite)
            if (etaitDansLaZone == true)
            {
                // On applique la grosse pénalité seulement si on n'est pas protégé par le temps de grâce
                if (timerGraceActuel <= 0)
                {
                    monScore = monScore - penaliteDeFranchissement;
                    timerGraceActuel = tempsDeGraceFranchissement;
                }
                etaitDansLaZone = false;
            }

            // Perte de points constante quand on est hors zone
            monScore = monScore - Time.deltaTime;

            // Feedback visuel : on allume en rouge la poignée que l'on dépasse
            if (positionXDuCurseur <= poigneeGauche.anchoredPosition.x)
            {
                PoigneeGaucheColor.color = Color.red;
                PoigneeDroiteColor.color = Color.green; 
            }
            else
            {
                PoigneeDroiteColor.color = Color.red;
                PoigneeGaucheColor.color = Color.green; 
            }
        }
        
        // --- ETAPE 6 : MISE À JOUR DU SCORE ET RECORDS ---
        // On envoie le score au script Total qui gère l'affichage et la sauvegarde JSON
        if (scoreSystem != null)
        {
            scoreSystem.MettreAJourAffichage(monScore);
        }

        // --- ETAPE 7 : GESTION DES CONDITIONS DE FIN ---
        if (timerSystem != null && jeuTermine == false)
        {
            // On récupère le temps restant calculé par le script Timer.cs
            float tempsRestant = timerSystem.GetTempsRestant();
            
            // CONDITION 1 : Plus de points = Game Over
            if (monScore <= 0)
            {
                monScore = 0;
                TerminerPartie(gameOverText);
            }
            // CONDITION 2 : Temps écoulé = Fin de partie classique
            else if (tempsRestant <= 0)
            {
                TerminerPartie(elapsedTimeText);
            }
        }
    }

    // --- FONCTION GÉNÉRALE DE FIN DE PARTIE ---
    // Cette fonction centralise l'arrêt du jeu pour éviter les répétitions de code
    void TerminerPartie(GameObject prefabFin)
    {
        jeuTermine = true; 
        
        // On demande au timer de s'arrêter visuellement
        if (timerSystem != null) timerSystem.FigerTimer();

        // On affiche le message de fin (Elapsed ou Game Over)
        if (prefabFin != null)
        {
            Transform parentFinal = (canvasParent != null) ? canvasParent : transform.parent;
            GameObject instance = Instantiate(prefabFin, parentFinal);
            
            RectTransform rt = instance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }
            instance.SetActive(true);
        }

        // On cache les éléments de jeu pour la propreté visuelle
        if (barreSlider != null) barreSlider.gameObject.SetActive(false);
        if (scoreSystem != null) scoreSystem.gameObject.SetActive(false);

        // On fige le moteur physique et temporel d'Unity
        Time.timeScale = 0; 
    }
}