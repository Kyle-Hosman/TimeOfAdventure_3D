using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    public string itemName;
    public Sprite itemIcon; // Ensure this property exists
    public enum StatToChange { Health, Mana, Stamina, Strength, Agility, Intelligence, Defense }
    public StatToChange statToChange;
    public int statChangeAmount;

    // ensure the id is always unique
    private void OnValidate()
    {
        #if UNITY_EDITOR
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}
