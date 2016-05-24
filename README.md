# ExoMail
An SMTP server library written in C#

## Synopsis

This is my first project involving some serious network code and trying to implement RFC guidelines. The ultimate goal is to create an extensible SMTP library that is RFC compliant and can be utilized in other projects.  

## Motivation

As a systems administrator I wanted to learn more about how SMTP works I also wanted to learn more about network code. Eventually I want to integrate this into other projects in the future such as a spam filtering gateway, an SMTP load balancing proxy, a full email server solution, email archiving, and Possibly more.

## Current Features

TLS/STARTTLS Support  
Async methods  
LOGIN Authentication  

## TODO

Build a more complete example  
More Documentation  
Network Validation Interfaces
Sender Validation Interfaces
Message Validation Interfaces ie. DKIM, SPF, Mime Validation.

## Contributors

I am not really looking for any contributors at this point in the project but you are welcome to make suggestions or submit pull requests.

## Special Thanks

- Alex Reinert - [ARSoft.Tools.Net](http://arsofttoolsnet.codeplex.com/)  
- Authors of [Microsoft.IO.RecyclableMemoryStream] (https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream)

## License

The MIT License (MIT)

Copyright (c) 2016 Matthew C. Soler  
<msoler.exorealms.com>