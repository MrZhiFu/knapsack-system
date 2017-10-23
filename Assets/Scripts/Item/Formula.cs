using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 配方类：用来保存锻造的所需的材料及锻造出来的结果
/// </summary>
public class Formula{

    public int Item1ID { get; private  set; }//用来锻造的物品1的ID
    public int Item1Amount { get; private set; }//用来锻造的物品1的数量
    public int Item2ID { get; private set; }//用来锻造的物品2的ID
    public int Item2Amount { get; private set; }//用来锻造的物品2的数量

    public int ResID { get; set; }//保存锻造结果的ID

    private List<int> needIDList = new List<int>();//保存锻造所需要材料的ID（相当于锻造的配方）
    public List<int> NeedIDList { get { return needIDList; } }

    public Formula(int item1ID, int item1Amount, int item2ID, int item2Amount, int resID )
    {
        this.Item1ID = item1ID;
        this.Item1Amount = item1Amount;
        this.Item2ID = item2ID;
        this.Item2Amount = item2Amount;
        this.ResID = resID;

        //初始化配方ID
        for (int i = 0; i < Item1Amount; i++)
        {
            needIDList.Add(Item1ID);
        }
        for (int i = 0; i < Item2Amount; i++)
        {
            needIDList.Add(Item2ID);
        }
    }

    /// <summary>
    /// 匹配所提供的物品是否 包含 锻造所需要的物品（利用ID匹配）
    /// </summary>
    /// <returns></returns>
    public bool Match( List<int> idList ) 
    {
        List<int> tempIDList = new List<int>(idList);//临时保存函数参数数据（所拥有的物品ID），防止下面Remove代码移除时被修改
        foreach (int id in needIDList)
        {
            bool isSuccess = tempIDList.Remove(id);
            if (isSuccess == false)
            {
                return false;//拥有的物品和锻造所需的物品匹配失败
            }
        }
        return true;//物品匹配成功
    }

}
