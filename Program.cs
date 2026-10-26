using System;
using SimMarket;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== INICIANDO PRUEBAS DE CASHIERLANE (SIN ANULABLES) ===");

        // Test 1: Creación y propiedades básicas
        var normalLane = new CashierLane(0, isExpress: false, productsPerIteration: 3);
        var expressLane = new CashierLane(1, isExpress: true, productsPerIteration: 4);

        Assert(!normalLane.IsExpress, "normalLane debe ser no express");
        Assert(expressLane.IsExpress, "expressLane debe ser express");
        Assert(normalLane.ProductsPerIteration == 3, "Velocidad de normalLane debe ser 3");
        Assert(expressLane.ProductsPerIteration == 4, "Velocidad de expressLane debe ser 4");
        Assert(!normalLane.IsFull, "normalLane no debe estar llena al inicio");
        Assert(normalLane.EstimatedWaitTime() == 0, "Espera debe ser 0 para cola vacía");

        // Test 2: Elegibilidad y CanAccept
        var normalClientSmall = new Customer(totalProducts: 5, collectionInterval: 2, collectionAmount: 2, isPreferent: false);
        var normalClientLarge = new Customer(totalProducts: 15, collectionInterval: 2, collectionAmount: 2, isPreferent: false);
        var prefClientLarge = new Customer(totalProducts: 15, collectionInterval: 2, collectionAmount: 2, isPreferent: true);

        // Caja normal acepta a todos
        Assert(normalLane.IsEligible(normalClientSmall), "Caja normal acepta cliente pequeño");
        Assert(normalLane.IsEligible(normalClientLarge), "Caja normal acepta cliente grande");
        Assert(normalLane.IsEligible(prefClientLarge), "Caja normal acepta cliente preferente grande");
        Assert(normalLane.CanAccept(normalClientLarge), "normalLane.CanAccept debe ser true");

        // Caja rápida
        Assert(expressLane.IsEligible(normalClientSmall), "Caja rápida acepta cliente <= 10 productos");
        Assert(!expressLane.IsEligible(normalClientLarge), "Caja rápida NO debe aceptar cliente con > 10 productos no preferente");
        Assert(expressLane.IsEligible(prefClientLarge), "Caja rápida acepta cliente preferente aunque exceda 10 productos");

        // Test 3: Llenado de caja (MAX_CUSTOMERS_PER_CASHIER = 4)
        for (int i = 0; i < SimulationConfig.MAX_CUSTOMERS_PER_CASHIER; i++)
        {
            Assert(!normalLane.IsFull, $"normalLane no debe estar llena con {i} clientes");
            normalLane.Enqueue(new Customer(6, 1, 1, false));
        }
        Assert(normalLane.IsFull, "normalLane debe estar llena con MAX_CUSTOMERS_PER_CASHIER clientes");
        Assert(!normalLane.CanAccept(normalClientSmall), "CanAccept debe ser false cuando la caja está llena");

        bool threwOnFull = false;
        try
        {
            normalLane.Enqueue(normalClientSmall);
        }
        catch (InvalidOperationException)
        {
            threwOnFull = true;
        }
        Assert(threwOnFull, "Enqueue en caja llena debe lanzar InvalidOperationException");

        // Test 4: EstimatedWaitTime con fórmula: Tj = sum ceil(Pij / Vj)
        var laneCalc = new CashierLane(2, false, productsPerIteration: 3);
        var c1 = new Customer(totalProducts: 7, 1, 1, false); // ceil(7/3) = 3
        var c2 = new Customer(totalProducts: 4, 1, 1, false); // ceil(4/3) = 2
        var c3 = new Customer(totalProducts: 3, 1, 1, false); // ceil(3/3) = 1
        laneCalc.Enqueue(c1);
        laneCalc.Enqueue(c2);
        laneCalc.Enqueue(c3);

        int expectedWait = 3 + 2 + 1; // 6 iteraciones
        Assert(laneCalc.EstimatedWaitTime() == expectedWait, $"EstimatedWaitTime esperado {expectedWait}, obtenido {laneCalc.EstimatedWaitTime()}");

        // Test 5: ProcessIteration
        // c1 tiene 7 productos. Con Vj = 3:
        // Iteración 1: procesa 3 prod -> quedan 4. No termina.
        laneCalc.ProcessIteration();
        Assert(c1.PendingCheckoutProducts == 4, $"Quedan 4 productos, actual: {c1.PendingCheckoutProducts}");
        Assert(laneCalc.Queue.Size == 3, "Cola sigue teniendo 3 clientes");
        Assert(laneCalc.Queue.Head() == c1, "c1 sigue al frente");
        // Nueva espera estimada: ceil(4/3) + 2 + 1 = 2 + 2 + 1 = 5
        Assert(laneCalc.EstimatedWaitTime() == 5, $"Espera estimada debe ser 5, actual: {laneCalc.EstimatedWaitTime()}");

        // Iteración 2: procesa 3 prod -> queda 1. No termina.
        laneCalc.ProcessIteration();
        Assert(c1.PendingCheckoutProducts == 1, $"Queda 1 producto, actual: {c1.PendingCheckoutProducts}");

        // Iteración 3: procesa 3 prod -> queda 0. Termina y sale de la cola!
        laneCalc.ProcessIteration();
        Assert(c1.FinishedCheckout, "c1.FinishedCheckout debe ser true");
        Assert(laneCalc.Queue.Size == 2, "La cola debe tener 2 clientes ahora");
        Assert(laneCalc.Queue.Head() == c2, "El siguiente cliente al frente debe ser c2");
        // c2 no se procesa en la misma iteración en la que terminó c1
        Assert(c2.PendingCheckoutProducts == 4, "c2 aún no debe haber sido procesado en la misma iteración en que terminó c1");

        // Iteración 4: ahora sí se atiende a c2 (4 productos, procesa 3 -> queda 1)
        laneCalc.ProcessIteration();
        Assert(c2.PendingCheckoutProducts == 1, $"c2 le queda 1 producto, actual: {c2.PendingCheckoutProducts}");

        Console.WriteLine("ToString output: " + laneCalc.ToString());
        Console.WriteLine("=== ¡TODAS LAS PRUEBAS PASARON EXITOSAMENTE SIN NINGÚN ANULABLE! ===");
    }

    static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception("FALLÓ ASSERT: " + message);
        }
    }
}
