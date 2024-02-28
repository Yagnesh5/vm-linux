function ajaxError(data) {
    $("#btnsubmit").removeClass('disabled');
    console.log(data);
    console.log("Error");
    toastr.error('Error - Something is wrong...!!!');
}
function ajaxSuccess(data) {
    console.log(data);
    $("#btnsubmit").removeClass('disabled');
    var RedirectToDashboard = $("#RedirectToDashboard").val();
    if (data.Error === true) {
        if (data.Message === "Login") {
            toastr.error('Error - Already login with ' + data.Data);
            location.href = RedirectToDashboard;
        } else {
            toastr.error('Error - ' + data.Message);
            $("#Email").val('');
            $("#Password").val('');
        }
    } else {
        toastr.success('Success - Login Successfully...!!!');

        if (data.IsClientOrCandidatePopup == true) {
            $("#popupMessage").text(data.Message);
            $('#ClientAlertModal').modal('show');
        }
        else {
            location.href = RedirectToDashboard;
        }
    }
}

$("#btnOK").click(function (e) {
    location.href = $("#RedirectToDashboard").val();
})
$("#btnsubmit").on("click", function (e) {
    $(this).addClass('disabled');
    var params = $.extend({}, doAjax_params_default);
    params['url'] = 'Account/Login';
    params['requestType'] = "POST";
    params['data'] = $('#submitform').serialize();
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;

});