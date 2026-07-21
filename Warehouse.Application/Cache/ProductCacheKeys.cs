namespace Warehouse.Application.Cache;


public static class ProductCacheKeys
    {
        public const string All = "products:all";
        public const string Available = "products:available";
        public const string HealthCheck = "redis-health-check";
        public static string ById(Guid productId)
        {
            return $"product:{productId}";
        }
    }
