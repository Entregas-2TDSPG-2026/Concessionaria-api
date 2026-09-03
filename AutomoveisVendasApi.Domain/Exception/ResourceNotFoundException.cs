namespace AutomoveisVendasApi.Domain.Exceptions
{
    
    public class ResourceNotFoundException : DomainException
    {
        public ResourceNotFoundException(string message) : base(message) { }
    }
}