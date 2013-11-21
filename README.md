[SolrSharp]

SolrSharp : 
	- SolrSharp 라이브러리 (vacusoft 기능확장, yskwun 추석추가)
	- 원자료 Url : http://solrsharp.codeplex.com/
			
SolrSharp.Example : SolrSharp 라이브러리 샘플


VaceSolrSharp : Vacusoft 샘플 (collection1 db로 설정을 맞추었음)
	- 의존 모듈 : SolrSharp
	
	- Global.asax.cs 필요시 웹 Secure 처리
        SolrSearcher.bSecure_Connet = true;
        SolrSearcher.loginID = "vacu";
        SolrSearcher.loginPW = "vacu10041";
        
   - web.config 파일의 collection 설정 url 변경하여 테스트 해볼것
		<solr>
			<server mode="ReadWrite" url="http://localhost:8983/solr/collection1"/>
		</solr>

   - Default.aspx.cs 파일의 부분 변경하여 테스트 해볼것
            searchResults.ExecuteSearch(solrSearchVO); //default => browse handler 호출
            //searchResults.ExecuteSearch("Collection1.listPage", solrSearchVO); //xml 설정 기본 param 으로 호출

   - SolrUtil/SolrUtil.cs (그룹과 소팅 관련된 기준 필드 설정)
		////////////////// 그룹 기준 필드 설정 //////////////////
		static public String[] groupFileds = new String[] { "manu_exact", "popularity" }; //collection1

        ////////////////// 소팅 기준 필드 설정 //////////////////
        static string[] sort_list = new String[] { "name asc", "price asc" }; //collection1

VacuSolrUploader : 
   - 의존 모듈 : SolrSharp
   - SolrSharp 을 이용한 xml 파일 업로드 색인 및 solr command 전송 유틸
