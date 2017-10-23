using UnityEngine;
using System.Collections;
/// <summary>
/// 材料类，直接继承自Item基类即可
/// </summary>
public class Material : Item {
    public Material(int id, string name, ItemType type, ItemQuality quality, string description, int capaticy, int buyPrice, int sellPrice, string sprite) : base(id, name, type, quality, description, capaticy, buyPrice, sellPrice, sprite) { }
}
