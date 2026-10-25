using System;
using LinkedListDev;
using ListsDev;

namespace SimMarket
{
    public class Simulation
    {
        private readonly Random _random;

        private readonly IList<Customer> _customersInStore;
        private readonly IList<CashierLane> _lanes;

        public int Iteration { get; private set; }
        public int AbandonedCustomers { get; private set; }
        public int TotalCustomersCreated { get; private set; }
        public int TotalCustomersFinished { get; private set; }

        public Simulation()
        {
            _random = new Random();
            _customersInStore = new ArrayList<Customer>();
            _lanes = new ArrayList<CashierLane>();

            InitializeLanes();
        }

        private void InitializeLanes()
        {
        }

        public void Run()
        {
        }

        public void GenerateCustomersForIteration()
        {
        }

        private Customer CreateRandomCustomer()
        {
            return null!;
        }

        public void UpdateCollection()
        {
        }

        private void MoveReadyCustomersToLanes()
        {
        }

        private bool TryAssignCustomerToLane(Customer customer)
        {
            return false;
        }

        private void ProcessLanes()
        {
        }

        private void RemoveFinishedCustomers()
        {
        }

        public void PrintIterationState()
        {
        }

        private bool SimulationCanEnd()
        {
            return false;
        }
    }
}
