using System;
using System.Collections.Generic;
using System.Text;
using org.apache.solr.SolrSharp.Configuration;
using org.apache.solr.SolrSharp.Configuration.Schema;
using org.apache.solr.SolrSharp.Update;
using org.apache.solr.SolrSharp.Indexing;
using org.apache.solr.SolrSharp.Query;
using org.apache.solr.SolrSharp.Query.Parameters;
using org.apache.solr.SolrSharp.Query.Highlights;
using org.apache.solr.SolrSharp.Results;
using Example;

namespace SolrSharpExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_Vacu();
        }

        static void Test_Vacu()
        {
            ExampleQueryBuilder queryBuilder = new ExampleQueryBuilder();
            queryBuilder.QT = "browse";

            queryBuilder.IsDebugEnabled = true;

            ExampleSearchResults searchResults = new ExampleSearchResults();

            //searchResults.ExecuteSearch(queryBuilder,"q=name:ipod&start=0&rows=5&facet=true&facet.field=cat&f.cat.facet.mincount=1&facet.field=manu_exact&f.manu_exact.facet.mincount=1&spellcheck=true&version=2.2");
            searchResults.ExecuteSearch(queryBuilder, "q=ipod&start=0&rows=5&wt=xml");


            Console.WriteLine();
            Console.WriteLine(searchResults.TotalResults.ToString());
            int x = 1;

            foreach (ExampleSearchRecord searchRecord in searchResults.SearchRecords)
            {
                Console.WriteLine(x.ToString() + ":" + searchRecord.Id);
                Console.WriteLine("Name: " + searchRecord.Name);
                Console.WriteLine("Manu:" + searchRecord.Manu);
                Console.WriteLine("InStock: " + searchRecord.InStock);
                Console.WriteLine("Includes: " + searchRecord.Includes);
                foreach (string s in searchRecord.Features)
                {
                    Console.WriteLine("Feature: " + s);
                }
                foreach (string s in searchRecord.Cat)
                {
                    Console.WriteLine("Cat: " + s);
                }
                Console.WriteLine("TimeStamp: " + searchRecord.TimeStamp.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("=============  Facets  =============");
            if (searchResults.ExampleCategoryFacetResults != null)
            {
                foreach (ExFacetResults facetResults in searchResults.ExampleCategoryFacetResults._results)
                {
                    foreach (KeyValuePair<string, int> kvp in facetResults.Facets)
                    {
                        Console.WriteLine(string.Format("{0} = {1}: count = {2}", facetResults.facetName, kvp.Key, kvp.Value));
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("=============  Debugging  =============");
            if (searchResults.DebugResults != null)
            {
                Console.WriteLine("Search query: " + searchResults.DebugResults.QueryString);
                Console.WriteLine("Executed query: " + searchResults.DebugResults.ParsedQuery);
                foreach (ExplanationRecord er in searchResults.DebugResults.ExplanationRecords)
                {
                    Console.WriteLine("Name: " + er.Name);
                    Console.WriteLine("ExplainInfo: " + Environment.NewLine + er.ExplainInfo);
                }

                /**************************************************************
                 * Because we added the OtherQuery object in our QueryBuilder
                 * instance above, we can evalute the results by accessing the
                 * OtherQuery property and OtherQueryExplanationRecords array.
                 ***************************************************************/
                Console.WriteLine("Executed other query: " + searchResults.DebugResults.OtherQuery);
                foreach (ExplanationRecord oer in searchResults.DebugResults.OtherQueryExplanationRecords)
                {
                    Console.WriteLine("Name: " + oer.Name);
                    Console.WriteLine("ExplainInfo: " + Environment.NewLine + oer.ExplainInfo);
                }
            }
        }        

        static void Test_Org()
        {
            /**************************************************************
             * Gain a reference to the searcher referenced in our configuration file
            ***************************************************************/
            SolrSearcher solrSearcher = SolrSearchers.GetSearcher(Mode.Read);

            /**************************************************************
             * Create a new IndexDocument to add to our search index.
             * ExampleIndexDocument inherits from IndexDocument.
            ***************************************************************/
            ExampleIndexDocument iDoc =
                new ExampleIndexDocument(
                "101",
                "One oh one",
                "Sony",
                new string[] { "Electronics", "Computer" },
                new string[] { "Good", "Fast", "Cheap" },
                "USB cable",
                (float)1.234,
                (float)99.99,
                1,
                true);


            /**************************************************************
             * Instantiate a new SolrUpdater, using our SolrSearcher instance
             * We'll then do a quick validation to be sure our defined 
             * ExampleIndexDocument doesn't violate the schema definition on 
             * the solr server. If ExampleIndexDocument is valid, post it to
             * the index
            ***************************************************************/
            SolrUpdater oUpdate = new SolrUpdater(solrSearcher);
            oUpdate.PostToIndex(iDoc, true);

            /**************************************************************
             * Instantiate ExampleQueryBuilder, which inherits from 
             * QueryBuilder. The simple constructor takes our search terms.
            ***************************************************************/
            ExampleQueryBuilder queryBuilder = new ExampleQueryBuilder("fast cheap usb");

            /**************************************************************
             * We add to our query by including facets on the category field. 
             * Facets are implemented by creating a class (like ExampleCategoryFacet) 
             * that inherits from the Facet class.  We instantiate ExampleCategoryFacet 
             * and add it to our QueryBuilder instance.
            ***************************************************************/
            ExampleCategoryFacet catFacet = new ExampleCategoryFacet();
            queryBuilder.AddFacet(catFacet);

            /**************************************************************
             * We also want to add highlighting to our search query. This 
             * is accomplished by instantiating a HighlightParameterCollection, 
             * then adding instances of HighlightParameter objects.
             * 
             * There are universal parameters related to highlighting, all of which 
             * can be superseded on a field-by-field basis.  We set the universal 
             * Snippets parameter to 3 by setting the property on the 
             * HighlightParameterCollection instance.
            ***************************************************************/
            HighlightParameterCollection highlightParameterCollection =
                new HighlightParameterCollection {Snippets = 3};



            /**************************************************************
             * HighlightParameter objects are initialized with a SolrField object. 
             * An instance of SolrField provides field information needed for 
             * highlight parameters and in evaluating the query results. After 
             * instantiating the HighlightParameter object, we addit to the 
             * HighlightParameterCollection instance using the Add method.
            ***************************************************************/
            SolrField solrField_Features = solrSearcher.SolrSchema.GetSolrField("features");
            HighlightParameter highlightParameter_Features = new HighlightParameter(solrField_Features);
            highlightParameterCollection.Add(highlightParameter_Features);

            SolrField solrField_Includes = solrSearcher.SolrSchema.GetSolrField("includes");
            HighlightParameter highlightParameter_Includes = new HighlightParameter(solrField_Includes);
            highlightParameterCollection.Add(highlightParameter_Includes);


            /**************************************************************
             * We add our HighlightParameterCollection to our QueryBuilder instance.
            ***************************************************************/
            queryBuilder.AddHighlightParameterCollection(highlightParameterCollection);


            /**************************************************************
             * Let's enable debugging information in our query.
            ***************************************************************/
            queryBuilder.IsDebugEnabled = true;


            /**************************************************************
             * To show the expansive capacity of solr's debugging support, we 
             * pass along a structured Query object to compare & contrast 
             * results of our actual query.
            ***************************************************************/
            Query otherQuery = new Query();
            List<QueryParameter> otherListQp = new List<QueryParameter> {new QueryParameter("cat", "Computer")};
            QueryParameterCollection otherQpc = new QueryParameterCollection("otherQuery", otherListQp);
            otherQuery.AddQueryParameters(otherQpc, ParameterJoin.AND);
            queryBuilder.ExplainOtherQuery = otherQuery;



            /**************************************************************
             * The QueryBuilder instance is initialized, so we execute our search
             * by initializing an ExampleSearchResults object, which inherits
             * from SearchResults.
             ***************************************************************/
            ExampleSearchResults searchResults = new ExampleSearchResults(queryBuilder);


            /**************************************************************
             * We can now being interrogating the properties of our
             * SearchResults instance. We loop through the SearchRecords array,
             * which are implemented as our ExampleSearchRecord custom class.
             * This is accomplished by our implementation of ExampleSearchResults,
             * using generics in the dotnet 2.0 framework. This permits us to
             * implement ExampleSearchRecord as a strongly-typed objects.
             ***************************************************************/
            Console.WriteLine();
            Console.WriteLine(searchResults.TotalResults.ToString());
            int x = 1;

            foreach (ExampleSearchRecord searchRecord in searchResults.SearchRecords)
            {
                Console.WriteLine(x.ToString() + ":" + searchRecord.Id);
                Console.WriteLine("Name: " + searchRecord.Name);
                Console.WriteLine("Manu:" + searchRecord.Manu);
                Console.WriteLine("InStock: " + searchRecord.InStock);
                Console.WriteLine("Includes: " + searchRecord.Includes);
                foreach (string s in searchRecord.Features)
                {
                    Console.WriteLine("Feature: " + s);
                }
                foreach (string s in searchRecord.Cat)
                {
                    Console.WriteLine("Cat: " + s);
                }
                Console.WriteLine("TimeStamp: " + searchRecord.TimeStamp.ToString());
            }

            /**************************************************************
             * To access the facet data in the search request, we implement
             * ExampleCategoryFacetResults, which inherit from FacetResults.
             * FacetResults provide access in the form of a strongly-typed
             * generic Dictionary, where the key is typed as the field on
             * which the facet is drawn, and the value is an int to reflect
             * the total count for the facet.
             ***************************************************************/
            Console.WriteLine();
            Console.WriteLine("=============  Facets  =============");
            if (searchResults.ExampleCategoryFacetResults != null)
            {
                foreach (ExFacetResults facetResults in searchResults.ExampleCategoryFacetResults._results)
                {
                    foreach (KeyValuePair<string, int> kvp in facetResults.Facets)
                    {
                        Console.WriteLine(string.Format("{0} = {1}: count = {2}", facetResults.facetName, kvp.Key, kvp.Value));
                    }
                }
            }


            /**************************************************************
             * To access the debugging information, we need only to inspect
             * the QueryString and ParsedQuery properties, as well as loop
             * through the ExplanationRecords array.
             * 
             * These properties are populated simply by setting IsDebugEnabled
             * to true in our QueryBuilder instance.
             ***************************************************************/
            Console.WriteLine();
            Console.WriteLine("=============  Debugging  =============");
            if (searchResults.DebugResults != null)
            {
                Console.WriteLine("Search query: " + searchResults.DebugResults.QueryString);
                Console.WriteLine("Executed query: " + searchResults.DebugResults.ParsedQuery);
                foreach (ExplanationRecord er in searchResults.DebugResults.ExplanationRecords)
                {
                    Console.WriteLine("Name: " + er.Name);
                    Console.WriteLine("ExplainInfo: " + Environment.NewLine + er.ExplainInfo);
                }

                /**************************************************************
                 * Because we added the OtherQuery object in our QueryBuilder
                 * instance above, we can evalute the results by accessing the
                 * OtherQuery property and OtherQueryExplanationRecords array.
                 ***************************************************************/
                Console.WriteLine("Executed other query: " + searchResults.DebugResults.OtherQuery);
                foreach (ExplanationRecord oer in searchResults.DebugResults.OtherQueryExplanationRecords)
                {
                    Console.WriteLine("Name: " + oer.Name);
                    Console.WriteLine("ExplainInfo: " + Environment.NewLine + oer.ExplainInfo);
                }
            }

            //List<QueryParameter> listQP = new List<QueryParameter>();
            //listQP.Add(new QueryParameter("id", "101"));
            //QueryParameterCollection queryParameterCollection = new QueryParameterCollection("delete", listQP);
            //Query query = new Query();
            //query.AddQueryParameters(queryParameterCollection, ParameterJoin.AND);
            //DeleteIndexDocument deleteIndexDocument = new DeleteIndexDocument(query);


            /**************************************************************
             * To remove our record from the search index, we instantiate a
             * DeleteIndexDocument object with the same value used as the 
             * field defined for the UniqueKey as defined in the solr schema.
             ***************************************************************/
            //DeleteIndexDocument deleteIndexDocument = new DeleteIndexDocument("101");
            //oUpdate.PostToIndex(deleteIndexDocument, true);

        }
    }
}
