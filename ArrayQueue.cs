using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueDev
{
    public class ArrayQueue<T> : IQueue<T>, IEnumerable<T>
    {
        private int INITIAL_CAPACITY = 10;
        private T[] data;
        private int index;
        public int Size => index + 1;
        public bool Empty => index == -1;
        public bool Full => index == data.Length - 1;
        public int Capacity { get ; private set; }


        public ArrayQueue()
        {
            Capacity = INITIAL_CAPACITY;
            data = new T[Capacity];
            index = -1;
        }

        public ArrayQueue(int capacity)
        {
            Capacity = capacity < INITIAL_CAPACITY ? INITIAL_CAPACITY : capacity;
            data = new T[Capacity];
            index = -1;
        }

        public ArrayQueue(ArrayQueue<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Capacity = other.Capacity;
            data = new T[Capacity];
            index = other.index;
            Array.Copy(other.data, data, other.Size);
        }

        public void Enqueue(T e)
        {
            if (Full)
            {
                throw new InvalidOperationException("La cola está llena.");
            }

            index++;
            data[index] = e;
        }

        public T Dequeue()
        {
            if (Empty)
            {
                throw new InvalidOperationException("La cola está vacía.");
            }

            T item = data[0];

            for (int i = 0; i < index; i++)
            {
                data[i] = data[i + 1];
            }

            data[index] = default!;
            index--;

            return item;
        }

        public T Head()
        {
            if (Empty)
            {
                throw new InvalidOperationException("La cola está vacía.");
            }

            return data[0];
        }

        public T Tail()
        {
            if (Empty)
            {
                throw new InvalidOperationException("La cola está vacía.");
            }

            return data[index];
        }

        public void Clear()
        {
            Array.Clear(data, 0, Size);
            index = -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i <= index; i++)
            {
                yield return data[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (Empty)
            {
                return "[]";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i <= index; i++)
            {
                sb.Append(data[i]);
                if (i < index)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }




    }
}
