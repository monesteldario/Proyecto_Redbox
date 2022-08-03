﻿using Microsoft.AspNetCore.Mvc;
using BackEnd.DAL.Implementations;
using BackEnd.DAL.Interfaces;
using BackEnd.Entities;
using FrontEnd.Models;
using FrontEnd.Help;
using FrontEnd.Permisos;

namespace FrontEnd.Controllers
{
    public class UsuarioController : Controller
    {
        IUsuarioDAL usuarioDAL;
        IRolDAL rolDAL;
        UsuarioModel model = new UsuarioModel();


        private UsuarioViewModel Convertir(Usuario usuario)
        {

            return new UsuarioViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Telefono = usuario.Telefono,
                Cedula = usuario.Cedula,
                Direccion = usuario.Direccion,
                NombreUsuario = usuario.NombreUsuario,
                ContraseniaUsuario = usuario.ContraseniaUsuario,
                IDRol = usuario.IdRol,




            };
        }


        #region Lista
        public IActionResult Index()
        {
            List<Usuario> usuarios;
            usuarioDAL = new UsuarioDALImpl();
            usuarios = usuarioDAL.GetAll().ToList();
            List<UsuarioViewModel> lista = new List<UsuarioViewModel>();

            foreach (Usuario item in usuarios)
            {
                lista.Add(Convertir(item));
            }

            return View(lista);
        }

        #endregion

        #region Agregar
        public IActionResult Create(UsuarioViewModel usuario)
        {
            try
            {
                UsuarioViewModel usuarios = new UsuarioViewModel();
                rolDAL = new RolDALImpl();


                usuarios.Cedula = usuario.Cedula.ToString();
                usuarios.Nombre = usuario.Nombre.ToString();
                usuarios.Roles = rolDAL.GetAll();

                return View(usuarios);
            }
            catch (Exception ex)
            {
                return View("Error");
            }



        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            usuarioDAL = new UsuarioDALImpl();
            usuarioDAL.Add(usuario);

            return RedirectToAction("Details", new { id = usuario.IdUsuario });
        }

        #endregion

        #region Detalles
        public IActionResult Details(int id)
        {

            usuarioDAL = new UsuarioDALImpl();
            UsuarioViewModel usuarios = Convertir(usuarioDAL.Get(id));

            rolDAL = new RolDALImpl();


            usuarios.Rol = rolDAL.Get(usuarios.IDRol);


            return View(usuarios);
        }
        #endregion
        public IActionResult BusquedaHacienda()
        {

            return View();

        }
        #region Hacienda
        public IActionResult Encontrado(UsuarioViewModel usuario)
        {
            var resultado = model.BuscarClientePorCedula(usuario.Cedula);

            if (resultado == null)
            {
                return View("Error");

            }
            else
            {
                return RedirectToAction("Create", resultado);
            }




        }
        #endregion

        #region Editar

        public IActionResult Edit(int id)
        {

            usuarioDAL = new UsuarioDALImpl();
            UsuarioViewModel usuarios = Convertir(usuarioDAL.Get(id));

            rolDAL = new RolDALImpl();

            usuarios.Roles = rolDAL.GetAll();

            return View(usuarios);

        }

        [HttpPost]
        public IActionResult Edit(Usuario usuario)
        {

            usuarioDAL = new UsuarioDALImpl();
            usuarioDAL.Update(usuario);
            return RedirectToAction("Details", new { id = usuario.IdUsuario });
        }


        #endregion

        #region Eliminar 
        public IActionResult Delete(int id)
        {
            usuarioDAL = new UsuarioDALImpl();
            UsuarioViewModel usuario = Convertir(usuarioDAL.Get(id));


            return View(usuario);
        }

        [HttpPost]
        public IActionResult Delete(Usuario usuario)
        {
            usuarioDAL = new UsuarioDALImpl();
            usuarioDAL.Remove(usuario);

            return RedirectToAction("Index");
        }
        #endregion

        #region Inicio Sesion
        [validacionSesionError]
        public ActionResult InicioSesion()
        {
            return View();
        }

        
        [HttpPost]
        
        public ActionResult Acceder(string NombreUsuario, string ContraseniaUsuario)
        {

            UsuarioViewModel objvacio = new UsuarioViewModel();

            try
            {
                var resultado = model.BuscarLogin(NombreUsuario, ContraseniaUsuario);
                if (resultado != null)
                {
                    string nombre = resultado.Nombre;
                    string usuario = resultado.NombreUsuario;
                    string rol = resultado.IDRol.ToString();

                    HttpContext.Session.SetString("nombre",nombre);
                    HttpContext.Session.SetString("usuario", usuario);
                    HttpContext.Session.SetString("rol", rol);

                    //return Json(resultado, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion
    }
}
