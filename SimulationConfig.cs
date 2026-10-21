namespace SimMarket 
{
	public static class SimulationConfig
	{
		public const int NEW_CLIENTS_PER_ITERATION = 3;  // Número de clientes nuevos por iteración
		public const int PRODUCTS_TO_COLLECT = 20; //Número de productos a recolectar
		public const int ITERATIONS_TO_COLLECT = 5; //Número de iteraciones para recolectar productos
		public const int ITERATIONS_TO_COLLECT_PRODUCTS = 3; //Número de iteraciones para recolectar productos

		public const int TOTAL_CASHIER_LANES = 5; ///Número total de cajas registradoras
		public const int EXPRESS_CASHIER_LANES = 2; //Número de cajas registradoras express
		public const int EXPRESS_LANE_MAX_PRODUCTS = 10; //Número máximo de productos para la caja express
		public const int MAX_CUSTOMERS_PER_CASHIER = 4; //Número máximo de clientes por caja registradora
		public const int CHECKOUT_PRODUCTS = 5; //Número de productos a pagar por iteración

		public const int CUSTOMER_ARRIVAL_ITERATIONS = 50; //Número de iteraciones para la llegada de clientes
	}
}
