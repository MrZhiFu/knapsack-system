using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 物品槽类
/// </summary>
public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler{

    public GameObject itemPrefab;//需要存储的物品预设

    /// <summary>
    ///(重点) 向物品槽中添加（存储）物品，如果自身下面已经有Item了，那就Item.amount++;
    /// 如果没有，那就根据ItemPrefab去实例化一个Item，放在其下面
    /// </summary>
    public void StoreItem(Item item) 
    {
        if (this.transform.childCount == 0)//如果这个物品槽下没有物品，那就实例化一个物品
        {
            GameObject itemGO = Instantiate<GameObject>(itemPrefab) as GameObject;
            itemGO.transform.SetParent(this.transform);//设置物品为物品槽的子物体
            itemGO.transform.localScale = Vector3.one;//正确保存物品的缩放比例
            itemGO.transform.localPosition = Vector3.zero;//设置物品的局部坐标，为了与其父亲物品槽相对应
            itemGO.GetComponent<ItemUI>().SetItem(item);//更新ItemUI
        }
        else
        {
            transform.GetChild(0).GetComponent<ItemUI>().AddItemAmount();//默认添加一个
        }
    }

    //根据索引器得到当前物品槽中的物品类型
    public Item.ItemType GetItemType() 
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }

    //根据索引器得到当前物品槽中的物品ID
    public int GetItemID()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }

    //判断物品个数是否超过物品槽的容量Capacity
    public bool isFiled() 
    {
        ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
        return itemUI.Amount >= itemUI.Item.Capacity; //true表示当前数量大于等于容量，false表示当前数量小于容量
    }

    //接口实现的方法，鼠标覆盖时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.transform.childCount > 0)
        {
            string toolTipText = this.transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
            InventroyManager.Instance.ShowToolTip(toolTipText);//显示提示框
        }
    }
    //接口实现的方法，鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.transform.childCount > 0)
        {
            InventroyManager.Instance.HideToolTip();//隐藏提示框
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)//为虚函数，方便子类EquipmentSlot重写
    {
        if (eventData.button == PointerEventData.InputButton.Right)//鼠标右键点击直接实现穿戴，不经过拖拽
        {
            if (transform.childCount > 0 && InventroyManager.Instance.IsPickedItem == false)//需要穿戴的物品得有，并且鼠标上要没有物品，否则就发生：当鼠标上有物品，在其他物品上点击鼠标右键也能穿上这种情况。
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                if (currentItemUI.Item is Equipment || currentItemUI.Item is Weapon)//只有装备和物品才可以穿戴
                {
                    Item currentItem = currentItemUI.Item;//临时存储物品信息，防止物品UI销毁导致物品空指针
                    currentItemUI.RemoveItemAmount(1);//当前物品槽中的物品减少1个
                    if (currentItemUI.Amount <= 0)//物品槽中的物品没有了
                    {
                        DestroyImmediate(currentItemUI.gameObject);//立即销毁物品槽中的物品
                        InventroyManager.Instance.HideToolTip();//隐藏该物品的提示框
                    }
                    CharacterPanel.Instance.PutOn(currentItem);//直接穿戴
                }
            }
        }
        if (eventData.button != PointerEventData.InputButton.Left) return;  //只有鼠标左键能够点击物品拖拽
        //情况分析如下：
        //一：自身是空
                ///1.pickedItem != null(即IsPickedItem == true)，pickedItem放在这个位置
                        ////①按下Ctrl键，放置当前鼠标上的物品的一个
                        ////②没有按下Ctrl键，放置当前鼠标上物品的所有
                ///2.pickedItem==null(即IsPickedItem == false)，不做任何处理
        //二：自身不是空
                ///1.pickedItem != null(即 IsPickedItem == true)
                        ////①自身的id == pickedItem.id
                                //////A.按下Ctrl键，放置当前鼠标上的物品的一个
                                //////B.没有按下Ctrl键，放置当前鼠标上物品的所有
                                       ///////a.可以完全放下
                                       ///////b.只能放下其中一部分
                        ////②自身的id != pickedItem.id，pickedItem跟当前物品交换
               ///2.pickedItem == null(即IsPickedItem == false)
                        ////①按下Ctrl键，取得当前物品槽中物品的一半
                        ////②没有按下Ctrl键，把当前物品槽里面的物品放到鼠标上

        if (transform.childCount > 0) //二：自身不是空
        {
            ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();//取得当前的物品
            if (InventroyManager.Instance.IsPickedItem == false)///2.pickedItem == null。如果当前没有选中任何物品，即当前鼠标上没有物品
            {
                if (Input.GetKey(KeyCode.LeftControl))////①按下Ctrl键，取得当前物品槽中物品的一半
                {
                    int amountPicked = (currentItem.Amount + 1) / 2;//如果物品为偶数就拾取刚好一般，如果为奇数就拾取一半多一个
                    InventroyManager.Instance.PickUpItem(currentItem.Item, amountPicked);
                    int amountRemained = currentItem.Amount - amountPicked;//拾取后剩余的物品个数
                    if (amountRemained<=0)//如果物品槽中没有剩余的物品了，就销毁ItemUI
                    {
                        Destroy(currentItem.gameObject);
                    }
                    else//如果物品槽中还有剩余的物品，就让它的数量减去被拾取的数量
                    {
                        currentItem.SetAmount(amountRemained);
                    }
                }
                else      ////②没有按下Ctrl键，把当前物品槽里面的物品放到鼠标上
                {
                    //InventroyManager.Instance.PickedItem.SetItemUI(currentItem);//把当前物品槽里的物品复制给PickedItem（跟随鼠标移动）
                    //InventroyManager.Instance.IsPickedItem = true;//注释的这两句合并为下面的一句
                    InventroyManager.Instance.PickUpItem(currentItem.Item, currentItem.Amount);
                    Destroy(currentItem.gameObject);//复制完之后销毁当前物品槽中的物品
                }
            }
            else
            {
                ///1.pickedItem != null(即 IsPickedItem == true)
                    ////①自身的id == pickedItem.id
                        //////A.按下Ctrl键，放置当前鼠标上的物品的一个
                        //////B.没有按下Ctrl键，放置当前鼠标上物品的所有
                                 ///////a.可以完全放下
                                 ///////b.只能放下其中一部分
                ////②自身的id != pickedItem.id，pickedItem跟当前物品交换
                if (currentItem.Item.ID == InventroyManager.Instance.PickedItem.Item.ID)
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (currentItem.Item.Capacity > currentItem.Amount)//如果物品槽里的物品容量大于当前物品数量，那就说明物品槽里还能再放
                        {
                            currentItem.AddItemAmount();//增加物品槽中物品数量
                            InventroyManager.Instance.ReduceAmountItem();//让鼠标上的物品减掉一个(默认移除一个)
                        }
                        else//如果物品槽里的物品容量小于当前物品数量，那就说明物品槽里不能再放了
                        {
                            return;
                        }
                    }
                    else 
                    {
                        if (currentItem.Item.Capacity > currentItem.Amount)//如果物品槽里的物品容量大于当前物品数量，那就说明物品槽里还能再放
                        {
                            int itemRemain = currentItem.Item.Capacity - currentItem.Amount;//物品槽还可以再放的数量
                            if (itemRemain >= InventroyManager.Instance.PickedItem.Amount)//如果物品槽还可以再放的数量大于等于鼠标上的数量，                                那就可以完全放下
                            {
                                currentItem.AddItemAmount(InventroyManager.Instance.PickedItem.Amount);
                                InventroyManager.Instance.ReduceAmountItem(itemRemain);
                            }
                            else//只能放下其中一部分
                            {
                                currentItem.AddItemAmount(itemRemain);
                                InventroyManager.Instance.PickedItem.RemoveItemAmount(itemRemain);
                            }
                        }
                        else//如果物品槽里的物品容量小于当前物品数量，那就说明物品槽里不能再放了
                        {
                            return;
                        }
                    }
                }
                else//②自身的id != pickedItem.id，pickedItem跟当前物品交换
                {
                    //保存当前鼠标捡起物品，用于和物品槽中的物品交换
                    Item pickedItemTemp = InventroyManager.Instance.PickedItem.Item;
                    int pickedItemAmountTemp = InventroyManager.Instance.PickedItem.Amount;

                    //保存当前物品槽中的物品，用于和鼠标上的物品交换
                    Item currentItemTemp = currentItem.Item;
                    int currentItemAmountTemp = currentItem.Amount;
                    //两者交换
                    currentItem.SetItem(pickedItemTemp, pickedItemAmountTemp);//把当前鼠标上的物品放入物品槽
                    InventroyManager.Instance.PickedItem.SetItem(currentItemTemp, currentItemAmountTemp);//把当前物品槽中的物品放在鼠标上
                }
            }
        }
        else//一：自身是空
        { 
            ///1.pickedItem != null(即IsPickedItem == true)，pickedItem放在这个位置
                ////①按下Ctrl键，放置当前鼠标上的物品的一个
                ////②没有按下Ctrl键，放置当前鼠标上物品的所有
            ///2.pickedItem==null(即IsPickedItem == false)，不做任何处理
            if (InventroyManager.Instance.IsPickedItem == true)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    this.StoreItem(InventroyManager.Instance.PickedItem.Item);
                    InventroyManager.Instance.ReduceAmountItem();
                }
                else//②没有按下Ctrl键，放置当前鼠标上物品的所有
                {
                    for(int i = 0 ; i<InventroyManager.Instance.PickedItem.Amount ; i++)
                    {
                        this.StoreItem(InventroyManager.Instance.PickedItem.Item);
                    }
                    InventroyManager.Instance.ReduceAmountItem(InventroyManager.Instance.PickedItem.Amount);
                }
            }
            else
            {
                return;
            }
        }
    }
}
