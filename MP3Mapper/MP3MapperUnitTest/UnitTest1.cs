using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MP3MapperUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string url = "https://en.wikipedia.org/wiki/List_of_Tamil_films_of_the_1930s";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(url);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='wikitable']");

            var table = new DataTable("MyTable");

            var headers = nodes[0]
                .Elements("th")
                .Select(th => th.InnerHtml.Trim());
            foreach (var header in headers)
            {
                table.Columns.Add(header);
            }

            var rows = nodes.Skip(1).Select(tr => tr
                .Elements("td")
                .Select(td => td.InnerText.Trim())
                .ToArray());
            foreach (var row in rows)
            {
                table.Rows.Add(row);
            }
        }

        [TestMethod]
        public void TestMethod()
        {
            string url = "https://en.wikipedia.org/wiki/List_of_Tamil_films_of_the_1930s";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(url);
            HtmlNodeCollection hnc = htmlDoc.DocumentNode.SelectNodes("//table[@class='wikitable']");
            foreach (HtmlNode hn in hnc) {
                DataTable dt = ParseTable(hn);
            }
        }

        private static DataTable[] ParseAllTables(HtmlDocument doc)
        {
            var result = new List<DataTable>();
            foreach (var table in doc.DocumentNode.Descendants("//table[@class='wikitable']"))
            {
                result.Add(ParseTable(table));
            }
            return result.ToArray();
        }

        private static DataTable ParseTable(HtmlNode table)
        {
            var result = new DataTable();

            var rows = table.Descendants("tr");

            //var header = rows.Take(1).First();
            //foreach (var column in header.Descendants("th"))
            //{
            //    result.Columns.Add(new DataColumn(column.InnerText, typeof(string)));
            //}

            //foreach (var column in header.Descendants("td"))
            //{
            //    result.Columns.Add(new DataColumn(column.InnerText, typeof(string)));
            //}

            foreach (var row in rows.Skip(1))
            {
                var data = new List<string>();
                foreach (var column in row.Descendants("td"))
                {
                    data.Add(column.InnerText);
                }
                result.Rows.Add(data.ToArray());
            }
            return result;
        }

        [TestMethod]
        public void TestMethod2()
        {
            string url = "https://en.wikipedia.org/wiki/List_of_Tamil_films_of_the_1930s";
            var tblList = ScrapHtmlTable("wikitable", "https://en.wikipedia.org/wiki/List_of_Tamil_films_of_2010");

            foreach (List<string> s in tblList)
            {
                foreach (string svalues in s)
                {
                    var sv = svalues.ToString();
                }
            }
        }

        public List<List<string>> ScrapHtmlTable(string className, string pageURL)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(pageURL);
            List<List<string>> parsedTbl = null;
            try { 
             parsedTbl = 
              document.DocumentNode.SelectNodes("//table[@class='" + className + "']")
              .Descendants("tr")
              .Skip(1) //To Skip Table Header Row
              .Where(tr => tr.Elements("td").Count() > 1)              
              .Select(tr => tr.Elements("td")
              .Select(td => td.InnerText.Trim()).ToList())
              .ToList();
            }
            catch(Exception ex)
            { throw ex;
            }
            return parsedTbl;
        }

    }
}

