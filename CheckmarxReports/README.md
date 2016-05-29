# To cover

* What? Checkmarx is a SAST (url). Remembers scan results from one run to the next. Can mark scan results as false positives (by changing the state from "To confirm" to "Not vulnerable").
* Why? Checkmarx UI build around decentralized team model. No easy way to get latest scans across teams.
* Overview: Design
	* Uses Checkmarx SOAP API (https://checkmarx.atlassian.net/wiki/display/KC/SOAP+API)
* Known limitations
	* Assumes HTTPS
	* One report only: Not false positive scan results
* Technical
 	* Requires C# 6.0 (uses interpolated strings and `nameof`)
	* (Future) Parallelism in the report generation
	* Limited automated tests
	* Only tested on Visual Studio 2015 on Windows 10, Visual Studio 2012
	* Convert more Checkmarx return values to stronger types, e.g. Severity enum, Status enum
