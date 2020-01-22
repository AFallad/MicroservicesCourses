using System;
using Catalog.Infrastructure.Exceptions;

namespace Catalog.Models.DomainModels
{
    public class CatalogItem
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int AvailableStock { get; set; }
        public int Stock { get; set; }
        public int MaxStockThreshold { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public int BrandId { get; set; }
        public CatalogBrand Brand { get; set; }
        public int TypeId { get; set; }
        public CatalogType Type { get; set; }

        public int AddStock(int quantity)
        {
            int original = this.Stock;

            if((this.AvailableStock + quantity) > this.MaxStockThreshold)
            {
                this.AvailableStock += (this.MaxStockThreshold - this.AvailableStock);
            }
            else
            {
                this.AvailableStock += quantity;
            }

            return this.AvailableStock - original;
        }

        public int RemoveStock(int quantity)
        {
            if(this.AvailableStock <= 0)
            {
                throw new CatalogDomainException($"SKU: {this.Sku} is sold out");
            }
            
            if(quantity <= 0)
            {
                throw new CatalogDomainException("Quantity to remove must be greater than 0");
            }

            var removed = Math.Min(this.AvailableStock, quantity);

            this.AvailableStock -= removed;

            return removed;
        }
    }
}