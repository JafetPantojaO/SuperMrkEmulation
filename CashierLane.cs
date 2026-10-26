using QueueDev;

namespace SimMarket
{
    public class CashierLane
    {
        public int Index { get; }
        public bool IsExpress { get; }
        public int ProductsPerIteration { get; }
        public ArrayQueue<Customer> Queue { get; }

        public bool IsFull => false;

        public CashierLane(int index, bool isExpress, int productsPerIteration)
        {
            Index = index;
            IsExpress = isExpress;
            ProductsPerIteration = productsPerIteration;
            Queue = new ArrayQueue<Customer>(SimulationConfig.MAX_CUSTOMERS_PER_CASHIER);
        }

        public int EstimatedWaitTime()
        {
            return 0;
        }

        public void ProcessIteration()
        {
        }

        public bool CanAccept(Customer customer)
        {
            return false;
        }
    }
}
