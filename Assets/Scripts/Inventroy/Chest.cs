using UnityEngine;
using System.Collections;
/// <summary>
/// 箱子类，继承自 存货类Inventroy
/// </summary>
public class Chest : Inventroy {
    //单例模式
    private static Chest _instance;
    public static Chest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("ChestPanel").GetComponent<Chest>();
            }
            return _instance;
        }
    }
}
