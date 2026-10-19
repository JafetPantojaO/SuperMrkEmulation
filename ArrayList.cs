using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsDev
{
    internal class ArrayList<T> : IList<T>
    {

        public const int INITIAL_CAPACITY =10;
        private T[] data;
        public int Capacity { get; private set; }
        public int Size => Size == index + 1;

        public bool Empty => Size == -1;

        public ArrayList(IEnumerable<T> elements): this()
        {
            foreach(var e in elements)
            {
                Add(e);
            }
        }

        public ArrayList(int capacity)
        {
            Capacity = capacity < INITIAL_CAPACITY ? INITIAL_CAPACITY : capacity;
            data = new T[Capacity];
            index = -1;
        }

        public ArrayLists() : this(INITIAL_CAPACITY) { }

        public T Get(int index) => data[index];

        public void Set(int index, T element) => data[index] = element;


        public void Add(T element)
        {
            if(Size == Capacity)
            {
                Capacity *= 2;
                Array.Resize(ref data, Capacity);
            }


            data[++index] = element;
        }


         public T this[int index] 
         { get => data[index]; 
           set => data[index] = value; 
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i  = 0; i < Size; i++){
                yield return data [i];
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddAll(T first, params T[] elements)
        {
            Add(first);

            foreach (T e in elements){
                Add(e);
            }
        }

        public void Insert(int index, T element)
        {
           if(index < 0 || index >= Size)
           {
            throw new IndexOutOfRangeException("Index");
           }

           if(Size == Capacity)
            {
                Capacity *= 2;
                Array.Resize(ref data, Capacity);
            }

            int indexToCopy = this.index +1;
            while(indexToCopy > index)
            {
                data[indexToCopy] = data[indexToCopy - 1];
                indexToCopy--;
            }

            data[index] = element;

        
        }
        public bool Contains(T element)
        {
            throw new NotImplementedException();
        }
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            index = -1;
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Size; i ++)
            {   
                action(data[i]);
            }
        }

        public bool All(Predicate<T> condition)
        {
            for (int i = 0; i < Size; i ++)
            {   
              if (condition(data[i]))
              {
                return false;
              }
            }

            return true;
        }


        public bool Any(Predicate<T> condition)
        {
            for (int i = 0; i < Size; i ++)
            {   
              if (condition(data[i]))
              {
                return true;
              }
            }

            return false;
        }


        public int Count(Predicate<T> condition)
        {
            int count = 0;
            for (int i = 0; i < Size; i ++)
            {   
              if (condition(data[i]))
              {
                count++;
              }
            }
            return count;
        }

        public T Find(Predicate<T> match)
        {
            for (int i = 0; i < Size; i ++)
            {   
              if (match(data[i]))
              {
                return data[i];
              }
            }

            return default(T);
        }

        public IList<T> FindAll(Predicate<T> match)
        {
            var newList = new ArrayList<T>();
        
            for (int i = 0; i < Size; i ++)
            {   
              if (match(data[i]))
              {
                newList.Add(data[i]);
              }
            }

            return newList;
        }


    }
}