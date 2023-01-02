
# Dapper Encapsulations And Batching

A set of types for encapsulating database operations and a way to execute them batched in a single round trip using Dapper.

## Usage

### 1. Create classes that inherit from any of the types provided:

- `QueryScalar<T>`: Represents a query that returns a single scalar value of type T.
- `QuerySingle<T>`: Represents a query that returns a single row in an object of type T.
- `QueryList<T>`: Represents a query that returns multiple rows in an `IEnumerable<T>` of type T.
- `ExecuteCommand`: Represents a command that does not return a result set.

```C#
public sealed class QueryCustomerCount : QueryScalar<int>
{
	private const string _SQL = "SELECT COUNT(*) FROM Customers;";
    public QueryCustomerCount () : base(_SQL) {}
}

public sealed class QueryCustomerById : QuerySingle<Customer>
{
	private const string _SQL = "SELECT * FROM Customers WHERE CustomerId = @Id;";
    public QueryCustomerById (int id) : base(_SQL, new { Id = id }) {}
}

public sealed class QueryOrdersByCustomerId : QueryList<Order>
{
	private const string _SQL = "SELECT * FROM Orders WHERE CustomerId = @Id;";
    public QueryOrdersByCustomerId (int id) : base(_SQL, new { Id = id }) {}
}

public sealed class UpdateCustomerName : ExecuteCommand
{
	private const string _SQL = "UPDATE Customers SET Name = @Name WHERE CustomerId = @Id;";
    public UpdateCustomerName (int id, string name) : base(_SQL, new { Id = id, Name = name }) {}
}
```

### 2. Instantiate the classes you just created, pass them as parameters to the `ExecuteAsync` extension method, and use the results stored in the `Result` property of each operation:

```C#
// Instantiate the classes you just created
var query1 = new QueryCustomerCount();
var query2 = new QueryCustomerById(1);
var query3 = new QueryOrdersByCustomerId(1);
var query4 = new UpdateCustomerName(1, "John Smith");

using (var dbConnection = new SqlConnection(connectionString))
{
    await dbConnection.OpenAsync();

	// Pass them as parameters to the `ExecuteAsync` extension method
    await dbConnection.ExecuteAsync(query1, query2, query3, query4);
}

// Access the results of the queries
int count = query1.Result.Value;
var customer = query2.Result;
var orders = query3.Result;
```
