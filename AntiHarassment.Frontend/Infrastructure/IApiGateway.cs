using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Infrastructure
{
    public interface IApiGateway
    {
        Task<ResponseModel> Get<ResponseModel>(string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
        Task<ResponseModel> Post<ResponseModel, RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
        Task Post<RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
        Task<ResponseModel> Delete<ResponseModel, RequestModel>(RequestModel model, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
        Task Delete(string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
        Task PostImage(MemoryStream memoryStream, string fileName, string controller, string action = null, string[] routeValues = null, params QueryParam[] queryParams);
    }
}
