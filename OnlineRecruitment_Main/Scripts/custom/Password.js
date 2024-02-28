

function ajaxcallError(data) {
    console.log(data);
    console.log("Error");
    toastr.error('Error - Something is wrong...!!!');
    //$(".toast-message").css('color', 'black');
}
function ajaxcallSuccess(data) {
    console.log(data);

    if (data.error === true) {
        toastr.error('Error - ' + data.message.Errors[0]);
         $(".toast-message").css('color', 'black');
        $("#CurrentPassword").val('');
        $("#Password").val('');
        $("#ConfirmPassword").val('');
    } else if (data.message === "Reset") {
        toastr.success('Success - Password Reset Successfully...!!!');
        $(".toast-message").css('color', 'black');
        window.location.href = "../../Account/Login";
    } else if (data.message === "Confirm") {
        $("#EmailConfirmation").removeClass('hidden');
    } else if (data.message === "Invaild") {
        toastr.error('Error - Invaild Email...!!!');
        $(".toast-message").css('color', 'black');
    } else {
        toastr.success('Success - Password Change Successfully...!!!');
        $('#ChangePasswordDiv').modal('hide');
        $(".toast-message").css('color', 'black');
    }
}
//$(document).on("click", "#btnGetpsd", function () {
$("#btnGetpsd").on("click", function (e) {
    debugger;
    if ($("#Email").val() === "") {
        toastr.error('Error - Enter your Email...!!!');
        $("#Email").val('');
    } else {
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Account/ForgotPassword';
        params['requestType'] = "POST";
        params['data'] = $('#submitform').serialize();
        params['successCallbackFunction'] = ajaxcallSuccess;
        params['errorCallBackFunction'] = ajaxcallError;
        doAjax(params);
        return false;
    }
});
$("#ChangePassword").click(function () {
    $('#ChangePasswordDiv').modal('');
});
$("#btnchnagepasswordsubmit").on("click", function (e) {
    debugger
    if ($("#CurrentPassword").val() === "") {
        toastr.error('Error - Enter your Current password...!!!');
        return false;
    } else if ($("#Password").val() === "") {
        toastr.error('Error - Enter your New password...!!!');
        return false;
    } if ($("#ConfirmPassword").val() !== $("#Password").val()) {
        toastr.error('Error -Your new Password and confirm password not match...!!!');
        return false;
    } else {
        var formData = $('#Changepsdform').serializeArray();
        formData.push({ name: "CurrentPassword", value: $("#CurrentPassword").val() });
        formData.push({ name: "Password", value: $("#Password").val() });
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Account/ChangePassword';
        params['requestType'] = "POST";
        params['data'] = formData;
        params['successCallbackFunction'] = ajaxcallSuccess;
        params['errorCallBackFunction'] = ajaxcallError;
        doAjax(params);
        return false;
    }
});

$("#btnRestpsd").on("click", function (e) {
    debugger
    if ($("#newPassword").val() === "") {
        toastr.error('Error - Enter your New password...!!!');
        $(".toast-message").css('color', 'red');
        return false;
    } else if ($("#Confirmpassword").val() !== $("#newPassword").val()) {
        toastr.error('Error -Your new Password and confirm password not match...!!!');
        $(".toast-message").css('color', 'red');
        return false;
    } else {
        var formData = $('#Resetpsdsubmitform').serializeArray();
        formData.push({ name: "reset", value: true });
        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Account/ResetUserPassword';
        params['requestType'] = "POST";
        params['data'] = formData;
        params['successCallbackFunction'] = ajaxcallSuccess;
        params['errorCallBackFunction'] = ajaxcallError;
        doAjax(params);
        return false;
    }
});

