namespace SimMarket
{
	public class Customer
	{
		public int Id { get; private set; }
		public int ProductsToCollect { get; private set; }
		public int IterationsToCollect { get; private set; }
		public int IterationsToCollectProducts { get; private set; }
		public Customer(int id, int productsToCollect, int iterationsToCollect, int iterationsToCollectProducts)
		{
			Id = id;
			ProductsToCollect = productsToCollect;
			IterationsToCollect = iterationsToCollect;
			IterationsToCollectProducts = iterationsToCollectProducts;
		}
	}
}
