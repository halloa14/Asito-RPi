using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace AsitoRPi
{
    class XmlHandler
    {
        public static async Task MainAsync()
        {
            var wfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Responses.xml");
            Stream wstream = await wfile.OpenStreamForWriteAsync();

            XDocument doc = new XDocument(new XElement("Objects"));

            doc.Root.Add(new XElement("Object",
                new XAttribute("ID", "00001"),
                new XElement("Question", "Do you like your job?"),
                new XElement("Responses", new XElement("Response", "Yes"), new XElement("Response", "No"), new XElement("Response", "Yes"))));
                doc.Root.Add(new XElement("Object",
                new XAttribute("ID", "00002"),
                new XElement("Question", "Question 2"),
                new XElement("Responses", new XElement("Response", "Yes"), new XElement("Response", "No"))));












            doc.Save(wstream);
            wstream.Close();



        }

        public async Task<String> RunQuery(String e)
        {
            var q = e.Split('/');

            if (q.Count() == 1)
            {
              return await GetQuestionAsync(e);
            }


            if (q.Count() == 2)
            {
                String target = q[0].Remove(0, 1);
                String ans = q[1];
                WriteResponse(ans, target);
                String res = "Done";
                return res;
            }

            else
            {
                String res = "Error";
                return res;
            }
            
        }

        public async Task<string> GetQuestionAsync(String e)
        {

            // var wfile = await ApplicationData.Current.LocalFolder.GetFileAsync("Responses.xml");
            // Stream wstream = await wfile.OpenStreamForWriteAsync();
            // XDocument wxmlFile = XDocument.Load(rstream);

            e = e.Remove(0, 1);
            Debug.WriteLine(e);

            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("Responses.xml");
                Stream rstream = await file.OpenStreamForReadAsync();
                var rxmlFile = XDocument.Load(rstream);
                rstream.Close();

                String xmlString = rxmlFile.Root.Elements("Object").Where(sel => sel.Attribute("ID").Value == e).FirstOrDefault().Element("Question").Value.ToString();
                return xmlString;
            }
            
                
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                String xmlString = "Not Found.";
                return xmlString;
            }
                

        }

        public async void WriteResponse(String res, String targ)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("Responses.xml");
            Stream rstream = await file.OpenStreamForReadAsync();
            var rxmlFile = XDocument.Load(rstream);
            rstream.Close();
            Debug.WriteLine("Response: " + res);
            Debug.WriteLine("Target: " + targ);

            try
            {
                var add = new XElement("Response", res);
                rxmlFile.Root.Elements("Object").Where(sel => sel.Attribute("ID").Value == targ).FirstOrDefault().Element("Responses").Add(add);
                Stream wstream = await file.OpenStreamForWriteAsync();
                rxmlFile.Save(wstream);
                wstream.Close();

            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
