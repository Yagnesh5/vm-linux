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
        ResetForm();
        if (data.Message === "Edit") {
            $("#Id").val(data.Data.Id);
            $("#UserName").val(data.Data.UserName);
            $("#RoleName").val(data.Data.RoleName).trigger('change');
            $("#Email").val(data.Data.Email);
            $("#CompanyId").val(data.Data.CompanyId).trigger('change');
            $("#PhoneNumber").val(data.Data.PhoneNumber);
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
            toastr.success('Success - Updated Successfully...!!!');
        } else if (data.Message === "Deleted") {
            location.reload();
            toastr.success('Success - Deleted Successfully...!!!');
        } else if (data.Message === "Inserted") {
            ResetForm();
            $("#btnUpdate").addClass('hidden');
            $("#btnsubmit").show();
            $("#userdetailsdata").show();
            $("#Usersdiv").addClass('hidden');
            location.reload();
            toastr.success('Success - Inserted Successfully...!!!');
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
    //toastr.error('Error - Something is wrong...!!!');
}
function ResetForm() {
    $("#Id").val('');
    $("#UserName").val('');
    $("#PasswordHash").val('');
    $("#CompanyName").val('');
    $("#Email").val('');
    $("#RoleName").val('').trigger('change');
    $("#CompanyId").val('').trigger('change');
    $("#PhoneNumber").val('');
    $("#Password").val('');
    $("#ConfirmPassword").val('');
}
$("#btnsubmit").on("click", function (e) {
    var email = $("#Email").val();
    //if (!validateEmail(email)) {
    //    toastr.error('Error - Enter valid email...!!!');
    //    $("Email").val();
    //    $("Email").focus();
    //    return false;
    //}
    //if ($("#RoleName").val() === "") {
    //    toastr.error('Error - Select the role');
    //    $("#RoleName").focus();
    //    return false;
    //}
    $("#submitform").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 3
            },
            Email: {
                required: true,
                email: true
            },
            Password: {
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            }
        },
        submitHandler: function (form) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Account/CreateAdmin';
            params['requestType'] = "POST";
            params['data'] = $('#submitform').serialize() + "&RoleName=ADMIN";
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
            "url": "/Master/CompanyAdminLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "UserName", "name": "UserName" },
            { "data": "RoleName", "name": "RoleName" },
            { "data": "Email", "name": "Email" },
            { "data": "CompanyName", "name": "CompanyName" },
            { "data": "PhoneNumber", "name": "PhoneNumber" },
            { "data": "LastUpdateDate", "name": "LastUpdateDate" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" type="button" title="Edit" id="" data-data="' + data.Id + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" title="Delete" id="" data-data="' + data.Id + '" data-AdminName="' + data.UserName + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    return IsEditBtn + IsDeleteBtn;
                }
            }
        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitAdmin',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitAdmin',
                exportOptions: {
                    columns: ':visible'
                }
            },
            'colvis'
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
    $(".CreateAdmin").removeClass("active");
    LoadData();
    $("#PhoneNumber").mask("(999) 999-9999");
    $(".acls_CreateAdmin").css("color", "white");
    $(".acls_CreateAdmin").mouseover(function () {
        $(".acls_CreateAdmin").css("color", "#6a6c6f");
    });
    $(".acls_CreateAdmin").mouseout(function () {
        $(".acls_CreateAdmin").css("color", "white");
    });
    oTable = $('#UserDatatable').DataTable();

    var input = document.getElementById("txtSearch");
    input.addEventListener("keyup", function (event) {
        event.preventDefault();
        if (event.keyCode === 13) {
            document.getElementById("btnSearch").click();
        }
    });

    $('#btnSearch').click(function () {
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.draw();
        setTimeout(function () { $(".details-control").text(''); }, 5000);
    });
});

$(document).on("click", ".btndelete", function () {
    var id = $(this).data("data");
    var adminname = $(this).data("adminname");

    swal({
        title: "Are you sure?",
        text: "Admin - \"" + adminname + "\" Login and all relevant data will also be deleted",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel it!",
        closeOnConfirm: false,
        closeOnCancel: false
    },
        function (isConfirm) {
            if (isConfirm) {

                var params = $.extend({}, doAjax_params_default);
                params['url'] = '/Account/DeleteUser';
                params['requestType'] = "GET";
                params['data'] = { Key: id };
                params['successCallbackFunction'] = ajaxSuccess;
                params['errorCallBackFunction'] = ajaxError;
                doAjax(params);
                return false;
            } else {
                swal("Cancelled", "Admin - \"" + adminname + "\" Login and relevant data is safe :)", "error");
            }
        });
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
    $("#userdetailsdata").hide();
    $("#Usersdiv").removeClass('hidden');
    ResetForm();
});
$("#btncancel").click(function () {
    $("#userdetailsdata").show();
    $("#Usersdiv").addClass('hidden');
});

$("#btnUpdate").on("click", function (e) {
    //if ($("#RoleName").val() === "") {
    //    toastr.error('Error - Select the role');
    //    $("#RoleName").focus();
    //} else
    //{
    $("#submitform").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 3
            },
            Email: {
                required: true,
                email: true
            },
            Password: {
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            }
        },
        submitHandler: function (form) {
            var id = $("#Id").val();
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
    //}
});
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}