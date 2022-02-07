using JiebaNet.Analyser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfIdfAnalyser.Model
{
    public class Analyser
    {
        private static Analyser _instance;
        public static Analyser Instance
        {
            get
            {
                return _instance ?? (_instance = new Analyser());
            }
        }
        public enum LanguageType 
        { 
            EN = 0,
            CH = 1,
        }

        public enum DocType
        {
            Artichle = 0,
            Segment = 1,
        }

        public static readonly string DirPath_Articles = Directory.GetCurrentDirectory() + @"\Doc\Articles";
        public static readonly string FielPath_EN = DirPath_Articles + @"\english.docx";
        public static readonly string FielPath_CH = DirPath_Articles + @"\chinese.docx";
        public static readonly string DirPath_Segments = Directory.GetCurrentDirectory() + @"\Doc\Segments\";
        public static readonly string DirPath_Segments_EN = DirPath_Segments + @"english";
        public static readonly string DirPath_Segments_CH = DirPath_Segments + @"chinese";

        public async Task<StringBuilder> TfidfExtractTagsWithWeight(FileInfo file)
        {
            //var str = await Fetcher.Instance.FetchWord(filePath);
            if (file == null)
            {
                return null;
            }
            return await Task.Run(()=> {
                var filePath = file.FullName;
                var str = ReadFile(filePath);
                var extractor = new TfidfExtractor();
                var s = extractor.ExtractTags(str, 20, null);
                var wordWeight = extractor.ExtractTagsWithWeight(str, 20, null);
                StringBuilder sbr = new StringBuilder();
                sbr.Append(file.Name);
                sbr.AppendLine(",");
                sbr.Append("主题词");
                sbr.Append(",");
                sbr.Append("权重");
                sbr.AppendLine(",");
                foreach (var item in wordWeight)
                {
                    sbr.Append(item.Word);
                    sbr.Append(",");
                    sbr.Append(item.Weight);
                    sbr.AppendLine(",");
                }
                return sbr;
            });
           
            //string filename = $"关键词权重统计.csv";
            //File.WriteAllText(filename, sbr.ToString(), Encoding.UTF8);
            //Console.WriteLine("关键词提取完成：" + filename);
        }

        public async void GetArticleReport()
        {

        }

        public async void GetSegmentReport(LanguageType lan = LanguageType.CH)
        {
            var dPath = lan == LanguageType.CH ? DirPath_Segments_CH : DirPath_Segments_EN;
            var files = await Fetcher.Instance.FetchAllFiles(dPath);
            var docType = lan == LanguageType.CH ? DocType.Artichle.ToString() + "_" + LanguageType.CH.ToString() : DocType.Artichle.ToString() + "_" + LanguageType.EN.ToString();
            StringBuilder sb = new StringBuilder();
            var count = 1;
            foreach (var file in files)
            {
                var sbr = await TfidfExtractTagsWithWeight(file);
                sb.Append(sbr);
                Console.WriteLine($"关键词提取完成_{file.Name} {count++}/{files.Length}");
            }
            string filename = $"关键词权重统计_{docType}.csv";
            File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
            Console.WriteLine("关键词提取完成：" + filename);
        }

        public string ReadFile(string path)
        {
            var gb18030 = Encoding.GetEncoding("GB18030");
            StreamReader sr;
            string str = "";
            using (sr = new StreamReader(path, gb18030, true))
            {
                str = sr.ReadToEnd();
                return str;
            }

        }
    }
}
