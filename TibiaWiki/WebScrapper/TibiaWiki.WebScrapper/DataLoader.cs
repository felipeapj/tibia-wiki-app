using HtmlAgilityPack;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace TibiaWiki.WebScrapper
{
    public class DataLoader
    {
        private readonly HttpClient _client;

        public DataLoader(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://www.tibiawiki.com.br/wiki/");
        }

        public async Task<string> GetData(string path, string xPath)
        {
            xPath = "//*[@id=\"tabelaDPL\"]";
            var response = await _client.GetAsync(path);
            var htmlContent = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode(xPath);
            HtmlNodeCollection childNodes = htmlBody.ChildNodes;

            foreach (var node in childNodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    var lines = node.OuterHtml.Replace("</tr>", "").Split("<tr>").ToList();
                    lines.RemoveAt(0);
                    var headers = lines[0];
                    lines.RemoveAt(0);
                    foreach (var line in lines)
                    {
                        var columns = line.Split("</td>").Select(x => x.Replace("\n", "") + "</td>").ToList();
                        columns.RemoveAt(columns.Count - 1);
                        var values = new List<string>();
                        foreach (var col in columns)
                        {
                            values.Add(GetColumnText(col));
                        }
                        var a = values;
                    }
                }
            }
            return htmlBody.OuterHtml;
        }

        public string GetColumnText(string content)
        {
            try
            {
                var result = GetTitleText(content);
                if(!string.IsNullOrEmpty(result)) return result;

                result = GetTdText(content);
                if (!string.IsNullOrEmpty(result)) return result;

                return GetImgSrc(content);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public string GetTdText(string content)
        {
            var refString = "</td>";
            return GetStringBetween(refString, ">", content, false);
        }

        public string GetTitleText(string content)
        {
            var refString = "title=\"";
            if (!content.Contains(refString)) return string.Empty;

            return GetStringBetween(refString, "\"", content);
        }

        public string GetImgSrc(string content)
        {
            var refTagString = "<img ";
            if (!content.Contains(refTagString)) return string.Empty;

            content = content.Substring(content.IndexOf(refTagString));
            var refSrcStr = "src=\"";

            return _client.BaseAddress!.ToString().Replace("/wiki/", "") + GetStringBetween(refSrcStr, "\"", content);
        }

        public string GetStringBetween(string startDelimiter, string endDelimiter, string content, bool forward = true)
        {
            var idx = content.IndexOf(startDelimiter) + (forward ? startDelimiter.Length : -1);
            var sb = new StringBuilder();

            while (content[idx].ToString() != endDelimiter)
            {
                sb.Append(content[idx]);
                if (forward)
                {
                    idx++;
                }
                else
                {
                    idx--;
                }
            }

            if (forward)
            {
                return sb.ToString();
            }

            var arr = sb.ToString().ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            try
            {
                return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
            }
            catch { return null; }
        }
    }
}