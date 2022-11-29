using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static Lazy<ItemDatabase> Instance;
    public List<Item> itemDb = new List<Item>();

    private void Awake()
    {
        Instance = new Lazy<ItemDatabase>(() => this, true);
        foreach (Item item in Resources.FindObjectsOfTypeAll<Item>())
        {
            itemDb.Add(item);
        }
    }

    private void Start()
    {
        
    }
}
