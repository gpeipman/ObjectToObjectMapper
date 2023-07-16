using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectToObjectMapper
{
    public class MapperManual : ObjectCopyBase
    {
        public override void MapTypes(Type source, Type target)
        {
            
        }

        public override void Copy(object source, object target)
        {
            OrderModel sourceModel = (OrderModel)source;
            OrderModel targetModel = (OrderModel)target;

            targetModel.Id = sourceModel.Id;
            targetModel.CustomerName = sourceModel.CustomerName;
            targetModel.DeliveryAddress = sourceModel.DeliveryAddress;
            targetModel.EstimatedDeliveryDate = sourceModel.EstimatedDeliveryDate;
            targetModel.OrderReference = sourceModel.OrderReference;
        }
    }
}
