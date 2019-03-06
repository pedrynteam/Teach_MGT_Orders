using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.Models;
using Teach_MGT_Orders.OrdersAPI.MVC;

namespace Teach_MGT_Orders.OrdersAPI.GraphQL
{
    public class OrderType : ObjectType<Order>
    {
        protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
        {            
            descriptor.Field(x => x.OrderId);
            descriptor.Field(x => x.Name);
            descriptor.Field(t => t.Customer)    
                .Type<CustomerType>()    
                .Name("customer")    
                .Resolver(context => context.Service<MVCDbContext>().Customer.FindAsync(context.Parent<Order>().CustomerId))
                ;
        }
    }
}
