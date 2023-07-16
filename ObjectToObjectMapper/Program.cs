using System;
using System.Diagnostics;
using AutoMapper;

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
            TestAutoMapper(source, target);

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
                                    new MapperDynamicCode(),
                                    new MapperLcg(),
                                    new MapperLcg2<OrderModel>(),
                                    new MapperManual(),
                              };            

            var sourceType = source.GetType();
            var targetType = target.GetType();
            var stopper = new Stopwatch();
            var testRuns = 1000000;

            foreach (var mapper in mappers)
            {
                mapper.MapTypes(sourceType, targetType);

                stopper.Restart();

                for (var i = 0; i < testRuns; i++)
                {
                    mapper.Copy(source, target);
                }

                stopper.Stop();

                var time = stopper.ElapsedMilliseconds;
                Console.WriteLine(mapper.GetType().Name + ": " + time);
            }
        }

        static void TestAutoMapper(OrderModel source, OrderModel target)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderModel, OrderModel>();
            });

            var mapper = new Mapper(config);

            mapper.Map(source, target);

            var stopper = new Stopwatch();
            var testRuns = 1000000;

            stopper.Start();

            for (var i = 0; i < testRuns; i++)
            {
                mapper.Map(source, target);
            }

            stopper.Stop();

            var time = stopper.ElapsedMilliseconds;
            Console.WriteLine("AutoMapper: " + time);
        }
    }
}