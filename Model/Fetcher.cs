using JiebaNet.Analyser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfIdfAnalyser.Model
{
    public class Fetcher
    {
        private static Fetcher _instance;
        public static Fetcher Instance
        {
            get
            {
                return _instance ?? (_instance = new Fetcher());
            }
        }

        public static readonly string filePath = Directory.GetCurrentDirectory() + @"\Doc\zh-hans.docx";

        public async Task<string> FetchWord(string filePath)
        {
            return await Task.Run(() => {
                Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = null;
                object unknow = Type.Missing;
                //object nullobj = System.Reflection.Missing.Value;
                app.Visible = true;
                object file = filePath;
                doc = app.Documents.Open(ref file,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow);

                var str = doc.Content.Text;
                doc.Close(ref unknow, ref unknow, ref unknow);
                app.Quit(ref unknow, ref unknow, ref unknow);
                return str;
            });

        }

        public async Task FetchText(string filePath)
        {
            await Task.Run(() => {
                using (StreamReader file = File.OpenText(filePath))
                {
                    var str = file.ReadToEnd();
                }
            });

        }

        private string[] docType = {".doc", ".docx", ".txt"};
        public async Task<FileInfo[]> FetchAllFiles(string dPath)
        {
            try
            {
                if (!Directory.Exists(dPath))
                {
                    Console.WriteLine("GetAllFiles dPath not exist");
                    return null;
                }
                return await Task.Run(()=>{
                    DirectoryInfo dir = new DirectoryInfo(dPath);
                    FileInfo[] files = dir.GetFiles();
                    var fileList = files.Where(x => docType.Contains(x.Extension)).ToArray();
                    return fileList;
                });
               
            }
            catch (Exception e)
            {
                Console.WriteLine($"FetchAllFiles error {e.Message}");
                return null;
            }

        }
      
    }
}
