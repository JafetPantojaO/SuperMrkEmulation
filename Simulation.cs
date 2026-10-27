using System;
using LinkedListDev;
using ListsDev;

namespace SimMarket
{
    public class Simulation
    {
        private readonly Random _random;

        private readonly LinkedListDev.IList<Customer> _customersInStore;
        private readonly LinkedListDev.IList<CashierLane> _lanes;

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
            // Ejemplo: N cajas, las primeras X rápidas
            for (int i = 0; i < SimulationConfig.TOTAL_CASHIER_LANES; i++)
            {
                bool isExpress = i < SimulationConfig.EXPRESS_CASHIER_LANES;
                int productsPerIteration = _random.Next(1, SimulationConfig.CHECKOUT_PRODUCTS + 1);
                _lanes.Add(new CashierLane(i + 1, isExpress, productsPerIteration));
            }
        }

        public void Run()
        {
            Console.WriteLine("=== SIMMARKET 2.0 INICIADO ===");

            while (true)
            {
                Iteration++;

                // 1. Cobro en cajas de los clientes al frente de las filas
                ProcessLanes();

                // 2. Mover a cajas a quienes terminaron de recolectar en el turno anterior
                MoveReadyCustomersToLanes();

                // 3. Recolección de productos en los pasillos
                UpdateCollection();

                // 4. Llegada de nuevos clientes
                GenerateCustomersForIteration();

                // 5. Mostrar en pantalla el estado actual
                PrintIterationState();

                // 6. Pausa para inspeccionar iteración por iteración
                Console.Write("\nPresiona ENTER para avanzar a la siguiente iteración...");
                Console.ReadLine();

                // 7. Evaluar fin de simulación
                if (SimulationCanEnd())
                {
                    break;
                }
            }

            Console.WriteLine("\n=======================================================");
            Console.WriteLine("                 SIMULACIÓN FINALIZADA                 ");
            Console.WriteLine("=======================================================");
            Console.WriteLine($"Iteraciones totales: {Iteration}");
            Console.WriteLine($"Clientes creados: {TotalCustomersCreated}");
            Console.WriteLine($"Clientes que completaron compra: {TotalCustomersFinished}");
            Console.WriteLine($"Clientes que abandonaron: {AbandonedCustomers}");
        }

        public void GenerateCustomersForIteration()
        {
            if (Iteration > SimulationConfig.CUSTOMER_ARRIVAL_ITERATIONS) return;

            int newCount = _random.Next(0, SimulationConfig.NEW_CLIENTS_PER_ITERATION + 1);
            for (int i = 0; i < newCount; i++)
            {
                _customersInStore.Add(CreateRandomCustomer());
                TotalCustomersCreated++;
            }
        }

        private Customer CreateRandomCustomer()
        {
            int totalProducts = _random.Next(1, SimulationConfig.PRODUCTS_TO_COLLECT + 1);
            int interval = _random.Next(1, SimulationConfig.ITERATIONS_TO_COLLECT + 1);
            int amount = _random.Next(1, SimulationConfig.ITERATIONS_TO_COLLECT_PRODUCTS + 1);
            bool preferent = _random.NextDouble() < SimulationConfig.PREFERRED_PROBABILITY;

            return new Customer(totalProducts, interval, amount, preferent);
        }

        public void UpdateCollection()
        {
            for (int i = 0; i < _customersInStore.Size; i++)
            {
                Customer c = _customersInStore.Get(i);

                if (c.ReadyForQueue || c.IsInCheckoutQueue) continue;

                c.CollectIteration();

                if (c.FinishedCollecting)
                {
                    c.ReadyForQueue = true;
                }
            }
        }

        private void MoveReadyCustomersToLanes()
        {
            // Al revés permite eliminar más fácil directo en el índice donde estoy
            for (int i = _customersInStore.Size - 1; i >= 0; i--)
            {
                Customer c = _customersInStore.Get(i);

                if (!c.ReadyForQueue || c.IsInCheckoutQueue) continue;

                if (TryAssignCustomerToLane(c))
                {
                    c.ReadyForQueue = false;
                    c.IsInCheckoutQueue = true;
                }
                else
                {
                    AbandonedCustomers++;
                }

                _customersInStore.RemoveAt(i);
            }
        }

