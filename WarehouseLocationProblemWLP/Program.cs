using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace WarehouseLocationProblemWLP
{
    class WLP
    {
        public static void Main(string[] args)
        {
            string filePath = "wl_500_1";

            List<double> warehouseCapacitiesList = new List<double>();
            List<double> warehouseCostList = new List<double>();
            int warehouseCount;
            int customerCount;

            List<int> customerDemandsList = new List<int>();//customerDemands

            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] datas = File.ReadAllLines(filePath);
                string line = sr.ReadLine();
                string[] firstLine = line.Split(' ');
                warehouseCount = int.Parse(firstLine[0].Trim());
                customerCount = int.Parse(firstLine[1].Trim());

                for (int i = 0; i < warehouseCount; i++)
                {
                    line = sr.ReadLine();
                    string[] lines = line.Split(' ');
                    warehouseCapacitiesList.Add(double.Parse(lines[0].Trim()));
                    warehouseCostList.Add(double.Parse(lines[1].Trim()));
                }
            }

            double[,] CustomerCostForWarehouse = new double[customerCount, warehouseCount];//customerCosts

            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] datas = File.ReadAllLines(filePath);
                int j = 0;
                int l = 0;
                for (int i = warehouseCount + 1; i < (customerCount * 2) + 1 + warehouseCount; i++)
                {//wl_3_1 dosyasında i % 2 == 0 ile çalışacaz, diğerlerinde i % 2 != 0 ile

                    if (i % 2 != 0)
                    {
                        customerDemandsList.Add(int.Parse(datas[i]));

                    }
                    else
                    {
                        string[] data = datas[i].Split(' ');
                        for (int k = 0; k < warehouseCount; k++)
                        {
                            CustomerCostForWarehouse[l, k] = double.Parse(data[k]);
                        }
                        l++;
                    }
                    j++;

                }
            }


            int[] warehouseCapacities = warehouseCapacitiesList.Select(x => (int)x).ToArray();
            double[] warehouseCosts = warehouseCostList.Select(x => (double)x).ToArray();

            int[] customerDemands = customerDemandsList.Select(x => (int)x).ToArray();




            int[] assignedWarehouses = new int[customerCount];
            double totalCost = SolveWLP(warehouseCount, customerCount, warehouseCapacities, warehouseCosts, customerDemands, CustomerCostForWarehouse, assignedWarehouses);

            Console.WriteLine("Optimum maliyet: " + totalCost);
            Console.WriteLine(string.Join(" ", assignedWarehouses.Select((w, i) => $"{w}")));
        }

        static double SolveWLP(int warehouseCount, int customerCount, int[] warehouseCapacities, double[] warehouseCosts, int[] customerDemands, double[,] CustomerCostForWarehouse, int[] assignedWarehouses)
        {
            double totalCost = 0;
            bool[] warehouseUsed = new bool[warehouseCount];

            for (int c = 0; c < customerCount; c++)
            {
                int bestWarehouse = -1;
                double bestCost = double.MaxValue;

                for (int w = 0; w < warehouseCount; w++)
                {
                    if (warehouseCapacities[w] >= customerDemands[c] && CustomerCostForWarehouse[c, w] < bestCost)
                    {
                        bestWarehouse = w;
                        bestCost = CustomerCostForWarehouse[c, w];
                    }
                }

                if (bestWarehouse != -1)
                {
                    assignedWarehouses[c] = bestWarehouse;
                    warehouseCapacities[bestWarehouse] -= customerDemands[c];
                    totalCost += bestCost;

                    if (!warehouseUsed[bestWarehouse])
                    {
                        totalCost += warehouseCosts[bestWarehouse];
                        warehouseUsed[bestWarehouse] = true;
                    }
                }
            }

            return totalCost;
        }
    }
}