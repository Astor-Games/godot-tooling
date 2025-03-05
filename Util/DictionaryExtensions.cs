using System;
using System.Collections.Generic;
using Godot;

namespace GodotLib.Util;

public static class DictionaryExtensions
{
   
}


public static class KeyValuePairExtensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }
    
    public static void Deconstruct<TKey, T1, T2>(this KeyValuePair<TKey, ValueTuple<T1, T2>> kvp, out TKey key, out T1 item1, out T2 item2)
    {
        key = kvp.Key;
        item1 = kvp.Value.Item1;
        item2 = kvp.Value.Item2;
    }
}
