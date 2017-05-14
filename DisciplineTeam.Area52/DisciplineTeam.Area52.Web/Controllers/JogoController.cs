﻿using DisciplineTeam.Area52.Web.Filtro;
using DisciplineTeam.Area52.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DisciplineTeam.Area52.Web.Controllers
{
    public class JogoController : Controller
    {
        //public static List<Jogo> lista = new List<Jogo>();
        [UsuarioFiltro]
        // GET: Jogos
        public ActionResult Index()
        {
            using (JogoModel model = new JogoModel())
            {
                Admin user = (Admin)Session["usuario"];
                ViewBag.StatusAdmin = user.Status;
                return View(model.ReadJogos());

            } // model.Dispose();
        }
        [UsuarioFiltro]
        public ActionResult Create()
        {
            Admin user = (Admin)Session["usuario"];
            ViewBag.StatusAdmin = user.Status;
            return View();
        }
        [UsuarioFiltro]
        [HttpPost]
        public ActionResult Create(Jogo e)
        {
            Admin user = (Admin)Session["usuario"];
            ViewBag.StatusAdmin = user.Status;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase arquivo = Request.Files[0];

                using (System.Drawing.Image pic = System.Drawing.Image.FromStream(arquivo.InputStream)) //Converte a arquivo para imagem para poder comparar a as dimensões
                {
                    if (pic.Height != 256 && pic.Width != 256)
                    {
                        TempData["ErroDimensao"] = "Please use a picture with 256x256 pixels.";         //Semelhante a viewbag porem ela "vive" fora da pagina que foi criada
                        return RedirectToAction("Create");
                    }
                    else if (arquivo.ContentType != "image/png" && arquivo.ContentType != "image/jpeg" && arquivo.ContentType != "image/jpg")           //Verifica o formato do arquivo
                    {
                        TempData["ErroFormato"] = "Application only supports PNG or JPG image types.";
                        return RedirectToAction("Create");
                    }
                    else if (arquivo.ContentLength > 2097152)                                              //Verifica se o arquivo não é > que 2 MiB
                    {
                        TempData["ErroTamanho"] = "Please upload picture with less than 2MiB.";
                        return RedirectToAction("Create");
                    }
                }
                DateTime today = DateTime.Now;                                                          //cria uma variavel da hora atual

                string nome = today.ToString("yyyyMMddhhmmss");                                         //converte a hora e data atual para ser usado como nome da imagem do perfil
                if (Request.Files.Count > 0)                                                            // Verifica se recebe algum arquivo
                {
                    if (arquivo.ContentLength > 0)                                                      //verifica se ele possui algo
                    {
                        //arquivo.FileName pegar nome arquivo
                        //string caminho = "C:/Users/Felipe/Pictures/testebd/" + arquivo.FileName;    //Uso apenas de protótipo
                        //String que vai para o banco, se tiver o caminho todo da imagem o site não mostra
                        string img = "/img/imgjogo/" + nome + System.IO.Path.GetExtension(arquivo.FileName);
                        string path = HostingEnvironment.ApplicationPhysicalPath;                        //Pega o diretório em que o projeto está
                        //Onde vai ser armazenado
                        string caminho = path + "\\img\\imgjogo\\" + nome + System.IO.Path.GetExtension(arquivo.FileName);
                        arquivo.SaveAs(caminho);
                        e.Imagem = img;
                        TempData["SucessoImg"] = "Game created succesfully.";
                    }
                }

                using (JogoModel model = new JogoModel())
                {
                    int id = ((Admin)Session["usuario"]).IdPessoa;
                    model.Create(e, id);
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Mensagem = "Falha no cadastro!";
                return View(e);
            }
        }
    }
}