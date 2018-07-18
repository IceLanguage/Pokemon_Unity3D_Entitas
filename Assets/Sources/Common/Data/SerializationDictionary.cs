using PokemonBattele;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataFileManager
{
    [Serializable]
    public class SerializationDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<TKey> keys = new List<TKey>();
        [SerializeField]
        public List<TValue> values = new List<TValue>();

        protected Dictionary<TKey, TValue> target;

        public Dictionary<TKey, TValue> Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
                keys = new List<TKey>(Target.Keys);
                values = new List<TValue>(Target.Values);
            }
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(Target.Keys);
            values = new List<TValue>(Target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (int i = 0; i < count; ++i)
            {
                try
                {
                    Target.Add(keys[i], values[i]);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }


    }
}

