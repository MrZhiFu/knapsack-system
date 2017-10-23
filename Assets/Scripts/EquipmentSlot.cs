using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 处理角色面板上的装备物品槽类（有特殊限制的Slot：只能存放装备），不同格子只能存放这个格子应该有的装备。
/// </summary>
public class EquipmentSlot :Slot
{
    public Equipment.EquipmentType equipmentSoltType;//角色装备里每个物品槽的物品类型。因为每个物品槽能装备的物品类型不同，所以才分类型，比如说，头部只能放头部，腿部只能放腿部之类的。主手只能放武器。
    public Weapon.WeaponType wpType;//武器类型，只有副手和主手能有武器，其他为Null



    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)//重写父类Slot的方法
    {
        if (eventData.button == PointerEventData.InputButton.Right)//鼠标右键点击直接实现脱掉，不经过拖拽
        {
            if (transform.childCount > 0 && InventroyManager.Instance.IsPickedItem == false)//需要脱掉的物品得有，并且鼠标上要没有物品，否则就发生：当鼠标上有物品，在其他物品上点击鼠标右键也能脱掉这种情况。
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                Item item = currentItemUI.Item;//临时保存物品信息，防止下面一销毁就没了
                DestroyImmediate(currentItemUI.gameObject);//立即销毁物品槽中的物品
                transform.parent.SendMessage("PutOff", item);//脱掉物品放入背包里（发送消息方式）
                InventroyManager.Instance.HideToolTip();//隐藏该物品的提示框
            }
        }
        if (eventData.button != PointerEventData.InputButton.Left) return;  //只有鼠标左键能够点击物品拖动
        //情况分析：
                //一：鼠标上有物品
                        //1.当前装备槽有装备
                        //2.当前装备槽无装备
               //二：鼠标上没有物品
                        //1.当前装备槽有装备
                       //2.当前装备槽无装备（不做任何处理）
        bool isUpdataProperty = false;//是否需要更新角色属性
        if (InventroyManager.Instance.IsPickedItem == true)//一：鼠标上有物品
        {
            ItemUI pickedItemUI = InventroyManager.Instance.PickedItem;//鼠标上的物品
            if (transform.childCount > 0)//1.当前装备槽有装备
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();//当前物品槽中的物品
                if (IsRightItem(pickedItemUI.Item) && pickedItemUI.Amount == 1)//如果鼠标上的物品是否适合放在该位置并且鼠标上的物品只有一个时，那就交换，否则不做处理
                {
                    pickedItemUI.Exchange(currentItemUI);
                    isUpdataProperty = true;//需要更新角色属性
                }
            }
            else//2.当前装备槽无装备
            {
                if (IsRightItem(pickedItemUI.Item))//如果鼠标上的物品是否适合放在该位置，那就把该物品放入角色面板，并且鼠标上的物品减少一个，否则不做处理
                {
                    this.StoreItem(pickedItemUI.Item);
                    InventroyManager.Instance.ReduceAmountItem(1);
                    isUpdataProperty = true;//需要更新角色属性
                }
            }
        }
        else//二：鼠标上没有物品
        {
            if (transform.childCount>0) //1.当前装备槽有装备，把装备去到鼠标上，销毁物品槽中的物品
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                InventroyManager.Instance.PickUpItem(currentItemUI.Item, currentItemUI.Amount);
                Destroy(currentItemUI.gameObject);
                isUpdataProperty = true;//需要更新角色属性
            } //2.当前装备槽无装备（不做任何处理）
        }
        if (isUpdataProperty == true)
        {
            transform.parent.SendMessage("UpdatePropertyText");
        }
    }

    //判断鼠标上的物品是否适合放在该位置
    public bool IsRightItem(Item item) 
    {
        if ((item is Equipment && ((Equipment)(item)).EquipType == this.equipmentSoltType) || (item is Weapon && ((Weapon)(item)).WpType == this.wpType))//如果此物品是装备并且此物品的装备类型和当前物品槽中的装备类型相同   或者   此物品是武器并且此武器的类型和当前物品槽中的武器类型相同，那就可以合适放在该位置，否则不合适。
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
