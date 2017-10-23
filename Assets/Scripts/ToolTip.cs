using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 提示框类
/// </summary>
public class ToolTip : MonoBehaviour {

    private Text toolTipText;//提示框的父Text，主要用来控制提示框的大小
    private Text contentText;//提示框的子Text，主要用来显示提示
    private CanvasGroup toolTipCanvasGroup;//提示框的CanvasGroup组件，用来制作显示和隐藏功能
    private float targetAlpha = 0.0f ;//设置提示框的Alpha值，0代表隐藏，1代表显示
    public float smothing = 1.0f;//用于显示和隐藏的插值运输

    void Awake() 
    {
        toolTipText = this.GetComponent<Text>();
        contentText = this.transform.Find("Content").GetComponent<Text>();
        toolTipCanvasGroup = this.GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        if (toolTipCanvasGroup.alpha != targetAlpha)
        {
            toolTipCanvasGroup.alpha = Mathf.Lerp(toolTipCanvasGroup.alpha, targetAlpha, smothing*Time.deltaTime);
            if (Mathf.Abs(targetAlpha - toolTipCanvasGroup.alpha) < 0.01f)//如果当前提示框的Alpha值与目标Alpha值相差很小，那就设置为目表值
            {
                toolTipCanvasGroup.alpha = targetAlpha;
            }
        }
	}

    //提示框的显示方法
    public void Show(string text) 
    {
        this.toolTipText.text = text;
        this.contentText.text = text;
        this.targetAlpha = 1;
    }
    //提示框的隐藏方法
    public void Hide() 
    {
        this.targetAlpha = 0;
    }

    //设置提示框自身的位置
    public void SetLocalPosition(Vector3 postion) 
    {
       this.transform.localPosition = postion;
    }
}
