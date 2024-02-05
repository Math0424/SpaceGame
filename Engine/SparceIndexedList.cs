using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    /// <summary>
    /// Will create a key pair value array much like a dictionary
    /// but without any hashing involved, this will be a sparce array
    /// that can efficiently retrieve something by an index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SparceIndexedList<T> where T : class
    {
        private List<T> _contents;
        private List<int> _indexes;
        private int _max;
        public int Count { get; private set; }


        public SparceIndexedList()
        {
            _contents = new List<T>();
            _indexes = new List<int>();
            _max = 0;
        }

        private int FirstFreeIndex()
        {
            if (_indexes.Count == 0)
                return 0;
            for(int i = 0; i < _indexes.Count; i++)
            {
                if (_indexes[i] < 0)
                    return i;
            }
            return _indexes.Count;
        }

        public int Add(T obj)
        {
            int index = FirstFreeIndex();
            if (index >= _max)
            {
                _indexes.Add(-1);
                _max = index;
            }

            if (index < 0)
            {
                index = -index;
                _contents[index] = obj;
                _indexes[index] = index;
            }
            else
            {
                _indexes[index] = _contents.Count;
                _contents.Add(obj);
            }
            Count++;
            return index;
        }

        public void Remove(int id)
        {
            if (id > _max)
                return;
            _contents[_indexes[id]] = null;
            _indexes[id] = -_indexes[id];
            Count--;
        }

        public void Clear()
        {
            _contents.Clear();
            _indexes.Clear();
            _max = 0;
            Count = 0;
        }

        public List<T> GetContents()
        {
            return _contents;
        }

        public T this[int key]
        {
            get
            {
                return _contents[_indexes[key]];
            }
        }

    }
}
