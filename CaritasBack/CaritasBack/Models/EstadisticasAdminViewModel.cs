namespace CaritasBack.Models
{
    // esto tendra cada elemento de la lista de localidades
    public class EstadisticasLocalidadAdmin
    {
        public string Localidad { get; set; }
        public int IntercambiosTotalesLocalidad { get; set; }
        public int IntercambiosConfirmadosSinDonacionLocalidadCount { get; set; }
        public int IntercambiosConfirmadosConDonacionLocalidadCount { get; set; }
        public int IntercambiosCanceladosLocalidadCount { get; set; }
        public string MotivoDeCancelacionMasFrecuenteLocalidad { get; set; }
        public int IntercambiosRechazadosLocalidadCount { get; set; }
        public string MotivoDeRechazoMasFrecuenteLocalidad { get; set; }
    }

    // esto tendra las estadisticas a nivel global no a nivel de localidad, tiene en cuenta toda la lista de localidades
    public class EstadisticasGlobablesAdmin
    {
        public Centro CentroConMasCantidadDeIntercambios { get; set; }
        // esto tendra la cantidad de productos donados del centro con mas cantidad de intercambios
        public int CantidadProductosDonadosCentro { get; set; }
        public UsuariosViewModel VoluntarioConMasIntercambiosConfirmados { get; set; }
        public int CantidadIntercambiosConfirmadosVoluntario { get; set; }
        public int IntercambiosTotalesCount { get; set; }
        public int IntercambiosConfirmadosConDonacionCount { get; set; }
        public int IntercambiosConfirmadosSinDonacionCount { get; set; }
        public int IntercambiosCanceladosCount { get; set; }
        public string MotivoDeCancelacionMasFrecuente { get; set; }
        public int IntercambiosRechazadosCount { get; set; }
        public string MotivoDeRechazoMasFrecuente { get; set; }
        public List<CategoriaCantidad> DonacionesPorCategoria { get; set; }
        public List<CategoriaCantidad> IntercambiosPorCategoria { get; set; }
        public List<IntercambioGrafico> IntercambiosTotalesPorFecha { get; set; }
    }

    public class CategoriaCantidad
    {
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
    }

    public class IntercambioGrafico
    {
        public string FechaIntercambio { get; set; }
        public int Confirmados { get; set; }
        public int Rechazados { get; set; }
        public int Cancelados { get; set; }
    }

    public class EstadisticasAdminViewModel
    {
        public List<EstadisticasLocalidadAdmin> EstadisticasLocalidades { get; set; }
        public EstadisticasGlobablesAdmin EstadisticasGlobales { get; set; }
        public EstadisticasVoluntarioViewModel EstadisticasVoluntario { get; set; }
    }
}