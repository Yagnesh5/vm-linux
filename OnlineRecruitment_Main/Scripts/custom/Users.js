function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.Error) {
        toastr.error('Error -' + data.Data);
        toastr.error(data.Errors[0]);
    }
    if (data.Errors) {
        toastr.error(data.Errors[0]);
    } else if (data.Message) {
        if (data.Message === "Edit") {
            debugger;
            $("#Id").val(data.Data.Id);
            $("#UserName").val(data.Data.UserName);
            $("#RoleName").val(data.Data.RoleName).trigger('change');
            $("#Email").val(data.Data.Email);
            $("#PhoneNumber").val(data.Data.PhoneNumber);
            if (comCurrentRole === "SUPER ADMIN") {
                if (data.Data.CompanyId) {
                    $("#chkCmp").prop("checked", true);
                    $("#CompanyId").val(data.Data.CompanyId).trigger('change');
                    $("#DivCompanyName").show();
                }
            } else {
                $("#CompanyId").val(data.Data.CompanyId).trigger('change');
            }
            $("#userdetailsdata").hide();
            $("#PsdDiv").hide();
            $("#Password").prop('required', false);
            $("#ConfirmPassword").prop('required', false);
            $("#Usersdiv").removeClass('hidden');
            $("#btnUpdate").removeClass('hidden');
            $("#btnsubmit").hide();
        } else if (data.Message === "Updated") {
            $("#btnUpdate").addClass('hidden');
            $("#btnsubmit").show();
            $("#userdetailsdata").show();
            $("#Usersdiv").addClass('hidden');
            location.reload();
            toastr.success('Success - Data Updated Successfully...!!!');

        } else if (data.Message === "Deleted") {
            location.reload();
            toastr.success('Success - Data Deleted Successfully...!!!');
        } else if (data.Message === "Inserted") {

            ResetForm();
            $("#btnUpdate").addClass('hidden');
            $("#btnsubmit").show();
            $("#userdetailsdata").show();
            $("#Usersdiv").addClass('hidden');
            location.reload();
            toastr.success('Success - User Data Inserted Successfully...!!!');
        }

    } else {
        LoadData();
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
        }
    }
}
function ajaxError(data) {
    console.log(data);
    console.log("Error");
    toastr.error('Error - Something is wrong...!!!');
}
function ResetForm() {
    $("#Id").val('');
    $("#UserName").val(" ");
    $("#PasswordHash").val(" ");
    $("#CompanyName").val(" ");
    $("#Email").val(" ");
    $("#PhoneNumber").val('');
    $("#RoleName").val("").trigger('change');
}
$("#btnsubmit").on("click", function (e) {
    $("#submitform").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 3
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            },
            Email: {
                required: true,
                email: true
            }
        },
        submitHandler: function (form) {
            var email = $("#Email").val();
            if (!validateEmail(email)) {
                toastr.error('Error - Enter valid email...!!!');
                $("#Email").val();
                $("#Email").focus();
                return false;
            }
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Account/CreateAdmin';
            params['requestType'] = "POST";
            params['data'] = $('#submitform').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            doAjax(params);
            return false;
        }
    });
});
function LoadData() {
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    $('#UserDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/CompanyUserLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "UserName", "name": "UserName" },
            { "data": "RoleName", "name": "RoleName" },
            { "data": "Email", "name": "Email" },
            { "data": "CompanyName", "name": "CompanyName" },
            { "data": "PhoneNumber", "name": "PhoneNumber" },
            { "data": "LastUpdateDate", "name": "lastUpdateDate" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button" id="" data-data="' + data.Id + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? '| <button class="btn btn-danger btn-xs btndelete" title="Delete" type="button" id="" data-data="' + data.Id + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    var IsResetpsd = ' | <button class="btn btn-warning btn-xs btnresetpsd" title="Reset password" type="button"  id="" data-data="' + data.Id + '"><i class="fa fa-key"></i></button>';
                    return IsEditBtn + IsResetpsd + IsDeleteBtn;
                }
            }
        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitUsers',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitUsers',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'colvis',
                columns: [0, 1, 2, 3, 4, 5]
            }
        ],
        columnDefs: [{
            targets: -1,
            visible: true
        }]
    });
    $(".dataTables_filter").hide();
    $('#loading').html("").hide();

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = '/Account/CreateAdmin';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}
function LoadCompanuDropdown() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'api/Master/GetCompanyName';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
}
$(document).ready(function () {
    console.log("ready!");
    $("#DivCompanyName").hide();
    $(function () {
        $("#chkCmp").click(function () {
            if ($(this).is(":checked")) {
                $("#DivCompanyName").show();
            } else {
                $("#DivCompanyName").hide();
                $("#CompanyId").val("").trigger('change');
            }
        });
    });
    $(".Users").removeClass("active");
    $(".acls_Users").css("color", "white");
    $(".acls_Users").mouseover(function () {
        $(".acls_Users").css("color", "#6a6c6f");
    });
    $(".acls_Users").mouseout(function () {
        $(".acls_Users").css("color", "white");
    });
    $("#PhoneNumber").mask("(999) 999-9999");
    LoadData();
    oTable = $('#UserDatatable').DataTable();
    $('#btnSearch').click(function () {
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.draw();
    });

    $("#RoleName option[value='ADMIN']").remove();
    $("#RoleName option[value='SUPER ADMIN']").remove();
    $("#RoleName option[value='CLIENT']").remove();
});
$(document).on("click", ".btndelete", function () {
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/DeleteUser';
    params['requestType'] = "GET";
    params['data'] = { Key: id };
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});

