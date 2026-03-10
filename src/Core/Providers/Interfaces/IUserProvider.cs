namespace Core.Providers.Interfaces
{
    public interface IUserProvider
    {
        public string? Id { get; set; }
        public string? Cpf { get; set; }
    }
}
