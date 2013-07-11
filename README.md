# Bookboon SDK for Microsoft .NET

This project provides an SDK for working with the Bookboon.com API. It can be used in either public or authenticated mode, where the latter requires a key. Because this is just a wrapper class you should familiarize yourself with the [Bookboon.com API](https://github.com/bookboon/api) before using it.

## Usage

### Public usage

	var bookboon = new BookboonClient();
	var categories = bookboon.Get('/categories');

	foreach (var category in await categories)
	{
		Console.WriteLine(category.name);
	}
	
### Authenticated usage

	var bookboon = new BookboonClient();
	var handle = new AuthenticationHandle("secretapikey", "userid");
	var recommendations = bookboon.Get('/recommendations', handle);
	
	foreach (var book in await recommendations)
	{
		Console.WriteLine(book.title);
	}

### Variables

To pass variables to the API, pass an anonymously typed object along with the request:
	
	var parameters = new { email = "test@example.com", newsletter = false };

	bookboon.Post("/profile", handle, parameters);

Use the array initializer if you need to pass multiple parameters with the same name:
	
	var paramters = new
	{
		answer = new []
		{
			"6230e12c-68d8-45d5-8f02-1d3997713150",
			"5aca0fe1-0d93-41b1-8691-aa242a526f17"
		}
	};

	bookboon.Post("/questions", handle, parameters);

> **Note:** You need to specify whether to pass variables using POST or GET methods, by using either the `Get()` or `Post()` methods. Both methods share the same signature.

### Result

The results is a `dynamic` object containing the decoded JSON response.

### Exceptions

The wrapper will throw an exception if API responds with an error code. You may wish to catch these errors, like so:

	try
	{
		await bookboon.Get("/recommendations", handle);
	} 
	catch (ApiException ex)
	{
	    if (ex.ErrorCode == "ApiKeyInvalid")
			Console.Write("ooops");
	}
