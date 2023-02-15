using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrilhaNetMVCDesafio.Context;
using TrilhaNetMVCDesafio.Models;

namespace TrilhaNetMVCDesafio.Controllers
{
    public class OrganizadorController : Controller
    {
        private readonly OrganizadorContext _context;

        public OrganizadorController(OrganizadorContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Tarefas.ToList());
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Tarefa novaTarefa)
        {
            if (!ModelState.IsValid)
            {
                return View(novaTarefa);
            }

            _context.Tarefas.Add(novaTarefa);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalhar(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        public IActionResult Editar(int id)
        {
            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        [HttpPost]
        public IActionResult Editar(Tarefa tarefaAlterada)
        {
            if (!ModelState.IsValid)
            {
                return View(tarefaAlterada);
            }

            var tarefaBanco = _context.Tarefas.Find(tarefaAlterada.Id);

            if (tarefaBanco == null)
            {
                return NotFound();
            }

            tarefaBanco.Titulo = tarefaAlterada.Titulo;
            tarefaBanco.Descricao = tarefaAlterada.Descricao;
            tarefaBanco.Data = tarefaAlterada.Data;
            tarefaBanco.Status = tarefaAlterada.Status;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Excluir(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            return tarefa == null ? NotFound() : View(tarefa);
        }

        [HttpPost]
        public IActionResult ExcluirDefinitivamente(int id)
        {
            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            _context.Tarefas.Remove(tarefa);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PesquisaAvancada(string sortOrder, string searchString, CurrentFilterType currentFilterType)
        {
            ViewData["TituloSortParm"] = String.IsNullOrEmpty(sortOrder) ? "titulo_desc" : string.Empty;
            ViewData["DataSortParm"] = sortOrder == "Data" ? "data_desc" : "Data";
            ViewData["CurrentFilter"] = searchString;

            var tarefas = _context.Tarefas.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (currentFilterType)
                {
                    case CurrentFilterType.Status:
                        tarefas = FiltrarPorStatus(searchString, tarefas);
                        break;
                    case CurrentFilterType.Data:
                        tarefas = FiltrarPorData(searchString, tarefas);
                        break;                        
                    default:
                        tarefas = tarefas.Where(t => t.Titulo.Contains(searchString)); //FiltrarPorTitulo
                        break;
                }
            }

            tarefas = OrdernarTarefas(sortOrder, tarefas);

            return View(tarefas.ToList());
        }

        private static IQueryable<Tarefa> FiltrarPorData(string searchString, IQueryable<Tarefa> tarefas)
        {
            if (DateTime.TryParse(searchString, out DateTime dateTimeParam))
            {
                tarefas = tarefas.Where(x => x.Data == dateTimeParam);
            }

            return tarefas;
        }

        private static IQueryable<Tarefa> FiltrarPorStatus(string searchString, IQueryable<Tarefa> tarefas)
        {
            if (searchString.ToLowerInvariant().Equals(EnumStatusTarefa.Finalizado.ToString().ToLower()))
            {
                tarefas = tarefas.Where(t => t.Status == EnumStatusTarefa.Finalizado);
            }
            else if (searchString.ToLowerInvariant().Equals(EnumStatusTarefa.Pendente.ToString().ToLower()))
            {
                tarefas = tarefas.Where(t => t.Status == EnumStatusTarefa.Pendente);
            }

            return tarefas;
        }

        private static IQueryable<Tarefa> OrdernarTarefas(string sortOrder, IQueryable<Tarefa> tarefas)
        {
            switch (sortOrder)
            {
                case "titulo_desc":
                    tarefas = tarefas.OrderByDescending(t => t.Titulo);
                    break;
                case "Data":
                    tarefas = tarefas.OrderBy(t => t.Data);
                    break;
                case "data_desc":
                    tarefas = tarefas.OrderByDescending(t => t.Data);
                    break;
                default:
                    tarefas = tarefas.OrderBy(s => s.Titulo);
                    break;
            }

            return tarefas;
        }
    }
}