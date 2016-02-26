# ExoMail
An SMTP server library written in C#

This is my first project involving some serious network code. The ultimate goal is to create an extensible SMTP library
that is RFC compliant and can be used in multiple other projects I would like to start making down the road.  

Some ideas for this library that I am considering developing this library for are a spam filtering gateway, an SMTP load 
balancing proxy, a full email server solution, email archiving, and Possibly more.

In its current state it can receive messages from another SMTP server or client and store them in file MessagStore. 

TODO: 
    Authentication
    Session Validation
    Remote network validation
    Extend SMTP Server types, ie. gateway mode, proxy mode, relay mode, delivery mode
    Error event handeling/logging
    Much more
