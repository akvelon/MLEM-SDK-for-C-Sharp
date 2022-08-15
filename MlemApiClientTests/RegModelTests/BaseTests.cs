﻿using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MlemApi;

namespace MlemApiClientTests.RegModelTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient _client;
        protected IMlemApiConfiguration _mlemApiConfiguration;
        protected const string _baseAddress = "http://127.0.0.1:8080";

        protected const string interface_json =
            "{\"version\": \"0.2.7\"," +
                "\"methods\": {" +
                    "\"predict\": {\"name\": \"predict\",\"args\": [{\"name\": \"data\",\"type_\": {\"columns\": [\"0\"],\"dtypes\": [\"float64\"],\"index_cols\": [],\"type\": \"dataframe\"},\"required\": true,\"default\": null,\"kw_only\": false}],\"returns\": {\"shape\": [null],\"dtype\": \"float64\",\"type\": \"ndarray\"},\"varargs\": null,\"varkw\": null}," +
                    "\"sklearn_predict\": {\"name\": \"predict\",\"args\": [{\"name\": \"X\",\"type_\": {\"columns\": [\"0\"],\"dtypes\": [\"float64\"],\"index_cols\": [],\"type\": \"dataframe\"},\"required\": true,\"default\": null,\"kw_only\": false}],\"returns\": {\"shape\": [null],\"dtype\": \"float64\",\"type\": \"ndarray\"},\"varargs\": null,\"varkw\": null}}}";

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IMlemApiConfiguration>();
            configurationMock.Setup(c => c.Url).Returns(_baseAddress);
            _mlemApiConfiguration = configurationMock.Object;

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            #region HttpMessageHandler Setup
            var _msgHandler = new Mock<HttpMessageHandler>();
            var mockedProtected = _msgHandler.Protected();
            
            mockedProtected.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.Equals(_baseAddress + "/predict")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[0]")
                });

            mockedProtected.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.Equals(_baseAddress + "/interface.json")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(interface_json)
                });
            #endregion

            var httpClient = new HttpClient(_msgHandler.Object);

            _client = new MlemApiClient(httpClient, _mlemApiConfiguration, new NewtonsoftRequestValueSerializer(), logger);
        }
    }
}
