using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{

    public Item Item { get; private set; } //UI上的物品
    public int Amount { get; private  set; }//物品数量

    private Image itemImage;//获取item的Image组件
    private Text amountText;//获取item下子物体用于显示数量的组件

    private float targetScale = 1f;//目标缩放大小
    private Vector3 animationScale = new Vector3(1.4f, 1.4f, 1.4f);
    private float smothing = 4f;//动画平滑过渡时间

    void Awake() //可用 属性get方式替代
    {
        itemImage = this.GetComponent<Image>();
        amountText = this.GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (this.transform.localScale.x != targetScale)//设置物品动画
        {
            float scaleTemp = Mathf.Lerp(this.transform.localScale.x, targetScale, smothing*Time.deltaTime);
            this.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
            if (Mathf.Abs(transform.localScale.x-targetScale) < 0.02f)//插值运算达不到临界值，比较耗性能，加上临界值判断能更好的控制
            {
                this.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
            }
        }
    }

    /// <summary>
    ///更新item的UI显示，默认数量为1个
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item, int amount = 1)
    {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        this.Item = item;
        this.Amount = amount;
        this.itemImage.sprite = Resources.Load<Sprite>(item.Sprite);        //更新UI
        if (this.Amount > 1)
        {
            this.amountText.text = Amount.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }
    /// <summary>
    /// 添加item数量
    /// </summary>
    /// <param name="num"></param>
    public void AddItemAmount(int num = 1)
    {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        this.Amount += num;
        if (this.Amount> 1)//更新UI
        {
            this.amountText.text = Amount.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }
    //设置item的个数
    public void SetAmount(int amount) {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        this.Amount = amount;
        if (this.Amount > 1)//更新UI
        {
            this.amountText.text = Amount.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }

    //减少物品数量
    public void RemoveItemAmount(int amount = 1) 
    {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        this.Amount -= amount;
        if (this.Amount > 1)//更新UI
        {
            this.amountText.text = Amount.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }

    //显示方法
    public void Show() {
        gameObject.SetActive(true);
    }

    //隐藏方法
    public void Hide() {
        gameObject.SetActive(false);
    }

    //设置位置方法
    public void SetLocalPosition(Vector3 position)
    {
        this.transform.localPosition = position;
    }

    //当前物品（UI）和 出入物品（UI）交换显示
    public void Exchange(ItemUI itemUI) 
    {
        Item itemTemp = itemUI.Item;
        int amountTemp = itemUI.Amount;
        itemUI.SetItem(this.Item, this.Amount);
        this.SetItem(itemTemp, amountTemp);
    }
}
