using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teach_MGT_Orders.GraphQLActions.Resources;
using Teach_MGT_Orders.Models;
using Teach_MGT_Orders.OrdersAPI.GraphQL;
using Teach_MGT_Orders.OrdersAPI.MVC;

namespace Teach_MGT_Orders.OrdersAPI.Transactions
{
    // 1. Create Model: Input and Output
    public class CreateCustomerAndOrders_Input
    {
        public Customer customer { get; set; }
        public List<Order> orders { get; set; }
    }

    public class CreateCustomerAndOrders_Output
    {
        public ResultConfirmation resultConfirmation { get; set; }
        public Customer customer { get; set; } // This will contain the Orders and all the players
    }

    // 2. Input Types.  Input type is used for Mutation, it should be included if needed
    public class CreateCustomerAndOrders_InputType : InputObjectType<CreateCustomerAndOrders_Input>
    {
        
    }

    // 3. Output Types. Output type is used for Mutation, it should be included if needed
    public class CreateCustomerAndOrders_OutputType : ObjectType<CreateCustomerAndOrders_Output>
    {
        protected override void Configure(IObjectTypeDescriptor<CreateCustomerAndOrders_Output> descriptor)
        {
            descriptor.Field(t => t.resultConfirmation)
                .Description("Result confirmation of the call");

            descriptor.Field(t => t.customer)
                .Description("The customer and orders created");

        }
    }

    // 4. Transaction - Logic Controller
    public class CreateCustomerAndOrdersTxn
    {
        public CreateCustomerAndOrdersTxn()
        {
        }

        public async Task<CreateCustomerAndOrders_Output> Execute(CreateCustomerAndOrders_Input _input, MVCDbContext _contextFather = null, bool _autoCommit = true)
        {
            CreateCustomerAndOrders_Output _output = new CreateCustomerAndOrders_Output();
            _output.resultConfirmation = ResultConfirmation.resultBad(_ResultMessage: "TXN_NOT_STARTED");

            // Error handling
            bool error = false; // To Handle Only One Error

            try
            {
                MVCDbContext _contextMGT = (_contextFather != null) ? _contextFather : new MVCDbContext();
                // An using statement is in reality a try -> finally statement, disposing the element in the finally. So we need to take advance of that to create a DBContext inheritance                
                try
                {
                    // DBContext by convention is a UnitOfWork, track changes and commits when SaveChanges is called
                    // Multithreading issue: so if we use only one DBContext, it will track all entities (_context.Customer.Add) and commit them when SaveChanges is called, 
                    // No Matter what transactions or client calls SaveChanges.
                    // Note: Rollback will not remove the entities from the tracker in the context. so better dispose it.

                    //***** 0. Make The Validations - Be careful : Concurrency. Same name can be saved multiple times if called at the exact same time. Better have an alternate database constraint
                    if (_contextMGT.Customer.Any(q => q.Name.Equals(_input.customer.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        error = true;
                        _output.resultConfirmation = ResultConfirmation.resultBad(_ResultMessage: "CUSTOMER_EXISTS", _ResultDetail: _input.customer.Name); // If OK                            
                    }

                    if (!error)
                    {
                        //***** 1. Save the Customer (Atomic because is same Context DB)
                        _input.customer.CustomerId = 0; // Just in case they send other thing. This is for the autoincrement                                                        
                        _contextMGT.Customer.Add(_input.customer);

                        if (_input.orders != null)
                        {
                            foreach (var _item in _input.orders)
                            {
                                _item.OrderId = 0; // Just in case they send other thing. This is for the autoincrement                        
                                _item.Customer = _input.customer;
                                _contextMGT.Order.Add(_item);
                            }
                        }

                        /* Parallelism issue... Analyze the problem and decide:
                         * - What actions can be done before Commit out DB
                         * - What actions can be rolled back
                         * - What actions cannot be rolled back
                         * - What actions can de done later (Delay): i.e. Send confirmation e-mail
                         */

                        //***** 2. Execute events that can be done before the database commits. 
                        //***** If the event fails, return the error, DB automatically rollbacks if SaveChangesAsync is not called

                        /* Call other transactions 
                        // a) Context Inheritance - Commit or not on inner transactions depends on the problem
                        Customer inheritedCustomer = new Customer { CustomerId = 0, Name = "FatherCustomer" };
                        CreateCustomerAndOrders_Input inheritedInput = new CreateCustomerAndOrders_Input { team = inheritedCustomer };
                        CreateCustomerAndOrders_Output inheritedOutput = await CreateCustomerAndOrdersTxn(_input: inheritedInput, _contextFather: _contextMGT, _autoCommit: false);

                        // b) Context No Inheritance - Commit or not on inner transactions depends on the problem
                        Customer inheritedCustomerAlone = new Customer { CustomerId = 0, Name = "FatherCustomerAlone" };
                        CreateCustomerAndOrders_Input inheritedInputAlone = new CreateCustomerAndOrders_Input { team = inheritedCustomerAlone };
                        CreateCustomerAndOrders_Output inheritedOutputAlone = await CreateCustomerAndOrdersTxn(_input: inheritedInputAlone);
                        */

                        //***** 3. Validate results from events. Define error or success

                        //***** 4. Save and Commit to the Database (Atomic because is same Context DB) 
                        if (!error && _autoCommit)
                        {
                            await _contextMGT.SaveChangesAsync(); // Call it only once so do all other operations first
                        }

                        //***** 5. Execute Send e-mails or other events once the database has been succesfully saved
                        //***** If this task fails, there are options -> 1. Retry multiple times 2. Save the event as Delay, 3.Rollback Database, Re

                        //***** 6. Confirm the Result (Pass | Fail) If gets to here there are not errors then return the new data from database
                        _output.resultConfirmation = ResultConfirmation.resultGood(_ResultMessage: "CUSTOMER_SUCCESSFULLY_SAVED"); // If OK
                        _output.customer = _input.customer; // The input customer also have the Players                            
                    }// if (!error)
                }
                finally
                {
                    // If the context Father is null the context was created on his own, so dispose it
                    if (_contextMGT != null && _contextFather == null)
                    {
                        _contextMGT.Dispose();
                    }
                }
            }
            catch (Exception ex) // Main try 
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                string innerError = (ex.InnerException != null) ? ex.InnerException.Message : "";
                System.Diagnostics.Debug.WriteLine("Error Inner: " + innerError);
                _output = new CreateCustomerAndOrders_Output(); // Restart variable to avoid returning any already saved data
                _output.resultConfirmation = ResultConfirmation.resultBad(_ResultMessage: "EXCEPTION", _ResultDetail: ex.Message);
            }
            finally
            {
                // Save Logs if needed
            }

            return _output;
        }

    }

}
