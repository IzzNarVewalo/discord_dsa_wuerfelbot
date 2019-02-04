using _04_dsa.Modules;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _04_dsa.Services {
    public class Helden_software_API {
        private readonly string url = "https://online.helden-software.de/returnheld/?format=datenxml&token=";

        // Get all heros that are right now online
        public async Task<List<string>> GetHeroList() {
            string action = "&action=listhelden";
            List<string> ret = new List<string>();

            var request = (HttpWebRequest)WebRequest.Create(url + Helper.HeldenToken + action);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            try {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var stream = response.GetResponseStream()) {
                    var xml = XDocument.Load(stream);
                    var query = from c in xml.Root.Descendants("held")
                                select c.Element("name");

                    foreach (string name in query) {
                        ret.Add(name);
                    }
                }
                return ret;
            } catch {
                return ret;
            }
        }

        // search for a full hero name and ID 
        public async Task<Dictionary<int, string>> GetHeroBasics(string heldenname) {
            string action = "&action=listhelden";
            heldenname = heldenname.ToLower();
            var retval = new Dictionary<int, string>();

            var request = (HttpWebRequest)WebRequest.Create(url + Helper.HeldenToken + action);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var stream = response.GetResponseStream()) {
                    var xml = XDocument.Load(stream);
                    var query = from c in xml.Root.Descendants("held")
                                select c;
                    foreach (var element in query) {
                        var x = element.Element("name").Value.ToString().ToLower();
                        if (x.StartsWith(heldenname))
                            retval.Add(Convert.ToInt32(element.Element("heldenid").Value.ToString()), element.Element("name").Value.ToString());
                    }
                }
                return retval;
            } catch {
                return retval;
            }
        }

        // get the basic values for a specific roll
        public async Task<_3_W_20_Throw> GetHeroValues(int heldenID, string talent, DBService service, List<Eigenschaften> special = null) {
            string ID = "&heldenid=" + heldenID;
            var request = (HttpWebRequest)WebRequest.Create(url + Helper.HeldenToken + ID);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            // try to connect to online.helden-software.de
            try {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                    string xml = await reader.ReadToEndAsync();
                    //remove all the newlines because that saves a huge amount of data
                    xml = Regex.Replace(xml, @"\r\n?|\n", "");
                    // cache the latest xml if connection was possible
                    await service.UpdateHeroXml(heldenID, xml);
                    if (special == null)
                        return ParseProbe(XDocument.Parse(xml), talent);
                    else
                        return ParseSpecialProbe(XDocument.Parse(xml), talent, special);
                }
                // use the cached backup if you can't connect to online.helden-software.de
            } catch {
                return new _3_W_20_Throw("", "", "", "", 0, 0, 0, 0, -999);
            }
        }

        // check wheter online.helden-software.de is online or not
        public async Task<string> IsOnline() {
            string action = "&action=listhelden";

            var request = (HttpWebRequest)WebRequest.Create(url + Helper.HeldenToken + action);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            try {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var stream = response.GetResponseStream()) {
                    var xml = XDocument.Load(stream);
                    var query = from c in xml.Root.Descendants("held")
                                select c.Element("name");
                }
                return "online";
            } catch {
                return "offline";
            }
        }

        // will probably never be used
        public async Task SaveXmlToPostgres(DBService service, ulong discordID) {
            var hero = await service.GetHeldenID(discordID);
            var request = (HttpWebRequest)WebRequest.Create(url + Helper.HeldenToken + "&heldenid=" + hero.Key);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                string xml = reader.ReadToEnd();
                xml = Regex.Replace(xml, @"\r\n?|\n", "");
                await service.UpdateHeroXml(hero.Key, xml);
            }
        }

        // helpermethod for a Basic Trial
        public _3_W_20_Throw ParseProbe(XDocument xml, string talent) {
            IEnumerable<XElement> query = (from c in xml.Root.Descendants("talent") select c).Distinct();
            IEnumerable<XElement> wquery = (from c in xml.Root.Descendants("zauber") select c).Distinct();
            query = query.Concat(wquery);
            foreach (var element in query) {
                string el = new string(element
                    .Element("name").Value
                    .ToString().ToLower()
                    .Where(x => char.IsLetterOrDigit(x))
                    .ToArray());
                talent = new string(talent.ToLower()
                    .Where(x => char.IsLetterOrDigit(x))
                    .ToArray());
                if (el.Contains(talent)) {
                    List<Eigenschaften> tmp = new List<Eigenschaften>();
                    Eigenschaften eg;
                    string[] probe = element.Element("probe").Value.ToString().ToUpper().Split('/');
                    for (int i = 0; i < 3; i++) {
                        Enum.TryParse(probe[i], out eg);
                        tmp.Add(eg);
                    }
                    if (element.Element("repräsentation") != null && element.Element("repräsentation").Value.ToString() == "Elf")
                        tmp = ReplaceProperty(xml, tmp);
                    return new _3_W_20_Throw(
                    element.Element("name").Value.ToString(),
                    tmp[0].ToString(),
                    tmp[1].ToString(),
                    tmp[2].ToString(),
                    Convert.ToInt32(element.Element("wert").Value.ToString()),
                    GetValueByPorpertyName(xml, tmp[0]),
                    GetValueByPorpertyName(xml, tmp[1]),
                    GetValueByPorpertyName(xml, tmp[2]));
                }
            }

            return new _3_W_20_Throw("F", "A", "I", "L", 0, 0, 0, 0, -999);
        }

        // helpermethod for a Special Trial  (Ritualkenntnis, Attributo)
        public _3_W_20_Throw ParseSpecialProbe(XDocument xml, string talent, List<Eigenschaften> special) {
            IEnumerable<XElement> query = (from c in xml.Root.Descendants("talent") select c).Distinct();
            IEnumerable<XElement> wquery = (from c in xml.Root.Descendants("zauber") select c).Distinct();
            query = query.Concat(wquery);
            foreach (var element in query) {
                string el = new string(element
                    .Element("name").Value
                    .ToString().ToLower()
                    .Where(x => char.IsLetterOrDigit(x))
                    .ToArray());
                talent = new string(talent.ToLower()
                    .Where(x => char.IsLetterOrDigit(x))
                    .ToArray());
                if (el.Contains(talent)) {
                    special = ReplaceProperty(xml, special);

                    return new _3_W_20_Throw(
                    element.Element("name").Value.ToString(),
                    special[0].ToString(),
                    special[1].ToString(),
                    special[2].ToString(),
                    Convert.ToInt32(element.Element("wert").Value.ToString()),
                    GetValueByPorpertyName(xml, special[0]),
                    GetValueByPorpertyName(xml, special[1]),
                    GetValueByPorpertyName(xml, special[2]));
                }
            }
            return new _3_W_20_Throw("F", "A", "I", "L", 0, 0, 0, 0, -999);
        }

        private int GetValueByPorpertyName(XDocument xml, Eigenschaften eigenschaften) {
            IEnumerable<XElement> query = (from c in xml.Root.Descendants("eigenschaften") select c).Distinct();
            string eigenschaft = "";
            switch (eigenschaften) {
                case Eigenschaften.MU:
                    eigenschaft = "mut";
                    break;
                case Eigenschaften.KL:
                    eigenschaft = "klugheit";
                    break;
                case Eigenschaften.IN:
                    eigenschaft = "intuition";
                    break;
                case Eigenschaften.CH:
                    eigenschaft = "charisma";
                    break;
                case Eigenschaften.FF:
                    eigenschaft = "fingerfertigkeit";
                    break;
                case Eigenschaften.GE:
                    eigenschaft = "gewandtheit";
                    break;
                case Eigenschaften.KO:
                    eigenschaft = "konstitution";
                    break;
                case Eigenschaften.KK:
                    eigenschaft = "koerperkraft";
                    break;
            }

            foreach (var element in query) {
                int el = Convert.ToInt32(new string(element
                    .Element(eigenschaft)
                    .Element("akt")
                    .Value
                    .ToString().ToArray()));
                return el;
            }
            return 0;
        }

        // replaces KL by IN if possible and if elf
        private List<Eigenschaften> ReplaceProperty(XDocument xml, List<Eigenschaften> e) {
            if (e.FindAll(x => x.Equals(Eigenschaften.KL)).Count > 0
                && e.FindAll(x => x.Equals(Eigenschaften.IN)).Count < 2
                && GetValueByPorpertyName(xml, Eigenschaften.KL) < GetValueByPorpertyName(xml, Eigenschaften.IN))
                e[e.FindIndex(i => i.Equals(Eigenschaften.KL))] = Eigenschaften.IN;
            return e;

            ;
        }
    }
}