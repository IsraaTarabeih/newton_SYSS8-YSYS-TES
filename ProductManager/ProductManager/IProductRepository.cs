namespace ProductManager
{
    /// <summary>
    /// Hämtar produkter för specifik category.
    /// </summary>
    public interface IProductRepository
    {
        List<Product> GetProductsByCategory(string category);
    }
}
