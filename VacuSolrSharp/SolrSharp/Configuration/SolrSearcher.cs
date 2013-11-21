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
using System.Net;
using System.Text;
using System.Xml;
using org.apache.solr.SolrSharp.Configuration.Schema;

namespace org.apache.solr.SolrSharp.Configuration
{
    /// <summary>
    /// Enumeration of solr-specific configuration file types.
    /// </summary>
    public enum ConfigurationFile
    {
        /// <summary>
        /// schema.xml
        /// </summary>
        Schema,
        /// <summary>
        /// solrconfig.xml
        /// </summary>
        Config
    }

    /// <summary>
    /// A class representing a solr server as defined under configuration. SolrSearcher is defined
    /// by its Url and Mode. These settings ensure search requests and updates are directed to
    /// the correct servers.
    /// </summary>
    public class SolrSearcher
    {
        /// <summary>
        /// The url for the solr webapp instance used by this SolrSearcher
        /// </summary>
        public string SOLR = "";

        /// <summary>
        /// The url to the administrative Ping() function for the solr webapp instance
        /// used by this SolrSearcher
        /// </summary>
        public string SOLR_PING = "";
        private string SOLR_CONFIG = "";
        private string SOLR_SCHEMA = "";
        private XmlDocument SOLR_XmlConfig;
        private XmlDocument SOLR_XmlSchema;
        private Mode _searcherMode;
        private readonly object syncLock = new object();

        //++ yskwun 20130210  add
        public static bool bSecure_Connet = true;
        public static string loginID = "vacu";
        public static string loginPW = "vacu10041";
        //--

        /// <summary>
        /// Constructor that constructs functionality by url and Mode.
        /// </summary>
        /// <param name="solrUrl">url pointer to the solr webapp instance</param>
        /// <param name="searcherMode">Read/Write basis for the solr webapp instance</param>
        public SolrSearcher(string solrUrl, Mode searcherMode, bool bRoadSchema) // yskwun 20130210  bRoadSchema option addd
        {
            this._searcherMode = searcherMode;
            this.SOLR = solrUrl;
            this.SetSolrPaths();
            if(bRoadSchema) // yskwun 20130210  bRoadSchema option add
                this.SetSolrSchema();
        }
        private void SetSolrPaths()
        {
            if (!this.SOLR.EndsWith("/"))
                this.SOLR += "/";
            this.SOLR_PING = this.SOLR + "admin/ping";
            this.SOLR_CONFIG = this.SOLR + "admin/file/?contentType=text/xml;charset=utf-8&file=solrconfig.xml";
            this.SOLR_SCHEMA = this.SOLR + "admin/file/?contentType=text/xml;charset=utf-8&file=schema.xml";
        }
        private void SetSolrSchema()
        {
            if (this.solrSchema != null) return;
            lock (this.syncLock)
            {
                this.solrSchema = new SolrSchema(this);
            }
        }

        #region Instance properties/methods
        /// <summary>
        /// Determines if the current instance supports the searcherMode. This is a flagged
        /// bitwise comparison, since Read mode equates to ReadWrite mode and Write mode equates
        /// to ReadWrite mode.
        /// </summary>
        /// <param name="searcherMode"></param>
        /// <returns></returns>
        public bool Supports(Mode searcherMode)
        {
            return ((this._searcherMode & searcherMode) == searcherMode);
        }

        /// <summary>
        /// Evaluates the current status of Ping() on the solr server. Ping() is a function
        /// of the solr web application.
        /// </summary>
        /// <returns>HttpStatusCode result from the ping() request</returns>
        public HttpStatusCode Ping()
        {
            HttpStatusCode iCode = HttpStatusCode.NotFound;
            HttpWebResponse oResponse = null;
            try
            {
                HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(this.SOLR_PING);
                //++ yskwun 20130210  add
                if(bSecure_Connet)
                    oRequest.Credentials = new NetworkCredential(loginID, loginPW);
                //--
                oResponse = (HttpWebResponse)oRequest.GetResponse();
                iCode = oResponse.StatusCode;
            }
            catch (WebException) { }
            finally
            {
                if (oResponse != null)
                    oResponse.Close();
            }
            return iCode;
        }

        /// <summary>
        /// Returns a string value result from the xpath query within the referenced ConfigurationFile
        /// for the solr server
        /// </summary>
        /// <param name="eConfigFile">ConfigurationFile type for this solr server</param>
        /// <param name="xpathquery">XPath-based query to be applied to this solr configuration file</param>
        /// <returns>string</returns>
        public string GetConfigurationValue(ConfigurationFile eConfigFile, string xpathquery)
        {
            XmlDocument xdoc = this.GetConfigurationXml(eConfigFile);
            return (string)SolrSearcher.GetXmlValue(xdoc.DocumentElement, xpathquery);
        }

