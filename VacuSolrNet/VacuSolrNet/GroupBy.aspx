<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupBy.aspx.cs" Inherits="VacuSolrNet.GroupBy" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>베커 솔라루신</title>
    <link href="./css/Site.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    function doSubmit(frm) {
        return true;
    }
</script>

</head>

<body>
<div class="page">

    <div id="header">
        <div id="title">
            <h1><a href="./">베커소프트(SolrNet)</a></h1>
        </div>
        <div id="menucontainer">
            <ul id="menu">              
                <li><a href="./">Simple</a></li>
                <li class="selected"><a href="./GroupBy.aspx">Group By</a></li>
            </ul>
        </div>
    </div>

    <div id="main">
        
    <div class="search_bar">
        <form method="get" action="" onsubmit="doSubmit(this);">
            <input type="hidden" name="group" value='true' />

            <input id="q" name="q" type="text" value='<%=solrSearchVO.q%>' />

			<select id="group" name="group.field" multiple="true">
                <asp:Literal ID="ltrGroupFieldSelectOpt" runat="server"></asp:Literal>
			</select>

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
    
        <div style="padding:5px 0 0 0;">
            검색그룹수 : <%=searchResults.Grouping.Count%></b> &nbsp;&nbsp;검색시간 : <%=searchResults.Header.QTime%> ms
        </div>
              
        <div class="results">
            <asp:Literal ID="ltrResultRecords" runat="server"></asp:Literal>
        </div>
        
        <div>
            검색그룹수 : <%=searchResults.Grouping.Count%></b> &nbsp;&nbsp;검색시간 : <%=searchResults.Header.QTime%> ms
        </div>

    </div>

    </div>
    
    <asp:Literal ID="ltrDebug" runat="server"></asp:Literal>
</div>
</body>
</html>
