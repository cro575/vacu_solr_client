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
using System.Collections.Generic;

namespace org.apache.solr.SolrSharp.Query.Highlights
{
    /// <summary>
    /// The HighlightParameterCollection is a container of HighlightParameter objects. It
    /// is also an implementation of BaseHighlighter, and is used to set properties
    /// for highlighting parameters on an aggregate basis.
    /// </summary>
    public class HighlightParameterCollection : BaseHighlighter, ICollection<HighlightParameter>
    {
        private List<HighlightParameter> _highlightParameters = new List<HighlightParameter>();

        /// <summary>
        /// The empty public constructor
        /// </summary>
        public HighlightParameterCollection()
        {
        }

        /// <summary>
        /// Strongly-typed array of contained HighlightParameter objects in the collection.
        /// </summary>
        public HighlightParameter[] HighlightParameters
        {
            get { return this._highlightParameters.ToArray(); }
        }

        /// <summary>
        /// Generates a series of name/value pairs structured for use in
        /// querystring usage for solr search queries.
        /// </summary>
        /// <returns>string of name/value pairs</returns>
        public override string ToString()
        {
            //take care of hl, hl.fl
            //take care of overall parameters, per field parameters
            List<string> highlightparams = new List<string>();
            if (this._highlightParameters.Count > 0)
            {
                highlightparams.Add(string.Format("{0}=true", BaseHighlighter.SOLR_PARAM_HIGHLIGHT));
                highlightparams.Add(base.RenderParameters());
                List<string> fields = new List<string>();
                foreach (HighlightParameter hp in this.HighlightParameters)
                {
                    fields.Add(hp.SolrField.Name);
                    string overrideparams = hp.ToString();
                    if (overrideparams != string.Empty)
                    {
                        highlightparams.Add(overrideparams);
                    }
                }
                highlightparams.Add(string.Format("{0}={1}", BaseHighlighter.SOLR_PARAM_HIGHLIGHTFIELDS, string.Join(",", fields.ToArray())));
            }
            return string.Join("&", highlightparams.ToArray());
            
        }


        #region IEnumerable<HighlightParameter> Members
        /// <summary>
        /// Implementation of generic GetEnumerator, per the IEnumerable interface
        /// </summary>
        /// <returns>a generic List's IEnumerator</returns>
        public IEnumerator<HighlightParameter> GetEnumerator()
        {
            return this._highlightParameters.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Implementation of GetEnumerator, per the IEnumerable interface
        /// </summary>
        /// <returns>a generic List's IEnumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._highlightParameters.GetEnumerator();
        }

        #endregion

        #region ICollection<HighlightParameter> Members
        /// <summary>
        /// Adds a HighlightParameter object to the underlying List
        /// </summary>
        /// <param name="item">instance of HighlightParameter</param>
        public void Add(HighlightParameter item)
        {
            if (!this._highlightParameters.Contains(item))
            {
                this._highlightParameters.Add(item);
            }
        }

        /// <summary>
        /// Removes all contained instances of HighlightParameter from
        /// the underlying list
        /// </summary>
        public void Clear()
        {
            this._highlightParameters.Clear();
        }

        /// <summary>
        /// Returns true if the underlying list contains the referenced
        /// HighlightParameter
        /// </summary>
        /// <param name="item">instance of HighlightParameter to evaluate</param>
        /// <returns>true if the underlying list contains the HighlightParameter</returns>
        public bool Contains(HighlightParameter item)
        {
            return this._highlightParameters.Contains(item);
        }

        /// <summary>
        /// Copies the referenced HighlightParameter objects in the underlying
        /// list to the referenced array, starting from the arrayIndex point
        /// in the list
        /// </summary>
        /// <param name="array">instance of a HighlightParameter array to copy into</param>
        /// <param name="arrayIndex">index of the list where copying should begin</param>
        public void CopyTo(HighlightParameter[] array, int arrayIndex)
        {
            this._highlightParameters.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The number of HighlightParameter instances in the underlying list
        /// </summary>
        public int Count
        {
            get { return this._highlightParameters.Count; }
        }

        /// <summary>
        /// Returns true if this instance cannot be changed. Presently returns
        /// false in all scenarios.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the referenced HighlightParameter object from the underlying
        /// list.
        /// </summary>
        /// <param name="item">instance of HighlightParameter to remove from the underlying list</param>
        /// <returns>true if the instance was removed from the list</returns>
        public bool Remove(HighlightParameter item)
        {
            return (this._highlightParameters.Remove(item));
        }

        #endregion

    }
}
