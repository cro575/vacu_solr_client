[구글독스 교육자료](https://docs.google.com/presentation/d/1rMSexKWU3E5kSUOafS_SpG_sM35gBwvmPqqVvxKv6zY/pub?start=false&loop=false&delayms=3000)
------------------

[SolrNet]
================================
빌드환경
----------
	.NetFramework 3.5
	Visual Studio2010


SolrNet 관련 다양한 라이브러리 존재
----------------------------------------
[원자료 Url : https://github.com/mausch/SolrNet]

주요라이브러리

	SolrNet : 메인 모듈 (vacusoft 기능확장, yskwun 주석추가)
	HttpWebAdapters : Http 관련처리
	SampleSolrApp : SolrNet관련 원자료 라이브러리 샘플


VacuSolrNet
----------------------------------------
	Vacusoft 샘플 (collection1 db로 설정을 맞추었음)
	주요 의존 모듈 : 
   		HttpWebAdapters, SolrNet, SolrNet.DSL, log4net, Microsoft.Practices.ServiceLocation, System.Xml.Linq

   	web.config 파일의 collection 설정 url 변경하여 테스트 해볼것 :
   	  <appSettings>
		<add key="solrUrl" value="http://localhost:8983/solr/collection1"/><!--c_aks_todae, collection1--><!--http://hq.vacusoft.co.kr:8983/solr/c_aks_todae-->
		<add key="credentials" value="vacu:vacu10041"/>
	  </appSettings>

   	Default.aspx.cs 파일의 부분 변경하여 테스트 해볼것 :
		//default => browse handler 호출
		queryOption.QT = "/browse";
		searchResults = solr.Query(solrQuery, queryOption);

		//xml 설정 기본 param 으로 호출
		//queryOption.QT = "";
		//searchResults = solr.Query("Collection1.listPage", solrQuery, queryOption); //Todae2013,Collection1 

   	SolrUtil/SolrUtil.cs (그룹과 소팅 관련된 기준 필드 설정)
		////////////////// 그룹 기준 필드 설정 //////////////////
		static public String[] groupFileds = new String[] { "manu_exact", "popularity" }; //collection1

		////////////////// 소팅 기준 필드 설정 //////////////////
		static string[] sort_list = new String[] { "name asc", "price asc" }; //collection1





[SolrSharp]
================================
빌드환경
----------------------------------------
	.NetFramework 3.5
	Visual Studio2008

SolrSharp
----------------------------------------
- [원자료 Url : http://solrsharp.codeplex.com/](http://solrsharp.codeplex.com/)
- SolrSharp 라이브러리 (vacusoft 기능확장, yskwun 주석추가)
			
SolrSharp.Example
----------------------------------------
  SolrSharp 라이브러리 샘플


VaceSolrSharp
----------------------------------------
	Vacusoft 샘플 (collection1 db로 설정을 맞추었음)
  
		의존모듈 : SolrSharp
	
		Global.asax.cs 필요시 웹 Secure 처리
        	SolrSearcher.bSecure_Connet = true;
        	SolrSearcher.loginID = "vacu";
        	SolrSearcher.loginPW = "vacu10041";
        
	web.config 파일의 collection 설정 url 변경하여 테스트 해볼것
		<solr>
			<server mode="ReadWrite" url="http://localhost:8983/solr/collection1"/>
		</solr>

	Default.aspx.cs 파일의 부분 변경하여 테스트 해볼것
		searchResults.ExecuteSearch(solrSearchVO); //default => browse handler 호출
		//searchResults.ExecuteSearch("Collection1.listPage", solrSearchVO); //xml 설정 기본 param 으로 호출

	SolrUtil/SolrUtil.cs (그룹과 소팅 관련된 기준 필드 설정)
		
		////////////////// 그룹 기준 필드 설정 //////////////////
		static public String[] groupFileds = new String[] { "manu_exact", "popularity" }; //collection1

		////////////////// 소팅 기준 필드 설정 //////////////////
		static string[] sort_list = new String[] { "name asc", "price asc" }; //collection1
        	

VacuSolrUploader 
----------------------------------------
   - 의존 모듈 : SolrSharp
   - SolrSharp 을 이용한 xml 파일 업로드 색인 및 solr command 전송 유틸
