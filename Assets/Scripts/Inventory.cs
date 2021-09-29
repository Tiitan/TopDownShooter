using System;
using System.Collections.Generic;
using System.IO;
using Framework;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int GoldCoinId = 0;
    public const int BoostId = 1;
    private const string PersistantFileName = "Inventory";

    private Dictionary<int, int> _items;

    public event EventHandler<(int,int)> InventoryChanged;

    public int GoldCoin => GetItem(GoldCoinId);
    public int Boost => GetItem(BoostId);
    
    void Awake()
    {
        _items = Deserialize(FileManager.LoadFromFile(PersistantFileName));
    }

    public void Add(int itemId, int itemCount)
    {
        int currentValue = GetItem(itemId);
        SetItem(itemId, currentValue + itemCount);
    }
    
    public bool TryUse(int itemId, int itemCount)
    {
        int currentValue = GetItem(itemId);
        if (currentValue < itemCount)
            return false;
        
        SetItem(itemId, currentValue - itemCount);
        return true;
    }
    
    public int GetItem(int itemId)
    {
        return _items.TryGetValue(itemId, out int itemCount) ? itemCount : 0;
    }
    
    private void SetItem(int itemId, int itemCount)
    {
        if (GetItem(itemId) == itemCount)
            return;
        if (itemCount > 0)
            _items[itemId] = itemCount;
        else
            _items.Remove(itemId);
        FileManager.WriteToFile(PersistantFileName, Serialize());
        InventoryChanged?.Invoke(this, (itemId, itemCount));
    }

    private byte[] Serialize()
    {
        var output = new byte[(_items.Count * 2 + 2) * sizeof(int)];
        var binaryWriter = new BinaryWriter(new MemoryStream(output));

        binaryWriter.Write(_items.Count);
        int checkValue = CheckFile(0, -1, _items.Count);
        int i = 0;
        foreach (var item in _items)
        {
            binaryWriter.Write(item.Key);
            checkValue = CheckFile(checkValue, i, item.Key);
            binaryWriter.Write(item.Value);
            checkValue = CheckFile(checkValue, i, item.Value);
            ++i;
        }
        binaryWriter.Write(checkValue);
        
        return output;
    }

    int CheckFile(int checkvalue, int index, int value)
    {
        return checkvalue + 394275487 * (index + 94271355) * (value + 83719546);
    }
    
    private Dictionary<int, int> Deserialize(BinaryReader binaryReader)
    {
        if (binaryReader == null)
        {
            Debug.Log("Inventory file not found, first launch? creating new one.");
            return new Dictionary<int, int>();
        }

        int itemCount = binaryReader.ReadInt32();
        int checkValue = CheckFile(0, -1, itemCount);
        var output = new Dictionary<int, int>(itemCount);
        for (int i = 0; i < itemCount; ++i)
        {
            int key = binaryReader.ReadInt32();
            int value = binaryReader.ReadInt32();
            output.Add(key, value);
            checkValue = CheckFile(checkValue, i, key);
            checkValue = CheckFile(checkValue, i, value);
        }

        if (checkValue != binaryReader.ReadInt32())
        {
            Debug.Log("Integrity check error, Inventory modified. reset inventory.");
            FileManager.Copy(PersistantFileName, PersistantFileName + "_corrupt");
            return new Dictionary<int, int>();
        }

        return output;
    }
}
