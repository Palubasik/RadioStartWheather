using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace RadioStart.WheatherGadgetProcess
{
    public class ConfigSerializer
    {
        public static bool Serialize<T>(T obj, string filename) where T:class
        {
            bool fOk = false;
            try
            {

                XmlWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);
                writer.Close();
                fOk = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return fOk;
        }

        public static T Deserialize<T>(string filename) where T:class
        {
            T obj = null;
            try
            {
                XmlReader reader = new XmlTextReader(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return obj;
        }
    }

}
