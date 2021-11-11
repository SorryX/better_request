# better_request
A simple library I made in about 2-3 hours.

It's not faster but even worse it's slower but this is built for security & customization
which basically means that this library gives you more data about the response and gives you the opportunity to choose what to request.

It's not made for basic API requests, go use RestAPI for that.
bettter_request is made for more safer requests e.g Login & Registration

## Usage

Currently only supporting **Get, Post**

### Get Requests

```csharp
using System;
using static better_webclient.better_webclient;

namespace better_webclient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Creating a better_client instance
            var client = new better_webclient();

            // Creating a request class an asigning a few values
            var requestData = new Request
            {
                // Address is basically the url, where you want this request to go
                Address = "https://pastebin.com/raw/s9EPhhfG",
                
                // If turned on this will throw an error if the status code isn't success (200)
                EnsureSuccessStatusCode = true,
            };

            // Adding headers is really easy ( Need a few changes tho )
            requestData.Headers.Add(new Query("Authorization", "test"));

            // Lastly printing out the content
            Console.WriteLine(client.Get(requestData).Content);
        }
    }
}
```

## Explanation

This is my first time using summary comments and asynchronous functions but I am learning and improving my code.

## Features

- Has an easy way to send requests
- Gives you a lot of information about the response
- Let's you choose what you want in the request
- It's just a class meaning you just put it in your project and you are good to go!
- Soon ( Has a safe way to request with miltary-grade encryption with an good algorithm to stop people from modifying the request )
  -  This will require modification from your end.
  -  Will only work on your own API

