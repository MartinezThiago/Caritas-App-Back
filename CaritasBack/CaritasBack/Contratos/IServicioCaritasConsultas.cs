using CaritasBack.Models;
using System.Drawing;

namespace CaritasBack.Contratos
{
    public interface IServicioCaritasConsultas
    {
        /// <summary>
        /// Metodo que devuelve true si se registro con exito y false si ya existia un user con ese mail
        /// </summary>
        /// <returns></returns>
        bool registrarUsuario(UsuarioRegistro model);

        /// <summary>
        /// Metodo que devuelve true si se registro el user interno con exito y false si ya existia un user con ese mail
        /// </summary>
        /// <returns></returns>
        bool registrarUsuarioInterno(CambiarModificarDatosVoluntario model);

        /// <summary>
        /// Metodo que devuelve la lista de usuarios
        /// </summary>
        /// <returns></returns>
        List<Usuarios> listadDeUsuarios();
        /// <summary>
        /// Metodo que devuelve la lista de publicaciones
        /// </summary>
        /// <returns></returns>
        List<Publicacion> getPublicaciones(int id_categorie);
        /// <summary>
        /// Metodo que devuelve una publicacion especifica con la info del usuario owner.
        /// </summary>
        /// <returns></returns>
        Publicacion_Usuario obtenerPublicacionEspecifica(int idpublicacion);
        /// <summary>
        /// Metodo que devuelve los comentarios de una publicacion especifica
        /// </summary>
        /// <returns></returns>
        List<Comentarios> obtenerComentariosPublicacionEspecifica(int idpublicacion);
        /// <summary>
        /// Metodo que devuelve el listado de publicaciones de un usuario dado su id de usuario
        /// </summary>
        /// <returns></returns>
        List<Publicacion_Usuario> obtenerPublicacionesPorIdUsuario(int idUsuario);
        /// <summary>
        /// Metodo que sube pregunta
        /// </summary>
        /// <returns></returns>
        void subirPregunta(PreguntaEnvio model);

        /// <summary>
        /// Metodo que sube respuesta
        /// </summary>
        /// <returns></returns>
        void subirRespuesta(RespuestaEnvio model);

        /// <summary>
        /// Metodo que elimina pregunta y su respuesta
        /// </summary>
        /// <returns></returns>
        void eliminarPregunta(EliminarPregunta model, bool esBasico);

        /// <summary>
        /// Metodo que elimina respues
        /// </summary>
        /// <returns></returns>
        void eliminarRespuesta(EliminarRespuesta model, bool esBasico);

        /// <summary>
        /// Crear Publicacion
        /// </summary>
        /// <returns></returns>
        void crearPublicacion(PublicacionViewModel model);

        /// <summary>
        /// Cambiar datos personales
        /// </summary>
        /// <returns></returns>
        void cambiarDatosPersonales(CambiarDatosViewModel model);
        /// <summary>
        /// Metodo que devuelve la foto de un user especifico
        /// </summary>
        /// <returns></returns>
        string obtenerFotoUsuario(int idUser);

        /// <summary>
        /// Metodo que devuelve los centros
        /// </summary>
        /// <returns></returns>
        public List<Centro> obtenerCentros();
        /// <summary>
        /// Metodo que devuelve los centros de un publicaicon
        /// </summary>
        /// <returns></returns>
        public List<Centro> obtenerCentrosPublicacionEspecifica(int idPublicacion);
        /// <summary>
        /// Metodo que devuelve los centros de un user especifico
        /// </summary>
        /// <returns></returns>
        public List<Centro> obtenerCentrosUserEspecifico(int idUser);
        /// <summary>
        /// Metodo que devuelve el centro de un voluntario dado su id
        /// </summary>
        /// <returns></returns>
        public Centro obtenerCentroVoluntario(int idUser);
        bool restablecerActualizarClave(int restablecer, string clave, string token);

        public UsuarioRestablecer obtenerUsuarioPorEmail(string correo);

        public string modificarFavorito(int idUser, int idPublicacion, bool action);

        public List<int> getFavsIdsUser(int idUser);

        public List<PublicacionMiniatura> obtenerPublicacionesFavoritas(int idUser);

        public OfertasInvolucradas obtenerOfertasUsuario(int idUser);
        public void borrarPublicacion(BorrarPublicacion model, bool esBasico, int idUser);
        public void rechazarCancelarOferta(Oferta model);
        public void aceptarOferta(AceptarOferta model); 
        public List<Intercambio>obtenerIntercambiosEnEstadoPendiente();
        public List<IntercambioDetalle> obtenerIntercambiosEnEstadoPendienteUser(int idUSer);
        public bool ofertarPublicacion(SolicitudOferta model);
        public void cancelarIntercambioPendiente(CancelarIntercambio model);
        public List<IntercambioDetalleCentro> obtenerIntercambiosEnEstadoPendienteCentro(int idcentro);
        public void rechazarIntercambio(IntercambioAceptarRechazar model,int idAuditor);
        public void aceptarIntercambio(IntercambioAceptarRechazar model,int idAuditor);
        public void insertarCentro(CentroParaInsertar model);
        public void actualizarCentro(CentroModificar model);
        public List<UsuariosViewModel> listaDeUsuariosViewModel();
        public void ActualizarUsuarioBorrado(int usuarioID);
        public void borrarCentro(int idCentro);
        public void cambiarCentro(CentroPublicacionModificar model);
        public EstadisticasVoluntarioViewModel getEstadisticasVoluntario(int idCentro, string? fechaInicio, string? fechaFin);
        public EstadisticasAdminViewModel getEstadisticasAdmin(int idCentro, string? fechaInicio, string? fechaFin);
        public void cambiarDatosPersonalesVoluntario(CambiarModificarDatosVoluntario model);
    }
}
