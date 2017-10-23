using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 存货总管理类
/// </summary>
public class InventroyManager : MonoBehaviour {
    //单例模式
    private static InventroyManager _instance;
    public static InventroyManager Instance 
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.Find("InventroyManager").GetComponent<InventroyManager>();
            }
           return _instance;
        }
    }
    private List<Item> itemList;//存储json解析出来的物品列表

    private ToolTip toolTip;//获取ToolTip脚本，方便对其管理
    private bool isToolTipShow = false;//提示框是否在显示状态
    private Canvas canvas;//Canva物体
    private Vector2 toolTipOffset =new Vector2(15, -10);//设置提示框跟随时与鼠标的偏移

    private ItemUI pickedItem;//鼠标选中的物品的脚本组件，用于制作拖动功能 
    public ItemUI PickedItem { get { return pickedItem; } }
    private bool isPickedItem = false;//鼠标是否选中该物品
    public bool IsPickedItem { get { return isPickedItem; } }

    void Awake() 
    {
        ParseItemJson();
    }

    void Start() 
    {
        toolTip = GameObject.FindObjectOfType<ToolTip>();//根据类型获取
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pickedItem = GameObject.Find("PickedItem").GetComponent<ItemUI>();
        pickedItem.Hide();//开始为隐藏状态
    }

    void Update() 
    {
        if (isToolTipShow == true && isPickedItem == false) //控制提示框跟随鼠标移动
        {
            Vector2 postionToolTip;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionToolTip);
            toolTip.SetLocalPosition(postionToolTip+toolTipOffset);//设置提示框的位置，二维坐标会自动转化为三维坐标但Z坐标为0
        }
        else if (isPickedItem == true) //控制盛放物品的容器UI跟随鼠标移动
        {
            Vector2 postionPickeItem;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionPickeItem);
            pickedItem.SetLocalPosition(postionPickeItem);//设置容器的位置，二维坐标会自动转化为三维坐标但Z坐标为0
        }
        //物品丢弃功能：
        if (isPickedItem == true && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false )//UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false 表示判断鼠标是否正在在UI上
        {
            isPickedItem = false;
            pickedItem.Hide();
        }
    }

    /// <summary>
    /// 解析Json文件
    /// </summary>
    public void ParseItemJson() 
    {
        itemList = new List<Item>();
        //文本在unity里面是TextAsset类型
        TextAsset itemText = Resources.Load<TextAsset>("Item");//加载json文本
        string itemJson = itemText.text;//得到json文本里面的内容
        //bug.Log(itemJson);
        JSONObject j = new JSONObject(itemJson);
        foreach (var temp in j.list)
        {
            //物品类型字符串转化为枚举类型
            Item.ItemType type = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), temp["type"].str);
            //print(type);
            //下面解析的是物品的共有属性：id，name，quality。。。注：temp["id"].n 的 .n，.str等是json插件的写法，用于解析
            int id = (int)(temp["id"].n);
            string name = temp["name"].str;
            Item.ItemQuality quality = (Item.ItemQuality)System.Enum.Parse(typeof(Item.ItemQuality), temp["quality"].str);
            string description = temp["description"].str;
            int capacity = (int)(temp["capacity"].n);
            int buyPrice = (int)(temp["buyPrice"].n);
            int sellPrice = (int)(temp["sellPrice"].n);
            string sprite = temp["sprite"].str;
            Item item = null;
            switch (type)
            {
                case Item.ItemType.Consumable:
                    int hp = (int)(temp["hp"].n);
                    int mp = (int)(temp["mp"].n);
                    item = new Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, hp, mp);
                    break;
                case Item.ItemType.Equipment:
                    int strength = (int)(temp["strength"].n);
                    int intellect = (int)(temp["intellect"].n);
                    int agility = (int)(temp["agility"].n);
                    int stamina = (int)(temp["stamina"].n);
                    Equipment.EquipmentType equiType = (Equipment.EquipmentType)System.Enum.Parse(typeof(Equipment.EquipmentType), temp["equipType"].str);
                    item = new Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite,strength,intellect,agility,stamina,equiType);
                    break;
                case Item.ItemType.Weapon:
                    int damage = (int)(temp["damage"].n);
                    Weapon.WeaponType weaponType = (Weapon.WeaponType)System.Enum.Parse(typeof(Weapon.WeaponType), temp["weaponType"].str);
                    item = new Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, damage, weaponType);
                    break;
                case Item.ItemType.Material:
                    item = new Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
            }
            itemList.Add(item);//把解析到的物品信息加入物品列表里面
            Debug.Log(item);
        }
    }

    //得到根据 id 得到Item
    public Item GetItemById(int id) 
    {
        foreach (Item item in itemList)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }

    //显示提示框方法
    public void ShowToolTip(string content) 
    {
        if (this.isPickedItem == true) return;//如果物品槽中的物品被捡起了，那就不需要再显示提示框了
        toolTip.Show(content);
        isToolTipShow = true;
    }
    //隐藏提示框方法
    public void HideToolTip() {
        toolTip.Hide();
        isToolTipShow = false;
    }

    //获取（拾取）物品槽里的指定数量的（amount）物品UI
    public void PickUpItem(Item item,int amount)
    {
        PickedItem.SetItem(item, amount);
        this.isPickedItem = true;
        PickedItem.Show();//获取到物品之后把跟随鼠标的容器（用来盛放捡起的物品的容器）显示出来
        this.toolTip.Hide();//同时隐藏物品信息提示框

        //控制盛放物品的容器UI跟随鼠标移动
            Vector2 postionPickeItem;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out postionPickeItem);
            pickedItem.SetLocalPosition(postionPickeItem);//设置容器的位置，二维坐标会自动转化为三维坐标但Z坐标为0

    }

    //从鼠标上减少（移除）指定数量的物品
    public void ReduceAmountItem(int amount = 1) 
    {
        this.pickedItem.RemoveItemAmount(amount);
        if (pickedItem.Amount <= 0)
        {
            isPickedItem = false;
            PickedItem.Hide();//如果鼠标上没有物品了那就隐藏了
        }
    }

    //点击保存按钮，保存当前物品信息
    public void SaveInventory() 
    {
        Knapscak.Instance.SaveInventory();
        Chest.Instance.SaveInventory();
        CharacterPanel.Instance.SaveInventory();
        Forge.Instance.SaveInventory();
        PlayerPrefs.SetInt("CoinAmount", GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount);//保存玩家金币
    }

    //点击加载按钮，加载当前物品
    public void LoadInventory() 
    {
        Knapscak.Instance.LoadInventory();
        Chest.Instance.LoadInventory();
        CharacterPanel.Instance.LoadInventory();
        Forge.Instance.LoadInventory();
        //加载玩家金币
        if (PlayerPrefs.HasKey("CoinAmount") == true)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount = PlayerPrefs.GetInt("CoinAmount");
        }
    }
}
