using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Services.DataStoreage
{

    /// <summary>
    /// <para>
    /// The class have been casually tested. 
    /// </para>
    /// This class will help you store object, 
    /// and do all the boring primary executions 
    /// on stream and shit like that. It should have 
    /// fancy interface for all different sorts of relative
    /// parameters. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectCache<T>
    {
        public string FileLocation { get; set; }
        public string FileName { get; set; }
        public T ObjectToStore { get; set; }
        //This bool determine adtion both read objects from disk and 
        //write object to disk. True means it will overwrite file on hardisk 
        //and overwrite object in the field of the instance. 
        public bool OverWrite { get; set; }

        protected Exception OccurredError { get; }
        /// <summary>
        /// Cached the object to a string 
        /// </summary>
        /// <param name="stuff"></param>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <param name="overwrite">
        /// A boolean to indicate whether to overwrite an 
        /// exiting file if the file is already there. 
        /// </param>
        public ObjectCache(T stuff, string dir, string filename, bool overwrite = true)
        {
            if (!(Directory.Exists(dir)||dir=="")) throw new Exception("Dir DNE. ");
            ObjectToStore = stuff;
            FileLocation = dir;
            FileName = filename;
            this.OverWrite = overwrite;
        }

        /// <summary>
        /// Store the object onto the harddisk, XML format. 
        /// </summary>
        /// <returns>
        /// True if operation succesful
        /// false if there is something wrong. 
        /// </returns>
        public bool serialize()
        {
            // All the information must be present. 
            if (ObjectToStore == null || FileName == null || FileLocation == null) return false; 

            TextWriter writer = null;

            try
            {
                var serializer = new XmlSerializer(typeof(T));

                //Handle direct relative path. 
                string temp = this.FileLocation==""?"":@"\";

                writer = new StreamWriter(this.FileLocation+temp+this.FileName, this.OverWrite);
                serializer.Serialize(writer, this.ObjectToStore);
            }
            catch (Exception exc)
            {
                Utilities.Stuff.ConsoleLog(exc.Message);
                Utilities.Stuff.ConsoleLog(exc.StackTrace);
                return false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
            return true; 
        }


        /// <summary>
        /// THis method reads from the file. 
        /// <param>
        /// Whether this method will overiwte the object in the field 
        /// depends on the overwrite boolean. 
        /// </param>
        /// </summary>
        /// <returns>
        /// true or false to indeicate wheter the execution 
        /// is seuccessful. 
        /// </returns>
        public bool deserialize()
        {
            if (this.FileLocation == null || this.FileName == null)
                return false;
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                //Handle direct relative path. 
                string temp = this.FileLocation == "" ? "" : @"\";
                reader = new StreamReader(FileLocation+temp+FileName);
                if (this.OverWrite) this.ObjectToStore = (T)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Utilities.Stuff.ConsoleLog(e.Message+"\n"+e.StackTrace);

            }
            finally
            {
                if (reader != null)
                reader.Close();
            }
            return true; 
        }





    }
}