        // Intenta poner a un cliente en una fila del cajero tomando en cuenta la prioridad de colas, las disponibles etc..
        private bool TryAssignCustomerToLane(Customer customer)
        {
            CashierLane? best = null;
            int bestWait = int.MaxValue;

            // Recorre la lista de cajas disponibles 
            for (int i = 0; i < _lanes.Size; i++)
            {
                CashierLane lane = _lanes.Get(i);

                // Si la caja no puede aceptar al cliente por reglas o capacidad, la descarta
                if (!lane.CanAccept(customer)) continue;

                int wait = lane.EstimatedWaitTime();

                // Compara el tiempo de espera
                if (best == null || wait < bestWait)
                {
                    bestWait = wait;
                    best = lane;
                }
                else if (wait == bestWait)
                {
                    // Desempate: Si empatan en tiempo estimado, priorizar caja rápida
                    if (!best.IsExpress && lane.IsExpress)
                    {
                        best = lane;
                    }
                }
            }

            // Si nunca hubo una caja disponible best seguirá siendo nulo y retornará falso (abandono)
            if (best == null) return false;

            customer.ReadyForQueue = false;
            customer.IsInCheckoutQueue = true;
            best.Enqueue(customer);
            return true;
        }

        private void ProcessLanes()
        {
            // Se recorre la lista de cajas y se realiza su respectivo cobro
            for (int i = 0; i < _lanes.Size; i++)
            {
                CashierLane lane = _lanes.Get(i);
                if (!lane.Queue.Empty)
                {
                    Customer current = lane.Queue.Head();
                    lane.ProcessIteration();

                    // Si con este cobro el cliente terminó, se contabiliza
                    if (current.FinishedCheckout)
                    {
                        TotalCustomersFinished++;
                    }
                }
            }
        }

        public void PrintIterationState()
        {
            string estadoPuerta = (Iteration <= SimulationConfig.CUSTOMER_ARRIVAL_ITERATIONS) ? "ABIERTA" : "CERRADA";

            Console.WriteLine("\n=================================================================================");
            Console.WriteLine($" ITERACIÓN {Iteration} | Puerta: {estadoPuerta} | En Pasillos: {_customersInStore.Size} | Atendidos: {TotalCustomersFinished} | Abandonos: {AbandonedCustomers}");
            Console.WriteLine("=================================================================================");

            Console.WriteLine("\n--- ESTADO DE CAJAS ---");
            for (int i = 0; i < _lanes.Size; i++)
            {
                CashierLane lane = _lanes.Get(i);
                string tipo = lane.IsExpress ? "Rápida" : "Normal";
                string cupo = $"{lane.Queue.Size}/{SimulationConfig.MAX_CUSTOMERS_PER_CASHIER}";

                Console.Write($"  Caja {lane.Index} ({tipo,-6}) | Vel: {lane.ProductsPerIteration,2} art/it | Fila: {cupo,5} | T_espera: {lane.EstimatedWaitTime(),2} it");

                if (lane.Queue.Empty)
                {
                    Console.WriteLine(" -> [Libre]");
                }
                else
                {
                    // Armamos la fila visual completa
                    // El primero es el que está cobrando; los demás están esperando en cola
                    var queueItems = new System.Collections.Generic.List<string>();
                    bool primero = true;

                    foreach (Customer c in lane.Queue)
                    {
                        if (primero)
                        {
                            queueItems.Add($"[Cobrando: C#{c.Id} ({c.PendingCheckoutProducts} arts)]");
                            primero = false;
                        }
                        else
                        {
                            string tagPref = c.IsPreferent ? "★" : "";
                            queueItems.Add($"C#{c.Id}{tagPref}({c.PendingCheckoutProducts})");
                        }
                    }

                    Console.WriteLine($" -> {string.Join(" <- ", queueItems)}");
                }
            }

            if (!_customersInStore.Empty)
            {
                Console.WriteLine("\n--- CLIENTES EN PASILLOS ---");
                for (int i = 0; i < _customersInStore.Size; i++)
                {
                    Customer c = _customersInStore.Get(i);
                    string tag = c.IsPreferent ? "[Preferente]" : "[Estándar]";
                    string status = c.ReadyForQueue
                        ? "-> Listo para formarse"
                        : $"-> Recolectando ({c.PendingProducts}/{c.TotalProducts} arts)";

                    Console.WriteLine($"  Cliente #{c.Id,-3} {tag,-15} {status}");
                }
            }
        }

        public bool SimulationCanEnd()
        {
            // 1. Debe haber terminado el período de llegada de clientes
            if (Iteration < SimulationConfig.CUSTOMER_ARRIVAL_ITERATIONS) return false;

            // 2. No deben quedar clientes en pasillos
            if (!_customersInStore.Empty) return false;

            // 3. No deben quedar clientes formados en ninguna caja
            for (int i = 0; i < _lanes.Size; i++)
            {
                if (!_lanes.Get(i).Queue.Empty) return false;
            }

            return true;
        }
    }
}