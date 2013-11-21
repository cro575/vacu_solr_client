//
//   Licensed to the Apache Software Foundation (ASF) under one or more
//   contributor license agreements.  See the NOTICE file distributed with
//   this work for additional information regarding copyright ownership.
//   The ASF licenses this file to You under the Apache License, Version 2.0
//   (the "License"); you may not use this file except in compliance with
//   the License.  You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace org.apache.solr.SolrSharp.Indexing
{
    /// <summary>
    /// Base type for documents that are applied to a solr index. IndexDocument
    /// handles the serialization of derived documents into xml format.
    /// </summary>
    [Serializable]
    public abstract class IndexDocument
    {
        /// <summary>
        /// Empty constructor, required by Xml serialization under .Net
        /// </summary>
        public IndexDocument()
        {
        }

        /// <summary>
        /// Serializes the given document to a string for application to a solr index.
        /// </summary>
        /// <returns>Xml-formatting string representation of the derived object</returns>
        public string SerializeToString()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(stream, Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(this.GetType(), "");
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xs.Serialize(xtw, this, xsn);
            stream = (MemoryStream)xtw.BaseStream;
            string xmlstring = UTF8ByteArrayToString(stream.ToArray());
            return xmlstring;
        }

        /// <summary>
        /// Converts a UTF8 byte array of characters to a string
        /// </summary>
        /// <param name="characters">Byte array set of characters to be converted</param>
        /// <returns>converted string</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            if (Convert.ToInt32(constructedString[0]) > 256)
                constructedString = constructedString.Substring(1, constructedString.Length - 1);
            return constructedString;
        }

/*
        /// <summary>
        /// Converts a string to a UTF8-encoded byte array of characters
        /// </summary>
        /// <param name="pXmlString">The string of characters to be converted</param>
        /// <returns>converted byte array</returns>
        private Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
*/

        // Reserved for future use
        // Adaptation of writing serialization to a file for asynchronous queueing
/*
        private string _fileName;
        [XmlIgnore]
        public string FileName
        {
            get
            {
                if (this._fileName == null)
                    this._fileName = Environment.MachineName + "_" + this.GetType().ToString() + "_" + DateTime.Now.Ticks.ToString() + "_" + Guid.NewGuid().ToString() + ".xml";

                return this._fileName;
            }
        }

        public void SerializeToFile(string filepath)
        {
            string filename = filepath + (filepath.EndsWith(@"\") ? "" : @"\") + this.FileName;
            using (TextWriter writer = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                XmlSerializer xs = new XmlSerializer(this.GetType());
                xs.Serialize(writer, this, null);
            }
        }
*/
    }
}
