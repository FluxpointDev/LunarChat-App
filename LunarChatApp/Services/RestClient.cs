using LunarChatApp.Shared.Rest;
using LunarChatApp.Shared.Rest.Optional;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LunarChatApp.Services;

public class RestClient
{
    public RestClient()
    {
        HttpClientHandler ClientHandler = new HttpClientHandler()
        {
            //UseProxy = Client.Config.RestProxy != null
        };
        //ClientHandler.Proxy = Client.Config.RestProxy;

        Http = new HttpClient(ClientHandler)
        {
            BaseAddress = new Uri(Url)
        };
        Http.DefaultRequestHeaders.Add("User-Agent", "LunarChatClient");
        Http.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public static JsonSerializer Serializer { get; internal set; } = new JsonSerializer
    {
        ContractResolver = new OptionalContractResolver()
    };

    public static JsonSerializer SerializerPretty { get; internal set; } = new JsonSerializer
    {
        ContractResolver = new OptionalContractResolver(),
        Formatting = Formatting.Indented
    };

    public static JsonSerializer Deserializer { get; internal set; } = CreateDes();

    internal static JsonSerializer CreateDes()
    {
        JsonSerializer des = new JsonSerializer();
        des.Converters.Add(new OptionalDeserializerConverter());
        return des;
    }

    public string Url = "https://localhost:7216/";
    public HttpClient Http = new HttpClient();

    public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, ILunarRequest? json = null)
    => InternalRequest(GetMethod(method), endpoint, json);

