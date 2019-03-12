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

    /*
    public class OrderInputType : ObjectType<Order>
    {
        protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
        {
            descriptor.Field(t => t.OrderId)
                .Description("The id of the Order")
                ;

            descriptor.Field(t => t.Name)
                .Type<NonNullType<StringType>>()
                .Description("The name of the Order")
                ;
        }
    }
    */

        /*
    public class OrderInputType : InputObjectType<Order>
    {
    }
    */


    public class OrderInputType : InputObjectType<Order>
    {
        /*
        protected override void Configure(IInputObjectTypeDescriptor<Order> descriptor)
        {
            descriptor.Field(t => t.OrderId)
                    .Type<IntType>()
                    .Description("The new order id");

            descriptor.Field(t => t.Name)
                .Type<NonNullType<StringType>>()
                .Description("The name of the order");

            descriptor.Field(t => t.Customer)
                .Ignore();

            descriptor.Field(t => t.CustomerId)
                .Ignore();
        }
        */
        
    }
}
