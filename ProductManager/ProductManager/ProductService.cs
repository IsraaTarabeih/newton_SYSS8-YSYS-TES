using Npgsql;

namespace ProductManager
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            return _productRepository.GetProductsByCategory(category);
        }
    }
}