using HotChocolate.Language;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.OrdersAPI.GraphQL;
using Teach_MGT_Orders.OrdersAPI.MVC;

namespace Teach_MGT_Orders.GraphQLActions
{

    // This is the main EventMessge Handler by Pedro Angulo 
    public class OnEventMessageDefault<T> : EventMessage
    {
        public OnEventMessageDefault(String eventName, String argumentTag, String argumentValue, T outputType)
            : base(CreateEventDescription(eventName, argumentTag, argumentValue), outputType)
        {
        }

        private static EventDescription CreateEventDescription(String _eventName, String _argumentTag, String _argumentValue)
        {
            ArgumentNode argumentNode = new ArgumentNode(_argumentTag, _argumentValue);
            EventDescription eventDescription = new EventDescription(_eventName, argumentNode);
            return eventDescription;
        }
    }

    public class GraphQLSubscription
    {
        public GraphQLSubscription()
        {
        }

        public List<Order> OnOrdersAdded(string customerId, IEventMessage message)
        {
            return (List<Order>)message.Payload;
        }

        public Customer OnCustomerCreated(string id, IEventMessage message)
        {
            return (Customer)message.Payload;
        }
    }

    public class GraphQLSubscriptionType : ObjectType<GraphQLSubscription>
    {
        protected override void Configure(IObjectTypeDescriptor<GraphQLSubscription> descriptor)
        {
           
            descriptor.Field(t => t.OnOrdersAdded(default, default))
                .Type<NonNullType<ListType<OrderType>>>()
                .Argument("customerId", arg => arg.Type<NonNullType<StringType>>());

            descriptor.Field(t => t.OnCustomerCreated(default, default))
                .Type<NonNullType<CustomerType>>()
                .Argument("id", arg => arg.Type<NonNullType<StringType>>());

        }
    }

}
