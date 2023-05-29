using IniParser;
using IniParser.Model;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AmbientService.src.utils
{
    public partial class IniReader
    {
        static string Path = "AmbientService.ini";
        //static string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        //[DllImport("kernel32", CharSet = CharSet.Unicode)]
        //static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        //[DllImport("kernel32", CharSet = CharSet.Unicode)]
        //static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //public static string Read(string Key, string Section = null)
        //{
        //    var RetVal = new StringBuilder(255);
        //    GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
        //    return RetVal.ToString("#.#");
        //}

        //public static void Write(string Key, string Value, string Section = null)
        //{
        //    WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        //}

        //public static void DeleteKey(string Key, string Section = null)
        //{
        //    Write(Key, null, Section ?? EXE);
        //}

        //public static void DeleteSection(string Section = null)
        //{
        //    Write(null, null, Section ?? EXE);
        //}

        //public static bool KeyExists(string Key, string Section = null)
        //{
        //    return Read(Key, Section).Length > 0;
        //}

        public static string Read(string Key, string Section = "")
        {
            var parser = new FileIniDataParser();
            IniData ini = parser.ReadFile(Path);

            return ini[Section][Key];
        }
    }
}
