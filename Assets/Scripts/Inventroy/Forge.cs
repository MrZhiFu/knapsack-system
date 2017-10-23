using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 锻造类：实现材料锻造成装备或者武器
/// </summary>
public class Forge : Inventroy {

    //单例模式
    private static Forge _instance;
    public static Forge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("ForgePanel").GetComponent<Forge>();
            }
            return _instance;
        }
    }

    private List<Formula> formulaList = null;//用来存放解析出来的材料

    public override void Start()
    {
        base.Start();
        ParseFormulaJSON();
        Hide();
    }
     
    //解析武器锻造配方Jso文件
    public void ParseFormulaJSON()
    {
        formulaList = new List<Formula>();
        TextAsset formulaText = Resources.Load<TextAsset>("Formulas");
        string formulaJson = formulaText.text;
        JSONObject jo = new JSONObject(formulaJson);
        foreach (JSONObject temp in jo.list)
        {
            int item1ID = (int)temp["Item1ID"].n;
            int item1Amount = (int)temp["Item1Amount"].n;
            int item2ID = (int)temp["Item2ID"].n;
            int item2Amount = (int)temp["Item2Amount"].n;
            int resID = (int)temp["ResID"].n;
            Formula formula = new Formula(item1ID, item1Amount, item2ID, item2Amount, resID);
            formulaList.Add(formula);
        }
        //Debug.Log(formulaList[1].ResID);
    }

    /// <summary>
    /// 锻造物品功能：重点
    /// </summary>
    public void ForgeItem() 
    {
        //得到当前锻造面板里面有哪些材料
        List<int> haveMaterialIDList = new List<int>();//存储当前锻造面板里面拥有的材料的ID
        foreach (Slot  slot in slotArray)
        {
            if (slot.transform.childCount > 0)
            {
                ItemUI currentItemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                for (int i = 0; i < currentItemUI.Amount; i++)
                {
                    haveMaterialIDList.Add(currentItemUI.Item.ID);//物品槽里有多少个物品，就存储多少个ID
                }
            }
        }
        //Debug.Log(haveMaterialIDList[0].ToString());
        //判断满足哪一个锻造配方的需求
        Formula matchedFormula = null;
        foreach (Formula formula in formulaList)
        {
            bool isMatch = formula.Match(haveMaterialIDList);
            //Debug.Log(isMatch);
            if (isMatch)
            {     
                matchedFormula = formula;
                break;
            }
        }
       // Debug.Log(matchedFormula.ResID);
        if (matchedFormula != null)
        {
           
            Knapscak.Instance.StoreItem(matchedFormula.ResID);//把锻造出来的物品放入背包
            //减掉消耗的材料
            foreach (int id in matchedFormula.NeedIDList)
            {
                foreach (Slot slot in slotArray)
                {
                    if (slot.transform.childCount > 0)
                    {
                        ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                        if (itemUI.Item.ID == id && itemUI.Amount > 0)
                        {
                            itemUI.RemoveItemAmount();
                            if (itemUI.Amount <= 0)
                            {
                                DestroyImmediate(itemUI.gameObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
