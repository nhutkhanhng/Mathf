using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewGameOver : View
{

    // inspector
    [Space(10)]
    [SerializeField] private Text textScore;
    [SerializeField] private Text txtPointCollected;

    [Space(10)]
    [SerializeField] private Text textHighScore;
    [SerializeField] private Transform highScorePanel;


    public Text userName;
    public Text txtPoints;

    public void Receive(UserInfo userInfo)
    {
        userName.text = userInfo.name;
        txtPoints.text = userInfo.points.ToString();
    }

    private bool _isDoneGameover;



    #region  UNITY
    // private void Start()
    // {
    // }

    // private void Update()
    // {
    // }
    #endregion



    #region STATE
    public override void StartState()
    {
        base.StartState();
        StartView();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        UpdateView();
    }

    public override void EndState()
    {
        base.EndState();
        EndView();
    }
    #endregion



    private void StartView()
    {
        Receive(GameMgr.Instance.userInfo);
        ShowScore();        
    }

    private void UpdateView()
    {
    }

    private void EndView()
    {
    }


    public void OnClickButtonReplay()
    {
        GameMgr.Instance.ReplayGame();
        SoundMgr.Instance.PlaySFX(SoundMgr.SFX_CLICK);
    }


    private async void ShowScore()
    {
        int score = ScoreMgr.Instance.score;
        textScore.text = score.ToString();

        int highScore = ScoreMgr.Instance.highscore;
        textHighScore.text = highScore.ToString();

        highScorePanel.gameObject.SetActive(score >= highScore);
        txtPointCollected.text = GameScene.Instance._coinCollcected.ToString();
        try
        {
            // await CoreGame.CoreClient.Instance.ReportHightScore("archer_score", score);
        }
        catch
        {
        }
    }


}
