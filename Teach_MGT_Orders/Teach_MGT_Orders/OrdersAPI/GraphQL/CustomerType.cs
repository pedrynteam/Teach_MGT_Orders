using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.Models;
using Teach_MGT_Orders.OrdersAPI.MVC;

namespace Teach_MGT_Orders.OrdersAPI.GraphQL
{
    public class CustomerType : ObjectType<Customer>
    {
        protected override void Configure(IObjectTypeDescriptor<Customer> descriptor)
        {
            descriptor.Field(t => t.CustomerId)
                .Description("The id of the Customer")
                ;

            descriptor.Field(t => t.Name)
                .Description("The name of the Customer")
                ;

            descriptor.Field(t => t.Orders)
                .Type<ListType<OrderType>>()                
                .Name("orders")
                .Argument("index", a => a.Type<IntType>())
                .Argument("take", a => a.Type<IntType>())
                .Resolver(context =>
                {
                    int _index = context.Argument<int>("index");

                    int _take = context.Argument<int>("take");
                    _take = _take == 0 ? 10 : _take;

                    // 1. Where index based
                    // 2. Order By
                    // 3. Take 
                    return context.Service<MVCDbContext>().Order.Where(x => x.CustomerId == context.Parent<Customer>().CustomerId).Where(x => x.OrderId > _index).OrderBy(y => y.OrderId).Take(_take);
                }
                )
                ;
            
        }
    }    
}
