using ScanerApp.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ScanerApp.Controllers
{
    public class ServiceController : Controller
    {
        private GeneralDbContext con = new GeneralDbContext();

        public ActionResult Demonstrate()
        {
            return View();
        }

        public RedirectToRouteResult Analyse()
        {
            DirectoryInfo dir_info = new DirectoryInfo(Server.MapPath("~/Uploads/"));
            foreach (var file_info in dir_info.GetFiles())
            {
                int numline = 1;
                using (StreamReader sr = new StreamReader(file_info.FullName))
                {
                    while (sr.Peek() > 0)
                    {
                        string input = sr.ReadLine();

                        foreach (string some_word in Regex.Matches(input, @"[a-z]\w+", RegexOptions.IgnoreCase).Cast<Match>().Select(x => x.Value).Distinct())
                        {
                            Doc d = con.Docs.SingleOrDefault(x => x.Name == file_info.Name) == null
                                ? new Doc() { Name = file_info.Name, FullPath = file_info.DirectoryName }
                                : con.Docs.SingleOrDefault(x => x.Name == file_info.Name);

                            Word w = con.Words.SingleOrDefault(x => x.Title == some_word) == null
                                ? new Word() { Title = some_word }
                                : con.Words.SingleOrDefault(x => x.Title == some_word);

                            WordByDoc wordbd = con.WordByDocs.SingleOrDefault(x => x.Doc.DocId == d.DocId && x.Word.WordId == w.WordId);
                            if (wordbd == null)
                            {
                                wordbd = new WordByDoc() { Doc = d, Word = w };
                                con.WordByDocs.Add(wordbd);
                            }
                            else
                                con.WordByDocs.Attach(wordbd);

                            foreach (int index in Regex.Matches(input, some_word).Cast<Match>().Select(x => x.Index))
                            {
                                Position p = con.Positions.SingleOrDefault(
                                    x => x.Line == numline && x.Step == (index + 1) && x.WordByDoc.WordByDocId == wordbd.WordByDocId) == null
                                    ? new Position() { Line = numline, Step = index + 1 }
                                    : con.Positions.SingleOrDefault(
                                        x => x.Line == numline && x.Step == (index + 1) && x.WordByDoc.WordByDocId == wordbd.WordByDocId);

                                wordbd.Positions.Add(p);
                            }

                            con.SaveChanges();
                        }
                        numline++;
                    }
                }
            }

             return RedirectToAction("GetPage");
        }

        public RedirectToRouteResult ClearData()
        {
            con.Database.ExecuteSqlCommand("DELETE FROM [Positions]");
            con.Database.ExecuteSqlCommand("DELETE FROM [WordByDocs]");
            con.Database.ExecuteSqlCommand("DELETE FROM [Words]");
            con.Database.ExecuteSqlCommand("DELETE FROM [Docs]");

            return RedirectToAction("GetPage");

        }

        public PartialViewResult GetPage(int skip = 0)
        {
            int pageSize = 10;

            IEnumerable<Word> wordsPage = con.Words.OrderBy(x => x.Title).Skip(skip).Take(pageSize).ToList();

            ViewBag.skip = skip;
            ViewBag.pageSize = pageSize;
            ViewBag.pageQnt = con.Words.Count()/pageSize + ((con.Words.Count() % pageSize) > 0 ? 1 : 0);

            return PartialView(wordsPage);
        }

   }
}