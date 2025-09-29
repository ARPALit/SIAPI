namespace Arpal.SiApi.WebApplication.DTO
{
    public class DynamicQueryRequestDTO
    {
        public List<DynamicQueryParameterDTO>? Parametri { get; set; }
    }

    public class DynamicQueryParameterDTO
    {
        public String? Alias { get; set; }
        public String? Value { get; set; }
    }
}
