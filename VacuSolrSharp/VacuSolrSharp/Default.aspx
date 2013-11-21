<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="VacuSolrSharp._Default" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>베커 솔라루신</title>
    <link href="./css/Site.css" rel="stylesheet" type="text/css" />
</head>

<body>
<div class="page">

    <div id="header">
        <div id="title">
            <h1><a href="./">베커소프트(Solr#)</a></h1>
        </div>
        <div id="menucontainer">
            <ul id="menu">              
                <li class="selected"><a href="./">Simple</a></li>
                <li><a href="./GroupBy.aspx">Group By</a></li>
            </ul>
        </div>
    </div>

    <div id="main">
        
    <div class="search_bar">
        <form method="get" action="">
            <input id="q" name="q" type="text" value='<%=solrSearchVO.q%>' />
            <input type="submit" value="검색" />
            <input type="submit" value="재설정" onclick="document.getElementById('q').value = '';" />
        </form>
        
        <asp:Literal ID="ltrRemoveFacet" runat="server"></asp:Literal>
    </div>
    
    <div class="leftColumn">
        <ul>
            <asp:Literal ID="ltrResultFacets" runat="server"></asp:Literal>
        </ul>
    </div>

    <div class="rightColumn">
    
        <div>
            정렬: <asp:Literal ID="ltrSort" runat="server"></asp:Literal>
        </div>
        
        <div style="padding:5px 0 0 0;">
            결과건수 <%=searchResults.StartAt+1%> - <%=searchResults.StartAt+searchResults.Rows%> of <b><%=searchResults.TotalResults%></b>
        </div>
              
		<div style="overflow:hidden;">                
            <div class="pagination">
                <asp:Literal ID="ltrPageNationTop" runat="server"></asp:Literal>
            </div>
        
            <div class="pagesize">
                목록건수 <asp:Literal ID="ltrPageSizeTop" runat="server"></asp:Literal>
            </div>
        </div>
  
        <div class="results">
            <asp:Literal ID="ltrResultRecords" runat="server"></asp:Literal>
        </div>
        
        <div>
            결과건수 <%=searchResults.StartAt+1%> - <%=searchResults.StartAt+searchResults.Rows%> of <b><%=searchResults.TotalResults%></b>
        </div>

        <div class="pagination">
            <asp:Literal ID="ltrPageNation" runat="server"></asp:Literal>
        </div>
        
        <div class="pagesize">
            목록건수 <asp:Literal ID="ltrPageSize" runat="server"></asp:Literal>
        </div>
    </div>

    </div>
    
    <asp:Literal ID="ltrDebug" runat="server"></asp:Literal>
</div>
</body>
</html>
