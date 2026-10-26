using System;
using QueueDev;

namespace SimMarket
{
    public class CashierLane
    {
        public int Index { get; }
        public bool IsExpress { get; }
        public int ProductsPerIteration { get; }
        public ArrayQueue<Customer> Queue { get; }

        public bool IsFull => Queue.Size >= SimulationConfig.MAX_CUSTOMERS_PER_CASHIER;

        public CashierLane(int index, bool isExpress, int productsPerIteration)
        {
            if (productsPerIteration <= 0)
            {
                throw new ArgumentException("Los productos por iteración deben ser mayores a 0.", nameof(productsPerIteration));
            }

            Index = index;
            IsExpress = isExpress;
            ProductsPerIteration = productsPerIteration;
            Queue = new ArrayQueue<Customer>(SimulationConfig.MAX_CUSTOMERS_PER_CASHIER);
        }

        /// <summary>
        /// Determina si un cliente cumple las condiciones para formarse en esta caja.
        /// - En cajas normales: todos los clientes son elegibles.
        /// - En cajas rápidas: clientes con productos <= EXPRESS_LANE_MAX_PRODUCTS o si son preferentes.
        /// </summary>
        public bool IsEligible(Customer customer)
        {
            if (!IsExpress)
            {
                return true;
            }

            return customer.IsPreferent || customer.TotalProducts <= SimulationConfig.EXPRESS_LANE_MAX_PRODUCTS;
        }

        /// <summary>
        /// Determina si la caja puede aceptar al cliente (es elegible y la caja no está llena).
        /// </summary>
        public bool CanAccept(Customer customer)
        {
            return !IsFull && IsEligible(customer);
        }

        /// <summary>
        /// Agrega un cliente a la cola de la caja si tiene espacio disponible.
        /// </summary>
        public void Enqueue(Customer customer)
        {
            if (IsFull)
            {
                throw new InvalidOperationException($"La caja #{Index} está llena.");
            }

            Queue.Enqueue(customer);
        }

        /// <summary>
        /// Calcula el tiempo de espera estimado de la caja en iteraciones según la fórmula del PDF:
        /// Tj = sum_{i=1}^{nj} ceil(Pij / Vj)
        /// donde Pij son los artículos pendientes de procesar del cliente i y Vj es ProductsPerIteration.
        /// </summary>
        public int EstimatedWaitTime()
        {
            if (Queue.Empty)
            {
                return 0;
            }

            int totalWaitTime = 0;
            foreach (Customer customer in Queue)
            {
                if (customer.PendingCheckoutProducts > 0)
                {
                    totalWaitTime += (int)Math.Ceiling((double)customer.PendingCheckoutProducts / ProductsPerIteration);
                }
            }

            return totalWaitTime;
        }

        /// <summary>
        /// Procesa los artículos del cliente en la primera posición de la cola en esta iteración.
        /// Si el cliente finaliza todos sus artículos, se retira de la cola.
        /// El siguiente cliente comenzará a ser atendido en la siguiente iteración.
        /// </summary>
        public void ProcessIteration()
        {
            if (Queue.Empty)
            {
                return;
            }

            Customer current = Queue.Head();
            current.ProcessCheckout(ProductsPerIteration);

            if (current.FinishedCheckout)
            {
                Queue.Dequeue();
            }
        }

        public override string ToString()
        {
            string laneType = "Normal";
            if (IsExpress)
            {
                laneType = "Rápida";
            }
            return $"Caja #{Index} [{laneType}] (Capacidad: {Queue.Size}/{SimulationConfig.MAX_CUSTOMERS_PER_CASHIER}, Vel: {ProductsPerIteration} prod/iter, Espera est: {EstimatedWaitTime()} iter) - Cola: {Queue}";
        }
    }
}