        /// <summary>
        /// Returns an XmlNodeList of values as the result from the xpath query within the 
        /// referenced ConfigurationFile for the solr server
        /// </summary>
        /// <param name="eConfigFile">ConfigurationFile type for this solr server</param>
        /// <param name="xpathquery">XPath-based query to be applied to this solr configuration file</param>
        /// <returns>XmlNodeList</returns>
        public XmlNodeList GetConfigurationData(ConfigurationFile eConfigFile, string xpathquery)
        {
            XmlDocument xdoc = this.GetConfigurationXml(eConfigFile);
            return SolrSearcher.GetXmlNodes(xdoc.DocumentElement, xpathquery);
        }

        /// <summary>
        /// Returns the referenced ConfigurationFile for the solr server as an XmlDocument
        /// </summary>
        /// <param name="eConfigFile">ConfigurationFile type for this solr server</param>
        /// <returns>XmlDocument</returns>
        public XmlDocument GetConfigurationXml(ConfigurationFile eConfigFile)
        {
            XmlDocument xdoc = null;
            switch (eConfigFile)
            {
                case ConfigurationFile.Config:
                    if (this.SOLR_XmlConfig == null)
                    {
                        this.SOLR_XmlConfig = SolrSearcher.GetXmlDocument(this.SOLR_CONFIG);
                    }
                    xdoc = this.SOLR_XmlConfig;
                    break;
                case ConfigurationFile.Schema:
                    if (this.SOLR_XmlSchema == null)
                    {
                        this.SOLR_XmlSchema = SolrSearcher.GetXmlDocument(this.SOLR_SCHEMA);
                    }
                    xdoc = this.SOLR_XmlSchema;
                    break;
            }
            return xdoc;
        }

        private SolrSchema solrSchema = null;
        /// <summary>
        /// SolrSchema object associated with this searcher.
        /// </summary>
        public SolrSchema SolrSchema
        {
            get { return this.solrSchema; }
        }
        #endregion

        #region Static properties/methods
        /// <summary>
        /// Creates a standard http request of type POST
        /// </summary>
        /// <param name="url">URL to post values</param>
        /// <param name="bytesToPost">Post value payload to pass to the URL</param>
        /// <param name="statusDescription">Standard http status code description text</param>
        /// <returns>HttpStatusCode</returns>
        public static HttpStatusCode WebPost(string url, byte[] bytesToPost, ref string statusDescription)
        {
            HttpStatusCode iCode = HttpStatusCode.BadRequest;
            try
            {
                HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(url);
                //++ yskwun 20130210  add
                if (bSecure_Connet)
                    oRequest.Credentials = new NetworkCredential(loginID, loginPW);
                //--
                oRequest.Method = "POST";
                oRequest.ContentType = "text/xml";
                oRequest.ContentLength = bytesToPost.Length;
                Stream dataStream = oRequest.GetRequestStream();
                dataStream.Write(bytesToPost, 0, bytesToPost.Length);
                dataStream.Close();
                HttpWebResponse oResponse = (HttpWebResponse)oRequest.GetResponse();
                statusDescription = oResponse.StatusDescription;
                iCode = oResponse.StatusCode;
                oResponse.Close();
            }
            catch (WebException we)
            {
                throw new Exception("Http error in request/response to " + url, we.InnerException);
            }
            catch (Exception e)
            {
                throw new Exception("General error in request/response to " + url, e.InnerException);
            }
            return iCode;
        }

        /// <summary>
        /// Formulates a string as an encoded byte array.  This is useful when posting values via the WebPost method.
        /// </summary>
        /// <param name="content">The string of content to encode</param>
        /// <param name="oEncoding">Encoding basis to be applied to the content</param>
        /// <returns>byte array</returns>
        public static byte[] GetContentToPost(string content, Encoding oEncoding)
        {
            byte[] byteXml = oEncoding.GetBytes(content);
            return byteXml;
        }

        /// <summary>
        /// Formulates the content of a file as an encoded byte array. 
        /// This is useful when posting values via the WebPost method.
        /// </summary>
        /// <param name="oFile">The file objecty containing the content to encode</param>
        /// <param name="oEncoding">Encoding basis to be applied to the content</param>
        /// <returns>byte array</returns>
        public static byte[] GetContentToPost(FileInfo oFile, Encoding oEncoding)
        {
            StreamReader reader = new StreamReader(oFile.FullName, oEncoding);
            string sXml = reader.ReadToEnd();
            sXml = sXml.Replace(Environment.NewLine, "");
            reader.Close();
            return SolrSearcher.GetContentToPost(sXml, oEncoding);
        }

        /// <summary>
        /// Returns an http response as a string
        /// </summary>
        /// <param name="xmlUrl">The url to request</param>
        /// <returns>string</returns>
        private static string GetXmlString(string xmlUrl)
        {
            HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(xmlUrl);
            //++ yskwun 20130210  add
            if (bSecure_Connet)
                oRequest.Credentials = new NetworkCredential(loginID, loginPW);
            //--
            HttpWebResponse oResponse = (HttpWebResponse)oRequest.GetResponse();
            StreamReader reader = new StreamReader(oResponse.GetResponseStream());
            string sr = reader.ReadToEnd();
            oResponse.Close();
            return sr.Replace("\n", "");
        }

