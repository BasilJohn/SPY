using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace SPY
{

    public class MainController : ApiController
    {
        // GET api/<controller>

        public string Get()
        {
            return new WebClient().DownloadString("https://www.pvrcinemas.com/nowshowing/Chennai");
        }

        // GET api/<controller>/5
        public string Get(string movieText)
        {
            return "value";
        }

        [HttpGet]
        [Route("api/{main}/{name}/{value}")]
        [ActionName("GetCinemaData")]
        public string Get(string name, string value)
        {

           return new WebClient().DownloadString("https://www.pvrcinemas.com/cinemasessions/Chennai/" + name.Replace(" ", "-") + "/" + value);
           
        }
       
        [HttpPost]
        [Route("api/{main}/{mobileNumber}/{selectedMovie}/{selectedDate}")]
        [ActionName("SaveUserDetails")]
        public bool Post(string mobileNumber, string selectedMovie,string selectedDate, [FromBody]string href)
        {

            string[] token = href.Split('*');
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"UserDetails.xml");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlElement root = xmlDoc.DocumentElement;

            XmlNode userNode = xmlDoc.CreateElement("User");
            
            XmlAttribute attributeHref = xmlDoc.CreateAttribute("Href");
            attributeHref.Value = token[0];
            userNode.Attributes.Append(attributeHref);

            XmlAttribute attributeTime = xmlDoc.CreateAttribute("Time");
            attributeTime.Value = token[1];
            userNode.Attributes.Append(attributeTime);

            XmlAttribute attributeselectedMovie = xmlDoc.CreateAttribute("Movie");
            attributeselectedMovie.Value = selectedMovie;
            userNode.Attributes.Append(attributeselectedMovie);

            XmlAttribute attributeselectedDate = xmlDoc.CreateAttribute("Date");
            attributeselectedDate.Value = selectedDate;
            userNode.Attributes.Append(attributeselectedDate);

            userNode.InnerText = mobileNumber;
            root.AppendChild(userNode);
            xmlDoc.Save(path);

            return true;

        }

        [HttpGet]
        [Route("api/{main}/{service}")]
        [ActionName("StartSPYService")]
        public bool StartSPYService(string service)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"UserDetails.xml");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList xmlnode =xmlDoc.GetElementsByTagName("User");

            List<string> urlList = new List<string>();
            for (int i = 0; i <= xmlnode.Count - 1; i++)
            {
                urlList.Add(xmlnode[i].Attributes["Href"].Value);
            }
            bool isticketsAvailable = false;
            var result =Parallel.ForEach(urlList, item => Process(item ,out isticketsAvailable));
            
            return isticketsAvailable;

        }

        public bool Process(string url,out bool isticketsAvailable)
        {
            url=url.Replace("localhost:53803", "pvrcinemas.com").Replace("amp;","");

            var html = new HtmlDocument();
            html.LoadHtml(new WebClient().DownloadString(url));
            var root = html.DocumentNode;

            var isServerDowntag = root.Descendants()
                .Where(r => r.Id == "errordiv").Any();
            var isServerDown = false;
            if (isServerDowntag)
            {
                isServerDown = root.Descendants()
                .Where(r => r.Id == "errordiv").SingleOrDefault().InnerHtml != string.Empty;
            }
            else
            {
                isServerDown = true;
            }
               

            var ticketsAvailable = false;
            if (!isServerDown)
            {
                ticketsAvailable = root.Descendants()
                    .Where(r => r.Id == "select-seats")
                    .SingleOrDefault().Descendants()
                    .Where(r => r.GetAttributeValue("class", "")
                    .Equals("color-3")).FirstOrDefault()
                    .Descendants().Where(r => r.GetAttributeValue("type", "").Equals("checkbox"))
                    .Where(r => r.GetAttributeValue("onchange", "").Contains("setSeat")).Any();
            }
            isticketsAvailable = ticketsAvailable;
            return ticketsAvailable;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}