using System;
using System.Collections.Generic;

namespace MonoGame.Training.Collections
{
    public class PackedArray<T>
    {
        private readonly T[] _array;
        private readonly int?[] _mappedIndexes;
        private readonly Dictionary<int, int> _indexesByMappedIndex;
        public int Count { get; private set; }
        public PackedArray(int size)
        {
            _array = new T[size];
            _mappedIndexes = new int?[size];
            _indexesByMappedIndex = new Dictionary<int, int>();
            Count = 0;
        }

        public T Get(int index)
        {
            int mappedIndex = (int)_mappedIndexes[index];
            return _array[mappedIndex];
        }

        public void Insert(int index, T item)
        {
            if (_mappedIndexes[index] != null)
            {
                RemoveAt(index);
            }

            int mappedIndex = Count;

            _mappedIndexes[index] = mappedIndex;
            _indexesByMappedIndex[mappedIndex] = index;
            _array[mappedIndex] = item;

            ++Count;
        }

        public void RemoveAt(int index)
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("No elements to remove");
            }
            /*if (index < 0 || index >= Count)
            {

            }*/

            int mappedIndex = (int)_mappedIndexes[index];
            int lastMappedIndex = Count - 1;

            if (mappedIndex != lastMappedIndex)
            {
                // Move last element to index we're removing to keep packed
                _array[mappedIndex] = _array[lastMappedIndex];

                int indexForLastMappedIndex = _indexesByMappedIndex[lastMappedIndex];

                _indexesByMappedIndex[mappedIndex] = indexForLastMappedIndex;
                _mappedIndexes[indexForLastMappedIndex] = mappedIndex;
            }

            // Remove last element
            _array[lastMappedIndex] = default;
            _mappedIndexes[index] = null;
            _indexesByMappedIndex.Remove(lastMappedIndex);

            --Count;
        }
    }
}
