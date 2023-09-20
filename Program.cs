using System;
using System.Collections.Generic;

[Serializable]
class Item
{
    public int Tag { get; set; }
    public string Value { get; set; }
}

class SimpleDatabase
{
    private List<Item> items;

    public SimpleDatabase()
    {
        items = new List<Item>();
    }

    public void Insert(int tag, string value)
    {
        var newItem = new Item { Tag = tag, Value = value };
        items.Add(newItem);
        Console.WriteLine("Inserted: " + newItem.Tag + ", " + newItem.Value);
    }

    public void Remove(int tag)
    {
        var itemToRemove = items.Find(item => item.Tag == tag);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            Console.WriteLine("Removed: " + itemToRemove.Tag + ", " + itemToRemove.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    public void Update(int tag, string newValue)
    {
        var itemToUpdate = items.Find(item => item.Tag == tag);
        if (itemToUpdate != null)
        {
            itemToUpdate.Value = newValue;
            Console.WriteLine("Updated: " + itemToUpdate.Tag + ", " + itemToUpdate.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }

    public void Search(int tag)
    {
        var itemToSearch = items.Find(item => item.Tag == tag);
        if (itemToSearch != null)
        {
            Console.WriteLine(itemToSearch.Value);
        }
        else
        {
            Console.WriteLine("Item not found");
        }
    }
}
