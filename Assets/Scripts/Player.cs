using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 模拟主角类
/// </summary>
public class Player : MonoBehaviour {

    //角色基本属性：
    private int basicStrength = 10;
    public int BasicStrength { get { return basicStrength;} }//基本力量

    private int basicIntellect = 10;
    public int BasicIntellect { get { return basicIntellect; } }//基本智力

    private int basicAgility = 10;
    public int BasicAgility { get { return basicAgility; } }//基本敏捷

    private int basicStamina = 10;
    public int BasicStamina { get { return basicStamina; } }//基本体力

    private int basicDamage = 10;
    public int BasicDamage { get { return basicDamage; } }//基本伤害

    private Text coinText;//对金币Text组件的引用
    private int coinAmount = 100;//角色所持有的金币，用于从商贩那里购买物品
    public int CoinAmount {
        get { return coinAmount; }
        set { coinAmount = value; coinText.text = coinAmount.ToString(); }
    }


	// Use this for initialization
	void Start () {
        coinText = GameObject.Find("Coin").GetComponentInChildren<Text>();
        coinText.text = coinAmount.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        //按G键随机得到一个物品
        if (Input.GetKeyDown(KeyCode.G))
        {
            int id = Random.Range(1, 23);
            Knapscak.Instance.StoreItem(id);
        }
        //按下B键控制背包的显示和隐藏
        if (Input.GetKeyDown(KeyCode.B))
        {
            Knapscak.Instance.DisplaySwitch();
        }
        //按下H键控制箱子的显示和隐藏
        if (Input.GetKeyDown(KeyCode.H))
        {
            Chest.Instance.DisplaySwitch();
        }
        //按下V键控制角色面板的显示和隐藏
        if (Input.GetKeyDown(KeyCode.V))
        {
            CharacterPanel.Instance.DisplaySwitch();
        }
        //按下N键商店小贩面板的显示和隐藏
        if (Input.GetKeyDown(KeyCode.N))
        {
            Vendor.Instance.DisplaySwitch();
        }
        //按下M键锻造面板的显示和隐藏
        if (Input.GetKeyDown(KeyCode.M))
        {
           Forge.Instance.DisplaySwitch();
        }
	}

    //消费金币
    public bool ConsumeCoin(int amount) 
    {
        if (coinAmount >= amount)
        {
            coinAmount -= amount;
            coinText.text = coinAmount.ToString();//更新金币数量
            return true;//消费成功
        }
        return false;//否则消费失败
    }

    //赚取金币
    public void EarnCoin(int amount) 
    {
        this.coinAmount += amount;
        coinText.text = coinAmount.ToString();//更新金币数量
    }
}
