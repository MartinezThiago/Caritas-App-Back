using Microsoft.AspNetCore.Mvc;
using CaritasBack.Services;
using Microsoft.AspNetCore.Authorization;
using CaritasBack.Models;
using Newtonsoft.Json;
using CaritasBack.Utils;
using System.Reflection.Metadata.Ecma335;

namespace CaritasBack.Controllers
{
    [ApiController]
    public class CaritasBackController : ControllerBase
    {
        private IValidarToken _validarToken;
        public CaritasBackController(IValidarToken validarToken)
        {
            this._validarToken = validarToken;
        }
        [HttpGet("[controller]/getPublicaciones")]
        public IActionResult getPublicaciones(int id_categorie)
        {
            try
            {
                return Ok(ServicioCaritasConsultas.Instance.getPublicaciones(id_categorie));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("[controller]/registrarUsuario")]
        public IActionResult registrarUsuario([FromBody] UsuarioRegistro model)
        {
            if (ModelState.IsValid)
            {
                if (this.esMayor(model.fecha_nacimiento) && (!this.fechaMayorALaActual(model.fecha_nacimiento)))
                {
                    if (model.centros_elegidos.Count >= 1 && model.centros_elegidos.Count <= 3)
                    {
                        if (!ServicioCaritasConsultas.Instance.listadDeUsuarios().Any(x => x.mail.ToLower() == model.email.ToLower()))
                        {
                            try
                            {
                                return ServicioCaritasConsultas.Instance.registrarUsuario(model) ? Ok() : BadRequest("El usuario ya esta registrado");
                            }
                            catch (Exception e)
                            {
                                return BadRequest(e.Message);
                            }
                        }
                        else
                        {
                            return BadRequest("El correo electrónico ingresado ya existe en el sistema");
                        }
                    }
                    else
                    {
                        return BadRequest("No puede elegir mas de tres centros");
                    }
                }
                else
                {
                    return BadRequest("Debe ser mayor de edad para poder registrarse/ La fecha ingresada es mayor a la actual");
                }
            }
            else
            {
                return BadRequest((model.password.Length < 6) ? "La contraseña debe contener al menos 6 caracteres" : "Datos incorrectos");
            }
        }

        [HttpPost("[controller]/registrarUsuarioInterno")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult registrarUsuarioInterno([FromBody] CambiarModificarDatosVoluntario model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                if (ModelState.IsValid)
                {
                    if (this.esMayor(model.nacimiento))
                    {
                        if (!ServicioCaritasConsultas.Instance.listadDeUsuarios().Any(x => x.mail.ToLower() == model.email.ToLower()))
                        {

                            try
                            {
                                return ServicioCaritasConsultas.Instance.registrarUsuarioInterno(model) ? Ok() : BadRequest("El usuario ya esta registrado");
                            }
                            catch (Exception e)
                            {
                                return BadRequest(e.Message);
                            }

                        }
                        else
                        {
                            return BadRequest("El correo electrónico ingresado ya existe en el sistema");
                        }
                    }
                    else
                    {
                        return BadRequest("Debe ser mayor de edad para poder registrarse");
                    }
                }
                else
                {
                    return BadRequest("Datos del modelo recibido inválidos");
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpPost("[controller]/subirPregunta")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult subirPregunta([FromBody] PreguntaEnvio model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    ServicioCaritasConsultas.Instance.subirPregunta(model);
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }
        [HttpPost("[controller]/subirRespuesta")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult subirRespuesta([FromBody] RespuestaEnvio model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    ServicioCaritasConsultas.Instance.subirRespuesta(model);
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpGet("[controller]/getPublicacion")]
        public IActionResult getPublicacion(int idPublicacion)
        {
            try
            {
                Publicacion_Usuario publicacion_user = ServicioCaritasConsultas.Instance.obtenerPublicacionEspecifica(idPublicacion);
                if (publicacion_user != null)
                {
                    return Ok(publicacion_user);
                }
                else
                {
                    return BadRequest("No se encontró la publicación solicitada.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[controller]/getComentarios")]
        public IActionResult getComentarios(int idPublicacion)
        {
            try
            {
                ComentariosViewModel coment = new ComentariosViewModel();
                coment.comentarios = ServicioCaritasConsultas.Instance.obtenerComentariosPublicacionEspecifica(idPublicacion);
                if (coment.comentarios.Count > 0)
                {
                    return Ok(coment);
                }
                else
                {
                    return BadRequest("No se encontraron comentarios para la publicación solicitada");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[controller]/getPublicacionesUser")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult getPublicacionesUser(int id)
        {
            try
            {
                List<Publicacion_Usuario> publicacion_usuario = ServicioCaritasConsultas.Instance.obtenerPublicacionesPorIdUsuario(id);
                if (publicacion_usuario.Count > 0)
                {
                    return Ok(publicacion_usuario);
                }
                else
                {
                    return BadRequest("Este usuario no tiene publicaciones.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("[controller]/eliminarPregunta")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult eliminarPregunta(EliminarPregunta model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "Rol" && c.Value == "usuario_basico");
                try
                {
                    if (roleClaim != null)
                    {
                        ServicioCaritasConsultas.Instance.eliminarPregunta(model, true);
                    }
                    else
                    {
                        ServicioCaritasConsultas.Instance.eliminarPregunta(model, false);
                    }
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpGet("[controller]/borrarCentro")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult borrarCentro(int idCentro)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    ServicioCaritasConsultas.Instance.borrarCentro(idCentro);
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpPost("[controller]/eliminarRespuesta")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult eliminarRespuesta(EliminarRespuesta model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "Rol" && c.Value == "usuario_basico");
                try
                {
                    if (roleClaim != null)
                    {
                        ServicioCaritasConsultas.Instance.eliminarRespuesta(model, true);
                    }
                    else
                    {
                        ServicioCaritasConsultas.Instance.eliminarRespuesta(model, false);
                    }
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpPost("[controller]/crearPublicacion")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult CrearPublicacion(PublicacionViewModel model)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                if (model.imagenesEnBase64.Count >= 1 && model.imagenesEnBase64.Count <= 4)
                {
                    try
                    {
                        ServicioCaritasConsultas.Instance.crearPublicacion(model);
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                else
                {
                    return BadRequest("Se espera una cantidad de 1 a 4 fotos");
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        private bool EsTamañoImagenValido(List<string> imagenesEnBase64)
        {
            const int maxSizeBytes = 5 * 1024 * 1024; // 5 MB en bytes
            long totalSizeBytes = 0;

            foreach (var imagenBase64 in imagenesEnBase64)
            {
                byte[] bytesImagen = Convert.FromBase64String(imagenBase64);
                totalSizeBytes += bytesImagen.Length;
            }
            return totalSizeBytes <= maxSizeBytes;
        }

        private bool esMayor(DateTime fechaNacimiento)
        {
            int edad = ((DateTime.Now - fechaNacimiento).Days / 365);
            return edad >= 18;
        }

        private bool fechaMayorALaActual(DateTime fechaNacimiento)
        {
            return fechaNacimiento >= DateTime.Now;
        }

        [HttpGet("[controller]/getFotoUsuario")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult getFotoUsuario()
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                int idUser = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.obtenerFotoUsuario(idUser));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpGet("[controller]/getCentros")]
        public IActionResult getCentros()
        {
            try
            {
                return Ok(ServicioCaritasConsultas.Instance.obtenerCentros());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[controller]/getCentrosPublicacionEspecifica")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult getCentrosPublicacionEspecifica(int idPublicacion)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.obtenerCentrosPublicacionEspecifica(idPublicacion));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpGet("[controller]/getCentrosUserEspecifico")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult getCentrosUserEspecifico(int idUser)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    var rolActual = User.Claims.FirstOrDefault(c => c.Type == "Rol").Value;
                    if (rolActual.ToString() == "voluntario")
                    {
                        return Ok(ServicioCaritasConsultas.Instance.obtenerCentroVoluntario(idUser));
                    }
                    else
                    {
                        return Ok(ServicioCaritasConsultas.Instance.obtenerCentrosUserEspecifico(idUser));
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpPost]
        [Route("[controller]/recuperarClave")]
        public IActionResult recuperarClave(string correo)
        {
            UsuarioRestablecer usuarioRestablecer = ServicioCaritasConsultas.Instance.obtenerUsuarioPorEmail(correo);

            if (usuarioRestablecer.Id > 0)
            {
                bool respuesta = ServicioCaritasConsultas.
                    Instance.restablecerActualizarClave(1, usuarioRestablecer.Clave, usuarioRestablecer.Token);

                if (respuesta)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Plantillas", "Restablecer.html");
                    string content = System.IO.File.ReadAllText(path);
                    //string url = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, "/CaritasBack/actualizarClave?token=" + usuarioRestablecer.Token);
                    string url = string.Format("{0}://{1}{2}", Request.Scheme, "localhost:3000", "/password-recovery/" + usuarioRestablecer.Token);
                    string htmlBody = string.Format(content, usuarioRestablecer.Nombre, url);

                    CorreoModel correoModel = new CorreoModel()
                    {
                        Para = correo,
                        Asunto = "Restablecer contraseña",
                        Contenido = htmlBody
                    };

                    bool enviado = EmailUtils.EnviarCorreo(correoModel);
                    return Ok();
                }
                else
                {
                    return BadRequest("No se pudo reestablecer la contraseña.");
                }
            }
            else
            {
                return BadRequest("No se encontraron coincidencias con el correo.");
            }
        }

        // este accion se ejecuta cuando el usuario ingresa la nueva contraseña y envia los datos
        [HttpPost]
        [Route("[controller]/actualizarClave")]
        public IActionResult actualizarClave(string token, string clave, string confirmarClave)
        {
            if (clave != confirmarClave)
            {
                return BadRequest("Las contraseñas no coinciden.");
            }

            // se actualzia la contraseña y por lo tanto el bit de restablecer clave se pone 0
            // indicando que ya no se pide restablecer la clave
            bool respuesta = ServicioCaritasConsultas.Instance.restablecerActualizarClave(0, clave, token);

            if (respuesta)
            {
                return Ok();
            }
            else
            {
                return BadRequest("No se pudo actualizar la contraseña.");
            }
        }

        [HttpPost("[controller]/cambiarDatosPersonales")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult cambiarDatosPersonales(CambiarDatosViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        if (model.password == "" || (model.password != "" && model.password.Length >= 6))
                        {
                            try
                            {
                                ServicioCaritasConsultas.Instance.cambiarDatosPersonales(model);
                                return Ok();
                            }
                            catch (Exception e)
                            {
                                return BadRequest(e.Message);
                            }
                        }
                        else
                        {
                            return BadRequest("Las contraseña no tiene 6 caracteres");
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/cambiarDatosPersonalesVoluntario")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult cambiarDatosPersonalesVoluntario([FromBody] CambiarModificarDatosVoluntario model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {

                            try
                            {
                                ServicioCaritasConsultas.Instance.cambiarDatosPersonalesVoluntario(model);
                                return Ok();
                            }
                            catch (Exception e)
                            {
                                return BadRequest(e.Message);
                            }                       
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
            
        }

        [HttpPost("[controller]/modificarFavorito")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult modificarFavorito(FavoritosModel model)
        {
            if (ModelState.IsValid)
            {
                if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                {
                    int idUser = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                    try
                    {
                        return Ok(ServicioCaritasConsultas.Instance.modificarFavorito(idUser, model.id_publicacion, model.action));
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                else
                {
                    return BadRequest("Las credenciales no son válidas");
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpGet("[controller]/getFavsIdsUser")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult getFavsIdsUser()
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                int idUser = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.getFavsIdsUser(idUser));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpGet("[controller]/getPostFavs")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult getPostFavs()
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                int idUser = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.obtenerPublicacionesFavoritas(idUser));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        // esta accion devuelve las ofertas que un usuario realizó y las que recibió
        // las ofertas devueltas estan en estado pendiente o rechazado
        [HttpGet("[controller]/getOfertasUser")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult getOfertasUser()
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                int idUser = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.obtenerOfertasUsuario(idUser));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }

        [HttpPost("[controller]/borrarPublicacion")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult borrarPublicacion(BorrarPublicacion model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "Rol" && c.Value == "usuario_basico");
                        try
                        {
                            if (roleClaim != null)//eliminada
                            {
                                ServicioCaritasConsultas.Instance.borrarPublicacion(model, true, Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value));
                            }
                            else
                            {
                                ServicioCaritasConsultas.Instance.borrarPublicacion(model, false, Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value));
                            }
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }


        [HttpPost("[controller]/rechazarCancelarOferta")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult rechazarOferta(Oferta model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {

                        try
                        {
                            ServicioCaritasConsultas.Instance.rechazarCancelarOferta(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/aceptarOferta")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult aceptarOferta(AceptarOferta model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        try
                        {
                            ServicioCaritasConsultas.Instance.aceptarOferta(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/cancelarIntercambioPendiente")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult cancelarIntercambioPendiente(CancelarIntercambio model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {

                        try
                        {
                            ServicioCaritasConsultas.Instance.cancelarIntercambioPendiente(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpGet("[controller]/obtenerIntercambiosEnEstadoPendiente")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult obtenerIntercambiosEnEstadoPendiente()
        {
            try
            {
                return Ok(ServicioCaritasConsultas.Instance.obtenerIntercambiosEnEstadoPendiente());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("[controller]/obtenerTodoLosUsuarios")]
        [Authorize(Policy = "TodosLosRolesCombinados")]
        public IActionResult obtenerTodoLosUsuarios()
        {
            try
            {
                return Ok(ServicioCaritasConsultas.Instance.listaDeUsuariosViewModel());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("[controller]/borrarVoluntario")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult ActualizarUsuarioBorrado(int idUser)
        {
            try
            {
                ServicioCaritasConsultas.Instance.ActualizarUsuarioBorrado(idUser);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[controller]/obtenerIntercambiosEnEstadoPendienteUserEspecifico")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult obtenerIntercambiosEnEstadoPendienteUserEspecifico()
        {
            try
            {
                if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                {
                    try
                    {
                        return Ok(ServicioCaritasConsultas.Instance.obtenerIntercambiosEnEstadoPendienteUser(Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value)));
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                else
                {
                    return BadRequest("Las credenciales no son válidas");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[controller]/obtenerIntercambiosEnCentroEspecifico")]
        [Authorize(Policy = "voluntario")]
        public IActionResult obtenerIntercambiosEnCentroEspecifico(int id_centro)
        {
            try
            {
                if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                {
                    try
                    {
                        return Ok(ServicioCaritasConsultas.Instance.obtenerIntercambiosEnEstadoPendienteCentro(id_centro));
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                else
                {
                    return BadRequest("Las credenciales no son válidas");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("[controller]/rechazarIntercambio")]
        [Authorize(Policy = "voluntario")]
        public IActionResult rechazarIntercambio(IntercambioAceptarRechazar model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {

                        try
                        {
                            ServicioCaritasConsultas.Instance.rechazarIntercambio(model, Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value));
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/aceptarIntercambio")]
        [Authorize(Policy = "voluntario")]
        public IActionResult aceptarIntercambio(IntercambioAceptarRechazar model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {

                        try
                        {
                            ServicioCaritasConsultas.Instance.aceptarIntercambio(model, Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value));
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/ofertarPublicacion")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult ofertarPublicacion(SolicitudOferta model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        try
                        {
                            if (ServicioCaritasConsultas.Instance.ofertarPublicacion(model))
                            {
                                return Ok("Se ha agregado la oferta exitosamente");
                            }
                            else
                            {
                                return BadRequest("No se pudo ofertar la publicación");
                            }

                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }
        [HttpPost("[controller]/insertarCentro")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult insertarCentro(CentroParaInsertar model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        try
                        {
                            ServicioCaritasConsultas.Instance.insertarCentro(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }
        [HttpPost("[controller]/cambiarCentro")]
        [Authorize(Policy = "usuarioBasico")]
        public IActionResult cambiarCentro(CentroPublicacionModificar model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        try
                        {
                            ServicioCaritasConsultas.Instance.cambiarCentro(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpPost("[controller]/actualizarCentro")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult actualizarCentro(CentroModificar model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
                    {
                        try
                        {
                            ServicioCaritasConsultas.Instance.actualizarCentro(model);
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Las credenciales no son válidas");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Datos Incorrectos");
            }
        }

        [HttpGet("[controller]/getEstadisticasVoluntario")]
        [Authorize(Policy = "voluntario")]
        public IActionResult getEstadisticasVoluntario(string? fechaInicio, string? fechaFin)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.getEstadisticasVoluntario(Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "Centro").Value), fechaInicio, fechaFin));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }
        [HttpGet("[controller]/getEstadisticasAdmin")]
        [Authorize(Policy = "UsuarioAdmin")]
        public IActionResult getEstadisticasAdmin(int idCentro, string? fechaInicio, string? fechaFin)
        {
            if (_validarToken.validarToken(_validarToken.getToken(HttpContext)))
            {
                try
                {
                    return Ok(ServicioCaritasConsultas.Instance.getEstadisticasAdmin(idCentro, fechaInicio, fechaFin));
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Las credenciales no son válidas");
            }
        }
    }
}