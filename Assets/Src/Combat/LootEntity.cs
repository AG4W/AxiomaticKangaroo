using UnityEngine;

public class LootEntity : MonoBehaviour
{
    [SerializeField]int _minItemCount;
    [SerializeField]int _maxItemCount;

    [SerializeField]ShipComponentRarity _topRarity;

    ShipComponent[] _loot;

    public void Initialize()
    {
        Generate();
    }
    public void Initialize(ShipComponentRarity topRarity, int minItemCount, int maxItemCount)
    {
        _topRarity = topRarity;

        _minItemCount = minItemCount;
        _maxItemCount = maxItemCount;

        Generate();
    }

    void Generate()
    {
        _loot = new ShipComponent[Random.Range(_minItemCount, _maxItemCount)];

        for (int i = 0; i < _loot.Length; i++)
            _loot[i] = ItemDB.GetRandom(_topRarity);

        WorldUIManager.getInstance.CreateContactItem("Loot", "Contains many items", this.transform.position);
    }
}
