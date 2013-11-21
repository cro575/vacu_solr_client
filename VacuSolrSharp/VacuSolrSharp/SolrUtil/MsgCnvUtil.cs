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
    public class MsgCnvUtil
    {
        public static string toCnv(string cate, string val)
        {
            if (String.IsNullOrEmpty(val)) return "";

            switch (cate)
            {
            case "todae":
                switch (val)
                {
                        case "section_area": return "분야";
                        case "section_type": return "유형";
                        case "section_period": return "시기";
                        case "section_name": return "섹션";
                        case "extendinfo": return "확장정보";
                        case "period": return "시기";

                        case "maintitle": return "대표표제어";
                        case "title_k": return "한글표제어";
                        case "title_c": return "한자표제어";
                        case "content": return "본문";
                        case "explanation": return "설명문";
                        case "keywords": return "키워드";
                        case "mainimage": return "대표이미지";
                        case "url": return "바로가기";
                        case "id": return "통합메타ID";
                        case "taskid": return "과제ID";
                        case "productid": return "산출물ID";

                        case "maintitle asc": return "대표표제어";
                        case "section_name asc": return "섹션";

                        case "maintitle_s asc": return "주제목";
                        case "maintitle_s desc": return "주제목(역순)";
                        case "keywords_s asc": return "키워드";
                        case "keywords_s desc": return "키워드(역순)";
                        case "period asc": return "시기";
                        case "period desc": return "시기(역순)";

                }
                break;
            }
            return val;
        }
    }
}
