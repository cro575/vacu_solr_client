using org.apache.solr.SolrSharp.Configuration.Schema;

namespace org.apache.solr.SolrSharp.Query.Highlights
{
    /// <summary>
    /// HighlightParameter is used to reference highlighting of a 
    /// solr index field for a search query. HighlightParameter
    /// implements BaseHighlighter, permitting overrides of all
    /// parameters on a field specific basis.
    /// </summary>
    public class HighlightParameter : BaseHighlighter
    {

        /// <summary>
        /// Public constructor of HighlightParameter. Instances
        /// require an instance of SolrField to be created.
        /// </summary>
        /// <param name="solrfield">instance of SolrField to apply highlighting</param>
        public HighlightParameter(SolrField solrfield)
        {
            this.SolrField = solrfield;
        }

        /// <summary>
        /// Instance of SolrField representing the index field to apply highlighting
        /// </summary>
        public SolrField SolrField { get; private set; }

        /// <summary>
        /// Override of BaseHighlighter implementation of ParameterReference. Returns
        /// the field reference for any parameter property overrides set for this
        /// instance.
        /// </summary>
        public override string ParameterReference
        {
            get
            {
                return string.Format("f.{0}.{1}", this.SolrField.Name, base.ParameterReference);
            }
        }

        /// <summary>
        /// Override of ToString, as required by BaseHighlighter
        /// </summary>
        /// <returns>string representing name/value pairs for this instance</returns>
        public override string ToString()
        {
            return this.RenderParameters();
        }

    }
}
