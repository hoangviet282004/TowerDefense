using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float range;
    public float shootInterval;
    public float projectileSpeed;
    public float projecttileDuration;
    public float damage;
    public float projectileSize; // Size shoot

    public int cost;
    public Sprite sprite; // Change UI

    public GameObject prefab;
}
