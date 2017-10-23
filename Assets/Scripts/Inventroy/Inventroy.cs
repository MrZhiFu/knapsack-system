using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
///存货类， 背包和箱子的基类
/// </summary>
public class Inventroy : MonoBehaviour {

    protected Slot[] slotArray;//存放物品槽的数组

    //控制背包的显示和隐藏相关变量
    private float targetAlpha = 1f;//显示目标值
    private float smothing = 4f;//渐变平滑速度
    private CanvasGroup canvasGroupMy;
    public CanvasGroup CanvasGroupMy      //对CanvasGroup的引用，用于制作隐藏显示效果
    {
        get
        {
            if (canvasGroupMy == null)
            {
                canvasGroupMy = GetComponent<CanvasGroup>();
            }
            return canvasGroupMy;
        }
    }

	// Use this for initialization
	public virtual void Start () {//声明为虚函数，方便子类重写
        slotArray = GetComponentsInChildren<Slot>();
         if (canvasGroupMy == null)
            {
                canvasGroupMy = GetComponent<CanvasGroup>();
            }
	}

    void Update() 
    {
        if (this.CanvasGroupMy.alpha != targetAlpha)
        {
            this.CanvasGroupMy.alpha = Mathf.Lerp(this.CanvasGroupMy.alpha, targetAlpha, smothing * Time.deltaTime);
            if (Mathf.Abs(this.CanvasGroupMy.alpha - targetAlpha)<.01f)
            {
                this.CanvasGroupMy.alpha = targetAlpha;
            }
        }
    }
    
    //根据Id存储物品
    public bool StoreItem(int id) 
    {
       Item item = InventroyManager.Instance.GetItemById(id);
       return StoreItem(item);
    }

    //根据Item存储物品(重点)
    public bool StoreItem(Item item) 
    {
        if (item.ID == 0)
        {
            Debug.LogWarning("要存储物品的ID不存在");
            return false;
        }

        if (item.Capacity == 1)//如果此物品只能放一个，那就找一个空的物品槽来存放即可
        {
           Slot slot = FindEmptySlot();
           if (slot == null)//如果空的物品槽没有了
           {
               Debug.LogWarning("没有空的物品槽可供使用");
               return false;//存储失败
           }
           else     //空的物品槽还有，那就把物品添加进去
           {
               slot.StoreItem(item);
           }
        }
        else//如果此物品能放多个
        {
            Slot slot = FindSameIDSlot(item);
            if (slot != null)//找到了符合条件的物品槽，那就把物品存起来
            {
                slot.StoreItem(item);
            }
            else //没有找到符合条件的物品槽，那就找一个没有存放物品的物品槽去存放物品
            {
                Slot emptySlot = FindEmptySlot();
                if (emptySlot != null)
                {
                    emptySlot.StoreItem(item);//空的物品槽去存放物品
                }
                else // 如果连空的物品槽，也就是没有存储物品的物品槽都找不到
                {
                    Debug.LogWarning("没有空的物品槽可供使用");
                    return false;//存储失败
                }
            }
        }
        return true; //存储成功
    }

    //寻找空的物品槽
    private Slot FindEmptySlot() 
    {
        foreach (Slot slot  in slotArray)
        {
            if (slot.transform.childCount == 0)//物品槽下面无子类，说明该物品槽为空，返回它即可
            {
                return slot;
            }
        }
        return null;
    }
    
    //查找存放的物品是相同的物品槽
    private Slot FindSameIDSlot(Item item)
    {
        foreach (Slot slot in slotArray)
        {
            //如果当前的物品槽已经有物品了，并且里面的物品的类型和需要找的类型一样，并且物品槽还没有被填满
            if (slot.transform.childCount >= 1 && item.ID == slot.GetItemID() && slot.isFiled() == false)
            {
                return slot;
            }
        }
        return null;
    }

    //面板的显示方法
    public void Show() 
    {
        this.CanvasGroupMy.blocksRaycasts = true;//面板显示时为可交互状态
        this.targetAlpha = 1;
    }
    //面板的隐藏方法
    public void Hide() 
    {
        this.CanvasGroupMy.blocksRaycasts = false;//面板隐藏后为不可交互状态
        this.targetAlpha = 0;
    }
    //控制面板的显示及隐藏关系
    public void DisplaySwitch() 
    {
        if (this.CanvasGroupMy.alpha == 0)
        {
            Show();
        }
        if (this.CanvasGroupMy.alpha == 1)
        {
            Hide();
        }
    }

    //控制物品信息的保存（ID，Amount数量）
    public void SaveInventory() 
    {
        StringBuilder sb = new StringBuilder();//用来保存物品信息的字符串
        foreach (Slot slot in slotArray)
        {
            if (slot.transform.childCount > 0 )
            {
                ItemUI  itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                sb.Append(itemUI.Item.ID + "," + itemUI.Amount + "-");//用逗号分隔一个物品中的ID和数量，用 - 分隔多个物品
            }
            else
            {
                sb.Append("0-");//如果物品槽里没有物品就是0
            }
        }
        PlayerPrefs.SetString(this.gameObject.name, sb.ToString());//保存字符串数据
    }

    //控制物品信息的加载（根据ID，Amount数量）
    public void LoadInventory() 
    {
        if (PlayerPrefs.HasKey(this.gameObject.name) == false) return;//判断保存的这个关键码Key是否存在,不存在就不做处理
        string str = PlayerPrefs.GetString(this.gameObject.name);//获取上面保存的字符串数据
        string[] itemArray = str.Split('-');//按照  -  分隔多个物品
        for (int i = 0; i < itemArray.Length-1; i++)//长度减1是因为最后一个字符是 “-”，不需要取它
        {
            string itemStr = itemArray[i];
            if (itemStr != "0")
            {
                string[] temp = itemStr.Split(',');//按照逗号分隔这个物品的信息（ID和Amoun数量）
                int id = int.Parse(temp[0]);
                Item item = InventroyManager.Instance.GetItemById(id);//通过物品ID得到该物品
                int amount = int.Parse(temp[1]);
                for (int j = 0; j < amount; j++)//执行Amount次StoreItem方法，一个一个的存
                {
                    slotArray[i].StoreItem(item);
                }
            }
        }
    }
}
