using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CaritasBack.Models
{
    public class EstadisticasVoluntarioViewModel
    {
        public int IdCentro { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public string Direccion { get; set; }
        public string HorarioApertura { get; set; }
        public string HorarioCierre { get; set; }
        public bool Borrado { get; set; }
        public int CantidadIntercambiosConfirmados { get; set; }
        public int CantidadIntercambiosCancelados { get; set; }
        public string MotivoMasComunCancelacion { get; set; }
        public int CantidadIntercambiosRechazados { get; set; }
        public string MotivoMasComunRechazo { get; set; }
        public string nombreVoluntarioMasAuditador { get; set; }
        public string apellidoVoluntarioMasAuditador { get; set; }
        public string fotoVoluntarioMasAuditador { get; set; }
        public int cantidadVoluntarioMasAuditador { get; set; }
        public int CantidadProductosDonados { get; set; }
        public List<Dias> dias { get; set; }
        public List<categoriaEstadistica> cantidadIntercambiosPorCategoria { get; set; }
        public List<IntercambioGrafico> intercambios { get; set; }
    }
    public class categoriaEstadistica
    {
        public int categoriaProducto { get; set; }
        public int cantidadIntercambios { get; set; }
    }
    public class Publicacion
    {
        public int ID { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string nombre_categoria_producto { get; set; }
        public int categoria_producto { get; set; }
        public int estado_producto { get; set; }
        public string ubicacion_trade { get; set; }
        public string nombre_estado_producto { get; set; }
        public int estado_publicacion { get; set; }
        public string fecha_publicacion { get; set; }
        public int usuario_owner { get; set; }
        public List<ImagenesCodificadas> Imagenes { get; set; }
        public List<Centro> centros { get; set; }
    }
    public class PublicacionCentroBorrar
    {
        public int ID { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string nombre_categoria_producto { get; set; }
        public int categoria_producto { get; set; }
        public int estado_producto { get; set; }
        public string ubicacion_trade { get; set; }
        public string nombre_estado_producto { get; set; }
        public int estado_publicacion { get; set; }
        public string fecha_publicacion { get; set; }
        public int usuario_owner { get; set; }
        public List<ImagenesCodificadas> Imagenes { get; set; }
        public List<CentroBorrar> centros { get; set; }
    }

    public class PublicacionMiniatura
    {
        public int ID { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string nombre_categoria_producto { get; set; }
        public string nombre_estado_producto { get; set; }
        public string localidad { get; set; }
        public string imagenEnBase64 { get; set; } 
    }

    public class PublicacionViewModel
    {
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string ubicacion_trade { get; set; }
        public List<string> imagenesEnBase64 { get; set; }
        public int usuario_owner { get; set; }
        public int categoria_producto { get; set; }
        public List<int> centros_elegidos { get; set; }
        public List<List<int>> dias_elegidos { get; set; } // lista de dias para cada centro
        public List<string> desde { get; set; } // horario elegido para los centros
        public List<string> hasta { get; set; } // horario elegido para los centros
        public int estado_producto { get; set; }
    }
    public class CambiarDatosViewModel
    {
        public int usuario_owner { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public string password { get; set; }
        public DateTime ?fecha_nacimiento { get; set; }
        public string foto { get; set; }
        public List<int> centros_elegidos { get; set; }
    }
    public class CambiarModificarDatosVoluntario
    {
        // id_usuario se utilizará para modificar voluntario en caso de crearse se recibe null
        public int? id_usuario { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public string email { get; set; }
        public DateTime nacimiento { get; set; }
        public int centro { get; set; }
        public string foto { get; set; }
    }

    public abstract class Usuario
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string email { get; set; }
        [MinLength(6, ErrorMessage = "La contraseña debe tener como mínimo 6 caracteres.")]
        public string password { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string foto { get; set; }
    }
    public class UsuarioRegistro : Usuario
    {
        public List<int> centros_elegidos { get; set; }
    }

    public class BorrarPublicacion
    {
        public int id_publicacion { get; set; }
    }
    public class Oferta
    {
        public int id_oferta { get; set; }
        public int id_post { get; set; }
        public bool cancelar { get; set; }
    }
    public class AceptarOferta:Oferta
    {
        public int id_centro { get; set; }
    }
    public class IntercambioAceptarRechazar
    {
        public int id_intercambio { get; set; }
        public int id_post_offer { get; set; }
        public int id_post_owner { get; set; }
        public string productoDonado { get; set; }
    }
    public class CancelarIntercambio
    {
        public int id_trade { get; set; }
        public int id_cancelante { get; set; }
        public int id_post_owner { get; set; }
        public int id_post_offer { get; set; }
        public string motivo_cancelacion { get; set; }
    }

    public class UsuarioRegistroInterno : Usuario
    {
        public int rol { get; set; }
        public int centro { get; set; }
    }
    public class rol
    {
        public int id { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
    }

    public class estado_producto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }

    public class categorias
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }

    public class Publicacion_Usuario
    {
        public int ID { get; set; } 
        public string titulo { get; set; } 
        public string descripcion { get; set; } 
        public int estado_producto { get; set; } 
        public string ubicacion_trade { get; set; } 
        public string fecha_publicacion { get; set; } 
        public string nombre_usuario { get; set; }
        public string apellido_usuario { get; set; }
        public int id_usuario { get; set; }
        public int estado_publicacion { get; set; } 
        public string nombre_estado_producto { get; set; }
        public int categoria_producto { get; set; } 
        public string base64_imagen { get; set; } 
        public string nombre_categoria_producto { get; set; }
        public List<Comentarios> comentarios { get; set; }
        public List<ImagenesCodificadas> Imagenes { get; set; }
        public List<Centro> centros { get; set; } 

        // agregado para obtener los centros de una publicación que tendra los dias en los que 
        // puede realizar el intercambio en cada centro, el horario en el que puede sera el mismo para todos los dias
        public List<Centros_Dias_Publicacion> centros_Publicacion { get; set; }

    }

    // modelo para obtener los centros, dias y horario para realizar intercambio que eligio el dueño de la publicación
    public class Centros_Dias_Publicacion
    {
        public int id_cp { get; set; }
        public int id_publicacion { get; set; }
        public string nombre_centro { get; set; }
        public List<string> diasDeIntercambio { get; set; }
        public string desde { get; set; }
        public string hasta { get; set; }      
    }

    public class Centro
    {
        public int id_centro { get; set; }
        public string nombre_centro { get; set; }
        public string ubicacion { get; set; }
        public string direccion { get; set; }
        public bool borrado { get; set; }
        public string horario_apertura { get; set; }
        public string horario_cierre { get; set; }
        public bool tiene_voluntario { get; set; }
        public List<Dias> dias { get; set; }

    }
    public class CentroBorrar : Centro
    {
        public bool borradoCentroPublicacion { get; set; }

    }
    public class CentroModificar
    {
        public int id_centro { get; set; }
        public string nombre_centro { get; set; }
        public string ubicacion { get; set; }
        public string direccion { get; set; }
        public string horario_apertura { get; set; }
        public string horario_cierre { get; set; }
        public List<int> dias { get; set; }
    }
    public class CentroParaInsertar
    {
        public string nombre_centro { get; set; }
        public string ubicacion { get; set; }
        public string direccion { get; set; }
        public string horario_apertura { get; set; }
        public string horario_cierre { get; set; }
        public List<int> dias { get; set; }
    }
    public class CentroPublicacionModificar
    {
        public int id_centro_nuevo { get; set; }
        public int id_centro_viejo { get; set; }
        public int id_publicacion { get; set; }
        public string desde { get; set; }
        public string hasta { get; set; }
        public List<int> dias { get; set; }
    }

    public class Dias
    {
        public int idDia { get; set; }
        public string descripcion { get; set; }
    }

    public class Comentarios 
    {
        public int user_id_pregunta { get; set; }
        public int user_id_respuesta { get; set; }
        public int id_pregunta { get; set; }
        public int id_respuesta { get; set; }
        public string nombre_pregunta { get; set; }
        public string apellido_pregunta { get; set; }
        public string pregunta { get; set; }
        public string fechaPregunta { get; set; }
        public string respuesta { get; set; }
        public string fechaRespuesta { get; set; }
    }

    public class ComentariosViewModel
    {
        public List<Comentarios> comentarios { get; set; }
    }


    public class PreguntaEnvio
    {
        public int usuario_owner_pregunta { get; set; }
        public string contenido_pregunta { get; set; }
        public DateTime fecha_publicacion_pregunta { get; set; }
        public int idPublicacion { get; set; }
    }
    public class Pregunta
    {
        public int id_pregunta { get; set; }
        public int usuario_owner_pregunta { get; set; }
        public string contenido_pregunta { get; set; }
        public string fecha_publicacion_pregunta { get; set; }
        public int idPublicacion { get; set; }
    }
    public class RespuestaEnvio
    {
        public int usuario_owner_respuesta { get; set; }
        public string contenido_respuesta { get; set; }
        public DateTime fecha_publicacion_respuesta { get; set; }
        public int id_pregunta { get; set; }
    }
    public class Respuesta
    {
        public int id_respuesta { get; set; }
        public int usuario_owner_respuesta { get; set; }
        public string contenido_respuesta { get; set; }
        public string fecha_publicacion_respuesta { get; set; }
        public int id_pregunta { get; set; }
    }

  
    public class ImagenesCodificadas
    {
        public string base64_imagen { get; set; }
    }

    public class EliminarPregunta
    {
        public int id_pregunta { get; set; }
        public int id_usuario_que_va_a_borrar { get; set; }
    } 
    public class EliminarRespuesta
    {
        public int id_respuesta { get; set; }
        public int id_usuario_que_va_a_borrar { get; set; }
    }

    public class  FavoritosModel
    {
        public int id_publicacion { get; set; }
        public bool action { get; set; }
    }

    public class Intercambio
    {
        public int IdIntercambio { get; set; }
        public int IdUsuarioOwner { get; set; }
        public int IdUsuarioOffer { get; set; }
        public int IdPostOwner { get; set; }
        public int IdPostOffer { get; set; }
        public int IdCentroElegido { get; set; }
        public string FechaIntercambio { get; set; }
        public int IdEstadoPublicacion { get; set; }
        public string ProductoDonado { get; set; }
        public int IdEstadoIntercambio { get; set; }
        public string Horario { get; set; }
    }

    public class IntercambioDetalle
    {
        public int IdIntercambio { get; set; }
        public int IdUsuarioOwner { get; set; }
        public string NombreUsuarioOwner { get; set; }
        public string ApellidoUsuarioOwner { get; set; }
        public string dniOwner { get; set; }
        public string FotoPerfilUsuarioOwner { get; set; }
        public int IdUsuarioOffer { get; set; }
        public string NombreUsuarioOffer { get; set; }
        public string ApellidoUsuarioOffer { get; set; }
        public string dniOffer { get; set; }
        public string FotoPerfilUsuarioOffer { get; set; }
        public int IdPostOwner { get; set; }
        public string FotoPostOwner { get; set; }
        public int IdPostOffer { get; set; }
        public string FotoPostOffer { get; set; }
        public int IdCentroElegido { get; set; }
        public int IdCentro { get; set; }
        public string NombreCentro { get; set; }
        public string localidad { get; set; }
        public string direccion { get; set; }
        public string FechaIntercambio { get; set; }
        public int IdEstadoPublicacion { get; set; }
        public string ProductoDonado { get; set; }
        public int IdEstadoIntercambio { get; set; }
        public string Horario { get; set; }
    }
    public class IntercambioDetalleCentro: IntercambioDetalle
    {
        public string dniAuditor { get; set; }
        public string nombreAuditor { get; set; }
        public string apellidoAuditor { get; set; }
        public int idAuditor { get; set; }
        public string fotoPerfilAuditor { get; set; }
    }
    public class IntercambioCentro
    {
        public int id_centro { get; set; }
    }

    public class SolicitudOferta
    {
        public int id_publicacion_a_la_que_se_oferta { get; set; }
        public int id_publicacion_con_la_que_se_oferta { get; set; }
        public int id_usuario_duenio_publicacion_a_la_que_se_oferta { get; set; }
        public int id_usuario_que_oferta { get; set; }
        public int centro_elegido { get; set; }
        public string dia_elegido { get; set; }
        public string hora_elegida { get; set; }

    }
}
