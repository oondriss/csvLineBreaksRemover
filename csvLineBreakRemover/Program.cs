using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csvLineBreakRemover.Managers;

namespace csvLineBreakRemover
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var manager = new CsvManager();

            var csvFileName = manager.GetCsvFileName();
            var newFileName = manager.GetNewCsvFileName();

            manager.SetPath(csvFileName);
            manager.GetColumnCount();
            manager.RemoveLineBreaks2();
            manager.SaveNewFileAsync(newFileName);
        }
    }
}
