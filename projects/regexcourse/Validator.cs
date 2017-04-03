﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace RegexCourse
{
    [TestClass]
    public class Validator
    {
		public static string ReportPath = "/project/target/";
		public static string ReportTemplate = Path.Combine(ReportPath,"report.html");
		public static string RowReport = @"<tr><td>%name%</td><td><span class=""glyphicon glyphicon-%ok1% text-%ok2%"" aria-hidden=""true""></span></td><td>%match1%</td><td>%match2%</td></tr>";
		
		
		public struct RegexUseCase
        {  
          public string Name;  
          public string Value;  
		  public RegexUseCase(string n, string v){Name = n; Value = v;}
        }  

		public string CreateHTMLMatches(Regex regex, RegexUseCase regexTest)
		{
			string HTMLMatch = "";
            bool[] char_captured = new bool[regexTest.Value.Length];
			//TODO: Add color to matches
            var matches = regex.Matches(regexTest.Value);
            foreach (Match m in matches)
            if (m.Success)
            {
                for (int i = 0; i < m.Length;++i )
                {
                    char_captured[m.Index + i] = true;
                }
            }

            if (char_captured[0]) HTMLMatch += "<span class='green-highlight'>";
            HTMLMatch += char_captured[0];
            for (int i = 1; i < regexTest.Value.Length;++i )
            {
                if ( char_captured[i - 1] && !char_captured[i]) HTMLMatch += "</span>";
                if (!char_captured[i - 1] &&  char_captured[i]) HTMLMatch += "<span class='green-highlight'>";
                HTMLMatch += regexTest.Value[i];
            }
            if (char_captured[regexTest.Value.Length - 1]) HTMLMatch += "</span>";
//                HTMLMatch = regexTest.Value;
			return HTMLMatch;
		}
		
        public bool VerifyMatches(string ReportName,string Title_Report,string RefPattern, string UserPattern, List<RegexUseCase> regexcases)
        {
			bool UnitTestOK = true;			
			string path = Path.Combine(ReportPath,ReportName);
            int countCorrect = 0;
			int percentage = 0;
			

            Regex Refregex = new Regex(RefPattern);
			Regex Userregex = new Regex(UserPattern);

			string contents = File.ReadAllText(ReportTemplate);

			contents = contents.Replace("%REPORT_NAME%",Title_Report); //Set Title
			
			string rowreport = "";
			foreach (var regexTest in regexcases)
			{
			  string Ref_char_captured = CreateHTMLMatches(Refregex,regexTest);
			  string User_char_captured = CreateHTMLMatches(Userregex,regexTest);
			  bool isCorrect = (Ref_char_captured == User_char_captured);

                // Temporal
                     Match refM = Refregex.Match(regexTest.Value);
                     Match userM = Userregex.Match(regexTest.Value);
                     isCorrect = refM.Success == userM.Success;
                //Temporal
                     if (isCorrect) ++countCorrect;
			  UnitTestOK = UnitTestOK && isCorrect;
			  rowreport += RowReport.Replace("%name%",regexTest.Name).Replace("%ok1%",(isCorrect?"ok":"remove")).Replace("%ok2%",(isCorrect?"success":"danger"))
			                        .Replace("%match1%",User_char_captured).Replace("%match2%",Ref_char_captured)+"\r\n";
			}
            if (countCorrect == regexcases.Count) //100%
               contents = contents.Replace("%globalresult%", "success");
            else if (countCorrect == 0) //0%
                contents = contents.Replace("%globalresult%", "danger");
            else contents = contents.Replace("%globalresult%", "warning");

            percentage = 100 * countCorrect / regexcases.Count;
            contents = contents.Replace("22", "" + percentage); //Set percentage

			contents = contents.Replace("%report_body%",rowreport);
			File.WriteAllText (path, contents);
            Console.WriteLine("CG> message Solved: "+countCorrect+"/"+regexcases.Count+" (" + percentage + "%). Report is:" + path + " Size:" + new System.IO.FileInfo(path).Length);
            Console.WriteLine("CG> open --static-dir "+ReportPath+" /" + ReportName);
			return UnitTestOK;
        }
		

        [TestMethod]
        public void VerifyExercise1()
        {
            string RefPattern = @"[aeiouAEIOU]";
			string UserPattern = Exercise1.Pattern_MatchVowels;
			List<RegexUseCase> regexcases = new List<RegexUseCase>();
			regexcases.Add( new RegexUseCase("Simple a","a"));
			regexcases.Add( new RegexUseCase("Simple e","e"));
			regexcases.Add( new RegexUseCase("Simple i","i"));
			regexcases.Add( new RegexUseCase("Simple o","o"));
			regexcases.Add( new RegexUseCase("Simple u","u"));
			regexcases.Add( new RegexUseCase("A with z","ZzzzzzzAzzzzzzZ"));
			regexcases.Add( new RegexUseCase("E with numbers","23124415235E5"));
			regexcases.Add( new RegexUseCase("I with numbers","452345I31234"));
			regexcases.Add( new RegexUseCase("O with zeros","0000O00O0OO0OO0"));
			regexcases.Add( new RegexUseCase("U with symbols","$)(=U:;_+-*/"));
			regexcases.Add( new RegexUseCase("Numbers","01235467689"));
			regexcases.Add( new RegexUseCase("Consonants","bCdFgHjKlMnPqRsTvWxYz"));
			regexcases.Add( new RegexUseCase("Alphabet","AbCdEfGhIjKlMnOpQrStUvWxYz"));
			regexcases.Add( new RegexUseCase("Lorem Ipsum","Lorem Ipsum dolor sit amet, consectetur adipiscing elit."));
			Assert.IsTrue(VerifyMatches("exercise1.html","Exercise 1 - Match vowels",RefPattern,UserPattern,regexcases));
        }
    }
}
