using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{

    [Space(10)]
    public Image imgAnswer;
    public Text txtAnswer;
    public float timeEffect = 1.2f;


    public string value;
    private Color _originColor;


    #region UNITY
    private void Start()
    {
        _originColor = imgAnswer.color;
    }

    // private void Update()
    // {
    // }
    #endregion



    public void OnClickButtonAnswer()
    {
        GameScene.Instance.AnswerTheQuestion(value);
    }


    public void SetValue(string newValue)
    {
        value = newValue;
        txtAnswer.text = value;
    }


    public void PlayEffectRight()
    {
        transform.DORotate(new Vector3(0, 0, 90), timeEffect);

        Sequence effectSq = DOTween.Sequence();
        effectSq
                // .Append(transform.DOScale(Vector3.one * 1.25f, timeEffect))
                .Append(txtAnswer.DOFade(0, timeEffect))
                .Append(imgAnswer.DOFade(0, timeEffect))
                .OnComplete(() =>
                {
                    transform.DOKill();
                    transform.localRotation = Quaternion.identity;
                });
    }


    public void PlayCorrectAnimation()
    {
        transform.DOKill();

        transform.GetComponent<Image>().DOFade(0, 1);
        transform.transform.DOScale(Vector3.one * 1.5f, 1).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.transform.localScale = Vector3.one;
                transform.GetComponent<Image>().DOFade(1, 0);
            });
    }


    public void PlayEffectWrong()
    {
        var timeEffect = 0.5f;

        imgAnswer.DOColor(Color.black, timeEffect);
        transform.DOShakeScale(timeEffect, Vector3.one * 0.25f, 20, 0)
            .OnComplete(() => { transform.localScale = Vector3.one; });
    }


    public void RefeshTurn()
    {
        transform.DOKill();

        imgAnswer.color = _originColor;
        imgAnswer.DOFade(1, 0);
        txtAnswer.DOFade(1, 0);

        transform.localRotation = Quaternion.identity;
        transform.transform.localScale = Vector3.one;
        transform.GetComponent<Image>().DOFade(1, 0);
    }


}
