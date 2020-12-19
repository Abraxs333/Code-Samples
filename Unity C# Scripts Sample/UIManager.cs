using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _ScoreText;

    [SerializeField]
    private Text _GameOVerText;

    [SerializeField]
    private Button _RestartButton;

    private int _score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GameOver()
    {
        _GameOVerText.gameObject.SetActive(true);
        _RestartButton.gameObject.SetActive(true);
    }

    public void UpdateScore(int addedscore)
    {
        _score += addedscore;
        _ScoreText.text = $"SCORE: {_score:0000000}";
    }
}
