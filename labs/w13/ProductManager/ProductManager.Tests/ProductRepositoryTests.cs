using Moq;
using Npgsql;
using ProductManager;
using System.Data;

namespace ProductManager.Tests
{
    [TestClass]
    public class ProductRepositoryTests
    {
        private const string ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=productsdb";

        // Clears the table before each test to ensure a clean state
        private void ResetProductsTable()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM products";
            command.ExecuteNonQuery();
        }

        // Adds some sample products to the database for testing purposes
        private void SeedProducts()
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = """
                 
                INSERT INTO products (name, category, price) 
                VALUES
                ('MacBook', 'Tech', 13000),
                ('Banana', 'Food', 11),
                ('Foundation', 'Beauty', 399)
                """;
                command.ExecuteNonQuery();
        }

        // This test verifies filtering against the actual database.
        [TestMethod]
        [TestCategory("Integration")]
        public void GetProductsByCategory_ReturnsOnlyMatchingProducts()
        {
            // Arrange
            ResetProductsTable();
            SeedProducts();

            var repository = new ProductRepository();

            // Act 
            var result = repository.GetProductsByCategory("Tech");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Tech", result[0].Category);
            Assert.AreEqual("MacBook", result[0].Name);
        }

        // This test verifies filtering with a mocked database response.
        [TestMethod]
        [TestCategory("UnitTest")]
        public void GetProductsByCategory_ShouldReturnOnlyMatchingProducts_FromMockedDatabase()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            var mockReader = new Mock<IDataReader>();

            mockConnection.Setup(connection => connection.Open());
            mockConnection.Setup(connection => connection.Close());
            mockConnection.Setup(connection => connection.CreateCommand()).Returns(mockCommand.Object);

            mockCommand.SetupSet(command => command.CommandText = It.IsAny<string>());
            mockCommand.Setup(command => command.ExecuteReader()).Returns(mockReader.Object);

            mockReader.SetupSequence(reader => reader.Read())
                .Returns(true)
                .Returns(true)
                .Returns(true)
                .Returns(false);

            mockReader.SetupSequence(reader => reader.GetString(0))
                .Returns("MacBook")
                .Returns("Banana")
                .Returns("Foundation");

            mockReader.SetupSequence(reader => reader.GetString(1))
                .Returns("Tech")
                .Returns("Food")
                .Returns("Beauty");

            mockReader.SetupSequence(reader => reader.GetString(2))
                .Returns("13000")
                .Returns("11")
                .Returns("399");

            var repository = new ProductRepository(mockConnection.Object);

            // Act
            var result = repository.GetProductsByCategory("Beauty");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Beauty", result[0].Category);
            Assert.AreEqual("Foundation", result[0].Name);
        }

    }
}
