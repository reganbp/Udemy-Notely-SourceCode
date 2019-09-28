﻿using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NotelyApp.Models;
using NotelyApp.Repositories;

namespace NotelyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly INoteRepository _noteRepository;
        public HomeController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public IActionResult Index()
        {
            var notes = _noteRepository.GetAllNotes().Where(n => n.IsDeleted == false);

            return View(notes);
        }

        public IActionResult NoteDetail(Guid id)
        {
            var note = _noteRepository.FindNoteById(id);

            return View(note);
        }

        [HttpGet]
        public IActionResult NoteEditor(Guid id = default)
        {
            if (id != Guid.Empty)
            {
                var note = _noteRepository.FindNoteById(id);

                return View(note);
            }

            return View();

        }

        [HttpPost]
        public IActionResult NoteEditor(NoteModel noteModel)
        {
            if (ModelState.IsValid)
            {
                var date = DateTime.Now;

                if (noteModel != null && noteModel.Id == Guid.Empty)
                {
                    noteModel.Id = Guid.NewGuid();
                    noteModel.CreatedDate = date;
                    noteModel.LastModified = date;

                    _noteRepository.SaveNote(noteModel);
                }
                else
                {
                    var note = _noteRepository.FindNoteById(noteModel.Id);
                    note.LastModified = date;
                    note.Subject = noteModel.Subject;
                    note.Detail = noteModel.Detail;
                }

                return RedirectToAction("Index"); 
            }
            else
            {
                return View();
            }
        }

        public IActionResult DeleteNote(Guid id)
        {
            var note = _noteRepository.FindNoteById(id);

            note.IsDeleted = true;

            return RedirectToAction("Index");
        }









        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
