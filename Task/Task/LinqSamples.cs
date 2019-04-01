// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using SampleSupport;
using System;
using System.Linq;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }
        }


        [Category("Homework")]
        [Title("Task 001")]
        [Description("This linq returns a list of all customers whose total turnover exceeds some value of X = 6000")]

        public void Linq001()
        {
            decimal x = 1000;
            var customersList = dataSource.Customers
            .Where(customer => customer.Orders.Sum(order => order.Total) > x);

            foreach (var customer in customersList)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 002")]
        [Description("This sample returns all customers and the list of their suppliers which are situated at the same regions and cities")]

        public void Linq002()
        {
            var customers = dataSource.Customers;
            var suppliers = dataSource.Suppliers;

            // To test using Grouping
            var customersResult =
               from supplier in suppliers
               join customer in customers on new { supplier.Country, supplier.City } equals new { customer.Country, customer.City }
               select new { customer.CompanyName, supplier.SupplierName };
            var query = customersResult.GroupBy(customer => customer.CompanyName);

            foreach (var customer in query)
            {
                ObjectDumper.Write("-------Customer-----");
                ObjectDumper.Write(customer.Key);
                ObjectDumper.Write("-------Collction of suppliers-----");
                foreach (var cust in customer)
                    ObjectDumper.Write(cust.SupplierName);
            }
            //var customersResult =
            //from customer in customers
            //select new
            //{
            //    customer.CompanyName,
            //    suppliersCollection = from supplier in suppliers
            //                          where (customer.City == supplier.City) && (customer.Country == supplier.Country)
            //                          select new { supplier.SupplierName }
            //};


            //foreach (var customer in customersResult)
            //{
            //    if (customer.suppliersCollection.Count() > 0)
            //    {
            //        ObjectDumper.Write("------Customer------");

            //        ObjectDumper.Write(customer.CompanyName);

            //        ObjectDumper.Write("------Collection of suppliers------");
            //        ObjectDumper.Write(customer.suppliersCollection);

            //    }
            //}

        }

        [Category("Homework")]
        [Title("Task 003")]
        [Description("This sample returns all customers which have any order price more than defined number")]

        public void Linq003()
        {
            var number = 1000;
            var customers = dataSource.Customers;

            var customersResult = from customer in customers
                                  select new
                                  {
                                      customer.CompanyName,
                                      orders = from order in customer.Orders
                                               where order.Total > number
                                               select order
                                  };

            foreach (var customer in customersResult)
            {
                if (customer.orders.Count() > 0)
                {
                    ObjectDumper.Write(customer.CompanyName);
                    foreach (var order in customer.orders)
                    {
                        ObjectDumper.Write(order);
                    }
                }

            }
        }
        [Category("Homework")]
        [Title("Task 004")]
        [Description("This sample returns list of customers with the month and year of the first order")]

        public void Linq004()
        {
            var customers = dataSource.Customers;

            var customersResult = from customer in customers
                                  where customer.Orders.Count() > 0
                                  select new
                                  {
                                      customer.CompanyName,
                                      customer.Orders.Min(order => order.OrderDate).Month,
                                      customer.Orders.Min(order => order.OrderDate).Year
                                  };

            foreach (var customer in customersResult)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 005")]
        [Description("This sample returns list of customers with the month and year of the first order ordered by  year, month, total turnover and name")]

        public void Linq005()
        {
            var customers = dataSource.Customers;

            var customersResult = from customer in customers
                                  where customer.Orders.Count() > 0
                                  select new
                                  {
                                      customer.CompanyName,
                                      customer.Orders.Min(order => order.OrderDate).Month,
                                      customer.Orders.Min(order => order.OrderDate).Year,
                                      Turnover = customer.Orders.Sum(order => order.Total)
                                  };

            var query = from customer in customersResult
                        orderby customer.Year, customer.Month, customer.Turnover descending, customer.CompanyName
                        select customer;


            foreach (var customer in query)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 006")]
        [Description("This sample returns list of customers who have a non-numeric postal code or a region is not filled or the operator code is not specified in the phone")]

        public void Linq006()
        {
            var customers = dataSource.Customers;

            var customersResult = from customer in customers
                                  where customer.PostalCode != null
                                  where customer.PostalCode.All(code => char.IsDigit(code)) || customer.Region == null ||
                                  !customer.Phone.Any(symbol => symbol == '(' || symbol == ')')
                                  select new { customer.CompanyName, customer.Region, customer.PostalCode, customer.Phone };

            foreach (var customer in customersResult)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 007")]
        [Description("This sample returns qroups all products by category, inside - by stock, within the last group sort by cost")]

        public void Linq7()
        {
            var groupProducts = dataSource.Products.GroupBy(prod => prod.Category, (category, products) => new
            {
                Category = category,
                Products = products.GroupBy(product => product.UnitsInStock, (units, prod) => new
                {
                    Count = units,
                    Products = prod.OrderBy(cost => cost.UnitPrice).ToList()
                }).ToList()
            });

            foreach (var product in groupProducts)
            {
                ObjectDumper.Write(product.Category);

                foreach (var prod in product.Products)
                {
                    ObjectDumper.Write(prod.Count);

                    foreach (var pr in prod.Products)
                    {
                        ObjectDumper.Write(pr);
                    }
                }
            }
        }

        [Category("Homework")]
        [Title("Task 008")]
        [Description("This sample returns groups the goods into groups Cheap, Average Price, Expensive")]

        public void Linq8()
        {
            var cheapProducts = dataSource.Products.Where(product => product.UnitPrice < 10);
            var averageProducts = dataSource.Products.GroupBy(product => product.UnitPrice >= 10 && product.UnitPrice < 40)
                .Where(group=>group.Key == true);
            var expensiveProducts = dataSource.Products.GroupBy(product => product.UnitPrice >= 40);

            ObjectDumper.Write("----Cheap----");
            foreach (var products in cheapProducts)
            {
              
                ObjectDumper.Write(products);
            }

            ObjectDumper.Write("----Average----");
            foreach (var product in averageProducts)
            {
                ObjectDumper.Write(product);
            }

            ObjectDumper.Write("----High Price----");
            foreach (var product in expensiveProducts)
            {
                ObjectDumper.Write(product);
            }
        }

        [Category("Homework")]
        [Title("Task 009")]
        [Description("This sample calculates the average profitability of each city and the average intensity")]

        public void Linq9()
        {
            var avgIndexes = dataSource.Customers.GroupBy(cust => cust.City, (city, customers) => new
            {
                City = city,
                AvgProfit = customers.Average(customer => customer.Orders.Sum(o => o.Total)),
                Intensivity = (int) customers.Average(c => c.Orders.Length)
            });

            foreach (var index in avgIndexes)
            {
                ObjectDumper.Write(index);
            }
        }


        [Category("Homework")]
        [Title("Task 010")]
        [Description("This sample returns the average annual activity statistics of clients by month, statistics by year, by year and by month")]

        public void Linq10()
        {
            var orders = dataSource.Customers.SelectMany(cust => cust.Orders);

            var byMonth = orders.GroupBy(order => order.OrderDate.Month, (monthNumber, ordersCount) => new
            {
                Month = monthNumber,
                Statistics = ordersCount.Count()
            }).OrderBy(stat => stat.Month);

            ObjectDumper.Write("By Month");

            foreach (var numberOfOrders in byMonth)
            {
                ObjectDumper.Write(numberOfOrders);
            }

            var byYear = orders.GroupBy(order => order.OrderDate.Year, (yearNumber, ordersCount) => new
            {
                Year = yearNumber,
                Statistics = ordersCount.Count()
            }).OrderBy(stat => stat.Year);

            ObjectDumper.Write("By Year");

            foreach (var numberOfOrders in byYear)
            {
                ObjectDumper.Write(numberOfOrders);
            }

            var byYearAndMonth = orders.GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month }, (yearMonth, ordersCount) => new
            {
                YearMonth = yearMonth,
                Statistics = ordersCount.Count()
            });

            ObjectDumper.Write("By Year and Month");

            foreach (var numberOfOrders in byYearAndMonth)
            {
                ObjectDumper.Write($"{numberOfOrders.YearMonth}, {numberOfOrders.Statistics}");
            }
        }
    }
}
