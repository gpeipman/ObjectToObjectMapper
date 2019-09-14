using System;

namespace ObjectToObjectMapper
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderReference { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
    }
}
