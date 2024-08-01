using OneSearch.Plugin.OneNote.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSearch.Plugin.OneNote
{
    public class OneNotePlugin
    {
        public void Execute()
        {

            var app = new OneNoteApplication();
            var books = app.GetNotebooks();


            foreach(var noteBook in books.Notebook)
            {
                Console.WriteLine(noteBook.name);
            }

        }
    }
}
