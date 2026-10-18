using System;
using VoiceML;
using VoiceML.Exceptions;
using VoiceML.Models;

var accountSid = Environment.GetEnvironmentVariable("VOICEML_ACCOUNT_SID")
    ?? throw new InvalidOperationException("Set VOICEML_ACCOUNT_SID");
var apiKey = Environment.GetEnvironmentVariable("VOICEML_API_KEY")
    ?? throw new InvalidOperationException("Set VOICEML_API_KEY");

// Keep this client for the application lifetime; dispose it at shutdown.
using var client = new VoiceMLClient(new ClientOptions
{
    AccountSid = accountSid,
    ApiKey = apiKey,
    Timeout = TimeSpan.FromSeconds(30),
});

try
{
    var call = await client.Calls.CreateAsync(new CreateCallRequest
    {
        To = "+14155550101",
        From = "+14155550100",
        Url = "https://example.com/answer.xml",
    });
    Console.WriteLine(call.Sid);

    await foreach (var completed in client.Calls.IterateAsync(
        new ListCallsParams { Status = "completed", PageSize = 50 }))
    {
        Console.WriteLine(completed.Sid);
        break; // No later page is fetched after the consumer stops.
    }
}
catch (VoiceMLTimeoutException error)
{
    Console.Error.WriteLine(error.Message);
}
catch (ApiException error)
{
    Console.Error.WriteLine($"HTTP {error.StatusCode}; code {error.Code}: {error.Message}");
}
