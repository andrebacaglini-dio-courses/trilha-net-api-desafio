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

    }
}