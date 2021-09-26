using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Framework;
using JetBrains.Annotations;
using UnityEngine;

public class Inventory : MonoBehaviour, INotifyPropertyChanged
{
    public const string GoldCoinId = nameof(GoldCoin);
    public const string BoostId = nameof(Boost);
    private const string PersistantFileName = "Inventory";

    private Dictionary<string, int> _items;

    public int GoldCoin => GetItem(GoldCoinId);
    public int Boost => GetItem(BoostId);
    
    void Awake()
    {
        _items = FileManager.LoadFromFile<Dictionary<string, int>>(PersistantFileName);
        if (_items == null)
        {
            Debug.Log("Inventory file not found, first launch? creating new one.");
            _items = new Dictionary<string, int>();
        }
    }

    public void Add(string itemName, int itemCount)
    {
        int currentValue = GetItem(itemName);
        SetItem(itemName, currentValue + itemCount);
    }
    
    public bool TryUse(string itemName, int itemCount)
    {
        int currentValue = GetItem(itemName);
        if (currentValue < itemCount)
            return false;
        
        SetItem(itemName, currentValue - itemCount);
        return true;
    }
    
    public int GetItem(string itemName)
    {
        return _items.TryGetValue(itemName, out int itemCount) ? itemCount : 0;
    }
    
    private void SetItem(string itemName, int itemCount)
    {
        if (GetItem(itemName) == itemCount)
            return;
        if (itemCount > 0)
            _items[itemName] = itemCount;
        else
            _items.Remove(itemName);
        FileManager.WriteToFile(PersistantFileName, _items);
        OnPropertyChanged(itemName);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
