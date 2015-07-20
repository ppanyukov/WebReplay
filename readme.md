# WebReplay

*NOTE: This is Windows-only (but may run on Mono)*


This is a little simple performance benchmarker / load tool for HTTP things (sites, web api etc)
similar to `Apache ab`. Essentially it fires off HTTP requests, measures how long it
took to get the responses and produces a summary report at the end.

I've created this to overcome some of the limitations of `ab` and add features I needed.
In particular:

- Replay URLs from a pre-defined file.
- Configure HTTP headers for groups of URLs.
- Report results in CSV format with columns that I need.
- Optionally calculate MD5 checksums of the responses.

## How to use

1. Create one or more "replay" files.
2. Run the tool.
3. See the results.


## The replay file

This looks like this:

(NOTE how you can use environmental variables everywhere and they will get replaced at runtime).

	{
	    "baseuri": "http://%COMPUTERNAME%", 
	    "headers": {
	        "Connection": "Keep-Alive", 
	        "Accept-Language": "en-GB,en;q=0.5", 
	        "Accept-Encoding": "gzip, deflate", 
	        "Pragma": "no-cache", 
	        "Cookie": "cookie1=value1; cookie2=value2", 
	        "Accept": "text/html, application/xhtml+xml, */*", 
	        "User-Agent": "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko"
	    }, 
	    "name": "short name, not currently used anywhere",
	    "description": "Give a good description, it will show in the reports",
	    "uris": [
	        "/you can use environmental variables anywhere like this %DIR%/bar", 
	        "/zoo/lala?whatever"
	    ]
	}


## Running the tool

Command-line options are:


	  -c, --concurrency    (Default: 1) The number of concurrent requests for each
	                       URI.
	  -i, --iterations     (Default: 1) The number of iterations for each for each
	                       URI.
	  -o, --out            The path to the output file. Optional.
	  --checksum           (Default: False) An indicator whether to caluclate MD5
	                       checksum of the reponse bodies. Will affect metrics like
	                       requests/sec.
	  --bodysize           (Default: False) An indicator whether to caluclate
	                       reponse body sizes. May affect metrics like requests/sec.
	  -r, --replayfile     Required. The path to the replay file. Can contain
	                       wildcard in the name of the file e.g. c:\temp\*.json.
	                       Required.
	  --help               Display this help screen.


Example:

	# Replay URIs in all json files in ReplayFiles directory.
	# Run as 5 concurrent users
	# Do 100 iterations of each URI for each user
	WebReplay.exe -c 5 -i 100 -r ReplayFiles\*.json

## Output

The output is in CSV format with these columns:

| Column              | Description                                                          |
|---------------------|----------------------------------------------------------------------|
| Sequence            | the sequence number of the request                                   |
| Iterations          | the number of iterations as per the -i parameter                     |
| Cuncurrent Requests | the number of concurrent users as per -c parameter                   |
| Status Code         | the last seen HTTP status code of the response                       |
| Response Size       | the response body size of the last seen HTTP response (if available) |
| Response Checksum   | the MD5 checksum of the last see HTTP response's body (if available) |
| Req/Sec             | recorded requests per second we managed to do                        |
| min                 | minimum response time                                                |
| perc50              | median response time                                                 |
| perc75              | 75th percentile response time                                        |
| perc90              | 90th percentile response time                                        |
| perc95              | 95th percentile response time                                        |
| max                 | maximum response time                                                |
| description         | description as in JSON replay file                                   |
| baseUri             | base URI as in JSON replay file                                      |
| uri                 | the URI as in JSON replay file                                       |



## Nuget package & other enhancements

Maybe later, unless someone does a pull request. :)





