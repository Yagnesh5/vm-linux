var BaseUrl = BaseUrl;

var comCurrentRole = CurrentRole;
var comCurrentUserId = CurrentUserId; 
var comcurrentCompanyName = currentCompanyName;
var comcurrentCompanyId = currentCompanyId;
var SuperAdminId = "6a9b74d1-73b3-44ee-ae99-af9fd9518cd1";

var doAjax_params_default = {
    'url': null,
    'requestType': "GET",
    'contentType': 'application/x-www-form-urlencoded; charset=UTF-8',
    'cache': false,
    'dataType': 'json',
    'data': {},
    'beforeSendCallbackFunction': null,
    'successCallbackFunction': null,
    'completeCallbackFunction': null,
    'errorCallBackFunction': null
};


function doAjax(doAjax_params) {

    var url = doAjax_params['url'];
    var requestType = doAjax_params['requestType'];
    var contentType = doAjax_params['contentType'];
    var dataType = doAjax_params['dataType'];
    var data = doAjax_params['data'];
    var beforeSendCallbackFunction = doAjax_params['beforeSendCallbackFunction'];
    var successCallbackFunction = doAjax_params['successCallbackFunction'];
    var completeCallbackFunction = doAjax_params['completeCallbackFunction'];
    var errorCallBackFunction = doAjax_params['errorCallBackFunction'];

    //make sure that url ends with '/'
    /*if(!url.endsWith("/")){
     url = url + "/";
    }*/

    $.ajax({
        url: url,
        crossDomain: true,
        type: requestType,
        contentType: contentType,
        cache: false,
        dataType: dataType,
        data: data,
        beforeSend: function (jqXHR, settings) {
            if (typeof beforeSendCallbackFunction === "function") {
                beforeSendCallbackFunction();
            }
        },
        success: function (data) {
            if (typeof successCallbackFunction === "function") {
                successCallbackFunction(data);
                $('#loading').html("").hide();
                $("html, body").animate({ scrollTop: 0 }, 600);
            }
        },
        error: function (errorThrown) {
            if (typeof errorCallBackFunction === "function") {
                errorCallBackFunction(errorThrown);
                $('#loading').html("").hide();
                $("html, body").animate({ scrollTop: 0 }, 600);
            }

        },
        complete: function (jqXHR, textStatus) {
            if (typeof completeCallbackFunction === "function") {
                completeCallbackFunction();
            }
        }
    });
}
$(document).ready(function () {
    //$(".js-source-states").select2();
    //$(".js-source-states-2").select2();
    //$('.datepicker').datepicker({
    //    format: 'd-M-yyyy'
    //});
});