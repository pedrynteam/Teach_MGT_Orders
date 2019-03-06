using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.Models;
using Teach_MGT_Orders.OrdersAPI.GraphQL;
using Teach_MGT_Orders.OrdersAPI.MVC;

namespace Teach_MGT_Orders.GraphQLActions
{
    // Uses GraphQL Types - Hot Chocolate combines MVCModel with GraphQLTypes (Sweet!)
    public class GraphQLQuery
    {
        private readonly MVCDbContext _contextMVC;

        public GraphQLQuery(MVCDbContext contextMVC)
        {
            _contextMVC = contextMVC;
        }

        /*
        query {
          customers {
            customerId
            name
            orders {
              orderId
              name
            }
          }
        }
        */
        

        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _contextMVC.Customer.ToListAsync();
        }

        public List<Customer> GetCustomers(List<int> customerIds)
        {
            return (from p in _contextMVC.Customer
                    where customerIds.Contains(p.CustomerId)
                    select p).ToList();
        }
        
        /*
        query {
          customer(id: 2) {
            customerId
            name
            orders {
              orderId
              name
            }
          }
        }
        */
        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await _contextMVC.Customer.FindAsync(id);
        }

        

    }

    public class GraphQLQueryType : ObjectType<GraphQLQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<GraphQLQuery> descriptor)
        {
            descriptor.Field(t => t.GetCustomerAsync(default))
                .Type<CustomerType>()
                .Argument("id", a => a.Type<NonNullType<IntType>>())
                .Name("customer");

            descriptor.Field(t => t.GetCustomers(default))
                .Type<ListType<CustomerType>>()
                .Argument("customerIds",
                    a => a.Type<NonNullType<ListType<NonNullType<IdType>>>>())
                 .Name("CustomerByIds")  
                ;
            
        }
    }
}
