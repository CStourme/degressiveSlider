using UnityEngine;
using TMPro;
using System.IO;

public class Total : MonoBehaviour
{
    [Header("Affichage")]
    private TextMeshProUGUI scoreText; // Le texte sur lequel est ce script
    
    [Header("Sauvegarde")]
    private int highscore = 0;
    private string savePath;

    void Awake()
    {
        // On récupère le composant texte du GameObject "TMP : Total"
        scoreText = GetComponent<TextMeshProUGUI>();
        
        // Chemin du fichier : C:/Users/Nom/AppData/LocalLow/DefaultCompany...
        savePath = Application.persistentDataPath + "/highscoredat.json";
        
        // On charge le record dès que le jeu s'éveille
        LoadHighScore();
    }

    // Cette fonction sera appelée par HandleSlide à chaque image
    public void MettreAJourAffichage(float scoreActuel)
    {
        int scoreEntier = Mathf.FloorToInt(scoreActuel);
        
        // On affiche le score actuel ET le record
        if (scoreText != null)
        {
            scoreText.text = "Score : " + scoreEntier + "\n" +
                             "<size=60%>Record : " + highscore + "</size>";
        }

        // Si on bat le record, on enregistre
        if (scoreEntier > highscore)
        {
            highscore = scoreEntier;
            SaveHighScore();
        }
    }

    // --- LOGIQUE JSON ---

    void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.highscore = highscore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    void LoadHighScore()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            highscore = data.highscore;
        }
    }
}

// Petit conteneur pour le JSON
[System.Serializable]
public class SaveData
{
    public int highscore;
}