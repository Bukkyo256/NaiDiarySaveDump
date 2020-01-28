using CommandLine;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiRegEdit
{
    public class Program
    {
        public Program()
        {

        }

        public void SaveToDisk(int slot)
        {
            var folder = Registry.CurrentUser.OpenSubKey(@"Software\BananaKing\Nai Training Diary");
            string keyPartial = $"s{slot}";
            string keyName = string.Empty;
            foreach(var s in folder.GetValueNames())
            {
                if(s.StartsWith(keyPartial))
                {
                    keyName = s;
                    break;
                }
            }
            if(!string.IsNullOrEmpty(keyName))
            {
                byte[] bytes = (byte[]) folder.GetValue(keyName);
                string json = StringFromBytes(bytes);
                var obj = JsonConvert.DeserializeObject(json);
                string formatted = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText($"Slot{slot}.json", formatted);
                Console.WriteLine($"Save slot {slot} save from register to disk: Slot{slot}.json.");
            }
        }

        public static string StringFromBytes(byte[] value)
        {
            string hex = BitConverter.ToString(value);
            byte[] bytes = FromHex(hex);
            string ret = Encoding.UTF8.GetString(bytes);
            return (ret);
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }     

        public static string ToHexString(string str)
        {
            var sb = new List<string>();

            var bytes = Encoding.UTF8.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Add(t.ToString("X2"));
            }

            return string.Join("-", sb);
        }

        public void SaveToRegister(int slot)
        {
            string fileName = $"Slot{slot}.json";
            if(File.Exists(fileName))
            {
                string rawJson = File.ReadAllText(fileName);
                var jsonObject = JsonConvert.DeserializeObject(rawJson);
                string compactedJson = JsonConvert.SerializeObject(jsonObject, Formatting.None);
                string hexString = ToHexString(compactedJson);
                byte[] raw = FromHex(hexString);

                var folder = Registry.CurrentUser.OpenSubKey(@"Software\BananaKing\Nai Training Diary", true);
                string keyPartial = $"s{slot}";
                string keyName = string.Empty;
                foreach (var s in folder.GetValueNames())
                {
                    if (s.StartsWith(keyPartial))
                    {
                        keyName = s;
                        break;
                    }
                }

                if(!string.IsNullOrEmpty(keyName))
                {
                    folder.SetValue(keyName, raw, RegistryValueKind.Binary);
                    Console.WriteLine($"Wrote slot {slot} save to register from disk: Slot{slot}.json.");
                }
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            Parser.Default.ParseArguments<CmdOptions>(args)
                   .WithParsed<CmdOptions>(o =>
                   {
                       if (o.ToDisk)
                       {
                           p.SaveToDisk(o.Slot);
                       }
                       if (o.ToRegister)
                       {
                           p.SaveToRegister(o.Slot);
                       }
                   });
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
