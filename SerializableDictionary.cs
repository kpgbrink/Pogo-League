using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoLevelMenu
{
    // From http://answers.unity.com/answers/809221/view.html
    // Question: https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html
    // Question by Iwa: https://answers.unity.com/users/138297/iwa.html
    // Answer by christophfranke123: https://answers.unity.com/users/456555/christophfranke123.html

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys = new List<TKey>();

        [SerializeField]
        List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
            {
                const string Format = "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.";
                throw new Exception(string.Format(Format));
            }

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}