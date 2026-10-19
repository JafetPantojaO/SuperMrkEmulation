using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueDev
{
    internal interface IQueue<T>
    {
        int Size { get; } //Tamaño de los datos
        bool Empty { get; } //Ver si esta vacio
        bool Full { get; }  //Ver si esta lleno
        
        void Enqueue(T e);
        T Head();
        T Dequeue();
        T Tail();

    }
}