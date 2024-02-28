using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace vrecruitOdataApi.CustomModels
{
    public class SuccessResult : IHttpActionResult
    {
        Success _success;
        HttpRequestMessage _request;
        object _data;

        public SuccessResult(Success success, HttpRequestMessage request)
        {
            _success = success;
            _request = request;
            // _data = Data;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {

            List<Success> _successList = new List<Success>();
            _successList.Add(_success);

            success succ = new success()
            {
                successlist = _successList
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<success>(succ, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}
public class Success
{
    public string Code { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}
public class success
{
    public List<Success> successlist { get; set; }
}