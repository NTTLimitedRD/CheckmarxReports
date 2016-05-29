# Custom Checkmarx Report Runner

[Checkmarx](http://checkmarx.com) is a [static analysis](https://www.owasp.org/index.php/Static_Code_Analysis) 
or static application security testing (SAS) product. 

Checkmarx's user interface is designed around creating one or more projects, representing different programs 
being checked, and scan runs that show what security issues were discovered at that time. The usual workflow
is:

1. Checkmarx runs a scan on a project and identifies issues.
1. Each issue is reviewed:
    1. If issue is confirmed, the state is set to "Confirmed" and an ticket raised in the teams bug or project tracking software.
    1. If the issue is a false positive or not exploitable, the state is set to "Not Exploitable".
1. The state is remembered so subsequent scans checking the same source code do not flag the same problem. When the team fixes issues, these are automatically removed from the scan results.

The Checkmarx UI, centered around projects, works well when members of each team are actively consuming 
and fixing Checkmarx issues (a "decentralised" model). However, when a security program is new or team 
members are not security literate, understanding static analysis tool results and differentiating false 
positives from real issues is difficult. Using a security team to monitor and interpret findings can 
address this (a "centralized" model). 

Unfortunately, the Checkmarx UI requires the secruity team to view the results of each project individually 
to see new or unreviewed issues. This tool creates either a HTML or CSV report showing the unreviewed
or confirmed issues form the latest scan across all projects, filling that gap. 

## Use

To generate an HTML report, run the following from a command prompt:

```
checkmarxreports not-false-positives --server <server> --user-name <username> --password <password> --output-file report.html --output-format Html
```

This creates a report in `report.html`. Note that this may take several minutes depending on the number 
and size of projects and who busy the Checkmarx server is.

To generate a CSV file instead, change `--output-format Html` to `--output-format Csv`. This can be useful to futher manipulate 
or format the data differently.

## Limitations

Note:

 1. There is limited control over the report generated. This tool is not meant to provide a broad reporting solution, merely a quick way to get data out of Checkmarx that the UI does not provide. Use the CSV output to get data easily manipulated, such as in a spreadsheet.
 1. The application assumes the Checkmarx server is accessible via HTTPS. After all, we are all security professionals and avoid using HTTP, dont' we?
 1. While the architecture supports multiple reports, only one report ("not false positives") is included at this time.
 1. It has only been tested against Checkmarx v8.0. 

## Technical Details

Note:

 1. The code uses the [Checkmarx SOAP API](https://checkmarx.atlassian.net/wiki/display/KC/SOAP+API) to access Checkmarx data. 
 1. The code runs reports in parallel, up to 3 at a time.
 1. The code requires C# 6.0 as it uses interpolated strings and the `nameof` keyword. It has only been run in Visual Studio 2015 running on Windows 10 and Windows 2012 R2.
 1. There are limited automated tests.
