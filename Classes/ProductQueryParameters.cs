namespace HPlusSport.API.Classes
{
    public class ProductQueryParameters : QueryParameters
    {
        public string Sku { get; set; }
        public decimal? MinPrice { get; set; } = 0;
        public decimal? MaxPrice { get; set; } = 1000;
        public string Name {get; set;}
    }
}