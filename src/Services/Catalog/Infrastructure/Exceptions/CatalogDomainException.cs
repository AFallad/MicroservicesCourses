using System;

namespace Catalog.Infrastructure.Exceptions
{
    public class CatalogDomainException : Exception
    {
        public CatalogDomainException(string message)
            : base(message)
        {
        }

        public CatalogDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}