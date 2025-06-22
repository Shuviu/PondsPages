using System.Net;

namespace PondsPages.Core.Tests.api_handlers;

public class HttpMessageHandlerMock : HttpMessageHandler
{
    private readonly HttpResponseMessage? _response;
    private readonly HttpStatusCode _code;

    public HttpMessageHandlerMock(HttpStatusCode code)
    {
        _code = code;  
    } 
    
    public HttpMessageHandlerMock(HttpResponseMessage response)
    {
        _response = response;
        _code = response.StatusCode;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _response != null ? Task.FromResult(_response) : Task.FromResult(new HttpResponseMessage(this._code));
    }
}