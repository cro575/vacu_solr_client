using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace VacuSolrSharp
{
    public class StringUtil
    {
        public static int intParse(string str)
        {
            int i = 0;

            if (!(str == null || str.Equals("") || str.Equals("undefined")))
            {
                try
                {
                    i = int.Parse(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }
            
            return i;
        }

        public static int intParse(string str, int def)
        {
            int i = def;

            if (!(str == null || str.Equals("") || str.Equals("undefined")))
            {
                try
                {
                    i = int.Parse(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }

            return i;
        }

        public static float parseFloat(string str) {
            return parseFloat(str, (float)0.0);
        }

        public static float parseFloat(string str, float def) {
    	    float i = def;

            if (!(str == null || str.Equals("") || str.Equals("undefined"))) {
                try {
                    i = float.Parse(str);
                } catch (Exception ex) {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }

            return i;
        }

        public static double parseDouble(string str) {
            return parseDouble(str, 0.0);
        }

        public static double parseDouble(string str, double def) {
    	    double i = def;

            if (!(str == null || str.Equals("") || str.Equals("undefined"))) {
                try {
                    i = double.Parse(str);
                } catch (Exception ex) {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }

            return i;
        }

        public static string[] pairSplit(string str,char delimiter) {
            int idx = str.IndexOf(delimiter);
            if (idx > 0) {
                string[] sp = new string[2];
                sp[0] = str.Substring(0, idx);
                sp[1] = str.Substring(idx + 1);

                return sp;
            }

            return new string[] { str };
        }

        public static int getParam(string str, int def)
        {
            int i = def;

            if (!(str == null || str.Equals("") || str.Equals("undefined")))
            {
                try
                {
                    i = int.Parse(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }
            
            return i;
        }

        public static int getParam(string str, int def, int min, int max)
        {
            int i = def;

            if (!(str == null || str.Equals("") || str.Equals("undefined")))
            {
                try
                {
                    i = int.Parse(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("intParse:" + ex.ToString() + ":" + str);
                }
            }

            if (i < min) i = min;
            if (i > max) i = max;

            return i;
        }

        public static string getParam(string str)
        {
            if (str == null || str.Equals("undefined"))
                return "";
            else
                return str;
        }

        public static string getParam(string str, String strdef)
        {
            if (str == null || str.Equals("undefined")) str = "";

            str = str.Trim();

            if (str.Equals(""))
                return strdef;
            else
                return str;
        }

        public static string getDecodeParam(string str, String strdef, String strCharset)
        {
            if (str == null || str.Equals("undefined")) str = "";

            str = str.Trim();

            if (str.Equals(""))
                return strdef;

           return System.Web.HttpUtility.UrlDecode(str, Encoding.GetEncoding(strCharset));
        }

        public static string collectionToString<T>(ICollection<T> collection, string delimiter)
        {
            var ret_val = "";
            foreach (var val in collection)
            {
                if (!ret_val.Equals("")) ret_val += delimiter;

                ret_val += val.ToString();
            }

            return ret_val;
        }

        public static string cutString(string thestr, int cutlen)
        {
            if (thestr == null) return "";

            if (thestr.Length <= cutlen) return thestr;

            return thestr.Substring(0, cutlen)+"...";
        }

        public static string StringCut(string inputString, int stringLength)
        {
            string outputString = "";

            Encoding ec = Encoding.Default;

            byte[] temp = ec.GetBytes(inputString);

            if (temp.Length > stringLength)
            {
                outputString = ec.GetString(temp, 0, stringLength) + "..";
            }
            else
            {
                outputString = ec.GetString(temp, 0, temp.Length);
            }
            return outputString;
        }

        public static string ReverseXmlData(string txtData)
        {
            string xmlData = "";

            xmlData = txtData.Replace("&lt;", "<");
            xmlData = xmlData.Replace("&gt;", ">");
            xmlData = xmlData.Replace("&apos;", "'");
            xmlData = xmlData.Replace("&quot;", "\"");
            xmlData = xmlData.Replace("&amp;", "&");

            return xmlData;
        }

        public static string getPaging(string param, int curPage, int totalCount, int blockSize, int itemPerPage, int iconType)
        {
            int intCurPage = curPage; //page 0이 1페이지임
            int totalPage = (totalCount - 1) / itemPerPage + 1;

            string first = "/images/btn/btn_pagenum_first.gif";
            string prev = "/images/btn/btn_pagenum_prev.gif";
            string next = "/images/btn/btn_pagenum_next.gif";
            string last = "/images/btn/btn_pagenum_last.gif";

            string first_d = "/images/btn/btn_pagenum_first_d.gif";
            string prev_d = "/images/btn/btn_pagenum_prev_d.gif";
            string next_d = "/images/btn/btn_pagenum_next_d.gif";
            string last_d = "/images/btn/btn_pagenum_last_d.gif";

            if(iconType==1)
            {
                first = "/images/btn/btn_pagenum_first2.gif";
                prev = "/images/btn/btn_pagenum_prev2.gif";
                next = "/images/btn/btn_pagenum_next2.gif";
                last = "/images/btn/btn_pagenum_last2.gif";
            }
            StringBuilder sb = new StringBuilder();

            // 하나이상 존재하는 경우
            if (totalCount > 0)
            {
                int curBlock = (intCurPage - 1) / blockSize + 1;	// 현재 페이지가 있는 페이지 블럭
                int curBlockFirst = ((curBlock - 1) * blockSize) + 1;	// 현재 페이지 블럭의 첫번째 페이지
                int curBlockLast = (curBlock * blockSize);	// 현재 페이지 블럭의 마지막 페이지

                // 이전 페이지 블럭이 존재하는 경우 [이전]링크를 걸어준다
                if (intCurPage > blockSize)
                {
                    sb
                    .Append("<a class='direction first' href='?" + param + "&curPage=1&pageSize=" + itemPerPage + "'>")
                    .Append("<img src='" + first + "' alt='처음' /></a>")
                    .Append("<a class='direction prev' href='?" + param + "&curPage=" + (curBlockFirst - 1) + "&pageSize=" + itemPerPage + "'>")
                    .Append("<img src='" + prev + "' alt='이전' /></a>");
                }
                else
                {
//                    if (intCurPage > 1)
//                        sb.Append("<a href='?" + param + "&curPage=1&pageSize=" + itemPerPage + "'><img src='" + first + "' align='absmiddle' /></a><img src='" + prev + "' align='absmiddle' />");
//                    else
//                        sb.Append("<img src='" + first_d + "' align='absmiddle' /><img src='" + prev_d + "' align='absmiddle' />");
                }

                // 페이지 번호
                for (int i = curBlockFirst; i <= curBlockLast; i++)
                {
                    if (i > totalPage)
                        break;

                    if (intCurPage == i)
                    {
                        sb
                        .Append("<a href='?").Append(param).Append("&curPage=" + i + "").Append("&pageSize=").Append(itemPerPage).Append("' class='on'>")
                        .Append(i)
                        .Append("</a>");
                    }
                    else
                    {
                        sb
                        .Append("<a href='?").Append(param).Append("&curPage=" + i + "").Append("&pageSize=").Append(itemPerPage).Append("'>")
                        .Append(i)
                        .Append("</a>");
                    }

                    if (i == curBlockLast || i == totalPage)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }

                if (curBlockLast < totalPage)
                {
                    sb
                    .Append("<a class='direction next' href='?" + param + "&curPage=" + (curBlockLast + 1) + "&pageSize=" + itemPerPage + "'>")
                    .Append("<img src='" + next + "' alt='다음' /></a>")
                    .Append("<a class='direction last' href='?" + param + "&curPage=" + totalPage + "&pageSize=" + itemPerPage + "'>")
                    .Append("<img src='" + last + "' alt='끝' /></a>");
                }
                else
                {
//                    if(intCurPage<totalPage)
//                        sb.Append("<img src='" + next_d + "' align='absmiddle' /><a href='?" + param + "&curPage=" + totalPage + "&pageSize=" + itemPerPage + "'><img src='" + last + "' align='absmiddle' /></a>");
//                    else
//                        sb.Append("<img src='" + next_d + "' align='absmiddle' /><img src='" + last_d + "' align='absmiddle' />");
                }
            }
            else
            { // if end
                //데이터가 없는경우
            }

            return sb.ToString();
        }

        public static string getPagingAdm(string param, int curPage, int totalCount, int blockSize, int itemPerPage)
        {
            int intCurPage = curPage; //page 0이 1페이지임
            int totalPage = (totalCount - 1) / itemPerPage + 1;

            StringBuilder sb = new StringBuilder();

            //sb.Append("<div class='paginate'>");

            // 하나이상 존재하는 경우
            if (totalCount > 0)
            {
                int curBlock = (intCurPage - 1) / blockSize + 1;	// 현재 페이지가 있는 페이지 블럭
                int curBlockFirst = ((curBlock - 1) * blockSize) + 1;	// 현재 페이지 블럭의 첫번째 페이지
                int curBlockLast = (curBlock * blockSize);	// 현재 페이지 블럭의 마지막 페이지

                // 이전 페이지 블럭이 존재하는 경우 [이전]링크를 걸어준다
                if (intCurPage > blockSize)
                {
                    sb
                    .Append("<a href='?" + param + "&curPage=1&pageSize=" + itemPerPage + "' class='pre_end'>맨앞</a> ")
                    .Append("<a href='?" + param + "&curPage=" + (curBlockFirst - 1) + "&pageSize=" + itemPerPage + "' class='pre'>이전</a> ");
                }
                else
                {
                    /*
                    if (intCurPage > 1)
                        sb.Append("<a href='?" + param + "&curPage=1&pageSize=" + itemPerPage + "' class='pre_end'>맨앞</a> <span class='pre'>이전</span> ");
                    else
                        sb.Append("<span class='pre'>이전</span> ");
                     */
                }

                // 페이지 번호
                for (int i = curBlockFirst; i <= curBlockLast; i++)
                {
                    if (i > totalPage)
                    {
                        break;
                    }

                    if (intCurPage == i)
                    {
                        sb.Append("<strong>" + i + "</strong> ");
                    }
                    else
                    {
                        sb
                        .Append("<a href='?").Append(param).Append("&curPage=" + i + "").Append("&pageSize=").Append(itemPerPage).Append("'>")
                        .Append(i)
                        .Append("</a> ");
                    }

                    if (i == curBlockLast || i == totalPage)
                    {
                        sb.Append("");
                    }
                    else
                    {
                        sb.Append("");
                    }
                }

                if (curBlockLast < totalPage)
                {
                    sb
                    .Append("<a href='?" + param + "&curPage=" + (curBlockLast + 1) + "&pageSize=" + itemPerPage + "' class='next'>다음</a> ")
                    .Append("<a href='?" + param + "&curPage=" + totalPage + "&pageSize=" + itemPerPage + "' class='next_end'>맨뒤</a> ");
                }
                else
                {
                    /*
                    if(intCurPage<totalPage)
                        sb.Append("<span class='next'>다음</span> <a href='?" + param + "&curPage=" + totalPage + "&pageSize=" + itemPerPage + "' class='next_end'>맨뒤</a> ");
                    else
                        sb.Append("<span class='next'>다음</span> <span class='next_end'>맨뒤</span> ");
                     */
                }
            }
            else
            { // if end
                //데이터가 없는경우
            }

            //sb.Append("</div>");

            return sb.ToString();
        }

        public static string getPagingSolr(string param, int curPage, int totalCount, int blockSize, int itemPerPage)
        {
            int intCurPage = curPage; //page 0이 1페이지임
            int totalPage = (totalCount - 1) / itemPerPage + 1;

            StringBuilder sb = new StringBuilder();

            //sb.Append("<div class='pagination'>");

            // 하나이상 존재하는 경우
            if (totalCount > 0)
            {
                int curBlock = (intCurPage - 1) / blockSize + 1;	// 현재 페이지가 있는 페이지 블럭
                int curBlockFirst = ((curBlock - 1) * blockSize) + 1;	// 현재 페이지 블럭의 첫번째 페이지
                int curBlockLast = (curBlock * blockSize);	// 현재 페이지 블럭의 마지막 페이지

                // 이전 페이지 블럭이 존재하는 경우 [이전]링크를 걸어준다
                if (intCurPage > blockSize)
                {
                    sb
                    .Append("<a href='?" + param + "&page=1&pageSize=" + itemPerPage + "'>맨앞</a> ")
                    .Append("<a href='?" + param + "&page=" + (curBlockFirst - 1) + "&pageSize=" + itemPerPage + "'>이전</a> ");
                }
                else
                {
                    if (intCurPage > 1)
                        sb.Append("<a href='?" + param + "&page=1&pageSize=" + itemPerPage + "'>맨앞</a> <span class='disabledPage'>이전</span> ");
                    else
                        sb.Append("<span class='disabledPage'>맨앞</span> <span class='disabledPage'>이전</span>");
                }

                // 페이지 번호
                for (int i = curBlockFirst; i <= curBlockLast; i++)
                {
                    if (i > totalPage)
                    {
                        break;
                    }

                    if (intCurPage == i)
                    {
                        sb.Append("<span class='currentPage'>" + i + "</span> ");
                    }
                    else
                    {
                        sb
                        .Append("<a href='?").Append(param).Append("&page=" + i + "").Append("&pageSize=").Append(itemPerPage).Append("'>")
                        .Append(i)
                        .Append("</a> ");
                    }

                    if (i == curBlockLast || i == totalPage)
                    {
                        sb.Append("");
                    }
                    else
                    {
                        sb.Append("");
                    }
                }

                if (curBlockLast < totalPage)
                {
                    sb
                    .Append("<a href='?" + param + "&page=" + (curBlockLast + 1) + "&pageSize=" + itemPerPage + "' class='next'>다음</a> ")
                    .Append("<a href='?" + param + "&page=" + totalPage + "&pageSize=" + itemPerPage + "' class='next_end'>맨뒤</a> ");
                }
                else
                {
                    if(intCurPage<totalPage)
                        sb.Append("<span class='disabledPage'>다음</span> <a href='?" + param + "&page=" + totalPage + "&pageSize=" + itemPerPage + "'>맨뒤</a> ");
                    else
                        sb.Append("<span class='disabledPage'>다음</span> <span class='disabledPage'>맨뒤</span> ");
                }
            }
            else
            { // if end
                //데이터가 없는경우
            }

            //sb.Append("</div>");

            return sb.ToString();
        }

        public static int IsCharPos(string str)
        {
            int nCount = 0;
            foreach (char ch in str)
            {
                if (!('0'.CompareTo(ch) <= 0 && '9'.CompareTo(ch) >= 0))
                    return nCount;

                nCount++;
            }

            return -1;
        }

        public static bool IsNumeric(string value)
        {
            foreach (char _char in value)
            {
                if (!Char.IsNumber(_char))
                    return false;
            }
            return true;
        }

        public static bool IsNumber(string str)
        {
            foreach (char ch in str)
            {
                if (!('0'.CompareTo(ch) <= 0 && '9'.CompareTo(ch) >= 0))
                {
                    if ('.'.CompareTo(ch) != 0)
                        return false;
                }
            }

            return true;

            /*
            decimal val = 0;

            return decimal.TryParse(str, out val);
             */
        }

        public static bool IsInArray(string value, string[] clsPIDs)
        {
            foreach (string id in clsPIDs)
            {
                if (value.Equals(id))
                    return true;
            }

            return false;
        }

        public static bool IsValidEmail(string email)
        {
            if (Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                return true;
            else
                return false;
        }

        public static bool IsValidPhoneNum(string phoneNum)
        {
            //@"((\(0\d\d\) |(\(0\d{3}\) )?\d )?\d\d \d\d \d\d|\(0\d{4}\) \d \d\d-\d\d?)"
            //@"(\d{3}).*(\d{3}).*(\d{4})"
            if (Regex.IsMatch(phoneNum, @"^[0-9]{2,4}-?[0-9]{3,4}-?[0-9]{4}$"))
                return true;
            else
                return false;
        }

        public static string toSurrogate(String hex)
        {
            int val = Convert.ToInt32(hex, 16);
            StringBuilder buf = new StringBuilder();

            if (val >= 0x10000)
            {
                buf.Append((char)((val - 0x10000) / 0x400 + 0xD800));
                buf.Append((char)((val - 0x10000) % 0x400 + 0xDC00));
            }
            else
            {
                buf.Append(val);
            }
            return buf.ToString();
        }

        public static string replaceK2HL(string strOrg, string strFind)
        {
            strFind = strFind.Trim();
            if (strFind.Length <= 0)
                return strOrg;

            return strOrg.Replace(strFind, "<span class='k2hl'>" + strFind + "</span>");
        }

     }

}
/*

	private String sqlFilter(String str) {
		String str1="";
		String str2="";
		
		//out.println(str);
		if(str==null)
			return str;
			
		str = str.replaceAll("<script","");
		str = str.replaceAll("<iframe","");
			
//		str1 = sqlInjectFilter (str);
//		str2 = clearXSS (str1,"");

		return (str);
	}

	//공격 위험성이 존재하는 문자들을 필터링
	//입력값: SQL 입력값
	private String sqlInjectFilter(String str) {
		//out.println(str);
		str = str.replaceAll("[']","''");
		
		str = str.replaceAll("[\"]","\"\"");
	//	str = str.replaceAll("[\\]","\\\\");
		str = str.replaceAll("[;]","");
		str = str.replaceAll("[#]","");
		str = str.replaceAll("[--]","");
		//str = str.replaceAll("[ ]","");
		
		
		return (str);
	}
    

	//XSS 필터 함수
	//$str - 필터링할 출력값
	//$avatag - 허용할 태그 리스트 예)  $avatag = "p,br"
	private String clearXSS(String str, String avatag) {
		str = str.replaceAll("<","&lt;");
		str = str.replaceAll("\0","");
		
		//허용할 태그를 지정할 경우
		if (!avatag.equals("")) {
			//avatag.replaceAll(" ",""); //kys kristal검색에 문제발생
			
			String [] st = avatag.split(",");
		
			//허용할 태그를 존재 여부를 검사하여 원상태로 변환
			for(int x = 0; x < st.length; x++ ) {
				str = str.replaceAll("&lt;"+st[x]+" ", "<"+st[x]+" ");
				str = str.replaceAll("&lt;"+st[x]+">", "<"+st[x]+">");
				str = str.replaceAll("&lt;/"+st[x], "</"+st[x]);
			}
	    	}
		
		return (str);
	}
*/