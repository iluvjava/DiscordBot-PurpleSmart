using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Services.DataStoreage
{

    /// <summary>
    /// This class will help you store object, 
    /// and do all the boring primary executions 
    /// on stream and shit like that. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ObjectCache <T>  where T: ISerializable
    {
        protected string FileLocation;
        protected string FileName; 
        protected T ObjectToStore;

        public ObjectCache(T stuff, string dir,string filename)
        {

        }
        


    }
}
