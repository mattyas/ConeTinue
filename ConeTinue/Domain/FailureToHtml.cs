using System;
using System.IO;
using System.Text;
using ConeTinue.Domain.CrossDomain;

namespace ConeTinue.Domain
{
	public class FailureToHtml
	{
		public static string GetHtml(TestFailure failure, TestFailureStack selectedStackFrame)
		{
			if (failure == null)
				return "<html></html>";
			var stackFrame = selectedStackFrame;
			if (stackFrame == null)
			{
				return GetStackTraceAsHtml(failure);
			}
			string readAllText;
			try
			{
				readAllText = File.ReadAllText(stackFrame.File);
			}
			catch (Exception)
			{
				return GetStackTraceAsHtml(failure);
			}

			string codeAsHtml = new External.XtractPro.Text.CSharpSyntaxHighlighter
				{
					ShowCollapsibleBlocks = false,
					ShowComments = true,
					ShowHyperlinks = false,
					ShowLineNumbers = true,
					LineNumberSpaces = 4,
					ShowRtf = false,
				}.Process(readAllText).Replace("<!--" + (stackFrame.Line) + "-->", "<div id=\"the_error\"><h1>" + failure.TestName + "</h1>" + failure.Message.Replace("\n", "<br />") + "</div>");

			return docTemplate.Replace("[error-id]", stackFrame.Line.ToString()) + codeAsHtml + docTemplateEnd.Replace("[line-before-error]", Math.Max(1, stackFrame.Line - 3).ToString());
		}

		private static string GetStackTraceAsHtml(TestFailure failure)
		{
			var sb = new StringBuilder();
			sb.Append("<html>");
			sb.Append("<h1>");
			sb.Append(failure.TestName);
			sb.Append("</h1>");
			sb.Append(failure.Message);
			sb.Append("<hr />");
			foreach (var stack in failure.StackTrace)
			{
				sb.Append(stack);
				sb.AppendLine("<br />");
			}
			sb.Append("</html>");
			return sb.ToString();
		}

		private const string docTemplate = @"<html>
<head>
    <title>Source</title>
    <style type=""text/css"">
/* All Syntax Highlighters */
div.sh_result { font-size: 10pt; font-family: Courier New, Verdana, Helvetica, Arial, sans-serif;background-color:#ffffff;font-size:90%; }
span.sh_error { color: #ff0000; }
span.sh_line { color: #008284; margin-right: 10px; border-right: 1px solid #008284; }
span.sh_collapsed { color: #848284;border:1px solid #848284; }
span.sh_collapsed:hover { cursor:pointer; }
span.sh_expanded { color: #848284;text-decoration:underline; }
span.sh_expanded:hover { cursor:pointer; }

/* C#/VB.NET Syntax Highlighters */
span.net_key { color: #0000ff; }
span.net_type { color: #008284; }
span.net_directive { color: #6699cc; }
span.net_string, span.net_string a { color: #a31515; }
span.net_comment, span.net_comment a { color: #008200; }
span.net_xml, span.net_xml a { color: #848284; }

/* XML/HTML Syntax Highlighters */
span.xml_elem { color: #a31515; }
span.xml_delim { color: #0000ff; }
span.xml_att { color: #ff0000; }
span.xml_val { color: #0000ff; }
span.xml_text { color: #6699cc; }
span.xml_comment { color: #008200; }
#row_[error-id] { background-color:#ECECEC; 
border: dashed .1em #3E62A6 !important;
}
#the_error {
background-color: #FFF4F4;
border-top: dashed .1em #3E62A6 !important;
padding: 10px 10px 10px 74px;
}

#the_error h1 {
font-size: 12pt; 
font-family: Arial, sans-serif;
font-weight: bold;
margin: 3px;
}</style>
    <script type=""text/javascript"">
function shToggle(elem, blkid)
{
    var blk = document.getElementById(""blk"" + blkid);
    var expanded = (blk.style.display!=""none"");
    blk.style.display = (expanded ? ""none"" : ""inline"");
    elem.className = (expanded ? ""sh_collapsed"" : ""sh_expanded"");
    return false;
}

</script>
</head>
<body>";
		const string docTemplateEnd = @"    <script type=""text/javascript"">
location.href = ""#"";
location.href = ""#row_[line-before-error]"";

</script>
</body></html>";
	}
}