using System;
using System.Collections;
using System.Collections.Generic;

namespace Spotlight.GUI
{


    public class SuperSet<T> : ISet<T>
    {
        public static readonly SuperSet<T> Instance = new SuperSet<T>();

        private SuperSet()
        {

        }

        #region not implemented
        public int Count => throw new NotImplementedException();
        public bool IsReadOnly => throw new NotImplementedException();
        public void CopyTo(T[] array, int arrayIndex){throw new NotImplementedException();}
        public void ExceptWith(IEnumerable<T> other){throw new NotImplementedException();}
        public IEnumerator<T> GetEnumerator(){throw new NotImplementedException();}
        public void IntersectWith(IEnumerable<T> other){throw new NotImplementedException();}
        public bool IsProperSubsetOf(IEnumerable<T> other) => false;
        public bool IsProperSupersetOf(IEnumerable<T> other) => true;
        public bool IsSubsetOf(IEnumerable<T> other) => false;
        public bool IsSupersetOf(IEnumerable<T> other) => false;
        public bool Overlaps(IEnumerable<T> other){throw new NotImplementedException();}
        public bool SetEquals(IEnumerable<T> other){throw new NotImplementedException();}
        public void SymmetricExceptWith(IEnumerable<T> other){throw new NotImplementedException();}
        public void UnionWith(IEnumerable<T> other){throw new NotImplementedException();}
        void ICollection<T>.Add(T item){throw new NotImplementedException();}
        IEnumerator IEnumerable.GetEnumerator(){throw new NotImplementedException();}
        #endregion

        public bool Add(T item)
        {
            return true;
        }
        public bool Remove(T item)
        { 
            return false; 
        }

        public void Clear()
        {
                
        }

        public bool Contains(T item)
        {
            return true;
        }
    }
}
