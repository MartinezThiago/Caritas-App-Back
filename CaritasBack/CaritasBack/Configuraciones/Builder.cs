namespace CaritasBack.Configuraciones
{
    public interface IBuilder
    {
        public string obtenerConnectionString(string connection);
    }

    public class Builder : IBuilder
    {
        private IConfiguration _configuration;

        public Builder(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            this._configuration = builder.Build();
        }

        public string obtenerConnectionString(string connection)
        {
            return this._configuration.GetConnectionString(connection);
        }
    }
}