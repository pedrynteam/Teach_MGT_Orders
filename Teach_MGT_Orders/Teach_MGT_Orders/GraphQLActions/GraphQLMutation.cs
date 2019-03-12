using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.OrdersAPI.GraphQL;
using Teach_MGT_Orders.OrdersAPI.MVC;
using Teach_MGT_Orders.OrdersAPI.Transactions;

namespace Teach_MGT_Orders.GraphQLActions
{

    public class GraphQLMutation
    {

        public async Task<CreateCustomerAndOrders_Output> CreateCustomerAndOrdersTxn(CreateCustomerAndOrders_Input input, [Service]IEventSender eventSender)
        {
            CreateCustomerAndOrders_Output output = await new CreateCustomerAndOrdersTxn().Execute(input);
            if (output.resultConfirmation.ResultPassed)
            {
                await eventSender.SendAsync(new OnEventMessageDefault<Customer>(eventName: "onCustomerCreated", argumentTag: "id", argumentValue: "any", outputType: output.customer));
            }
            return output;
        }

        public async Task<AddOrdersToCustomerTxn_Output> AddOrdersToCustomerTxn(AddOrdersToCustomerTxn_Input input, [Service]IEventSender eventSender)
        {
            AddOrdersToCustomerTxn_Output output = await new AddOrdersToCustomerTxn().Execute(input);
            if (output.resultConfirmation.ResultPassed)
            {
                await eventSender.SendAsync(new OnEventMessageDefault<List<Order>>(eventName: "onOrdersAdded", argumentTag: "customerId", argumentValue: output.customer.CustomerId.ToString(), outputType: output.customer.Orders));
            }

            return output;
        }

    }
    
    public class GraphQLMutationType : ObjectType<GraphQLMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<GraphQLMutation> descriptor)
        {
            descriptor.Field(t => t.CreateCustomerAndOrdersTxn(default,default))
                .Type<NonNullType<CreateCustomerAndOrders_OutputType>>()
                .Argument("input", a => a.Type<NonNullType<CreateCustomerAndOrders_InputType>>())
                .Description("Create a new customer with orders")
                ;

            descriptor.Field(t => t.AddOrdersToCustomerTxn(default, default))
                .Type<NonNullType<AddOrdersToCustomerTxn_OutputType>>()
                .Argument("input", a => a.Type<NonNullType<AddOrdersToCustomerTxn_InputType>>())
                .Description("Add orders to a customer")
                ;

        }
    }

}
