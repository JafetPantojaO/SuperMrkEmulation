namespace SimMarket
{
    public class Customer
    {
        private static int _nextId = 1;

        public int Id { get; }

        public int TotalProducts { get; }
        public int PendingProducts { get; private set; }

        public int CollectionInterval { get; }
        public int CollectionAmount { get; }
        public int IterationsSinceLastCollection { get; private set; }

        public bool IsPreferent { get; }

        //Estadowhs de la simulación
        public bool ReadyForQueue { get; set; } 
        public bool IsInCheckoutQueue { get; set; }

        public bool FinishedCollecting => PendingProducts == 0;

        public Customer(int totalProducts, int collectionInterval, int collectionAmount, bool isPreferent)
        {
            Id = _nextId++;
            TotalProducts = totalProducts;
            PendingProducts = totalProducts;
            CollectionInterval = collectionInterval;
            CollectionAmount = collectionAmount;
            IsPreferent = isPreferent;
            IterationsSinceLastCollection = 0;
        }

        //Se llama cada iteración mientras esté recolectando
        public void CollectIteration()
        {
            if (FinishedCollecting) return;

            IterationsSinceLastCollection++;
            if (IterationsSinceLastCollection >= CollectionInterval)
            {
                IterationsSinceLastCollection = 0;
                PendingProducts -= CollectionAmount;
                if (PendingProducts < 0) PendingProducts = 0;
            }
        }

        public override string ToString()
        {
            return $"Cliente#{Id} (prod:{PendingProducts}/{TotalProducts}, int:{CollectionInterval}, cant:{CollectionAmount}, pref:{IsPreferent})";
        }
    }
}
