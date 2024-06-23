using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace CacheSim {
    static class Program
    {
        public static void Main() {
            Configuration config = GetConfigutation();
            List<Address> lstAddresses = GetAdresses();

            Tests.Run(lstAddresses);

            //new MemoryTest(config, lstAddresses);
        }

        static Configuration GetConfigutation() {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Config.ini");
            List<string> lstLines = File.ReadAllLines(path).Where(p => !p.StartsWith("#") && p.Trim() != string.Empty).ToList();

            Configuration Config = new Configuration();
            List<PropertyInfo> lstProps = Config.GetType().GetProperties().ToList();

            foreach (string line in lstLines) {
                try {
                    List<string> lst = line.Split(':').ToList();
                    if (lst.Count() < 2) { continue; }

                    string PropName = lst[0];
                    PropertyInfo? Prop = lstProps.FirstOrDefault(p => p.Name.Equals(PropName));
                    if (Prop == null) { continue; }

                    dynamic z = Convert.ChangeType(lst[1].Trim(), Prop.PropertyType);
                    Prop.SetValue(Config, z, null);
                }
                catch { }
            }
            return Config;
        }
    
        static List<Address> GetAdresses() {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Oficial.cache");
            List<string> lstLines = File.ReadAllLines(path).Where(p => p.Trim() != string.Empty).ToList();

            List<Address> lst = new();

            foreach(string address in lstLines) {
                try {
                    List<string> lstSplit = address.Split(" ").ToList();
                    ReadWrite RW = lstSplit[1].Trim().ToLower() == "r" ? ReadWrite.Read : ReadWrite.Write;

                    lst.Add(new Address(lstSplit[0].Trim(), RW));
                }
                catch { }
            }

            return lst;
        }
    }
}