$(document).on("click", ".btnedit", function () {
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/GetuserData';
    params['requestType'] = "GET";
    params['data'] = { Key: id };
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});

$("#btnIn").click(function () {
    ResetForm();
    $("#PsdDiv").show();
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").show();
    $("#userdetailsdata").hide();
    $("#Usersdiv").removeClass('hidden');

});
$("#btncancel").click(function () {
    $("#userdetailsdata").show();
    $("#Usersdiv").addClass('hidden');
    $("#chkCmp").prop("checked", false);
    $("#CompanyId").val("").trigger('change');
    $("#DivCompanyName").hide();

});
$("#btnUpdate").on("click", function (e) {

    $("#submitform").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 3
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            },
            Email: {
                required: true,
                email: true
            },
        },
        submitHandler: function (form) {
            var email = $("#Email").val();
            if (!validateEmail(email)) {
                toastr.error('Error - Enter valid email...!!!');
                $("#Email").val();
                $("#Email").focus();
                return false;
            }
            var id = $("#Id").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Account/UpdateUser';
            params['requestType'] = "POST";
            params['data'] = $('#submitform').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            doAjax(params);
            return false;
        }
    });
});
$(document).on("click", ".btnresetpsd", function () {
    var id = $(this).data("data");
    $("#uid").val(id);
    $('#psdModel').modal('show');
    $("#Password2").prop('required', true);
    $("#ConfirmPassword2").prop('required', true);
});

$("#btnUpdatePsd").click(function ()  //input button click
{
    if ($("#Password2").val() === "") {
        toastr.error('Error - Enter Password...!!!');
        $("#Password2").focus();
    } else if ($("#ConfirmPassword2").val() === "") {
        toastr.error('Error - Enter Confirm Password...!!!');
        $("#ConfirmPassword2").focus();
    } else if ($("#Password2").val() !== $("#ConfirmPassword2").val()) {
        toastr.error('Error - Enter Confirm Password Not Match!!!');
    } else {
        $.ajax({
            url: '/Account/ResetUserPassword',
            type: "POST",
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            dataType: 'json',
            data: {
                userId: $("#uid").val(),
                newPassword: $("#Password2").val()
            },
            success: function (result) {
                console.log(result);
                if (result.error === false) {
                    $('#psdModel').modal('hide');
                    toastr.success('Success - Password Change Successfully...!!!');
                } else {
                    toastr.error('Error -' + result.message.Errors[0]);
                    $("#Password2").val('');
                    $("#Password2").focus();
                }
                return false;
            },
            error: function (err) {
                toastr.error(err.statusText);
                return false;
            }
        });
    }
});
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}
