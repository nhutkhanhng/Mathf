using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewMenu : View
{

    public Text txtCoinCollecetd;

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
    }

    private void UpdateView()
    {
    }

    private void EndView()
    {
    }


    public void OnClickButtonPlay()
    {
        GameMgr.Instance.PlayGame();
        SoundMgr.Instance.PlaySFX(SoundMgr.SFX_CLICK);
    }



}
