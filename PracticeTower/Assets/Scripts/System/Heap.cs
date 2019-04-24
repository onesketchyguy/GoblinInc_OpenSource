using System;

namespace LowEngine
{
    public class Heap<T> where T : IHeapItem<T>
    {
        T[] items;

        public int Count { get; private set; }

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(item, items[item.HeapIndex]);
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            items[Count] = item;

            SortUp(item);
            Count++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            Count--;

            items[0] = items[Count];
            items[0].HeapIndex = 0;

            SortDown(items[0]);

            return firstItem;
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex-1)/2;

            while (true)
            {
                T parentItem = items[parentIndex];

                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }
            }
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = (item.HeapIndex * 2) + 1;
                int childIndexRight = (item.HeapIndex * 2) + 2;
                int swapIndex = 0;

                if (childIndexLeft < Count)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < Count)
                    {
                        if (GetLeftChild(item).CompareTo(GetRightChild(item)) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }

        public T GetRightChild(T item)
        {
            int childIndexRight = (item.HeapIndex * 2) + 2;

            return items[childIndexRight];
        }

        public T GetLeftChild(T item)
        {
            int childIndexLeft = (item.HeapIndex * 2) + 1;

            return items[childIndexLeft];
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}