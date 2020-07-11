﻿using AntiHarassment.Frontend.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Infrastructure
{
    public class ApiGateway : IApiGateway
    {
        private readonly string apiBaseAddress;
        private readonly HttpClient _httpClient;
        private readonly IApplicationStateManager applicationStateManager;

        public ApiGateway(string apiBaseAddress, IApplicationStateManager applicationStateManager, HttpClient httpClient)
        {
            this.apiBaseAddress = apiBaseAddress;
            _httpClient = httpClient;
            this.applicationStateManager = applicationStateManager;
        }

        public async Task<ResponseModel> Get<ResponseModel>(string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);
            var request = await BuildBaseRequest("GET", requestUri).ConfigureAwait(false);
            request.RequestUri = requestUri;

            return await ExecuteRequest<ResponseModel>(request).ConfigureAwait(false);
        }

        public async Task<ResponseModel> Post<ResponseModel, RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);

            var request = await BuildBaseRequest("POST", requestUri).ConfigureAwait(false);
            var body = Serialization.Serialize(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await ExecuteRequest<ResponseModel>(request).ConfigureAwait(false);
        }

        public async Task Post<RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);
            var request = await BuildBaseRequest("POST", requestUri).ConfigureAwait(false);
            var body = Serialization.Serialize(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            await ExecuteRequest(request).ConfigureAwait(false);
        }

        public async Task<ResponseModel> Delete<ResponseModel, RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);
            var request = await BuildBaseRequest("DELETE", requestUri).ConfigureAwait(false);
            var body = Serialization.Serialize(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await ExecuteRequest<ResponseModel>(request).ConfigureAwait(false);
        }

        public async Task Delete(string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);
            var request = await BuildBaseRequest("DELETE", requestUri).ConfigureAwait(false);
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                // LOG
            }
        }

        public async Task PostImage(MemoryStream memoryStream, string fileName, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var requestUri = BuildRequestUri(controller, action, routeValues, queryParams);
            var request = await BuildBaseRequest("post", requestUri).ConfigureAwait(false);
            request.Content = new MultipartFormDataContent
            {
                {
                new ByteArrayContent(memoryStream.GetBuffer()),
                    "file",
                    fileName
                }
            };

            await ExecuteRequest(request).ConfigureAwait(false);
        }

        private async Task ExecuteRequest(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception(content);
            }
        }

        private async Task<T> ExecuteRequest<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception(content);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            var responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Serialization.Deserialize<T>(responseValue);
        }

        private async Task<HttpRequestMessage> BuildBaseRequest(string requestType, Uri requestUri)
        {
            var jwtToken = await applicationStateManager.GetJwtToken().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(jwtToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            return new HttpRequestMessage
            {
                Method = new HttpMethod(requestType),
                RequestUri = requestUri
            };
        }

        private Uri BuildRequestUri(string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams)
        {
            var routeBuilder = new StringBuilder();
            routeBuilder.Append(apiBaseAddress);
            if (!apiBaseAddress.EndsWith("/"))
                routeBuilder.Append("/");

            routeBuilder.Append(controller);

            if (!string.IsNullOrEmpty(action))
                routeBuilder.Append("/").Append(action);

            if (routeValues != null)
            {
                for (int i = 0; i < routeValues.Length; i++)
                    routeBuilder.Append("/").Append(routeValues[i]);
            }

            for (int i = 0; i < queryParams.Length; i++)
            {
                if (i == 0)
                    routeBuilder.Append("?");
                else
                    routeBuilder.Append("&");

                routeBuilder.Append(queryParams[i].Name).Append("=").Append(queryParams[i].Value);
            }

            return new Uri(routeBuilder.ToString());
        }
    }
}
