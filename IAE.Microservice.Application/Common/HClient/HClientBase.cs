using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common.Operation;
using Newtonsoft.Json;

namespace IAE.Microservice.Application.Common.HClient
{
    public abstract partial class HClientBase
    {
        private readonly HttpClient _httpClient;

        protected HClientBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// <see cref="JsonSerializerSettings"/> for <see cref="JsonConvert"/> serializer or deserializer
        /// </summary>
        protected abstract JsonSerializerSettings JsonSettings { get; }

        /// <summary>
        /// Handle response for <see cref="HClientProfile{TIn, TOut}"/> and create <see cref="OperationApiResult{T}"/>
        /// </summary>
        private async Task<OperationApiResult<TOut>> SendAsync<TIn, TOut>(HClientProfile<TIn, TOut> profile,
            CancellationToken token = default)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            string stringContent = null;
            try
            {
                using (var response = await GetResponseAsync(profile, token))
                {
                    statusCode = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                    {
                        return OperationApiResult<TOut>.Failed(Code(profile), 
                            await response.Content.ReadAsStringAsync(), statusCode);
                    }

                    if (profile.ResponseHandler != null)
                    {
                        return OperationApiResult<TOut>.Success(await profile.ResponseHandler.Invoke(response.Content),
                            response.StatusCode);
                    }

                    stringContent = await response.Content.ReadAsStringAsync();
                    var content = DeserializeContentAsync<TOut>(stringContent, JsonSettings);
                    return OperationApiResult<TOut>.Success(content, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                var description = stringContent != null && statusCode >= HttpStatusCode.MultipleChoices
                    ? stringContent
                    : e.Message;
                statusCode = statusCode >= HttpStatusCode.MultipleChoices
                    ? statusCode
                    : HttpStatusCode.BadRequest;
                return OperationApiResult<TOut>.Failed(Code(profile), description, statusCode);
            }
        }

        /// <summary>
        /// Send request <see cref="HttpRequestMessage"/> with: method <see cref="HClientProfile{TIn, TOut}.Method"/>,
        /// URL <see cref="HClientProfile{TIn, TOut}.Url"/> and content <see cref="HClientProfile{TIn, TOut}.Content"/>
        /// and get <see cref="HttpResponseMessage"/>
        /// </summary>
        private async Task<HttpResponseMessage> GetResponseAsync<TIn, TOut>(HClientProfile<TIn, TOut> profile,
            CancellationToken token = default)
        {
            using (var request = profile.Url != null
                       ? new HttpRequestMessage(profile.Method, profile.Url)
                       : new HttpRequestMessage(profile.Method, profile.PathAndQuery))
            {
                using (var requestContent = SerializeAndCreateContent(profile, JsonSettings))
                {
                    request.Content = requestContent;
                    profile.SetRequestHeaders?.Invoke(request.Headers);
                    return await _httpClient.SendAsync(request, profile.CompletionOption, token)
                        .ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Serialize <see cref="TIn"/> and create <see cref="HttpContent"/> with it.
        /// </summary>
        /// <remarks>
        /// This method could made as asynchronous but it is not sure whether it will improve performance or not.
        /// It all depends on the serialization time because starting a new Task is costly.
        /// </remarks>
        private static HttpContent SerializeAndCreateContent<TIn, TOut>(HClientProfile<TIn, TOut> request,
            JsonSerializerSettings jsonSettings = null)
        {
            jsonSettings = jsonSettings ?? new JsonSerializerSettings();
            var content = string.Empty;
            if (request.Content != null)
            {
                content = JsonConvert.SerializeObject(request.Content, typeof(TIn), Formatting.None, jsonSettings);
            }

            return new StringContent(content, request.Encoding, request.MediaType);
        }

        /// <summary>
        /// Deserialize <see cref="HttpContent"/> to <see cref="T"/> 
        /// </summary>
        private static T DeserializeContentAsync<T>(string stringContent, JsonSerializerSettings jsonSettings = null)
        {
            return jsonSettings == null
                ? JsonConvert.DeserializeObject<T>(stringContent)
                : JsonConvert.DeserializeObject<T>(stringContent, jsonSettings);
        }

        /// <summary>
        /// Build error code 
        /// </summary>
        private string Code<TIn, TOut>(HClientProfile<TIn, TOut> profile)
        {
            var url = profile.Url != null
                ? profile.Url.OriginalString
                : _httpClient.BaseAddress.OriginalString + profile.PathAndQuery;
            return $"{nameof(HClientBase)} ({url})";
        }
    }
}