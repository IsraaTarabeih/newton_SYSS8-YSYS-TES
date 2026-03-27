using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using Npgsql;

namespace ProductManager
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT name, category, price FROM products WHERE category = @category";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@category", category);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Name = reader.GetString(0),
                    Category = reader.GetString(1),
                    Price = reader.GetDecimal(2)
                });
            }
            return products;

        }
    }
}