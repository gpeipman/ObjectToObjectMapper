using System;
using System.Diagnostics;

namespace ObjectToObjectMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new OrderModel
            {
                Id = 1,
                CustomerName = "John Doe",
                DeliveryAddress = "Lonely Souls Blvd. 1382", 
                EstimatedDeliveryDate = DateTime.Now, 
                OrderReference = "ODF/SDP/1929242111-237821"
            };
            var target = new OrderModel();

            TestMappers(source, target);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }

        static void TestMappers(object source, object target)
        {
            var mappers = new ObjectCopyBase[]
                              {
                                    new MapperUnoptimized(),
                                    new MapperOptimized(),
                                    //new MapperDynamicCode(),
                                    new MapperLcg()
                              };

            var sourceType = source.GetType();
            var targetType = target.GetType();

            foreach (var mapper in mappers)
            {
                mapper.MapTypes(sourceType, targetType);

                var stopper = new Stopwatch();
                stopper.Start();

                for (var i = 0; i < 100000; i++)
                {
                    mapper.Copy(source, target);
                }

                stopper.Stop();

                var time = stopper.ElapsedMilliseconds / (double)100000;
                Console.WriteLine(mapper.GetType().Name + ": " + time);
            }
        }

        private class OrderModel
        {
            public int Id { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public string OrderReference { get; set; }
            public DateTime EstimatedDeliveryDate { get; set; }
        }
    }
}