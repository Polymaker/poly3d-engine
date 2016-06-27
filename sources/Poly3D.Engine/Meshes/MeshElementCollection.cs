using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class MeshElementCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection where T : IMeshElement
    {
        private Mesh _Owner;
        private ObservableCollection<T> innerList;
        private bool SuppressCollectionChanged;
        private bool DelayedCollectionChange;
        private NotifyCollectionChangedEventArgs LastChange;

        public Mesh Owner
        {
            get { return _Owner; }
        }

        public T this[int index]
        {
            get { return innerList[index]; }
            set
            {
                innerList[index] = value;
            }
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)innerList).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)innerList).IsSynchronized;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IList)innerList).IsFixedSize;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)innerList)[index];
            }

            set
            {
                ((IList)innerList)[index] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public MeshElementCollection(Mesh owner)
        {
            _Owner = owner;
            LastChange = null;
            innerList = new ObservableCollection<T>();
            innerList.CollectionChanged += InnerCollectionChanged;
        }

        public MeshElementCollection(Mesh owner, IEnumerable<T> source)
        {
            _Owner = owner;
            LastChange = null;
            innerList = new ObservableCollection<T>(source);
            foreach (var item in innerList)
                (item as MeshElement).SetParent(Owner, true);
            innerList.CollectionChanged += InnerCollectionChanged;
        }

        private void InnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SuppressCollectionChanged)
                return;
            if (DelayedCollectionChange)
                LastChange = e;
            else
            {
                OnCollectionChanged(e);
                LastChange = null;
            }
        }

        private void PushLastCollectionChange()
        {
            if (LastChange != null)
            {
                OnCollectionChanged(LastChange);
                LastChange = null;
            }
            DelayedCollectionChange = false;
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, e);
        }

        public int IndexOf(T element)
        {
            return innerList.IndexOf(element);
        }

        public int IndexOf(IMeshElement element)
        {
            return IndexOf((T)element);
        }

        public void Add(T item)
        {
            (item as MeshElement).SetParent(Owner, true);
            innerList.Add(item);
        }

        public void Insert(int index, T item)
        {
            DelayedCollectionChange = true;
            innerList.Insert(index, item);
            (item as MeshElement).SetParent(Owner, true);
            PushLastCollectionChange();
        }

        public void Insert(int index, IMeshElement item)
        {
            Insert(index, (T)item);
        }

        public void Clear()
        {
            foreach (var elem in innerList)
                (elem as MeshElement).SetParent(null, true);
            innerList.Clear();
        }

        public bool Contains(T item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IMeshElement[] array, int arrayIndex)
        {
            innerList.Cast<IMeshElement>().ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            DelayedCollectionChange = true;
            if (innerList.Remove(item))
            {
                (item as MeshElement).SetParent(null, true);
                return true;
            }
            PushLastCollectionChange();
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                (this[index] as MeshElement).SetParent(null, true);
            }
            innerList.RemoveAt(index);
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            if (innerList.Count == 0)
                return 0;

            SuppressCollectionChanged = true;

            var removedItems = new List<T>();
            for (int i = 0; i < innerList.Count; i++)
            {
                if (match(innerList[i]))
                {
                    removedItems.Add(innerList[i]);
                    RemoveAt(i);
                    i--;
                }
            }

            SuppressCollectionChanged = false;
            InnerCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems));

            return removedItems.Count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IList/ICollection

        public void CopyTo(Array array, int index)
        {
            ((ICollection)innerList).CopyTo(array, index);
        }

        public int Add(object value)
        {
            Add((T)((object)value));
            return Count - 1;
        }

        public bool Contains(object value)
        {
            return ((IList)innerList).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)innerList).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        #endregion

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }
    }
}
