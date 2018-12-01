using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TweenPlayer))]
public class UIAppearTween : MonoBehaviour
{
    public Action OnFinishEvent = null;
    public bool IsScale = true;//缩放动画
    public bool IsFade = false;//渐隐动画// 只用于单图片
    public bool Loop = false;//播放玩循环播待机动画

    private void Update()
    {
       
    }

    private void OnEnable()
    {
        PlayAppearAnim();
    }

    public void OnForceAppearAnim()
    {
        PlayAppearAnim();
    }

    private void PlayAppearAnim()
    {
        iTween.Stop(gameObject);
        if (IsScale)
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        if (IsFade)
        {
            var ui = GetComponent<MaskableGraphic>();
            if (null != ui)
            {
                var color = ui.color;
                color.a = 0;
                ui.color = color;
            }
        }
        TweenPlayer.PlayTween(gameObject, OnFinish);
    }

    private void OnFinish()
    {
        if (null != OnFinishEvent)
        {
            OnFinishEvent();
        }


        if (!Loop)
            return;
        transform.localScale = Vector3.one;
        TweenPlayer.PlayLoopTween(gameObject);
    }
}
