# ExoMail
An SMTP server component written in C#

## Synopsis

This is my first project involving some serious network code and trying to implement RFC guidelines. The ultimate goal is to create an extensible SMTP server component that is RFC compliant and can be utilized in other projects. Functionality is implemented through different public interfaces. 

## Motivation

As a systems administrator I wanted to learn more about how SMTP works I also wanted to learn more about network code. Eventually I want to integrate this into other projects in the future such as a spam filtering gateway, an SMTP load balancing proxy, a full email server solution, email archiving, and Possibly more.

## Current Features

Async methods  
Authentication support  
Extensible UserStore  
Extensible ServerConfig  
Extensible SaslMechanism  
Extensible MessagStore  
UserManagement  

## Implemented Commands

HELO, EHLO, MAIL, RCPT, SIZE, DATA, HELP, AUTH, NOOP, QUIT, RSET

## Implemented Extensions

STARTTLS, 8BITMIME, PIPELINING

## Example

An example project is included to get you started.  
Here are some snippets on how to do a minimal implementation:  

```csharp
public async Task StartListeningAsync()
{
	// Build the config.
	var config = MemoryConfig.Create()
					.WithHostname("exomail01.example.com")
					.WithPort(2525)
					.WithServerId("FC30BD4D-1C93-4FBF-BF8F-9788059AF0DC")
					.WithSessionTimeout(5 * 60 * 1000) // 5 minutes
					.WithX509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localhost.pfx"), null)
					//.WithEncryptionRequired()
					.WithStartTlsSupported()
					.WithAuthRelayAllowed();

	// Create the MessageStore
	var messageStore = new FileMessageStore();

	// Create the UserStore
	var userStore = new TestUserStore();

	// Create a test userstore repository.
	for (int i = 0; i < 5; i++)
	{
		userStore.AddUser(
			EmailUser.CreateMailbox(
				"Test", 
				"User0" + i.ToString(), 
				"user0" + i.ToString() + "@example.net")
				);
	}

	// Add UserStore to the UserManager
	UserManager.GetUserManager.AddUserStore(userStore);

	// Create the server
	SmtpServer server = new SmtpServer(config, messageStore);

	await server.StartAsync(this._token);
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
	private List<IUserIdentity> _users { get; set; }

	public TestUserStore()
	{
		this._users = new List<IUserIdentity>();
	}

	public string Domain
	{
		get
		{
			return "example.net";
		}
	}

	public void AddUser(IUserIdentity userIdentity)
	{
		this._users.Add(userIdentity);
	}

	public List<IUserIdentity> GetIdentities()
	{
		return this._users;
	}

	public bool IsUserAuthenticated(string userName, string password)
	{
		userName = userName.ToUpper();
		var user = this._users.FirstOrDefault(u => u.UserName.ToUpper() == userName);
		if (user == null)
			return false;

		// In a real world implementation this would compare a hashed version of the password.
		return user.Password == password;
	}

	public bool IsValidRecipient(string emailAddress)
	{
		emailAddress = emailAddress.ToUpper();

		return _users.Any(u => u.EmailAddress.ToUpper() == emailAddress);
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