//
// C# (cross-platform)
// MSol.XMLSaved
// v 0.4, 06.06.2022 (cutted)
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System.IO;
using System.Xml.Serialization;

namespace System.Xml
{
    [Serializable]
    public class XMLSaved<T>
    {
        /// <summary>
        ///     Сохранение структуры в файл
        /// </summary>
        /// <param name="file">Полный путь к файлу</param>
        /// <param name="obj">Структура</param>
        public static void Save(string file, T obj)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(); ns.Add("", "");
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.StreamWriter writer = System.IO.File.CreateText(file);
            xs.Serialize(writer, obj, ns);
            writer.Flush();
            writer.Close();
        }

        public static void SaveHere(string file, T obj)
        {
            Save(System.IO.Path.Combine(CurrentDirectory(), file), obj);
        }

        public static string Save(T obj)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(); ns.Add("", "");
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
            System.IO.MemoryStream ms = new MemoryStream();
            System.IO.StreamWriter writer = new StreamWriter(ms);
            xs.Serialize(writer, obj, ns);
            writer.Flush();
            ms.Position = 0;
            byte[] bb = new byte[ms.Length];
            ms.Read(bb, 0, bb.Length);
            writer.Close();
            return System.Text.Encoding.UTF8.GetString(bb); ;
        }

        /// <summary>
        ///     Подключение структуры из файла
        /// </summary>
        /// <param name="file">Полный путь к файлу</param>
        /// <returns>Структура</returns>
        public static T Load(string file)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader reader = System.IO.File.OpenText(file);
                T c = (T)xs.Deserialize(reader);
                reader.Close();
                return c;
            }
            catch { };
            {
                Type type = typeof(T);
                System.Reflection.ConstructorInfo c = type.GetConstructor(new Type[0]);
                return (T)c.Invoke(null);
            };
        }

        public static T LoadHere(string file)
        {
            return Load(System.IO.Path.Combine(CurrentDirectory(), file));
        }

        public static T Load()
        {
            try { return Load(CurrentDirectory() + @"\config.xml"); }
            catch { };
            Type type = typeof(T);
            System.Reflection.ConstructorInfo c = type.GetConstructor(new Type[0]);
            return (T)c.Invoke(null);
        }

        public static string CurrentDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
            // return Application.StartupPath;
            // return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // return System.IO.Directory.GetCurrentDirectory();
            // return Environment.CurrentDirectory;
            // return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            // return System.IO.Path.GetDirectory(Application.ExecutablePath);
        }
    }
}
