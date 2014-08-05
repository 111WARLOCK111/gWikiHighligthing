using System;

using System.IO;

using System.Xml;

using System.Net;

using System.Text;
using System.Text.RegularExpressions;

using System.Collections.Generic;

using Microsoft.CSharp;

using System.CodeDom;
using System.CodeDom.Compiler;

//using ProjectBase.Tools.Wiki;

public enum WebPages : int
{
    Librares = 0,
    Classes = 1
}

namespace gWikiGrabber
{
    class Program
    {
      /*const string DefaultString     = "String"        ;
        const bool DefaultBool         = true            ;
        const int DefaultNumber        = 1               ;*/

        const string DirName           = "generates"     ;

        const string DefaultSpace      = "			"    ;
        const string KeySpace          = "				";

        static WebPages Choice;

        static void Main(string[] args)
        {
            if (!Directory.Exists(DirName)) Directory.CreateDirectory(DirName);

            Console.WriteLine("Enter The number of page you're looking to grab:");
            Console.WriteLine();

            GetPage();
            //GetData();
        }

        static void GetPage()
        {
            foreach (var db in Enum.GetValues(typeof(WebPages)))
                Console.WriteLine("[" + (int)db + "] " + Enum.GetName(typeof(WebPages), db));

            Console.WriteLine();

            WebPages Web;

            if (!Enum.TryParse(Console.ReadLine(), out Web))
            {
                Console.WriteLine("Invalid webpage, Please neter then number of page.");

                Console.WriteLine();
                Console.WriteLine();

                GetPage();
                return;
            }

            Choice = Web;
            GetData();
        }

        static void GetData()
        {
            var web = (Choice == WebPages.Librares ? "http://wiki.garrysmod.com/page/Category:Library_Functions" : "http://wiki.garrysmod.com/page/Category:Class_Functions");

            Console.WriteLine(web);

            byte[] row = DLPage(web);

            var ut = new UTF8Encoding();

            var page = ut.GetString(row);

            if (page == null)
            {
                GetData();
                return;
            }

            Console.WriteLine("Data successfully received from the server.");

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Please enter a valid regex string in order to get the data:");

            Console.WriteLine();

            var rgx = @"(?<=\btitle="")[^""]*";

            Console.WriteLine(rgx);

            var regex = new Regex(rgx);

            var match = regex.Matches(page);

            //var print = false;

            var file = "default.xml";

            if (match.Count > 0)
            {
                Console.WriteLine("There are " + match.Count + " matches that were found, Would you like to see them in console?");

                Console.WriteLine();

                //var read = Console.ReadKey(true);

                /*if (read.Key == ConsoleKey.Enter || read.Key == ConsoleKey.Y)
                    print = true;
                else
                    print = false;
                */
                Console.WriteLine();

                Console.WriteLine("Alright, Enter a name for file you're looking to write them:");

                Console.WriteLine("NOTE: This removes any existing file, so watch out!!");

                Console.WriteLine();

                file = "default.xml";

                Console.WriteLine(file);
            }

            List<string> Categories = new List<string>();

            var _last = "";

            using (var sw = new StreamWriter(DirName + "/" + file))
            {
                // Lib names into one seperated Keywords
                if (Choice == WebPages.Librares)
                {
                    sw.WriteLine(DefaultSpace + "<KeyWords name=\"Libraries\" bold=\"false\" italic=\"false\" color=\"#8000FF\">");

                    foreach (var ms in match)
                    {
                        if (!ms.ToString().Contains("/")) continue;

                        var getraw = ms.ToString().Split('/');

                        if (getraw[0] == "Global") continue;

                        if (!Categories.Contains(getraw[0]))
                        {
                            Categories.Add(getraw[0]);
                            sw.WriteLine(KeySpace + "<Key word=\"" + getraw[0] + "\" />");
                        }

                    }

                    sw.WriteLine(DefaultSpace + "</KeyWords>");

                    sw.WriteLine();

                    Categories.Clear();
                }


                if (Choice == WebPages.Classes) sw.WriteLine(DefaultSpace + "<KeyWords name=\"Classes\" bold=\"true\" italic=\"false\" color=\"#804040\">");


                foreach (var m in match)
                {
                    if (!m.ToString().Contains("/")) continue;

                    var getraw = m.ToString().Split('/');

                    if (Choice == WebPages.Librares)
                        if (!Categories.Contains(getraw[0]))
                        {
                            if (_last != "")
                            {
                                sw.WriteLine(DefaultSpace + "</KeyWords>");
                                sw.WriteLine();
                            }
                            Categories.Add(getraw[0]);

                            var key = getraw[0].Replace(" ", "").Replace("_", "");

                            key = Char.ToUpper(getraw[0][0]) + key.Substring(1);

                            sw.WriteLine(DefaultSpace + "<KeyWords name=\"" + (getraw[0] == "Global" ? "" : "Lib") + key + "\" bold=\"false\" italic=\"false\" color=\"#" + (getraw[0] == "Global" ? "0080C0" : "8000FF") + "\">");
                            _last = getraw[0];
                        }

                    if (Choice == WebPages.Classes && Categories.Contains(getraw[1])) continue;

                    sw.WriteLine(KeySpace + "<Key word=\"" + (Choice == WebPages.Librares ? (getraw[0] != "Global" ? getraw[0].Replace(" ", "").Replace("_", "") + "." : "") : "") + getraw[1] + "\" />");

                    if (Choice == WebPages.Classes) Categories.Add(getraw[1]);
                }
                sw.WriteLine(DefaultSpace + "</KeyWords>");
            }

            Console.WriteLine("Done!");

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");

            Console.ReadKey(true);
        }

        static byte[] DLPage(string page)
        {
            byte[] row;

            try
            {
                using (var db = new WebClient())
                {
                    row = db.DownloadData(page);
                }
            }
            catch
            {
                Console.WriteLine("There was an error, Retrying to connect...");
                return DLPage(page);
                //return null;
            }

            return row;
        }
    }
}