        /// <summary>
        /// Returns an http response as an XmlDocument
        /// </summary>
        /// <param name="xmlUrl">The url to request</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument GetXmlDocument(string xmlUrl)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(SolrSearcher.GetXmlString(xmlUrl));
            return xdoc;
        }

        /// <summary>
        /// Returns an http response from an http POST request as an XmlDocument
        /// </summary>
        /// <param name="url">The url to request</param>
        /// <param name="queryparameters">String of content to post to the Url</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument GetXmlDocumentFromPost(string url, string queryparameters)
        {
            XmlDocument xdoc = new XmlDocument();
            HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(url);
            //++ yskwun 20130210  add
            if (bSecure_Connet)
                oRequest.Credentials = new NetworkCredential(loginID, loginPW);
            //--
            byte[] postBytes = SolrSearcher.GetContentToPost(queryparameters, Encoding.UTF8);
            oRequest.Method = "POST";
            oRequest.ContentType = "application/x-www-form-urlencoded";
            oRequest.ContentLength = postBytes.Length;
            Stream dataStream = null;
            try
            {
                dataStream = oRequest.GetRequestStream();
            }
            catch (Exception ex)
            {
                string errmessage = "Error in SolrSearchers-GetXml-GetRequestStream" + Environment.NewLine;
                errmessage += "Url: " + url + Environment.NewLine;
                errmessage += "QueryParameters: " + queryparameters + Environment.NewLine;
                throw new Exception(errmessage, ex);
            }
            finally
            {
                if (dataStream != null)
                {
                    dataStream.Write(postBytes, 0, postBytes.Length);
                    dataStream.Close();
                }
            }
            HttpWebResponse oResponse = null;
            try
            {
                oResponse = (HttpWebResponse)oRequest.GetResponse();
            }
            catch (WebException e)
            {
                HttpStatusCode intCode = 0;
                if (oResponse != null)
                    intCode = oResponse.StatusCode;

                Stream errstream = e.Response.GetResponseStream();
                string errsr = "";
                if (errstream != null)
                {
                    StreamReader errreader = new StreamReader(errstream);
                    errsr = errreader.ReadToEnd();
                }
                string errmessage = "Error in SolrSearchers-GetXml-GetResponseStream" + Environment.NewLine;
                if (intCode != 0)
                    errmessage += "HttpStatusCode: " + intCode.ToString() + Environment.NewLine;
                if (errsr != "")
                    errmessage += "StreamResponse: " + errsr + Environment.NewLine + "End-of-stream-response" + Environment.NewLine;
                throw new Exception(errmessage, e);
            }
            StreamReader reader = new StreamReader(oResponse.GetResponseStream());
            string sr = reader.ReadToEnd();
            if (oResponse != null)
                oResponse.Close();
            xdoc.LoadXml(sr.Replace("\n", ""));
            return xdoc;
        }

        /// <summary>
        /// Returns the object value per a given xpath query
        /// </summary>
        /// <param name="xnData">The XmlNode to evaluate</param>
        /// <param name="xpathquery">XPath structured query to apply to the XmlNode</param>
        /// <returns>object</returns>
        public static object GetXmlValue(XmlNode xnData, string xpathquery)
        {
            object xmlvalue = null;
            XmlNode xn = xnData.SelectSingleNode(xpathquery);
            if (xn != null)
                xmlvalue = xn.InnerText;
            return xmlvalue;
        }

        /// <summary>
        /// Returns an XmlNodeList of values per a given xpath query
        /// </summary>
        /// <param name="xnData">The XmlNode to evaluate</param>
        /// <param name="xpathquery">XPath structured query to apply to the XmlNode</param>
        /// <returns>XmlNodeList</returns>
        public static XmlNodeList GetXmlNodes(XmlNode xnData, string xpathquery)
        {
            XmlNodeList xn = xnData.SelectNodes(xpathquery);
            return xn;
        }

        /// <summary>
        /// Returns an XmlNode as the value per a given xpath query
        /// </summary>
        /// <param name="xDoc">The XmlDocument to evaluate</param>
        /// <param name="xpathquery">XPath structured query to apply to the XmlNode</param>
        /// <returns>XmlNode</returns>
        public static XmlNode GetXmlNode(XmlDocument xDoc, string xpathquery)
        {
            return xDoc.SelectSingleNode(xpathquery);
        }

        /// <summary>
        /// Returns an XmlNode as the value per a given xpath query
        /// </summary>
        /// <param name="xnode">The XmlNode to evaluate</param>
        /// <param name="xpathquery">XPath structured query to apply to the XmlNode</param>
        /// <returns>XmlNode</returns>
        public static XmlNode GetXmlNode(XmlNode xnode, string xpathquery)
        {
            return xnode.SelectSingleNode(xpathquery);
        }

        #endregion
    }
}
