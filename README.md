# ExoMail
An SMTP server library written in C#

## Synopsis

This is my first project involving some serious network code and trying to implement RFC guidelines. The ultimate goal is to create an extensible SMTP library that is RFC compliant and can be utilized in other projects.  

## Motivation

As a systems administrator I wanted to learn more about how SMTP works I also wanted to learn more about network code. Eventually I want to integrate this into other projects in the future such as a spam filtering gateway, an SMTP load balancing proxy, a full email server solution, email archiving, and Possibly more.

## Current Features

TLS/SSL Support
STARTTLS Support
Async methods
Authentication support
Extensible UserStore
Extensible ServerConfig
Extensible SaslMechanism
Extensible MessagStore
UserManagement

## Implemented Commands

HELO, EHLO, MAIL, RCPT, DATA, HELP, STARTTLS, AUTH, NOOP, QUIT, RSET

## Example

An example project is included to get you started.  
Here are some snippets on how to do a minimal implementation:  

```csharp
public class AppStart 
{
	public async Task StartListeningAsync(CancellationToken token)
	{
		// Build the config.
		var config = MemoryConfig.Create();

		// Create the MessageStore
		var messageStore = new FileMessageStore();

		// Create the UserStore
		var userStore = new TestUserStore();

		// Add UserStore to the UserManager
		UserManager.GetUserManager.AddUserStore(userStore);

		// Create the server
		SmtpServer server = new SmtpServer(config, messageStore);

		await server.StartAsync(this.token);
	}
}

public class FileMessageStore : IMessageStore
{
	public async Task Save(
		MemoryStream memoryStream, 
		SmtpReceivedHeader receivedHeader, 
		IMessageEnvelope messageEnvelope
		)
	{
		var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queue");
		var path = Path.Combine(directory, Guid.NewGuid().ToString() + ".eml");

		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		using (var stream = new FileStream(path, FileMode.Create))
		{
			var headers = await receivedHeader.GetReceivedHeaders();

			await headers.CopyToAsync(stream);
			await memoryStream.CopyToAsync(stream);
		}

		messageEnvelope.SaveEnvelope(path);
	}
}

public class TestUserStore : IUserStore
{
	public string Domain
	{
		get
		{
			return "example.net";
		}
	}

	public void AddUser(IUserIdentity userIdentity)
	{
		throw new NotImplementedException();
	}

	public List<IUserIdentity> GetIdentities()
	{
		throw new NotImplementedException();
	}

	public bool IsUserAuthenticated(string userName, string password)
	{
		return userName.ToUpper() == "TUSER" && password == "Str0ngP@$$!!";
	}

	public bool IsValidRecipient(string emailAddress)
	{
		return emailAddress.Contains(this.Domain);
	}
}
```

## TODO

Build a more complete example  
Add more unit tests  
More Documentation  
Network Validation Interfaces  
Sender Validation Interfaces  
Message Validation Interfaces ie. DKIM, SPF, Mime Validation.  

## Contributors

You are welcome to make suggestions or submit pull requests.

## Special Thanks

- Alex Reinert - [ARSoft.Tools.Net](http://arsofttoolsnet.codeplex.com/)  
- Authors of [Microsoft.IO.RecyclableMemoryStream] (https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream)

## License

The MIT License (MIT)

Copyright (c) 2016 Matthew C. Soler  
<msoler.exorealms.com>