namespace ProductManager.Tests
{
    [TestClass]
    public class ProductServiceIntegrationTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void GetProductsByCategory_ShouldReturnProducts()
        {
            // Arrange 
            var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=productsdb";
            var repository = new ProductRepository(connectionString);
            var service = new ProductService(repository);

            // Act 
            var result = service.GetProductsByCategory("Beauty");

            // Assert 
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
            Assert.IsTrue(result.All(p => p.Category == "Beauty"));

        }
    }
}