using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rug.Osc.Namespaces.Collections
{
    public class List<T> : IList<T>, IList where T : INamespaceObject
    {
        private readonly System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>();

        /// <inheritdoc />
        public int Count => ((IList<T>)list).Count;

        /// <inheritdoc />
        public bool IsFixedSize => ((IList)list).IsFixedSize;

        public bool IsReadOnly => ((IList<T>)list).IsReadOnly;

        /// <inheritdoc />
        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => ((ICollection)list).SyncRoot;

        public Namespace Namespace { get; }

        public List(Namespace @namespace)
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            Namespace = @namespace;
        }

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                return ((IList<T>)list)[index];
            }

            set
            {
                Namespace.Remove(((IList<T>)list)[index]);

                ((IList<T>)list)[index] = value;

                Namespace.Add(((IList<T>)list)[index]);
            }
        }

        /// <inheritdoc />
        object IList.this[int index]
        {
            get { return ((IList)list)[index]; }
            set
            {
                Namespace.Remove(((IList<T>)list)[index]);

                ((IList)list)[index] = value;

                Namespace.Add(((IList<T>)list)[index]);
            }
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            ((IList<T>)list).Add(item);

            Namespace.Add(item);
        }

        /// <inheritdoc />
        public int Add(object value)
        {
            int index = ((IList)list).Add(value);

            Namespace.Add((T)value);

            return index;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            list.AddRange(collection);

            foreach (T item in collection)
            {
                Namespace.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (T value in list)
            {
                Namespace.Remove((T)value);
            }

            ((IList<T>)list).Clear();
        }

        /// <inheritdoc />
        public bool Contains(object value)
        {
            return ((IList)list).Contains(value);
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return ((IList<T>)list).Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo(array, index);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)list).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)list).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)list).GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(object value)
        {
            return ((IList)list).IndexOf(value);
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return ((IList<T>)list).IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, object value)
        {
            ((IList)list).Insert(index, value);

            Namespace.Add((T)value);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            ((IList<T>)list).Insert(index, item);

            Namespace.Add(item);
        }

        /// <inheritdoc />
        public void Remove(object value)
        {
            ((IList)list).Remove(value);

            Namespace.Remove((T)value);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            bool removed = ((IList<T>)list).Remove(item);

            if (removed == true)
            {
                Namespace.Remove(item);
            }

            return removed; 
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            T value = list[index]; 

            ((IList<T>)list).RemoveAt(index);

            Namespace.Remove(value); 
        }
    }
}