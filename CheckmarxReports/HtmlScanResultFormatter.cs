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
    public class HtmlScanResultFormatter: IReportResultFormatter<ScanResult>
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
        /// <param name="server">
        /// The Checkmarx server the report was run on. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="username">
        /// The user the report was run by. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/> and <paramref name="username"/> cannot be null, empty or whitespace.
        /// </exception>
        public void Format(IList<ScanResult> reportResults, TextWriter output, string server, string username)
        {
            if (reportResults == null)
            {
                throw new ArgumentNullException(nameof(reportResults));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(server));
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(username));
            }

            using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(output))
            {
                htmlTextWriter.WriteLine("<!DOCTYPE html>");
                htmlTextWriter.AddAttribute("lang", "en");
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Html);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Head);
                WriteBootstrapHeaders(htmlTextWriter);
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Body);
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "container-fluid");
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Div);

                // Header
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.H1);
                htmlTextWriter.WriteEncodedText("Not False Positive Checkmarx Results");
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.WriteEncodedText($"Generated at {DateTime.Now} on {server} by {username}");
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Br);
                htmlTextWriter.RenderEndTag();

                // Results table
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "table table-striped");
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Table);
                WriteHeader(htmlTextWriter);
                htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tbody);
                foreach (ScanResult scanResult in reportResults.OrderBy(sr => sr.ProjectName).ThenBy(sr => sr.RuleName))
                {
                    WriteRow(htmlTextWriter, scanResult);
                }
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderEndTag();

                htmlTextWriter.RenderEndTag();
                WriteBootstrapScripts(htmlTextWriter);
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.Flush();
            }
        }

        private void WriteBootstrapScripts(HtmlTextWriter htmlTextWriter)
        {
            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Src, "https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js");
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Script);
            htmlTextWriter.RenderEndTag();

            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Src, "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js");
            htmlTextWriter.AddAttribute("integrity", "sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS");
            htmlTextWriter.AddAttribute("crossorigin", "anonymous");
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Script);
            htmlTextWriter.RenderEndTag();
        }

        private void WriteBootstrapHeaders(HtmlTextWriter htmlTextWriter)
        {
            // From http://getbootstrap.com/getting-started/

            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Href, "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css");
            htmlTextWriter.AddAttribute("integrity", "sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7");
            htmlTextWriter.AddAttribute("crossorigin", "anonymous");
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Link);
            htmlTextWriter.RenderEndTag();

            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Href, "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap-theme.min.css");
            htmlTextWriter.AddAttribute("integrity", "sha384-fLW2N01lMqjakBkx3l/M9EahuwpSfeNvV63J5ezn3uZzapT0u7EYsXMjQV+0En5r");
            htmlTextWriter.AddAttribute("crossorigin", "anonymous");
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Link);
            htmlTextWriter.RenderEndTag();

            // From http://getbootstrap.com/css/

            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Name, "viewport");
            htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Content, "width=device-width, initial-scale=1");
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Meta);
            htmlTextWriter.RenderEndTag();
        }

        private void WriteHeader(HtmlTextWriter htmlTextWriter)
        {
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Thead);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            htmlTextWriter.Write("New");
            htmlTextWriter.RenderEndTag();
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
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
        }

        private void WriteRow(HtmlTextWriter htmlTextWriter, ScanResult scanResult)
        {
            if (scanResult.Severity == Severity.High)
            {
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "danger");
            }
            else if (scanResult.Severity == Severity.Medium)
            {
                htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Class, "warning");
            }

            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlTextWriter.Write(scanResult.Status == Status.New ? "New" : "");
            htmlTextWriter.RenderEndTag();
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
            htmlTextWriter.WriteEncodedText($"{scanResult.FilePath} ({scanResult.Line})");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
        }
    }
}
