using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> m_keys;
    [SerializeField] private List<TValue> m_values;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (m_keys == null)
        {
            m_keys = Keys.ToList();
            m_values = Values.ToList();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (Count == 0)
        {
            int length = m_keys.Count;

            Clear();
            for (int i = 0; i < length; i++)
            {
                this[m_keys[i]] = m_values[i];
            }

            m_keys = null;
            m_values = null;
        }
    }
}
