using CaritasBack.Configuraciones;
using CaritasBack.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using CaritasBack.Contratos;
using System.Transactions;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Globalization;

namespace CaritasBack.Services
{
    public class ServicioCaritasConsultas : IServicioCaritasConsultas
    {
        private readonly IBuilder builder;
        private readonly ICurrentUser currentUser;
        //SINGLETON , con variables static para poder hacer referencia a la clase.
        private static IServicioCaritasConsultas instance;

        public static IServicioCaritasConsultas Instance
        {
            get
            {
                if (instance == null)
                {
                    IBuilder builder = new Builder(new ConfigurationBuilder());
                    ICurrentUser currentUser = new CurrentUser();
                    instance = new ServicioCaritasConsultas(builder, currentUser);
                }
                return instance;
            }
            set => instance = value;
        }
        public ServicioCaritasConsultas(IBuilder builder, ICurrentUser currentUser)
        {
            this.builder = builder;
            this.currentUser = currentUser;
        }
        public bool registrarUsuario(UsuarioRegistro model)
        {
            bool exito = true;
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    SqlCommand cmdInsertCentros = new SqlCommand("spInsertarEnCentros", connection);
                    cmdInsertCentros.CommandType = CommandType.StoredProcedure;
                    cmdInsertCentros.CommandTimeout = 0;
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spRegistrarUsuario", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter nombre = new SqlParameter("@nombre", model.nombre);
                    cmd.Parameters.Add(nombre);
                    SqlParameter Apellido = new SqlParameter("@apellido", model.apellido);
                    cmd.Parameters.Add(Apellido);
                    SqlParameter DNI = new SqlParameter("@DNI", model.dni);
                    cmd.Parameters.Add(DNI);
                    SqlParameter fechaRegistro = new SqlParameter("@FechaRegistro", DateTime.Now);
                    cmd.Parameters.Add(fechaRegistro);
                    SqlParameter fechaNacimiento = new SqlParameter("@FechaNacimiento", model.fecha_nacimiento);
                    cmd.Parameters.Add(fechaNacimiento);
                    SqlParameter email = new SqlParameter("@Email", model.email);
                    cmd.Parameters.Add(email);
                    SqlParameter pass = new SqlParameter("@password", model.password);
                    cmd.Parameters.Add(pass);
                    SqlParameter foto = new SqlParameter("@foto", model.foto);
                    cmd.Parameters.Add(foto);
                    //Agrego el parametro para token
                    SqlParameter token = new SqlParameter("@token", Utils.PasswordUtils.GenerarToken());
                    cmd.Parameters.Add(token);
                    //Agrego el parametro para restablecer
                    SqlParameter restablecer = new SqlParameter("@restablecer", false);
                    cmd.Parameters.Add(restablecer);
                    int idUser = (int)cmd.ExecuteScalar();
                    setearCentrosUsuarios(model, cmdInsertCentros, idUser, false);
                    connection.Close();
                    return exito;
                }
            }
            catch (Exception)
            {
                exito = false;
            }
            return exito;
        }

        private void setearCentrosUsuarios(UsuarioRegistro model, SqlCommand cmdInsertCentros, int idUser, bool esPublicacion)
        {
            foreach (var idCentro in model.centros_elegidos)
            {
                SqlParameter userID = new SqlParameter("@idUser", idUser);
                cmdInsertCentros.Parameters.Add(userID);
                SqlParameter centro = new SqlParameter("@centro", idCentro);
                cmdInsertCentros.Parameters.Add(centro);
                SqlParameter publicacionBool = new SqlParameter("@EsPublicacion", esPublicacion);
                cmdInsertCentros.Parameters.Add(publicacionBool);
                cmdInsertCentros.ExecuteNonQuery();
                cmdInsertCentros.Parameters.Clear();
            }
        }

        public bool registrarUsuarioInterno(CambiarModificarDatosVoluntario model)
        {
            bool exito = true;
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    SqlCommand cmdInsertCentros = new SqlCommand("spInsertarEnCentros", connection);
                    cmdInsertCentros.CommandType = CommandType.StoredProcedure;
                    cmdInsertCentros.CommandTimeout = 0;
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spRegistrarUsuarioInterno", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter nombre = new SqlParameter("@nombre", model.nombre);
                    cmd.Parameters.Add(nombre);
                    SqlParameter Apellido = new SqlParameter("@apellido", model.apellido);
                    cmd.Parameters.Add(Apellido);
                    SqlParameter DNI = new SqlParameter("@DNI", model.dni);
                    cmd.Parameters.Add(DNI);
                    SqlParameter password = new SqlParameter("@password", "123123");
                    cmd.Parameters.Add(password);
                    SqlParameter fechaRegistro = new SqlParameter("@FechaRegistro", DateTime.Now);
                    cmd.Parameters.Add(fechaRegistro);
                    SqlParameter fechaNacimiento = new SqlParameter("@FechaNacimiento", model.nacimiento);
                    cmd.Parameters.Add(fechaNacimiento);
                    SqlParameter email = new SqlParameter("@Email", model.email);
                    cmd.Parameters.Add(email);
                    SqlParameter rol = new SqlParameter("@rol", 2);
                    cmd.Parameters.Add(rol);
                    SqlParameter centro = new SqlParameter("@centro", model.centro);
                    cmd.Parameters.Add(centro);
                    SqlParameter foto = new SqlParameter("@foto", model.foto);
                    cmd.Parameters.Add(foto);
                    //Agrego el parametro para token
                    SqlParameter token = new SqlParameter("@token", Utils.PasswordUtils.GenerarToken());
                    cmd.Parameters.Add(token);
                    //Agrego el parametro para restablecer
                    SqlParameter restablecer = new SqlParameter("@restablecer", false);
                    cmd.Parameters.Add(restablecer);
                    cmd.ExecuteScalar();
                    connection.Close();
                    return exito;
                }
            }
            catch (Exception)
            {
                exito = false;
            }
            return exito;
        }

        public List<Usuarios> listadDeUsuarios()
        {
            List<Usuarios> listado = new List<Usuarios>();

            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerUsuarios", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Usuarios Usuarios = new Usuarios();
                        Usuarios.nombre = reader.GetString(reader.GetOrdinal("nombre"));
                        Usuarios.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        Usuarios.apellido = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido"));
                        Usuarios.DNI = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("DNI")) == true ? "" : reader.GetString(reader.GetOrdinal("DNI"));
                        Usuarios.mail = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("mail")) == true ? "" : reader.GetString(reader.GetOrdinal("mail"));
                        Usuarios.password = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("password")) == true ? "" : reader.GetString(reader.GetOrdinal("password"));
                        Usuarios.rol = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreRol")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreRol"));
                        Usuarios.fecha_nacimiento = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_nacimiento")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")).ToString("dd-MM-yyyy");
                        Usuarios.centro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("centro")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("centro"));
                        listado.Add(Usuarios);
                    }
                    reader.Close();
                }
            }
            return listado;
        }
        public List<UsuariosViewModel> listaDeUsuariosViewModel()
        {
            List<UsuariosViewModel> listado = new List<UsuariosViewModel>();

            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerUsuarios", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UsuariosViewModel Usuarios = new UsuariosViewModel();
                        Usuarios.nombre = reader.GetString(reader.GetOrdinal("nombre"));
                        Usuarios.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        Usuarios.apellido = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido"));
                        Usuarios.DNI = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("DNI")) == true ? "" : reader.GetString(reader.GetOrdinal("DNI"));
                        Usuarios.mail = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("mail")) == true ? "" : reader.GetString(reader.GetOrdinal("mail"));
                        Usuarios.rol = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreRol")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreRol"));
                        Usuarios.fecha_nacimiento = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_nacimiento")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")).ToString("dd-MM-yyyy");
                        Usuarios.centro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("centro")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("centro"));
                        Usuarios.foto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("base64_imagen")) == true ? "" : reader.GetString(reader.GetOrdinal("base64_imagen"));
                        Usuarios.fecha_registro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_registro")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_registro")).ToString("dd-MM-yyyy");
                        Usuarios.borrado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("borrado")) == true ? false : reader.GetBoolean(reader.GetOrdinal("borrado"));
                        listado.Add(Usuarios);
                    }
                    reader.Close();
                }
            }
            return listado;
        }
        private bool obtenerSiEsNuloElCampo(SqlDataReader reader, int colIndex)
        {
            return reader.IsDBNull(colIndex);

        }

        public List<Publicacion> getPublicaciones(int id_categoria)
        {
            List<Publicacion> listadepublicaciones = new List<Publicacion>();
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerPublicaciones", connection);
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentroParaUnaPublicacionEspecifica", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idcat = new SqlParameter("@id_categoria", id_categoria == 0 ? null : id_categoria);
                cmd.Parameters.Add(idcat);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Publicacion publicacion = new Publicacion();
                        publicacion.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        publicacion.titulo = reader.GetString(reader.GetOrdinal("titulo"));
                        publicacion.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                        publicacion.categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("categoria_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("categoria_producto"));
                        publicacion.estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_producto"));
                        publicacion.ubicacion_trade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_trade")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_trade"));
                        publicacion.estado_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_publicacion")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_publicacion"));
                        publicacion.fecha_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion")).ToString("dd/MM/yyyy");
                        publicacion.usuario_owner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("usuario_owner")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("usuario_owner"));
                        publicacion.nombre_estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreEstado")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreEstado"));
                        publicacion.nombre_categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreCategoria")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreCategoria"));
                        listadepublicaciones.Add(publicacion);
                    }
                    reader.Close();
                }

                foreach (var publicacion in listadepublicaciones)
                {
                    List<ImagenesCodificadas> listaimagenes = new List<ImagenesCodificadas>();
                    using (SqlCommand cdmImagenes = new SqlCommand("spMultimedia", connection))
                    {
                        cdmImagenes.CommandType = CommandType.StoredProcedure;
                        cdmImagenes.Parameters.AddWithValue("@idpublicacion", publicacion.ID);
                        using (SqlDataReader readerImagenes = cdmImagenes.ExecuteReader())
                        {
                            while (readerImagenes.Read())
                            {
                                ImagenesCodificadas imagen = new ImagenesCodificadas();
                                imagen.base64_imagen = obtenerSiEsNuloElCampo(readerImagenes, readerImagenes.GetOrdinal("base64_imagen")) == true ? "" : readerImagenes.GetString(readerImagenes.GetOrdinal("base64_imagen"));
                                listaimagenes.Add(imagen);
                            }
                            publicacion.Imagenes = listaimagenes;
                            readerImagenes.Close();
                        }

                    }

                }
                foreach (var publicacion in listadepublicaciones)
                {
                    SqlParameter PublicacionIDCentros = new SqlParameter("@idPublicacion", publicacion.ID);
                    cmdCentros.Parameters.Add(PublicacionIDCentros);
                    List<Centro> listaCentros = new List<Centro>();
                    getCentros(cmdCentros, listaCentros);
                    publicacion.centros = listaCentros;
                    setDiasALosCentros(cmdDias, listaCentros);
                    cmdCentros.Parameters.Clear();
                }
                connection.Close();
            }
            return listadepublicaciones;
        }
        public List<PublicacionCentroBorrar> getPublicacionesParaBorrarCentro(int id_categoria)
        {
            List<PublicacionCentroBorrar> listadepublicaciones = new List<PublicacionCentroBorrar>();
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("[spObtenerPublicacionesParaBorrarCentro]", connection);
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentroParaUnaPublicacionEspecifica", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idcat = new SqlParameter("@id_categoria", id_categoria == 0 ? null : id_categoria);
                cmd.Parameters.Add(idcat);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PublicacionCentroBorrar publicacion = new PublicacionCentroBorrar();
                        publicacion.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        publicacion.titulo = reader.GetString(reader.GetOrdinal("titulo"));
                        publicacion.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                        publicacion.categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("categoria_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("categoria_producto"));
                        publicacion.estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_producto"));
                        publicacion.ubicacion_trade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_trade")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_trade"));
                        publicacion.estado_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_publicacion")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_publicacion"));
                        publicacion.fecha_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion")).ToString("dd/MM/yyyy");
                        publicacion.usuario_owner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("usuario_owner")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("usuario_owner"));
                        publicacion.nombre_estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreEstado")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreEstado"));
                        publicacion.nombre_categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreCategoria")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreCategoria"));
                        listadepublicaciones.Add(publicacion);
                    }
                    reader.Close();
                }

                foreach (var publicacion in listadepublicaciones)
                {
                    List<ImagenesCodificadas> listaimagenes = new List<ImagenesCodificadas>();
                    using (SqlCommand cdmImagenes = new SqlCommand("spMultimedia", connection))
                    {
                        cdmImagenes.CommandType = CommandType.StoredProcedure;
                        cdmImagenes.Parameters.AddWithValue("@idpublicacion", publicacion.ID);
                        using (SqlDataReader readerImagenes = cdmImagenes.ExecuteReader())
                        {
                            while (readerImagenes.Read())
                            {
                                ImagenesCodificadas imagen = new ImagenesCodificadas();
                                imagen.base64_imagen = obtenerSiEsNuloElCampo(readerImagenes, readerImagenes.GetOrdinal("base64_imagen")) == true ? "" : readerImagenes.GetString(readerImagenes.GetOrdinal("base64_imagen"));
                                listaimagenes.Add(imagen);
                            }
                            publicacion.Imagenes = listaimagenes;
                            readerImagenes.Close();
                        }

                    }

                }
                foreach (var publicacion in listadepublicaciones)
                {
                    SqlParameter PublicacionIDCentros = new SqlParameter("@idPublicacion", publicacion.ID);
                    cmdCentros.Parameters.Add(PublicacionIDCentros);
                    List<CentroBorrar> listaCentros = new List<CentroBorrar>();
                    getCentrosBorrarCentroFlag(cmdCentros, listaCentros);
                    publicacion.centros = listaCentros;
                    cmdCentros.Parameters.Clear();
                }
                connection.Close();
            }
            return listadepublicaciones;
        }

        public void subirPregunta(PreguntaEnvio model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spSubirPregunta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter usuario_owner = new SqlParameter("@usuario_owner_pregunta ", model.usuario_owner_pregunta);
                cmd.Parameters.Add(usuario_owner);
                SqlParameter contenido = new SqlParameter("@contenido_pregunta", model.contenido_pregunta);
                cmd.Parameters.Add(contenido);
                SqlParameter fecha_publicacion = new SqlParameter("@fecha_publicacion_pregunta", model.fecha_publicacion_pregunta);
                cmd.Parameters.Add(fecha_publicacion);
                SqlParameter idPublicacion = new SqlParameter("@idPublicacion", model.idPublicacion);
                cmd.Parameters.Add(idPublicacion);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void subirRespuesta(RespuestaEnvio model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spSubirRespuesta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter usuario_owner = new SqlParameter("@usuario_owner_respuesta", model.usuario_owner_respuesta);
                cmd.Parameters.Add(usuario_owner);
                SqlParameter contenido = new SqlParameter("@contenido_respuesta", model.contenido_respuesta);
                cmd.Parameters.Add(contenido);
                SqlParameter fecha_publicacion = new SqlParameter("@fecha_publicacion_respuesta", model.fecha_publicacion_respuesta);
                cmd.Parameters.Add(fecha_publicacion);
                SqlParameter id_pregunta = new SqlParameter("@idpregunta", model.id_pregunta);
                cmd.Parameters.Add(id_pregunta);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public Publicacion_Usuario obtenerPublicacionEspecifica(int id)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerPublicacionDeUserEspecifico", connection);
                SqlCommand cmdComentarios = new SqlCommand("spObtenerPublicacionDeUserEspecificoComentarios", connection);
                SqlCommand cmdImagenes = new SqlCommand("spMultimedia", connection);
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentroParaUnaPublicacionEspecifica", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                // sp para obtener centros, dias y horarios elegidos por el dueño de la publicacion
                SqlCommand cmdCentros_dias_publicacion = new SqlCommand("spObtenerCentrosConDiasYHorario", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmdComentarios.CommandType = CommandType.StoredProcedure;
                cmdComentarios.CommandTimeout = 0;
                cmdImagenes.CommandType = CommandType.StoredProcedure;
                cmdImagenes.CommandTimeout = 0;
                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                cmdCentros_dias_publicacion.CommandType = CommandType.StoredProcedure;
                cmdCentros_dias_publicacion.CommandTimeout = 0;

                SqlParameter PublicacionID = new SqlParameter("@PublicacionID", id);
                cmd.Parameters.Add(PublicacionID);

                SqlParameter PublicacionIDComentarios = new SqlParameter("@PublicacionID", id);
                cmdComentarios.Parameters.Add(PublicacionIDComentarios);
                SqlParameter urlImagenes = new SqlParameter("@idpublicacion", id);
                cmdImagenes.Parameters.Add(urlImagenes);

                SqlParameter PublicacionIDCentros = new SqlParameter("@idPublicacion", id);
                cmdCentros.Parameters.Add(PublicacionIDCentros);

                SqlParameter PublicacionIDCentrosDias = new SqlParameter("@idPublicacion", id);
                cmdCentros_dias_publicacion.Parameters.Add(PublicacionIDCentrosDias);

                Publicacion_Usuario publi_user = new Publicacion_Usuario();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        publi_user.nombre_usuario = reader.GetString(reader.GetOrdinal("nombre"));
                        publi_user.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        publi_user.apellido_usuario = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido"));
                        publi_user.id_usuario = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("usuario_owner")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("usuario_owner"));
                        publi_user.titulo = reader.GetString(reader.GetOrdinal("titulo"));
                        publi_user.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                        publi_user.categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("categoria_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("categoria_producto"));
                        publi_user.estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_producto"));
                        publi_user.ubicacion_trade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_trade")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_trade"));
                        publi_user.estado_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_publicacion")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_publicacion"));
                        publi_user.fecha_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion")).ToString("dd/MM/yyyy");
                        publi_user.nombre_estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreEstado")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreEstado"));
                        publi_user.nombre_categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreCategoria")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreCategoria"));
                        publi_user.base64_imagen = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("base64_imagen")) == true ? "" : reader.GetString(reader.GetOrdinal("base64_imagen"));
                    }

                    reader.Close();
                }
                List<Comentarios> listacomentarios = new List<Comentarios>();
                using (SqlDataReader reader = cmdComentarios.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Comentarios comentarios = new Comentarios();
                        comentarios.user_id_pregunta = reader.GetInt32(reader.GetOrdinal("usuario_owner_pregunta"));
                        comentarios.id_pregunta = reader.GetInt32(reader.GetOrdinal("ID_pregunta"));
                        comentarios.user_id_respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("usuario_owner_respuesta")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("usuario_owner_respuesta")); ;
                        comentarios.id_respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ID_respuesta")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("ID_respuesta")); ;
                        comentarios.pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("contenido_pregunta")) == true ? "" : reader.GetString(reader.GetOrdinal("contenido_pregunta"));
                        comentarios.nombre_pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre"));
                        comentarios.apellido_pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido"));
                        comentarios.respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("contenido")) == true ? "" : reader.GetString(reader.GetOrdinal("contenido"));
                        comentarios.fechaPregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion_pregunta")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion_pregunta")).ToString("dd/MM/yyyy");
                        comentarios.fechaRespuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion_respuesta")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion_respuesta")).ToString("dd/MM/yyyy");
                        listacomentarios.Add(comentarios);
                    }
                    reader.Close();
                    publi_user.comentarios = listacomentarios;
                }
                List<ImagenesCodificadas> listaimagenes = new List<ImagenesCodificadas>();
                using (SqlDataReader reader = cmdImagenes.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ImagenesCodificadas imagen = new ImagenesCodificadas();
                        imagen.base64_imagen = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("base64_imagen")) == true ? "" : reader.GetString(reader.GetOrdinal("base64_imagen"));
                        listaimagenes.Add(imagen);
                    }
                    reader.Close();
                    publi_user.Imagenes = listaimagenes;
                }
                List<Centro> listaCentros = new List<Centro>();
                getCentros(cmdCentros, listaCentros);
                publi_user.centros = listaCentros;
                setDiasALosCentros(cmdDias, listaCentros);

                List<Centros_Dias_Publicacion> centros_dias_publicacion = new List<Centros_Dias_Publicacion>();
                getCentros_Dias_Publicacion(cmdCentros_dias_publicacion, centros_dias_publicacion);
                publi_user.centros_Publicacion = centros_dias_publicacion;

                return publi_user;
            }
        }

        // este metodo obtiene los centros elegidos por el usuario (dueño de la publicacion)
        // con los dias en que puede asistir para el intercambio y su franja horario que es unica para todos los dias
        private void getCentros_Dias_Publicacion(SqlCommand cmdCentros_dias_publicacion, List<Centros_Dias_Publicacion> centros_dias_publicacion)
        {
            Dictionary<int, Centros_Dias_Publicacion> centrosDict = new Dictionary<int, Centros_Dias_Publicacion>();

            using (SqlDataReader reader = cmdCentros_dias_publicacion.ExecuteReader())
            {
                while (reader.Read())
                {
                    int idCp = reader.GetInt32(reader.GetOrdinal("id_cp"));
                    int idPublicacion = reader.GetInt32(reader.GetOrdinal("id_publicacion"));
                    string nombreCentro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre")) ? "" : reader.GetString(reader.GetOrdinal("nombre"));
                    string desde = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("desde")) ? "" : reader.GetString(reader.GetOrdinal("desde"));
                    string hasta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("hasta")) ? "" : reader.GetString(reader.GetOrdinal("hasta"));
                    string dia = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion"));

                    if (centrosDict.ContainsKey(idCp))
                    {
                        centrosDict[idCp].diasDeIntercambio.Add(dia);
                    }
                    else
                    {
                        Centros_Dias_Publicacion centro = new Centros_Dias_Publicacion
                        {
                            id_cp = idCp,
                            id_publicacion = idPublicacion,
                            nombre_centro = nombreCentro,
                            desde = desde,
                            hasta = hasta,
                            diasDeIntercambio = new List<string> { dia }
                        };

                        centrosDict[idCp] = centro;
                    }
                }
                centros_dias_publicacion.AddRange(centrosDict.Values);
                reader.Close();
            }
        }
        private void getCentrosBorrarCentroFlag(SqlCommand cmdCentros, List<CentroBorrar> listaCentros)
        {
            using (SqlDataReader reader = cmdCentros.ExecuteReader())
            {
                while (reader.Read())
                {
                    CentroBorrar centro = new CentroBorrar();
                    centro.id_centro = reader.GetInt32(reader.GetOrdinal("id_centro"));
                    centro.nombre_centro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre"));
                    centro.horario_apertura = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("horario_apertura")) == true ? "" : reader.GetString(reader.GetOrdinal("horario_apertura"));
                    if (centro.horario_apertura.IndexOf(":") == 1)
                    {
                        centro.horario_apertura = "0" + centro.horario_apertura;
                    }
                    centro.horario_cierre = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("horario_cierre")) == true ? "" : reader.GetString(reader.GetOrdinal("horario_cierre"));
                    centro.direccion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("direccion")) == true ? "" : reader.GetString(reader.GetOrdinal("direccion"));
                    centro.ubicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion"));
                    centro.borrado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("borrado")) == true ? false : reader.GetBoolean(reader.GetOrdinal("borrado"));
                    centro.borradoCentroPublicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("borradoCentroPublicacion")) == true ? false : reader.GetBoolean(reader.GetOrdinal("borradoCentroPublicacion"));
                    centro.tiene_voluntario = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("tiene_voluntario")) == true ? false : reader.GetBoolean(reader.GetOrdinal("tiene_voluntario"));
                    listaCentros.Add(centro);
                }
                reader.Close();
            }
        }
        private void getCentros(SqlCommand cmdCentros, List<Centro> listaCentros)
        {
            using (SqlDataReader reader = cmdCentros.ExecuteReader())
            {
                while (reader.Read())
                {
                    Centro centro = new Centro();
                    centro.id_centro = reader.GetInt32(reader.GetOrdinal("id_centro"));
                    centro.nombre_centro = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre"));
                    centro.horario_apertura = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("horario_apertura")) == true ? "" : reader.GetString(reader.GetOrdinal("horario_apertura"));
                    if (centro.horario_apertura.IndexOf(":") == 1)
                    {
                        centro.horario_apertura = "0" + centro.horario_apertura;
                    }
                    centro.horario_cierre = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("horario_cierre")) == true ? "" : reader.GetString(reader.GetOrdinal("horario_cierre"));
                    centro.direccion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("direccion")) == true ? "" : reader.GetString(reader.GetOrdinal("direccion"));
                    centro.ubicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion"));
                    centro.borrado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("borrado")) == true ? false : reader.GetBoolean(reader.GetOrdinal("borrado"));
                    centro.tiene_voluntario = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("tiene_voluntario")) == true ? false : reader.GetBoolean(reader.GetOrdinal("tiene_voluntario"));
                    listaCentros.Add(centro);
                }
                reader.Close();
            }
        }

        private void setDiasALosCentros(SqlCommand cmdDias, List<Centro> listaCentros)
        {
            foreach (var centro in listaCentros)
            {
                List<Dias> listaDias = new List<Dias>();
                SqlParameter idCentro = new SqlParameter("@idCentro", centro.id_centro);
                cmdDias.Parameters.Add(idCentro);

                using (SqlDataReader reader = cmdDias.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dias dias = new Dias();
                        dias.idDia = reader.GetInt32(reader.GetOrdinal("idDia"));
                        dias.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                        listaDias.Add(dias);
                    }
                    reader.Close();
                    centro.dias = listaDias;
                    cmdDias.Parameters.Clear();
                }
            }
        }

        public List<Comentarios> obtenerComentariosPublicacionEspecifica(int id)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlCommand cmdComentarios = new SqlCommand("spObtenerPublicacionDeUserEspecificoComentarios", connection);

                cmdComentarios.CommandType = CommandType.StoredProcedure;
                cmdComentarios.CommandTimeout = 0;
                SqlParameter PublicacionIDComentarios = new SqlParameter("@PublicacionID", id);
                cmdComentarios.Parameters.Add(PublicacionIDComentarios);
                List<Comentarios> listacomentarios = new List<Comentarios>();
                using (SqlDataReader reader = cmdComentarios.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Comentarios comentarios = new Comentarios();
                        comentarios.user_id_pregunta = reader.GetInt32(reader.GetOrdinal("usuario_owner_pregunta"));
                        comentarios.id_pregunta = reader.GetInt32(reader.GetOrdinal("ID_pregunta"));
                        comentarios.user_id_respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("usuario_owner_respuesta")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("usuario_owner_respuesta")); ;
                        comentarios.id_respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ID_respuesta")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("ID_respuesta")); ;
                        comentarios.pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("contenido_pregunta")) == true ? "" : reader.GetString(reader.GetOrdinal("contenido_pregunta"));
                        comentarios.nombre_pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre"));
                        comentarios.apellido_pregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido"));
                        comentarios.respuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("contenido")) == true ? "" : reader.GetString(reader.GetOrdinal("contenido"));
                        comentarios.fechaPregunta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion_pregunta")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion_pregunta")).ToString("dd/MM/yyyy");
                        comentarios.fechaRespuesta = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion_respuesta")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion_respuesta")).ToString("dd/MM/yyyy");
                        listacomentarios.Add(comentarios);
                    }
                    reader.Close();
                }
                return listacomentarios;
            }
        }
        public List<Centro> obtenerCentros()
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentros", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                //Setear el command object para que ejecute el stored procedure 

                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;

                List<Centro> listaCentros = new List<Centro>();
                getCentros(cmdCentros, listaCentros);
                setDiasALosCentros(cmdDias, listaCentros);
                return listaCentros;
            }
        }
        public List<Centro> obtenerCentrosPublicacionEspecifica(int idPublicacion)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentroParaUnaPublicacionEspecifica", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                SqlParameter PublicacionIDCentros = new SqlParameter("@idPublicacion", idPublicacion);
                cmdCentros.Parameters.Add(PublicacionIDCentros);
                //Setear el command object para que ejecute el stored procedure 
                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                List<Centro> listaCentros = new List<Centro>();
                getCentros(cmdCentros, listaCentros);
                setDiasALosCentros(cmdDias, listaCentros);
                return listaCentros;
            }
        }
        public List<Centro> obtenerCentrosUserEspecifico(int idUser)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlCommand cmdCentros = new SqlCommand("spObtenerCentroParaUnUserEspecifico", connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                SqlParameter id = new SqlParameter("@idUser", idUser);
                cmdCentros.Parameters.Add(id);
                //Setear el command object para que ejecute el stored procedure 
                cmdCentros.CommandType = CommandType.StoredProcedure;
                cmdCentros.CommandTimeout = 0;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                List<Centro> listaCentros = new List<Centro>();
                getCentros(cmdCentros, listaCentros);
                setDiasALosCentros(cmdDias, listaCentros);
                return listaCentros;
            }
        }

        public Centro obtenerCentroVoluntario(int idUser)
        {
            List<Centro> listaCentros = new List<Centro>();
            Centro centro = new Centro();
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                string query = "SELECT * FROM Centro WHERE id_centro = (SELECT centro FROM Usuarios WHERE ID = @idUser);";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                cmd.Parameters.AddWithValue("@idUser", idUser);
                cmd.CommandType = CommandType.Text;
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        centro.id_centro = reader.GetInt32(0);
                        centro.nombre_centro = reader.GetString(1);
                        centro.ubicacion = reader.GetString(2);
                        centro.direccion = reader.GetString(3);
                        centro.horario_apertura = reader.GetString(4);
                        centro.horario_cierre = reader.GetString(5);
                        centro.borrado = reader.GetBoolean(6);
                    }
                    reader.Close();
                }

                listaCentros.Add(centro);
                this.setDiasALosCentros(cmdDias, listaCentros);
                connection.Close();
            }
            return listaCentros[0];
        }

        // Obtener publicaciones por id de usuario
        // Hay código duplicado, crear uno o dos metodos privados
        public List<Publicacion_Usuario> obtenerPublicacionesPorIdUsuario(int idUsuario)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerPublicacionesDelUsuario", connection);
                //Setear el command object para que ejecute el stored procedure
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_usuario = new SqlParameter("@id_usuario", idUsuario);
                cmd.Parameters.Add(id_usuario);

                List<Publicacion_Usuario> listaPublicacionesDelUsuario = new List<Publicacion_Usuario>();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Publicacion_Usuario publi_user = new Publicacion_Usuario();
                        publi_user.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        publi_user.titulo = reader.GetString(reader.GetOrdinal("titulo"));
                        publi_user.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                        publi_user.categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("categoria_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("categoria_producto"));
                        publi_user.nombre_categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreCategoria")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreCategoria"));
                        publi_user.estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_producto")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_producto"));
                        publi_user.nombre_estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreEstado")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreEstado"));
                        publi_user.ubicacion_trade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_trade")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_trade"));
                        publi_user.estado_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_publicacion")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_publicacion"));
                        publi_user.fecha_publicacion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_publicacion")) == true ? "" : reader.GetDateTime(reader.GetOrdinal("fecha_publicacion")).ToString("dd/MM/yyyy");
                        publi_user.id_usuario = reader.GetInt32(reader.GetOrdinal("usuario_owner"));
                        listaPublicacionesDelUsuario.Add(publi_user);
                    }
                    reader.Close();
                }

                // Obtener imagenes de las publicaciones
                foreach (var publicacion in listaPublicacionesDelUsuario)
                {
                    List<ImagenesCodificadas> listaimagenes = new List<ImagenesCodificadas>();
                    using (SqlCommand cdmImagenes = new SqlCommand("spMultimedia", connection))
                    {
                        cdmImagenes.CommandType = CommandType.StoredProcedure;
                        cdmImagenes.Parameters.AddWithValue("@idpublicacion", publicacion.ID);
                        using (SqlDataReader readerImagenes = cdmImagenes.ExecuteReader())
                        {
                            while (readerImagenes.Read())
                            {
                                ImagenesCodificadas imagen = new ImagenesCodificadas();
                                imagen.base64_imagen = obtenerSiEsNuloElCampo(readerImagenes, readerImagenes.GetOrdinal("base64_imagen")) == true ? "" : readerImagenes.GetString(readerImagenes.GetOrdinal("base64_imagen"));
                                listaimagenes.Add(imagen);
                            }
                            publicacion.Imagenes = listaimagenes;
                            readerImagenes.Close();
                        }
                    }

                }
                return listaPublicacionesDelUsuario;
            }
        }

        public void eliminarPregunta(EliminarPregunta model, bool esBasico)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spEliminarPregunta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_pregunta = new SqlParameter("@id_pregunta", model.id_pregunta);
                cmd.Parameters.Add(id_pregunta);
                SqlParameter id_usuario_pregunta = new SqlParameter("@id_usuario_pregunta", model.id_usuario_que_va_a_borrar);
                cmd.Parameters.Add(id_usuario_pregunta);
                SqlParameter esBasicoUser = new SqlParameter("@esBasico", esBasico);
                cmd.Parameters.Add(esBasicoUser);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void borrarCentro(int idCentro)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_BorrarCentro", connection);
                SqlCommand cmd2 = new SqlCommand("sp_actualizarPublicacionAPausado", connection);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                SqlParameter centro = new SqlParameter("@idCentro", idCentro);
                cmd.Parameters.Add(centro);
                cmd.ExecuteNonQuery();
                List<PublicacionCentroBorrar> listadoPublicacion = this.getPublicacionesParaBorrarCentro(0);
                List<int> idPublicaciones = new List<int>();
                foreach (var item in listadoPublicacion)
                {
                    if (item.centros.All(x => x.borrado))
                    {
                        if (item.estado_publicacion == 1)
                        {
                            idPublicaciones.Add(item.ID);
                        }
                    }


                    /*List<CentroBorrar> listado = item.centros.Where(x => x.borradoCentroPublicacion == false).ToList();
                    if (listado.Count() == 1)
                    {
                        if(listado.FirstOrDefault().id_centro == idCentro && listado.FirstOrDefault().borrado)
                        {
                            idPublicaciones.Add(item.ID);
                        }
                    }*/

                }

                if (idPublicaciones.Count > 0)
                {
                    foreach (var item in idPublicaciones)
                    {
                        SqlParameter idPubli = new SqlParameter("@idPublicacion", item);
                        cmd2.Parameters.Add(idPubli);
                        cmd2.ExecuteNonQuery();
                        cmd2.Parameters.Clear();
                    }
                }
                connection.Close();
            }
        }

        public void eliminarRespuesta(EliminarRespuesta model, bool esBasico)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spEliminarRespuesta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_respuesta = new SqlParameter("@id_respuesta", model.id_respuesta);
                cmd.Parameters.Add(id_respuesta);
                SqlParameter id_usuario_respuesta = new SqlParameter("@id_usuario_respuesta", model.id_usuario_que_va_a_borrar);
                cmd.Parameters.Add(id_usuario_respuesta);
                SqlParameter esBasicoUser = new SqlParameter("@esBasico", esBasico);
                cmd.Parameters.Add(esBasicoUser);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void crearPublicacion(PublicacionViewModel model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                // Comienza la transacción
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spCrearPublicacionSinImagenes", connection);
                    SqlCommand cmdInsertMultimedia = new SqlCommand("spInsertarEnMultimedia", connection);
                    SqlCommand cmdInsertCentros = new SqlCommand("spInsertarEnCentros", connection);
                    SqlCommand cmdInsertCentro_Publicacion_Dias = new SqlCommand("InsertarIndentificadorACentro_Dias_Publicacion", connection);
                    // Asigna la transacción a los comandos
                    cmd.Transaction = transaction;
                    cmdInsertMultimedia.Transaction = transaction;
                    cmdInsertCentros.Transaction = transaction;
                    cmdInsertCentro_Publicacion_Dias.Transaction = transaction;
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmdInsertMultimedia.CommandType = CommandType.StoredProcedure;
                    cmdInsertMultimedia.CommandTimeout = 0;
                    cmdInsertCentros.CommandType = CommandType.StoredProcedure;
                    cmdInsertCentros.CommandTimeout = 0;
                    cmdInsertCentro_Publicacion_Dias.CommandType = CommandType.StoredProcedure;
                    cmdInsertCentro_Publicacion_Dias.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter titulo = new SqlParameter("@titulo", model.titulo);
                    cmd.Parameters.Add(titulo);
                    SqlParameter descripcion = new SqlParameter("@descripcion", model.descripcion);
                    cmd.Parameters.Add(descripcion);
                    SqlParameter categoria_producto = new SqlParameter("@categoria_producto", model.categoria_producto);
                    cmd.Parameters.Add(categoria_producto);
                    SqlParameter usuario_owner = new SqlParameter("@usuario_owner", model.usuario_owner);
                    cmd.Parameters.Add(usuario_owner);
                    SqlParameter fecha_publicacion = new SqlParameter("@fecha_publicacion", DateTime.Now);
                    cmd.Parameters.Add(fecha_publicacion);
                    SqlParameter estado_producto = new SqlParameter("@estado_producto", model.estado_producto);
                    cmd.Parameters.Add(estado_producto);
                    SqlParameter ubication_trade = new SqlParameter("@ubication_trade", model.ubicacion_trade);
                    cmd.Parameters.Add(ubication_trade);
                    // Ejecutar el comando y obtener el ID de la publicación creada
                    var idPublicacion = cmd.ExecuteScalar();

                    foreach (var imagen in model.imagenesEnBase64)
                    {
                        SqlParameter publicacion = new SqlParameter("@publicacionId", idPublicacion);
                        cmdInsertMultimedia.Parameters.Add(publicacion);
                        SqlParameter base64 = new SqlParameter("@base64Imagen", imagen);
                        cmdInsertMultimedia.Parameters.Add(base64);
                        cmdInsertMultimedia.ExecuteNonQuery();
                        cmdInsertMultimedia.Parameters.Clear();
                    }

                    /*foreach (var idCentro in model.centros_elegidos)
                    {
                        SqlParameter Publicacion = new SqlParameter("@idPublicacion", idPublicacion);
                        cmdInsertCentros.Parameters.Add(Publicacion);
                        SqlParameter centro = new SqlParameter("@centro", idCentro);
                        cmdInsertCentros.Parameters.Add(centro);
                        SqlParameter esPublicacion = new SqlParameter("@EsPublicacion", true);
                        cmdInsertCentros.Parameters.Add(esPublicacion);
                        cmdInsertCentros.ExecuteNonQuery();
                        cmdInsertCentros.Parameters.Clear();
                    }*/

                    for (int i = 0; i < model.centros_elegidos.Count; i++)
                    {
                        SqlParameter Publicacion = new SqlParameter("@idPublicacion", idPublicacion);
                        cmdInsertCentros.Parameters.Add(Publicacion);
                        SqlParameter centro = new SqlParameter("@centro", model.centros_elegidos[i]);
                        cmdInsertCentros.Parameters.Add(centro);
                        SqlParameter esPublicacion = new SqlParameter("@EsPublicacion", true);
                        cmdInsertCentros.Parameters.Add(esPublicacion);
                        SqlParameter desde = new SqlParameter("@desde", model.desde[i]);
                        cmdInsertCentros.Parameters.Add(desde);
                        SqlParameter hasta = new SqlParameter("@hasta", model.hasta[i]);
                        cmdInsertCentros.Parameters.Add(hasta);
                        //cmdInsertCentros.ExecuteNonQuery();
                        var idCentroPublicacion = cmdInsertCentros.ExecuteScalar();
                        cmdInsertCentros.Parameters.Clear();

                        foreach (var dia in model.dias_elegidos[i])
                        {
                            SqlParameter centroPublicacionFK = new SqlParameter("@id_centro_publicacion", idCentroPublicacion);
                            cmdInsertCentro_Publicacion_Dias.Parameters.Add(centroPublicacionFK);
                            SqlParameter idDia = new SqlParameter("@id_dia", dia);
                            cmdInsertCentro_Publicacion_Dias.Parameters.Add(idDia);
                            cmdInsertCentro_Publicacion_Dias.ExecuteNonQuery();
                            cmdInsertCentro_Publicacion_Dias.Parameters.Clear();
                        }
                    }

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private string ConvertToBase64(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(bytes);
                return "data:image/jpeg;base64," + base64String;
            }
        }
        public void cambiarDatosPersonales(CambiarDatosViewModel model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spCambiarDatosUsuario", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter @usuario_owner = new SqlParameter("@usuario_owner", model.usuario_owner);
                cmd.Parameters.Add(@usuario_owner);
                SqlParameter nombre = new SqlParameter("@nombre", model.nombre);
                cmd.Parameters.Add(nombre);
                SqlParameter apellido = new SqlParameter("@apellido", model.apellido);
                cmd.Parameters.Add(apellido);
                SqlParameter DNI = new SqlParameter("@dni", model.dni);
                cmd.Parameters.Add(DNI);
                SqlParameter password = new SqlParameter("@password", model.password);
                cmd.Parameters.Add(password);
                SqlParameter fecha_nacimiento = new SqlParameter("@fecha_nacimiento", model.fecha_nacimiento);
                cmd.Parameters.Add(fecha_nacimiento);
                SqlParameter foto_perfil_base64 = new SqlParameter("@foto_perfil_base64", model.foto);
                cmd.Parameters.Add(foto_perfil_base64);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            if (model.centros_elegidos.Count > 0)
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("spBorrarCentroPorIdDelUsuario", connection);
                    SqlCommand cmdInsertCentros = new SqlCommand("spInsertarEnCentros", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmdInsertCentros.CommandType = CommandType.StoredProcedure;
                    cmdInsertCentros.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario = new SqlParameter("@id_usuario", model.usuario_owner);
                    cmd.Parameters.Add(id_usuario);
                    cmd.ExecuteNonQuery();
                    foreach (var idCentro in model.centros_elegidos)
                    {
                        SqlParameter userID = new SqlParameter("@idUser", model.usuario_owner);
                        cmdInsertCentros.Parameters.Add(userID);
                        SqlParameter centro = new SqlParameter("@centro", idCentro);
                        cmdInsertCentros.Parameters.Add(centro);
                        SqlParameter publicacionBool = new SqlParameter("@EsPublicacion", false);
                        cmdInsertCentros.Parameters.Add(publicacionBool);
                        cmdInsertCentros.ExecuteNonQuery();
                        cmdInsertCentros.Parameters.Clear();
                    }
                    connection.Close();
                }
            }
        }

        public void cambiarDatosPersonalesVoluntario(CambiarModificarDatosVoluntario model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spCambiarDatosVoluntario", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_usuario = new SqlParameter("@id_usuario", model.id_usuario);
                cmd.Parameters.Add(id_usuario);
                SqlParameter nombre = new SqlParameter("@nombre", model.nombre);
                cmd.Parameters.Add(nombre);
                SqlParameter apellido = new SqlParameter("@apellido", model.apellido);
                cmd.Parameters.Add(apellido);
                SqlParameter DNI = new SqlParameter("@dni", model.dni);
                cmd.Parameters.Add(DNI);
                SqlParameter email = new SqlParameter("@correo", model.email);
                cmd.Parameters.Add(email);
                SqlParameter nacimiento = new SqlParameter("@nacimiento", model.nacimiento);
                cmd.Parameters.Add(nacimiento);
                SqlParameter centro = new SqlParameter("@centro", model.centro);
                cmd.Parameters.Add(centro);
                SqlParameter foto = new SqlParameter("@foto_perfil_base64", model.foto);
                cmd.Parameters.Add(foto);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public string obtenerFotoUsuario(int idUser)
        {
            string base64Imagen = "";
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerFotoUsuario", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id = new SqlParameter("@userID", idUser);
                cmd.Parameters.Add(id);
                base64Imagen = (string)cmd.ExecuteScalar();
                connection.Close();
            }
            var jsonObj = new { fotoUser = base64Imagen };
            string jsonString = JsonSerializer.Serialize(jsonObj);
            return jsonString;
        }

        // Método para indicar que un usuario quiere restablecer su clave y se vuelve a
        // utilizar cuando se restablece la clave.
        public bool restablecerActualizarClave(int restablecer, string clave, string token)
        {
            bool respuesta = false;
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    // Se tendría que hacer un sp para mas seguridad, pero es corta la consulta.
                    string query = "update Usuarios set " +
                        "Restablecer = @restablecer, " +
                        "password = @clave " +
                        "where Token = @token";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@restablecer", restablecer);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.CommandType = CommandType.Text;

                    connection.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        respuesta = true;
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Metodo para obtener un usuario por su correo
        public UsuarioRestablecer obtenerUsuarioPorEmail(string correo)
        {
            UsuarioRestablecer usuario = new UsuarioRestablecer();
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    // Se tendría que hacer un sp para mas seguridad, pero es corta la consulta.
                    string query = "select * from Usuarios where mail = @correo";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Se cuentan las columnas desde 0.
                            usuario.Id = reader.GetInt32(0);
                            usuario.Nombre = reader.GetString(1);
                            usuario.Clave = reader.GetString(5);
                            usuario.Token = reader.GetString(12);
                        }
                    }
                    reader.Close();
                }
                return usuario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // metodo para guardar una publicacion como favoritos o quitar de favoritos
        public string modificarFavorito(int idUser, int idPublicacion, bool action)
        {
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spModificarFavorito", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario = new SqlParameter("@id_usuario", idUser);
                    cmd.Parameters.Add(id_usuario);
                    SqlParameter id_publicacion = new SqlParameter("@id_publicacion", idPublicacion);
                    cmd.Parameters.Add(id_publicacion);
                    SqlParameter accion = new SqlParameter("@action", action);
                    cmd.Parameters.Add(accion);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                return "Se ha modificado la publicación como favorito";
            }
            catch (Exception ex)
            {
                return "Ha ocurrido un error al modificar la publicación como favorito";
            }
        }

        // metodo para obtener las publicaciones favoritas de un usuario siempre que esten con estado 1
        public List<int> getFavsIdsUser(int idUser)
        {
            List<int> listaFavoritos = new List<int>();
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spObtenerPublicacionesFavoritas", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario = new SqlParameter("@idUsuario", idUser);
                    cmd.Parameters.Add(id_usuario);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaFavoritos.Add(reader.GetInt32(reader.GetOrdinal("ID")));
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
                return listaFavoritos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // metodo para obtener las publicaciones favoritas de un usuario, solo uso las tablas de Publicaciones
        // y las tablas de multimedia siempre que esten con estado 1 para las miniaturas
        public List<PublicacionMiniatura> obtenerPublicacionesFavoritas(int idUser)
        {
            List<PublicacionMiniatura> publicacionMiniaturas = new List<PublicacionMiniatura>();
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spObtenerPublicacionesFavoritas", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario = new SqlParameter("@idUsuario", idUser);
                    cmd.Parameters.Add(id_usuario);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PublicacionMiniatura publicacion = new PublicacionMiniatura();
                            publicacion.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                            publicacion.titulo = reader.GetString(reader.GetOrdinal("titulo"));
                            publicacion.descripcion = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion"));
                            publicacion.nombre_categoria_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("categoria_nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("categoria_nombre"));
                            publicacion.nombre_estado_producto = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_producto_nombre")) == true ? "" : reader.GetString(reader.GetOrdinal("estado_producto_nombre"));
                            publicacion.localidad = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_trade")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_trade"));
                            publicacion.imagenEnBase64 = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("base64_imagen")) == true ? "" : reader.GetString(reader.GetOrdinal("base64_imagen"));
                            publicacionMiniaturas.Add(publicacion);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
                return publicacionMiniaturas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // obtener ofertas recibidas y ofertas realizadas por un usuario dado su id
        public OfertasInvolucradas obtenerOfertasUsuario(int idUser)
        {
            OfertasInvolucradas ofertas = new OfertasInvolucradas();
            ofertas.ofertasRecibidas = new List<OfertaViewModel>();
            ofertas.ofertasRealizadas = new List<OfertaViewModel>();
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spObtenerOfertasInvolucradasDeUsuario", connection);
                    //Setear el command object para que ejecute el stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario = new SqlParameter("@idUsuario", idUser);
                    cmd.Parameters.Add(id_usuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OfertaViewModel oferta = new OfertaViewModel();
                            // id de la oferta
                            oferta.idOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("idOferta")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("idOferta"));
                            // estado de la oferta
                            oferta.offerState = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("estado_oferta")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("estado_oferta"));
                            // el dueño de la publicacion que recibe la oferta
                            oferta.idUserOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_usuario_owner")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_usuario_owner"));
                            oferta.nameUserOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre_uOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre_uOwner"));
                            oferta.surnameUserOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido_uOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido_uOwner"));
                            oferta.profilePicUserOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("img_perfil_owner")) == true ? "" : reader.GetString(reader.GetOrdinal("img_perfil_owner"));
                            oferta.idPostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_pOwner")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_pOwner"));
                            oferta.titlePostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("titulo_pOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("titulo_pOwner"));
                            oferta.descriptionPostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion_pOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion_pOwner"));
                            oferta.nameProductCategoriePostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion_cOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion_cOwner"));
                            oferta.nameProductStatePostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre_epOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre_epOwner"));
                            oferta.locationTradePostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_pOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_pOwner"));
                            oferta.imagePostOwner = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("imagen_fiOwner")) == true ? "" : reader.GetString(reader.GetOrdinal("imagen_fiOwner"));

                            // el usuario que hace la oferta
                            oferta.idUserOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_usuario_offer")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_usuario_offer"));
                            oferta.nameUserOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre_uOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre_uOffer"));
                            oferta.surnameUserOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido_uOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido_uOffer"));
                            oferta.profilePicUserOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("img_perfil_offer")) == true ? "" : reader.GetString(reader.GetOrdinal("img_perfil_offer"));
                            oferta.idPostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_pOffer")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_pOffer"));
                            oferta.titlePostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("titulo_pOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("titulo_pOffer"));
                            oferta.descriptionPostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion_pOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion_pOffer"));
                            oferta.nameProductCategoriePostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("descripcion_cOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("descripcion_cOffer"));
                            oferta.nameProductStatePostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre_epOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre_epOffer"));
                            oferta.locationTradePostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacion_pOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacion_pOffer"));
                            oferta.imagePostOffer = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("imagen_fiOffer")) == true ? "" : reader.GetString(reader.GetOrdinal("imagen_fiOffer"));

                            // los datos que elegio el usuario que hace la oferta: centro, hora y fecha elegida para el intercambio
                            oferta.idCenterPostChoosedTrade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_centro_elegido")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_centro_elegido"));
                            oferta.idRawCenterPostChoosed = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_centro")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("id_centro"));
                            oferta.nameCenterPostChoosedTrade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombreCentro")) == true ? "" : reader.GetString(reader.GetOrdinal("nombreCentro"));
                            oferta.addressCenterPostChoosedTrade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("direccionCentro")) == true ? "" : reader.GetString(reader.GetOrdinal("direccionCentro"));
                            oferta.hourCenterPostChoosedTrade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("horario")) == true ? "" : reader.GetString(reader.GetOrdinal("horario"));
                            oferta.locationTradeCenterChoosed = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("ubicacionCentro")) == true ? "" : reader.GetString(reader.GetOrdinal("ubicacionCentro"));
                            oferta.dateCenterPostChoosedTrade = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("fecha_intercambio")) == true ? "" : reader.GetString(reader.GetOrdinal("fecha_intercambio"));

                            // si soy el dueño de la publicacion entonces recibi una oferta
                            if (oferta.idUserOwner == idUser)
                            {
                                ofertas.ofertasRecibidas.Add(oferta);
                            }
                            else
                            {
                                ofertas.ofertasRealizadas.Add(oferta);
                            }
                        }
                        reader.Close();
                    }
                }
                return ofertas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void borrarPublicacion(BorrarPublicacion model, bool esBasico, int idUser)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spEliminarPublicacion", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_pregunta = new SqlParameter("@idPublicacion", model.id_publicacion);
                cmd.Parameters.Add(id_pregunta);
                SqlParameter esBasicoUser = new SqlParameter("@esBasico", esBasico);
                cmd.Parameters.Add(esBasicoUser);
                SqlParameter idUsuario = new SqlParameter("@id_usuario", idUser);
                cmd.Parameters.Add(idUsuario);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void rechazarCancelarOferta(Oferta model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spRechazarCancelarOferta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idOfer = new SqlParameter("@idOferta", model.id_oferta);
                cmd.Parameters.Add(idOfer);
                SqlParameter post = new SqlParameter("@idpost", model.id_post);
                cmd.Parameters.Add(post);
                SqlParameter cancelar = new SqlParameter("@cancelar", model.cancelar);
                cmd.Parameters.Add(cancelar);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void aceptarOferta(AceptarOferta model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spAceptarOferta", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idOfer = new SqlParameter("@idOferta", model.id_oferta);
                cmd.Parameters.Add(idOfer);
                SqlParameter post = new SqlParameter("@idpost", model.id_post);
                cmd.Parameters.Add(post);
                SqlParameter id_centro = new SqlParameter("@idCentro", model.id_centro);
                cmd.Parameters.Add(id_centro);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void cancelarIntercambioPendiente(CancelarIntercambio model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spCancelarIntercambioPendiente", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_trade = new SqlParameter("@id_trade", model.id_trade);
                cmd.Parameters.Add(id_trade);
                SqlParameter postOffer = new SqlParameter("@idpostoffer", model.id_post_offer);
                cmd.Parameters.Add(postOffer);
                SqlParameter postOwner = new SqlParameter("@idpostowner", model.id_post_owner);
                cmd.Parameters.Add(postOwner);
                SqlParameter motivo_cancelacion = new SqlParameter("@motivo_cancelacion", model.motivo_cancelacion);
                cmd.Parameters.Add(motivo_cancelacion);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public List<Intercambio> obtenerIntercambiosEnEstadoPendiente()
        {
            List<Intercambio> listado = new List<Intercambio>();
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spObtenerListadoDeIntercambiosPendientes", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Intercambio inter = new Intercambio();
                        inter.IdIntercambio = reader.GetInt32(reader.GetOrdinal("idIntercambio"));
                        inter.IdUsuarioOwner = reader.GetInt32(reader.GetOrdinal("id_usuario_owner"));
                        inter.IdUsuarioOffer = reader.GetInt32(reader.GetOrdinal("id_usuario_offer"));
                        inter.IdPostOwner = reader.GetInt32(reader.GetOrdinal("id_post_owner"));
                        inter.IdPostOffer = reader.GetInt32(reader.GetOrdinal("id_post_offer"));
                        inter.IdCentroElegido = reader.GetInt32(reader.GetOrdinal("id_centro_elegido"));
                        inter.FechaIntercambio = reader.GetString(reader.GetOrdinal("fecha_intercambio"));
                        inter.IdEstadoPublicacion = reader.GetInt32(reader.GetOrdinal("id_estado_publicacion"));
                        inter.ProductoDonado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("producto_donado")) == true ? "" : reader.GetString(reader.GetOrdinal("producto_donado"));
                        inter.IdEstadoIntercambio = reader.GetInt32(reader.GetOrdinal("id_estado_intercambio"));
                        inter.Horario = reader.GetString(reader.GetOrdinal("horario"));
                        listado.Add(inter);
                    }
                    reader.Close();
                }
                return listado;
            }
        }
        public List<IntercambioDetalle> obtenerIntercambiosEnEstadoPendienteUser(int idUser)
        {
            var detalles = new List<IntercambioDetalle>();

            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("GetIntercambioCompleto", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetro idUSer al procedimiento almacenado
                    cmd.Parameters.AddWithValue("@idUser", idUser);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var detalle = new IntercambioDetalle
                            {
                                IdIntercambio = (int)reader["idIntercambio"],
                                IdUsuarioOwner = (int)reader["id_usuario_owner"],
                                NombreUsuarioOwner = reader["nombre_usuario_owner"].ToString(),
                                ApellidoUsuarioOwner = reader["apellido_usuario_owner"].ToString(),
                                dniOwner = reader["dni_owner"].ToString(),
                                FotoPerfilUsuarioOwner = reader["foto_perfil_usuario_owner"].ToString(),
                                IdUsuarioOffer = (int)reader["id_usuario_offer"],
                                NombreUsuarioOffer = reader["nombre_usuario_offer"].ToString(),
                                ApellidoUsuarioOffer = reader["apellido_usuario_offer"].ToString(),
                                dniOffer = reader["dni_offer"].ToString(),
                                FotoPerfilUsuarioOffer = reader["foto_perfil_usuario_offer"].ToString(),
                                IdPostOwner = (int)reader["id_post_owner"],
                                FotoPostOwner = reader["foto_post_owner"].ToString(),
                                IdPostOffer = (int)reader["id_post_offer"],
                                FotoPostOffer = reader["foto_post_offer"].ToString(),
                                IdCentroElegido = (int)reader["id_centro_elegido"],
                                IdCentro = (int)reader["id_centro"],
                                NombreCentro = reader["nombre_centro"].ToString(),
                                localidad = reader["localidad_centro"].ToString(),
                                direccion = reader["direccion_centro"].ToString(),
                                FechaIntercambio = reader["fecha_intercambio"].ToString(),
                                IdEstadoPublicacion = (int)reader["id_estado_publicacion"],
                                ProductoDonado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("producto_donado")) == true ? "" : reader.GetString(reader.GetOrdinal("producto_donado")),
                                IdEstadoIntercambio = (int)reader["id_estado_intercambio"],
                                Horario = reader["horario"].ToString()
                            };
                            detalles.Add(detalle);
                        }
                    }
                }
            }

            return detalles;
        }
        public List<IntercambioDetalleCentro> obtenerIntercambiosEnEstadoPendienteCentro(int idcentro)
        {
            var detalles = new List<IntercambioDetalleCentro>();

            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("GetIntercambioCompletoCentro", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetro idUSer al procedimiento almacenado
                    cmd.Parameters.AddWithValue("@idCentro", idcentro);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var detalle = new IntercambioDetalleCentro
                            {
                                IdIntercambio = (int)reader["idIntercambio"],
                                IdUsuarioOwner = (int)reader["id_usuario_owner"],
                                NombreUsuarioOwner = reader["nombre_usuario_owner"].ToString(),
                                ApellidoUsuarioOwner = reader["apellido_usuario_owner"].ToString(),
                                dniOwner = reader["dni_owner"].ToString(),
                                FotoPerfilUsuarioOwner = reader["foto_perfil_usuario_owner"].ToString(),
                                IdUsuarioOffer = (int)reader["id_usuario_offer"],
                                NombreUsuarioOffer = reader["nombre_usuario_offer"].ToString(),
                                ApellidoUsuarioOffer = reader["apellido_usuario_offer"].ToString(),
                                dniOffer = reader["dni_offer"].ToString(),
                                dniAuditor = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("dni_auditor")) == true ? "" : reader.GetString(reader.GetOrdinal("dni_auditor")),
                                nombreAuditor = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("nombre_auditor")) == true ? "" : reader.GetString(reader.GetOrdinal("nombre_auditor")),
                                apellidoAuditor = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("apellido_auditor")) == true ? "" : reader.GetString(reader.GetOrdinal("apellido_auditor")),
                                idAuditor = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("id_usuario_auditor")) == true ? -1 : reader.GetInt32(reader.GetOrdinal("id_usuario_auditor")),
                                fotoPerfilAuditor = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("foto_perfil_auditor")) == true ? "" : reader.GetString(reader.GetOrdinal("foto_perfil_auditor")),
                                FotoPerfilUsuarioOffer = reader["foto_perfil_usuario_offer"].ToString(),
                                IdPostOwner = (int)reader["id_post_owner"],
                                FotoPostOwner = reader["foto_post_owner"].ToString(),
                                IdPostOffer = (int)reader["id_post_offer"],
                                FotoPostOffer = reader["foto_post_offer"].ToString(),
                                IdCentroElegido = (int)reader["id_centro_elegido"],
                                IdCentro = (int)reader["id_centro"],
                                NombreCentro = reader["nombre_centro"].ToString(),
                                localidad = reader["localidad_centro"].ToString(),
                                direccion = reader["direccion_centro"].ToString(),
                                FechaIntercambio = reader["fecha_intercambio"].ToString(),
                                IdEstadoPublicacion = (int)reader["id_estado_publicacion"],
                                ProductoDonado = obtenerSiEsNuloElCampo(reader, reader.GetOrdinal("producto_donado")) == true ? "" : reader.GetString(reader.GetOrdinal("producto_donado")),
                                IdEstadoIntercambio = (int)reader["id_estado_intercambio"],
                                Horario = reader["horario"].ToString()
                            };
                            detalles.Add(detalle);
                        }
                    }
                }
            }
            return detalles;
        }

        public void rechazarIntercambio(IntercambioAceptarRechazar model, int idAuditor)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spRechazarIntercambio", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idIntercambio = new SqlParameter("@idIntercambio", model.id_intercambio);
                cmd.Parameters.Add(idIntercambio);
                SqlParameter idpostOffer = new SqlParameter("@idpostOffer", model.id_post_offer);
                cmd.Parameters.Add(idpostOffer);
                SqlParameter idpostOwner = new SqlParameter("@idpostOwner", model.id_post_owner);
                cmd.Parameters.Add(idpostOwner);
                SqlParameter auditor = new SqlParameter("@idAuditor", idAuditor);
                cmd.Parameters.Add(auditor);
                SqlParameter productoDonado = new SqlParameter("@productoDonado", model.productoDonado);
                cmd.Parameters.Add(productoDonado);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void cambiarCentro(CentroPublicacionModificar model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("sp_ModificarCentroNuevo", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idCentroViejo = new SqlParameter("@idCentroViejo", model.id_centro_viejo);
                cmd.Parameters.Add(idCentroViejo);
                SqlParameter idCentroNuevo = new SqlParameter("@idCentroNuevo", model.id_centro_nuevo);
                cmd.Parameters.Add(idCentroNuevo);
                SqlParameter idPublicacion = new SqlParameter("@idPublicacion", model.id_publicacion);
                cmd.Parameters.Add(idPublicacion);
                SqlParameter Desde = new SqlParameter("@Desde", model.desde);
                cmd.Parameters.Add(Desde);
                SqlParameter hasta = new SqlParameter("@hasta", model.hasta);
                cmd.Parameters.Add(hasta);
                int idCentro = Convert.ToInt32(cmd.ExecuteScalar());
                SqlCommand cmdCentroDias = new SqlCommand("sp_InsertarCentroDiasPublicacion", connection);
                cmdCentroDias.CommandType = CommandType.StoredProcedure;
                cmdCentroDias.CommandTimeout = 0;
                foreach (var dia in model.dias)
                {
                    cmdCentroDias.Parameters.Clear();
                    cmdCentroDias.Parameters.Add(new SqlParameter("@idCentroPublicacion", idCentro));
                    cmdCentroDias.Parameters.Add(new SqlParameter("@idDia", dia));
                    cmdCentroDias.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void aceptarIntercambio(IntercambioAceptarRechazar model, int idAuditor)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spAceptarIntercambio", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter idIntercambio = new SqlParameter("@idIntercambio", model.id_intercambio);
                cmd.Parameters.Add(idIntercambio);
                SqlParameter idpostOffer = new SqlParameter("@idpostOffer", model.id_post_offer);
                cmd.Parameters.Add(idpostOffer);
                SqlParameter idpostOwner = new SqlParameter("@idpostOwner", model.id_post_owner);
                cmd.Parameters.Add(idpostOwner);
                SqlParameter auditor = new SqlParameter("@idAuditor", idAuditor);
                cmd.Parameters.Add(auditor);
                SqlParameter productoDonado = new SqlParameter("@productoDonado", model.productoDonado);
                cmd.Parameters.Add(productoDonado);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void insertarCentro(CentroParaInsertar model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Crear el Command Object para insertar en la tabla Centro
                    SqlCommand cmdCentro = new SqlCommand("sp_InsertarCentro", connection, transaction);
                    cmdCentro.CommandType = CommandType.StoredProcedure;
                    cmdCentro.CommandTimeout = 0;

                    // Agregar los parámetros del stored procedure sp_InsertarCentro
                    cmdCentro.Parameters.Add(new SqlParameter("@nombre", model.nombre_centro));
                    cmdCentro.Parameters.Add(new SqlParameter("@ubicacion", model.ubicacion));
                    cmdCentro.Parameters.Add(new SqlParameter("@direccion", model.direccion));
                    cmdCentro.Parameters.Add(new SqlParameter("@horario_apertura", model.horario_apertura));
                    cmdCentro.Parameters.Add(new SqlParameter("@horario_cierre", model.horario_cierre));
                    int idCentro = Convert.ToInt32(cmdCentro.ExecuteScalar());

                    // Crear el Command Object para insertar en la tabla Centro_dias
                    SqlCommand cmdCentroDias = new SqlCommand("sp_InsertarCentroDias", connection, transaction);
                    cmdCentroDias.CommandType = CommandType.StoredProcedure;
                    cmdCentroDias.CommandTimeout = 0;

                    // Insertar cada día asociado al centro
                    foreach (var dia in model.dias)
                    {
                        cmdCentroDias.Parameters.Clear();
                        cmdCentroDias.Parameters.Add(new SqlParameter("@idCentro", idCentro));
                        cmdCentroDias.Parameters.Add(new SqlParameter("@idDia", dia));
                        cmdCentroDias.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback the transaction if any error occurs
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }



        public bool ofertarPublicacion(SolicitudOferta model)
        {
            try
            {
                using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
                {
                    connection.Open();
                    //Crear el Command Object con el stored procedure
                    SqlCommand cmd = new SqlCommand("spInsertarOferta", connection);
                    //Setear el command object para que ejecute el stored procedure 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //Agregar los parámetros del stored procedure
                    SqlParameter id_usuario_owner = new SqlParameter("@id_usuario_duenio_publicacion_a_la_que_se_oferta", model.id_usuario_duenio_publicacion_a_la_que_se_oferta);
                    cmd.Parameters.Add(id_usuario_owner);
                    SqlParameter id_usuario_offer = new SqlParameter("@id_usuario_que_oferta", model.id_usuario_que_oferta);
                    cmd.Parameters.Add(id_usuario_offer);
                    SqlParameter id_post_owner = new SqlParameter("@id_publicacion_a_la_que_se_oferta", model.id_publicacion_a_la_que_se_oferta);
                    cmd.Parameters.Add(id_post_owner);
                    SqlParameter id_post_offer = new SqlParameter("@id_publicacion_con_la_que_se_oferta", model.id_publicacion_con_la_que_se_oferta);
                    cmd.Parameters.Add(id_post_offer);
                    SqlParameter id_centro_elegido = new SqlParameter("@centro_elegido", model.centro_elegido);
                    cmd.Parameters.Add(id_centro_elegido);
                    SqlParameter fecha_intercambio = new SqlParameter("@dia_elegido", model.dia_elegido);
                    cmd.Parameters.Add(fecha_intercambio);
                    SqlParameter horario = new SqlParameter("@hora_elegida", model.hora_elegida);
                    cmd.Parameters.Add(horario);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void ActualizarUsuarioBorrado(int usuarioID)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                //Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spActualizarUsuarioBorrado", connection);
                //Setear el command object para que ejecute el stored procedure 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //Agregar los parámetros del stored procedure
                SqlParameter id_pregunta = new SqlParameter("@UsuarioID", usuarioID);
                cmd.Parameters.Add(id_pregunta);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void actualizarCentro(CentroModificar model)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Crear el Command Object para actualizar en la tabla Centro
                    SqlCommand cmdCentro = new SqlCommand("sp_ActualizarCentro", connection, transaction);
                    cmdCentro.CommandType = CommandType.StoredProcedure;
                    cmdCentro.CommandTimeout = 0;

                    // Agregar los parámetros del stored procedure sp_ActualizarCentro
                    cmdCentro.Parameters.Add(new SqlParameter("@idCentro", model.id_centro));
                    cmdCentro.Parameters.Add(new SqlParameter("@nombre", model.nombre_centro));
                    cmdCentro.Parameters.Add(new SqlParameter("@ubicacion", model.ubicacion));
                    cmdCentro.Parameters.Add(new SqlParameter("@direccion", model.direccion));
                    cmdCentro.Parameters.Add(new SqlParameter("@horario_apertura", model.horario_apertura));
                    cmdCentro.Parameters.Add(new SqlParameter("@horario_cierre", model.horario_cierre));
                    cmdCentro.ExecuteNonQuery();
                    if (model.dias != null && model.dias.Count > 0)
                    {
                        SqlCommand cmdBorrarDias = new SqlCommand("DELETE FROM Centro_dias WHERE id_centro = @idCentro", connection, transaction);
                        cmdBorrarDias.Parameters.Add(new SqlParameter("@idCentro", model.id_centro));
                        cmdBorrarDias.ExecuteNonQuery();

                        SqlCommand cmdInsertarDia = new SqlCommand("sp_InsertarCentroDias", connection, transaction);
                        cmdInsertarDia.CommandType = CommandType.StoredProcedure;
                        cmdInsertarDia.CommandTimeout = 0;
                        cmdInsertarDia.Parameters.Add(new SqlParameter("@idCentro", model.id_centro));

                        foreach (var dia in model.dias)
                        {
                            cmdInsertarDia.Parameters.Clear();
                            cmdInsertarDia.Parameters.Add(new SqlParameter("@idCentro", model.id_centro));
                            cmdInsertarDia.Parameters.Add(new SqlParameter("@idDia", dia));
                            cmdInsertarDia.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback the transaction if any error occurs
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public EstadisticasAdminViewModel getEstadisticasAdmin(int idCentro, string? fechaInicio, string? fechaFin)
        {
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("spObtenerEstadisticasAdmin", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fecha_fin", fechaFin);

                EstadisticasAdminViewModel estadisticas = new EstadisticasAdminViewModel();
                estadisticas.EstadisticasLocalidades = new List<EstadisticasLocalidadAdmin>();
                estadisticas.EstadisticasGlobales = new EstadisticasGlobablesAdmin();
                estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios = new Centro();
                estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados = new UsuariosViewModel();

                try
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // obtener las estadisticas por localidad para agregar al listado
                        while (reader.Read())
                        {
                            estadisticas.EstadisticasLocalidades.Add(new EstadisticasLocalidadAdmin
                            {
                                Localidad = reader["Localidad"].ToString(),
                                IntercambiosTotalesLocalidad = (int)reader["IntercambiosTotalesLocalidad"],
                                IntercambiosConfirmadosSinDonacionLocalidadCount = (int)reader["IntercambiosConfirmadosSinDonacionLocalidadCount"],
                                IntercambiosConfirmadosConDonacionLocalidadCount = (int)reader["IntercambiosConfirmadosConDonacionLocalidadCount"],
                                IntercambiosCanceladosLocalidadCount = (int)reader["IntercambiosCanceladosLocalidadCount"],
                                MotivoDeCancelacionMasFrecuenteLocalidad = reader["MotivoDeCancelacionMasFrecuenteLocalidad"].ToString(),
                                IntercambiosRechazadosLocalidadCount = (int)reader["IntercambiosRechazadosLocalidadCount"],
                                MotivoDeRechazoMasFrecuenteLocalidad = reader["MotivoDeRechazoMasFrecuenteLocalidad"].ToString()
                            });
                        }

                        // obtener estadisticas globales
                        // avanzar a la siguiente tabla
                        reader.NextResult();

                        if (reader.Read())
                        {
                            //estadisticas.EstadisticasGlobales.CategoriaMasSolicitada = reader["CategoriaMasSolicitada"] != DBNull.Value ? (int)reader["CategoriaMasSolicitada"] : 0;
                            // datos del centro con mas cantidad de intercambios
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.id_centro = reader["CentroConMasCantidadDeIntercambiosID"] != DBNull.Value ? (int)reader["CentroConMasCantidadDeIntercambiosID"] : 0;
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.nombre_centro = reader["CentroConMasCantidadDeIntercambiosNombre"].ToString();
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.ubicacion = reader["CentroConMasCantidadDeIntercambiosUbicacion"].ToString();
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.direccion = reader["CentroConMasCantidadDeIntercambiosDireccion"].ToString();
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.horario_apertura = reader["CentroConMasCantidadDeIntercambiosHorarioApertura"].ToString();
                            estadisticas.EstadisticasGlobales.CentroConMasCantidadDeIntercambios.horario_cierre = reader["CentroConMasCantidadDeIntercambiosHorarioCierre"].ToString();
                            estadisticas.EstadisticasGlobales.CantidadProductosDonadosCentro = (int)reader["CantidadProductosDonadosCentro"];
                            // datos del usuario voluntario con mas intercambios confirmados
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.ID = reader["VoluntarioConMasIntercambiosConfirmadosID"] != DBNull.Value ? (int)reader["VoluntarioConMasIntercambiosConfirmadosID"] : 0;
                            estadisticas.EstadisticasGlobales.CantidadIntercambiosConfirmadosVoluntario = (int)reader["CantidadIntercambiosConfirmadosVoluntario"];
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.nombre = reader["VoluntarioConMasIntercambiosConfirmadosNombre"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.apellido = reader["VoluntarioConMasIntercambiosConfirmadosApellido"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.DNI = reader["VoluntarioConMasIntercambiosConfirmadosDNI"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.foto = reader["VoluntarioConMasIntercambiosConfirmadosImagen"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.mail = reader["VoluntarioConMasIntercambiosConfirmadosMail"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.fecha_nacimiento = reader["VoluntarioConMasIntercambiosConfirmadosFechaNacimiento"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.fecha_registro = reader["VoluntarioConMasIntercambiosConfirmadosFechaRegistro"].ToString();
                            estadisticas.EstadisticasGlobales.VoluntarioConMasIntercambiosConfirmados.centro = reader["VoluntarioConMasIntercambiosConfirmadosCentro"] != DBNull.Value ? (int)reader["VoluntarioConMasIntercambiosConfirmadosCentro"] : 0;
                            // cantidad de intercambios totales 
                            estadisticas.EstadisticasGlobales.IntercambiosTotalesCount = (int)reader["IntercambiosTotalesCount"];
                            estadisticas.EstadisticasGlobales.IntercambiosConfirmadosSinDonacionCount = (int)reader["IntercambiosConfirmadosSinDonacionCount"];
                            estadisticas.EstadisticasGlobales.IntercambiosConfirmadosConDonacionCount = (int)reader["IntercambiosConfirmadosConDonacionCount"];
                            estadisticas.EstadisticasGlobales.IntercambiosCanceladosCount = (int)reader["IntercambiosCanceladosCount"];
                            estadisticas.EstadisticasGlobales.MotivoDeCancelacionMasFrecuente = reader["MotivoDeCancelacionMasFrecuente"].ToString();
                            estadisticas.EstadisticasGlobales.IntercambiosRechazadosCount = (int)reader["IntercambiosRechazadosCount"];
                            estadisticas.EstadisticasGlobales.MotivoDeRechazoMasFrecuente = reader["MotivoDeRechazoMasFrecuente"].ToString();
                        }

                        reader.NextResult();

                        // obtener las donaciones por categoria
                        estadisticas.EstadisticasGlobales.DonacionesPorCategoria = new List<CategoriaCantidad>();
                        while (reader.Read())
                        {
                            estadisticas.EstadisticasGlobales.DonacionesPorCategoria.Add(new CategoriaCantidad
                            {
                                Categoria = reader["nombre"].ToString(),
                                Cantidad = (int)reader["CantidadProductosDonados"]
                            });
                        }

                        reader.NextResult();

                        //obtener los intercambios realizados por categoria
                        estadisticas.EstadisticasGlobales.IntercambiosPorCategoria = new List<CategoriaCantidad>();
                        while (reader.Read())
                        {
                            estadisticas.EstadisticasGlobales.IntercambiosPorCategoria.Add(new CategoriaCantidad
                            {
                                Categoria = reader["Categoria"].ToString(),
                                Cantidad = (int)reader["CantidadIntercambios"]
                            });
                        }

                        reader.NextResult();

                        // obtener todos los intercambios por fecha
                        estadisticas.EstadisticasGlobales.IntercambiosTotalesPorFecha = new List<IntercambioGrafico>();
                        while (reader.Read())
                        {
                            estadisticas.EstadisticasGlobales.IntercambiosTotalesPorFecha.Add(new IntercambioGrafico
                            {
                                FechaIntercambio = reader["fecha_intercambio"].ToString(),
                                Confirmados = (int)reader["Confirmados"],
                                Rechazados = (int)reader["Rechazados"],
                                Cancelados = (int)reader["Cancelados"]
                            });
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                connection.Close();

                // obtener si se pasa el idCentro el campo EstadisticasVoluntarioViewModel 
                if (idCentro > 0)
                {
                    estadisticas.EstadisticasVoluntario = getEstadisticasVoluntario(idCentro, fechaInicio, fechaFin);
                }

                return estadisticas;
            }
        }

        public EstadisticasVoluntarioViewModel getEstadisticasVoluntario(int idCentro, string? fechaInicio, string? fechaFin)
        {
            EstadisticasVoluntarioViewModel estadisticas = new EstadisticasVoluntarioViewModel();
            List<categoriaEstadistica> listaCategoriaEstadisticas = new List<categoriaEstadistica>();
            using (var connection = new SqlConnection(builder.obtenerConnectionString("CaritasConnectionTesting")))
            {
                connection.Open();
                // Crear el Command Object con el stored procedure
                SqlCommand cmd = new SqlCommand("spEstadisticasVoluntario", connection);
                SqlParameter centro = new SqlParameter("@idCentro", idCentro);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                SqlParameter idCentroParam = new SqlParameter("@id_centro", idCentro);
                cmd.Parameters.Add(idCentroParam);
                SqlParameter fechaInicioParam = new SqlParameter("@fecha_inicio", fechaInicio == null ? null : fechaInicio);
                cmd.Parameters.Add(fechaInicioParam);
                SqlParameter fechaFinParam = new SqlParameter("@fecha_fin", fechaFin == null ? null : fechaFin);
                cmd.Parameters.Add(fechaFinParam);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read(); // Leer la primera fila
                    estadisticas.IdCentro = reader.GetInt32(reader.GetOrdinal("id_centro"));
                    estadisticas.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                    estadisticas.Ubicacion = reader.GetString(reader.GetOrdinal("ubicacion"));
                    estadisticas.Direccion = reader.GetString(reader.GetOrdinal("direccion"));
                    estadisticas.HorarioApertura = reader.GetString(reader.GetOrdinal("horario_apertura"));
                    estadisticas.HorarioCierre = reader.GetString(reader.GetOrdinal("horario_cierre"));
                    estadisticas.Borrado = reader.GetBoolean(reader.GetOrdinal("borrado"));
                    estadisticas.CantidadIntercambiosConfirmados = reader.GetInt32(reader.GetOrdinal("CantidadIntercambiosConfirmados"));
                    estadisticas.CantidadIntercambiosCancelados = reader.GetInt32(reader.GetOrdinal("CantidadIntercambiosCancelados"));
                    estadisticas.MotivoMasComunCancelacion = reader.IsDBNull(reader.GetOrdinal("MotivoMasComunCancelacion")) ? "Ningún motivo encontrado" : reader.GetString(reader.GetOrdinal("MotivoMasComunCancelacion"));
                    estadisticas.CantidadIntercambiosRechazados = reader.GetInt32(reader.GetOrdinal("CantidadIntercambiosRechazados"));
                    estadisticas.MotivoMasComunRechazo = reader.IsDBNull(reader.GetOrdinal("MotivoMasComunRechazo")) ? "Ningún motivo encontrado" : reader.GetString(reader.GetOrdinal("MotivoMasComunRechazo"));
                    estadisticas.CantidadProductosDonados = reader.GetInt32(reader.GetOrdinal("CantidadProductosDonados"));
                }

                // Avanzar al siguiente conjunto de resultados (estadísticas por categoría)
                if (reader.NextResult() && reader.HasRows)
                {
                    // Ejemplo para leer resultados de estadísticas por categoría
                    while (reader.Read())
                    {
                        categoriaEstadistica est = new categoriaEstadistica();
                        est.categoriaProducto = reader.GetInt32(reader.GetOrdinal("categoria_producto"));
                        est.cantidadIntercambios = reader.GetInt32(reader.GetOrdinal("CantidadIntercambiosRealizados"));
                        listaCategoriaEstadisticas.Add(est);
                    }
                    estadisticas.cantidadIntercambiosPorCategoria = listaCategoriaEstadisticas;
                }
                // Leer auditorías
                if (reader.NextResult() && reader.HasRows)
                {
                    reader.Read(); // Leer la fila del auditor
                    estadisticas.nombreVoluntarioMasAuditador = reader.GetString(reader.GetOrdinal("nombre"));
                    estadisticas.apellidoVoluntarioMasAuditador = reader.GetString(reader.GetOrdinal("apellido"));
                    estadisticas.fotoVoluntarioMasAuditador = reader.GetString(reader.GetOrdinal("base64_imagen"));
                    estadisticas.cantidadVoluntarioMasAuditador = reader.GetInt32(reader.GetOrdinal("cantidadVoluntarioMasAuditador"));
                }
                // Leer intercambios
                List<IntercambioGrafico> listadoInter = new List<IntercambioGrafico>();
                if (reader.NextResult() && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IntercambioGrafico intercambio = new IntercambioGrafico();
                        intercambio.FechaIntercambio = reader.GetString(reader.GetOrdinal("fecha_intercambio"));
                        intercambio.Confirmados = reader.GetInt32(reader.GetOrdinal("Confirmados"));
                        intercambio.Rechazados = reader.GetInt32(reader.GetOrdinal("Rechazados"));
                        intercambio.Cancelados = reader.GetInt32(reader.GetOrdinal("Cancelados"));
                        listadoInter.Add(intercambio);
                    }
                    estadisticas.intercambios = listadoInter.OrderBy(i => DateTime.ParseExact(i.FechaIntercambio, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
                }
                reader.Close();
                SqlCommand cmdDias = new SqlCommand("spObtenerDiasParaCadaCentro", connection);
                cmdDias.CommandType = CommandType.StoredProcedure;
                cmdDias.CommandTimeout = 0;
                cmdDias.Parameters.Add(new SqlParameter("@idCentro", idCentro));
                using (SqlDataReader readerDias = cmdDias.ExecuteReader())
                {
                    List<Dias> listaDias = new List<Dias>();
                    while (readerDias.Read())
                    {
                        Dias dias = new Dias();
                        dias.idDia = readerDias.GetInt32(readerDias.GetOrdinal("idDia"));
                        dias.descripcion = readerDias.IsDBNull(readerDias.GetOrdinal("descripcion")) ? "" : readerDias.GetString(readerDias.GetOrdinal("descripcion"));
                        listaDias.Add(dias);
                    }
                    estadisticas.dias = listaDias;
                }
                connection.Close();
            }
            return estadisticas;
        }
    }
}
