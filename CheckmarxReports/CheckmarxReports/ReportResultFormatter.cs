using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports
{
    /// <summary>
    /// Convert the report results from a list of <see cref="ScanResult"/> human readable results 
    /// in the given <see cref="TextWriter"/>.
    /// </summary>
    public class ReportResultFormatter
    {
        /// <summary>
        /// Format the report results.
        /// </summary>
        /// <param name="reportResults">
        /// The report results. This cannot be null.
        /// </param>
        /// <param name="output">
        /// The <see cref="TextWriter"/> to write the results to. This cannot be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        public void Format(IList<ScanResult> reportResults, TextWriter output)
        {
            if (reportResults == null)
            {
                throw new ArgumentNullException(nameof(reportResults));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(output))
            {
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Html);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Body);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Table);
                WriteHeader(htmlTextWriter);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tbody);
                foreach (ScanResult scanResult in reportResults)
                {
                    WriteRow(htmlTextWriter, scanResult);
                }
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.Flush();
            }
        }

        private void WriteHeader(HtmlTextWriter htmlTextWriter)
        {
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Thead);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("Project");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("Rule");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("Severity");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("Location");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("Status");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
        }

        private void WriteRow(HtmlTextWriter htmlTextWriter, ScanResult scanResult)
        {
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.Write(scanResult.ProjectName);
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.Write(scanResult.RuleName);
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.Write(scanResult.Severity);
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Href, scanResult.DeepLink.ToString());
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.A);
            htmlTextWriter.Write($"{scanResult.FilePath} ({scanResult.Line})");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.Write(scanResult.Status);
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
        }
    }
}
