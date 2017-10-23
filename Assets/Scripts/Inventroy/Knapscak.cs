using UnityEngine;
using System.Collections;
/// <summary>
/// 背包类，继承自 存货类Inventroy
/// </summary>
public class Knapscak : Inventroy {
    //单例模式
    private static Knapscak _instance;
    public static Knapscak Instance {
        get {
            if (_instance == null)
            {
                _instance = GameObject.Find("KnapscakPanel").GetComponent<Knapscak>();
            }
            return _instance;
        }
    }
}
