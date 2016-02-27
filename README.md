# ExoMail
An SMTP server library written in C#

This is my first project involving some serious network code. The ultimate goal is to create an extensible SMTP library
that is RFC compliant and can be used in multiple other projects I would like to start making down the road.  

Some ideas for this library that I am considering during development of this project are a spam filtering gateway, an SMTP load 
balancing proxy, a full email server solution, email archiving, and Possibly more.

In its current state it can receive messages from another SMTP server or client and store them in file MessageStore. The example project shows some of the currently implemented features and listens on port 2525, 465, and 587 for SMTP connections.

Currently implemented:
    Multiple server instances listening on different ports.
    JSON based server configuration
    Commands: HELO/EHLO, MAIL, RCPT, STARTTLS, HELP, RSET, QUIT, DATA
    SSL/TLS Connections
    STARTTLS Connections

TODO: 
    Authentication
    Sender Validation
    Recipient Validation
    Remote network validation, ie. DNSRBL Integration, Authorized Relay networks
    Extend SMTP Server types, ie. gateway mode, proxy mode, relay mode, delivery mode
    Error event handeling/logging
    Much more
