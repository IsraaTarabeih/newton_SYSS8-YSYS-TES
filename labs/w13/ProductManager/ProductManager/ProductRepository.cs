using System.Data;
using Npgsql;

namespace ProductManager
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public ProductRepository()
        : this(new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=productsdb")) { }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();

            _connection.Open();

            try
            {
                using var command = _connection.CreateCommand();
                command.CommandText = "SELECT name, category, price FROM products";

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Name = reader.GetString(0),
                        Category = reader.GetString(1),
                        Price = reader.GetString(2)
                    });
                }

            }
            finally
            {
                _connection.Close();
            }

            return products.Where(product => product.Category == category).ToList();

        }
    }
}