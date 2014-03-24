# Custom error handling for Nancyfx applications

This work is *heavily* influenced by Paul Stovell's excellent [Consistent error handling with Nancy](http://paulstovell.com/blog/consistent-error-handling-with-nancy) article.

## Features

- Simple setup of custom views for error pages, 404 pages and authorization/authentication failure pages
- Supports any Nancy view engine
- Will send JSON serialized representation of errors if client has requested JSON
- Easily add support for custom error handling logic or for mapping application specific exception types to appropriate error responses

## Installation

## Usage

### Bootstrapping

Custom error handling is set up in the ApplicationStartup method of your Nancy Bootstrapper. In its simplest form, your simply call Nancy.CustomErrors.Enable, passing an IPipelines instance.

``` c#
protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
{
	base.ApplicationStartup(container, pipelines);

	// ...
	CustomErrors.Enable(pipelines);
}
```

### Error views

### Configuration

### Custom error handling logic

