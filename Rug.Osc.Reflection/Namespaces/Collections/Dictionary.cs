using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rug.Osc.Namespaces.Collections
{
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary where TValue : INamespaceObject 
    {
        private readonly System.Collections.Generic.Dictionary<TKey, TValue> dictionary = new System.Collections.Generic.Dictionary<TKey, TValue>();

        /// <inheritdoc />
        public int Count => ((IDictionary<TKey, TValue>)dictionary).Count;

        /// <inheritdoc />
        public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        /// <inheritdoc />
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)dictionary).IsReadOnly;

        /// <inheritdoc />
        public bool IsSynchronized => ((IDictionary)dictionary).IsSynchronized;

        /// <inheritdoc />
        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;

        /// <inheritdoc />
        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        /// <inheritdoc />
        public object SyncRoot => ((IDictionary)dictionary).SyncRoot;

        /// <inheritdoc />
        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dictionary).Values;

        /// <inheritdoc />
        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        /// <inheritdoc />
        public object this[object key]
        {
            get
            {
                return ((IDictionary)dictionary)[key];
            }

            set
            {
                ((IDictionary)dictionary)[key] = value;
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get
            {
                return ((IDictionary<TKey, TValue>)dictionary)[key];
            }

            set
            {
                ((IDictionary<TKey, TValue>)dictionary)[key] = value;
            }
        }

        public Namespace Namespace { get; }

        public Dictionary(Namespace @namespace)
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            Namespace = @namespace;
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)dictionary).Add(item);
        }

        /// <inheritdoc />
        public void Add(object key, object value)
        {
            ((IDictionary)dictionary).Add(key, value);
        }

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>)dictionary).Add(key, value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            ((IDictionary<TKey, TValue>)dictionary).Clear();
        }

        /// <inheritdoc />
        public bool Contains(object key)
        {
            return ((IDictionary)dictionary).Contains(key);
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dictionary).Contains(item);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>)dictionary).ContainsKey(key);
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            ((IDictionary)dictionary).CopyTo(array, index);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)dictionary).GetEnumerator();
        }

        /// <inheritdoc />
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dictionary).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)dictionary).GetEnumerator();
        }

        /// <inheritdoc />
        public void Remove(object key)
        {
            ((IDictionary)dictionary).Remove(key);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dictionary).Remove(item);
        }

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>)dictionary).Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>)dictionary).TryGetValue(key, out value);
        }
    }
}