    internal Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, object> json) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);

    internal Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, string> json) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);

    internal Task<TResponse?> GetAsync<TResponse>(string endpoint, ILunarRequest json = null, bool throwGetRequest = false) where TResponse : class
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        => SendRequestAsync<TResponse>(RequestType.Get, endpoint, json, throwGetRequest);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    internal Task DeleteAsync(string endpoint, ILunarRequest json = null)
        => SendRequestAsync(RequestType.Delete, endpoint, json);

    internal Task<TResponse> DeleteAsync<TResponse>(string endpoint, ILunarRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Delete, endpoint, json);

    internal Task<TResponse> PatchAsync<TResponse>(string endpoint, ILunarRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Patch, endpoint, json);

    internal Task PatchAsync(string endpoint, ILunarRequest json = null)
        => SendRequestAsync(RequestType.Patch, endpoint, json);

    internal Task<TResponse> PutAsync<TResponse>(string endpoint, ILunarRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Put, endpoint, json);

    internal Task PutAsync(string endpoint, ILunarRequest json = null)
        => SendRequestAsync(RequestType.Put, endpoint, json);

    internal Task<TResponse> PostAsync<TResponse>(string endpoint, ILunarRequest json = null, bool isWebhookRequest = false) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Post, endpoint, json, false, isWebhookRequest);

    internal Task PostAsync(string endpoint, ILunarRequest json = null)
        => SendRequestAsync(RequestType.Post, endpoint, json);

    public Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, ILunarRequest json = null, bool throwGetRequest = false, bool isWebhookRequest = false) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json, throwGetRequest, isWebhookRequest);


    internal static HttpMethod GetMethod(RequestType method)
    {
        switch (method)
        {
            case RequestType.Post:
                return HttpMethod.Post;
            case RequestType.Delete:
                return HttpMethod.Delete;
            case RequestType.Patch:
                return HttpMethod.Patch;
            case RequestType.Put:
                return HttpMethod.Put;
            case RequestType.Get:
                break;
        }
        return HttpMethod.Get;
    }

    internal async Task<HttpResponseMessage> InternalRequest(HttpMethod method, string endpoint, object? request)
    {
        HttpRequestMessage Mes = new HttpRequestMessage(method, Url + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            //if (Client.Config.Debug.LogRestRequestJson)
            //    Client.Logger.LogJson("Rest Request", request);
        }

        HttpResponseMessage Req = await Http.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            //RetryRequest Retry = null;
            //if (Req.Content.Headers.ContentLength.HasValue)
            //{
            //    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
            //    {
            //        Retry = DeserializeJson<RetryRequest>(Stream);
            //    }
            //}

            //if (Retry != null)
            //{
            //    await Task.Delay(Retry.retry_after + 2);
            //    HttpRequestMessage MesRetry = new HttpRequestMessage(method, Url + endpoint);
            //    if (request != null)
            //        MesRetry.Content = Mes.Content;
            //    Req = await Http.SendAsync(MesRetry);
            //}
        }

        //if (Client.Config.Debug.LogRestRequest)
        //{
        //    Client.Logger.LogRestMessage(Req, method, endpoint);

        //    if (Client.Config.LogMode == StoatLogSeverity.Debug)
        //        Client.Logger.LogJson("Rest Send", Req);
        //}


        if (method != HttpMethod.Get && !Req.IsSuccessStatusCode)
        {
            //RestError Error = null;
            //if (Req.Content.Headers.ContentLength.HasValue)
            //{
            //    try
            //    {
            //        using (Stream Stream = await Req.Content.ReadAsStreamAsync())
            //        {
            //            Error = DeserializeJson<RestError>(Stream);
            //        }
            //    }
            //    catch { }
            //}
            //if (Error != null)
            //{
            //    if (string.IsNullOrEmpty(Error.Permission))
            //        throw new StoatRestException($"Request failed due to {Error.Type} ({(int)Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
            //    else
            //        throw new StoatPermissionException(Error.Permission, (int)Req.StatusCode, Error.Type == StoatErrorType.MissingUserPermission);
            //}
            //else
            throw new LunarRestException(Req.ReasonPhrase, (int)Req.StatusCode);
        }

        //if (Req.IsSuccessStatusCode && Req.Content.Headers.ContentLength.HasValue && Client.Config.Debug.LogRestResponseJson)
        //{
        //    string Content = await Req.Content.ReadAsStringAsync();
        //    Client.Logger.LogJson("Rest Request", Content);
        //}
        return Req;
    }

    internal async Task<TResponse> InternalJsonRequest<TResponse>(HttpMethod method, string endpoint, object request, bool throwGetRequest = false, bool isWebhookRequest = false)
        where TResponse : class
    {
        HttpRequestMessage Mes = new HttpRequestMessage(method, isWebhookRequest ? endpoint : Url + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            //if (Client.Config.Debug.LogRestRequestJson)
            //    Client.Logger.LogJson("Rest Request", request);
        }
        HttpResponseMessage Req = await Http.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            //RetryRequest Retry = null;
            //if (Req.Content.Headers.ContentLength.HasValue)
            //{
            //    try
            //    {
            //        using (Stream Stream = await Req.Content.ReadAsStreamAsync())
            //        {
            //            Retry = DeserializeJson<RetryRequest>(Stream);
            //        }
            //    }
            //    catch { }
            //}

            //if (Retry != null)
            //{
            //    //Client.InvokeLog($"Retrying request: {endpoint} for {Retry.retry_after}s", StoatLogSeverity.Warn);
            //    await Task.Delay(Retry.retry_after + 2);
            //    HttpRequestMessage MesRetry = new HttpRequestMessage(method, Url + endpoint);
            //    if (request != null)
            //        MesRetry.Content = Mes.Content;
            //    Req = await Http.SendAsync(MesRetry);
            //}

        }

        //if (Client.Config.Debug.LogRestRequest)
        //{
        //    Client.Logger.LogRestMessage(Req, method, endpoint);

        //    if (Client.Config.LogMode == StoatLogSeverity.Debug)
        //        Client.Logger.LogJson("Rest Send", Req);
        //}


        if (endpoint == "/" && !Req.IsSuccessStatusCode)
        {
            //Client.InvokeLog("Stoat API is down", StoatLogSeverity.Warn);
            throw new LunarRestException("The Lunar API is down. Please try again later.", 500);
        }


        if (!Req.IsSuccessStatusCode && (throwGetRequest || method != HttpMethod.Get))
        {
            //RestError Error = null;
            //if (Req.Content.Headers.ContentLength.HasValue)
            //{
            //    try
            //    {
            //        using (Stream Stream = await Req.Content.ReadAsStreamAsync())
            //        {
            //            Error = DeserializeJson<RestError>(Stream);
            //        }
            //    }
            //    catch { }
            //}
            //if (Client.Config.Debug.LogRestRequest)
            //    Client.Logger.LogRestMessage(Req, method, endpoint);

            //if (Error != null)
            //{
            //    if (string.IsNullOrEmpty(Error.Permission))
            //        throw new LunarRestException($"Request failed due to {Error.Type} ({(int)Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
            //    else
            //        throw new StoatPermissionException(Error.Permission, (int)Req.StatusCode, Error.Type == StoatErrorType.MissingUserPermission);
            //}
            //else
            throw new LunarRestException(Req.ReasonPhrase, (int)Req.StatusCode);
        }

        TResponse Response = null;
        if (Req.IsSuccessStatusCode)
        {
            string Data = await Req.Content.ReadAsStringAsync();
            try
            {
                using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                {
                    Response = DeserializeJson<TResponse>(Stream);
                }
            }
            catch (Exception ex)
            {
                //Client.InvokeLog($"Failed to parse json for {endpoint}", StoatLogSeverity.Error);
                throw new LunarRestException("Failed to parse json response: " + ex.Message, 500);
            }

            //if (Response != null && Client.Config.Debug.LogRestResponseJson)
            //    Client.Logger.LogJson("Rest Response", Response);
        }
#pragma warning disable CS8603 // Possible null reference return.
        return Response;
#pragma warning restore CS8603 // Possible null reference return.
    }

    internal static string SerializeJson(object value)
    {
        StringBuilder sb = new StringBuilder(256);
        using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
        using (JsonWriter writer = new JsonTextWriter(text))
            Serializer.Serialize(writer, value);
        return sb.ToString();
    }

    internal static string SerializeJsonPretty(object value)
    {
        StringBuilder sb = new StringBuilder(256);
        using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
        using (JsonWriter writer = new JsonTextWriter(text))
            SerializerPretty.Serialize(writer, value);
        return sb.ToString();
    }

    internal static T? DeserializeJson<T>(Stream jsonStream)
    {
        using (TextReader text = new StreamReader(jsonStream))
        using (JsonReader reader = new JsonTextReader(text))
            return Deserializer.Deserialize<T>(reader);
    }
}

