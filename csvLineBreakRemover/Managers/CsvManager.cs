using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csvLineBreakRemover.Utils;

namespace csvLineBreakRemover.Managers
{
    public static class Utils
    {
        public static int IndexOfNth(this string input,
                             string value, int startIndex, int nth)
        {
            if (nth < 1)
                throw new NotSupportedException("Param 'nth' must be greater than 0!");
            if (nth == 1)
                return input.IndexOf(value, startIndex, StringComparison.Ordinal);
            var idx = input.IndexOf(value, startIndex, StringComparison.Ordinal);
            if (idx == -1)
                return -1;
            return input.IndexOfNth(value, idx + 1, --nth);
        }
    }
    public class CsvManager
    {
        private string _path;
        private int _columnCount;
        private string _allLines;
        private List<string> _newLines = new List<string>();
        private List<List<string>> _newLinesData = new List<List<string>>();

        public string GetCsvFileName()
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            throw new InvalidOperationException("Je nutne vybrat subor");
        }

        public string GetNewCsvFileName()
        {
            var sfd = new SaveFileDialog
            {
                CheckPathExists = true
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            throw new InvalidOperationException("Je nutne vybrat subor");
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public void GetColumnCount()
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                throw new Exception("Najprv je treba nastavit cestu!!");
            }

            //zistit pocet stlpcov
            using (var reader = new StreamReader(File.Open(_path, FileMode.Open)))
            {
                var firstLine = reader.ReadLine() ?? "";
                var colCnt = firstLine.Count(t => t == ';') + 1;
                if (colCnt == 1)
                {
                    throw new Exception("Malo stlpcov");
                }
                _columnCount = colCnt;
            }
        }
        //5. bodkociarka od konca
        //pred nu dopisat "0;"
        private string GetRepairedLine(string line)
        {
            return line.Insert(line.IndexOfNth(";", 0, 49), ";0");
        }

        private bool IsFullRow(string line, out string newLine)
        {
            newLine = "";
            var editedLine = line.Replace("; ", " ");
            var columns = editedLine.Count(i => i == ';') + 1;
            if (columns == 54)
            {
                newLine = editedLine;
                return true;
            }
            else if (columns == 53)
            {
                newLine = GetRepairedLine(editedLine);
                return true;
            }
            else if (columns > 54)
            {
                throw new Exception();
            }
            else
            {
                return false;
            }
        }

        private string FixString(string line)
        {
            return line.Replace("\r", "").Replace("\n", "") + ";";
        }

        public void RemoveLineBreaks2()
        {
            var currLine = "";
            bool firstLine = true;

            foreach (var line in File.ReadLines(_path))
            {
                //v line mam riadok
                //zoberiem ho, zratam pocet stlpcov
                //ak neni ok, pridam dalsi riadok a robim znova

                //if newline.count(';') == columnCount
                //then newlines.Add(newline)
                //else newline+=newline
                string newLine;

                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }
                try
                {
                    if (IsFullRow(currLine, out newLine))
                    {
                        _newLines.Add(FixString(newLine));
                        currLine = line;
                    }
                    else
                    {
                        currLine += line;
                    }
                }
                catch (Exception e)
                {
                    currLine = "";
                    continue;
                }
                
            }
        }

        public void RemoveLineBreaks()
        {
            //loopovat stlpce na danom riadku
            //ak ich este neni dost, tak zmazat newline
            //inac nechat
            //pridat bodkociarku na koniec riadka

            using (var reader = new StreamReader(File.OpenRead(_path)))
            {
                _allLines = reader.ReadToEnd();
            }



            var charCnt = _allLines.Length;
            var currLine = "";
            var currCollCnt = 1;

            for (var c = 0; c < charCnt; c++)
            {
                Console.WriteLine($"{c}/{charCnt}");
                switch (_allLines[c])
                {
                    case ';':
                        currCollCnt++;
                        currLine += _allLines[c];
                        break;
                    case '\r':
                    case '\n':
                        if (currCollCnt == _columnCount)
                        {
                            _newLines.Add($"{currLine};{Environment.NewLine}");
                            currLine = "";
                            currCollCnt = 1;
                        }
                        else
                        {
                            var a = new List<string>();
                        }
                        break;
                    default:
                        currLine += _allLines[c];
                        break;
                }
            }
        }

        public async void SaveNewFileAsync(string fileName)
        {
            await FileAsync.WriteAllLinesAsync(fileName, _newLines);
        }
    }
}
