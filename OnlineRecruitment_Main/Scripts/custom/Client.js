$(document).ready(function () {
    console.log("ready!");
    ClientLoadData();
    $("#DivCompanyName").show();
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
    $("#ClientPhone").mask("(999) 999-9999");
    $("#ClientMobile").mask("(999) 999-9999");
    $(".Client").removeClass("active");
    $(".acls_Client").css("color", "white");
    $(".acls_Client").mouseover(function () {
        $(".acls_Client").css("color", "#6a6c6f");
    });
    $(".acls_Client").mouseout(function () {
        $(".acls_Client").css("color", "white");
    });
    $("#divClientStatus").hide();
    oTable = $('#tblDatatable').DataTable();
    oTable.columns(4).search($('#SearchClientStatus').val().trim());
    oTable.draw();
    $('#btnSearch').click(function () {
        $('#loading').html("").show();
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.columns(4).search($('#SearchClientStatus').val().trim());
        oTable.draw();
        $('#loading').html("").hide();
        setTimeout(function () { $(".details-control").text(''); }, 5000);
    });
});
function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Code === "0") {
        toastr.error('Error - ' + data.successlist[0].Message);
    }
    if (data.successlist[0].Message === "Inserted") {
        toastr.success('Success - Data Inserted Successfully...!!!');
        ClientLoadData();
        ResetClientForm();
        $("#detailsdata").show();
        $("#Creatediv").addClass('hidden');
        location.reload();
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        ClientLoadData();
        location.reload();
    } else if (data.successlist[0].Message === "Edit") {
        ResetClientForm();
        $("#divusername").hide();
        $("#divClientStatus").show();
        $("#UserName").val(data.successlist[0].Data.UserName);
        $("#ClientId").val(data.successlist[0].Data.ClientId);
        $("#ClientName").val(data.successlist[0].Data.ClientName);
        $("#ClientEmail").val(data.successlist[0].Data.ClientEmail);
        $("#ClientWWW").val(data.successlist[0].Data.ClientWWW);
        $("#ClientPhone").val(data.successlist[0].Data.ClientPhone);
        $("#ClientMobile").val(data.successlist[0].Data.ClientMobile);
        $("#ClientGST").val(data.successlist[0].Data.ClientGST);
        $("#ClientAddress").val(data.successlist[0].Data.ClientAddress);
        $("#Contact_Person").val(data.successlist[0].Data.Contact_Person);
        $("#Client_PAN_No").val(data.successlist[0].Data.Client_PAN_No);
        $("#ClientCity").val(data.successlist[0].Data.ClientCity);
        $("#CountryId").val(data.successlist[0].Data.CountryId).trigger('change');
        $("#ClientIsactive").val(data.successlist[0].Data.ClientIsactive).trigger('change');
        if (comCurrentRole === "SUPER ADMIN") {
            if (data.successlist[0].Data.CompanyId) {
                $("#chkCmp").prop("checked", true);
                $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');
                $("#DivCompanyName").show();
            }
        } else {
            $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');
        }
        $("#psdDiv").hide();
        $("#Password").prop('required', false);
        $("#ConfirmPassword").prop('required', false);
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
        $("#Creatediv").removeClass('hidden');
        $("#detailsdata").hide();
    } else if (data.successlist[0].Message === "Update") {
        toastr.success('Success - Data Updated Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#Creatediv").addClass('hidden');
        $("#detailsdata").show();
        ClientLoadData();
        location.reload();
    }
    else {
        //var newList = [];
        //$.each(data.successlist[0].Data.filter(
        //    function (el) {
        //        if (el.CompanyId === parseInt(currentCompanyId)) {
        //            newList.push(el);
        //        }
        //    }),
        //);
        ClientLoadData();
       
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
        }

    }
}
function ajaxError(data) {
    console.log("Error");
    console.log(data.responseText);
    var obj = jQuery.parseJSON(data.responseText);
    console.log(obj);
    if (obj.errors[0].Code === "0") {
        toastr.error('Error - ' + obj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }

}
function ClientLoadData() {
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
   
    $('#tblDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/ClientLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "ClientName", "name": "ClientName"},
            { "data": "ClientEmail", "name": "ClientEmail" },
            { "data": "ClientPhone", "name": "ClientPhone" },
            { "data": "ClientMobile", "name": "ClientMobile"},
            { "data": "ClientGST", "name": "ClientGST" },
            { "data": "ClientAddress", "name": "ClientAddress" },            
            { "data": "LastUpdateDateString", "name": "LastUpdateDate" },
            { "data": "ClientCity", "name": "ClientCity" },
            { "data": "CountryName", "name": "CountryName" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    return IsEditBtn = data.ClientIsactive === "Active" ? '<lable class="label label-success">Active</lable>' : '<lable class="label label-danger">In-active</lable>';
                    //return IsEditBtn = data.IsActive === true ? '<lable class="label label-success">Active</lable>' : '<lable class="label label-danger">Deactive</lable>';
                }
            },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" type="button" id="" title="Edit" data-data="' + data.ClientId + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" id="" title="Delete" data-data="' + data.ClientId + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    var ChangesPsdbtn = ' | <button class="btn btn-warning btn-xs btnresetpsd" type="button" title="Reset password" id="" data-data="' + data.UserId + '"><i class="fa fa-key"></i></button>';
                    return IsEditBtn + IsDeleteBtn + ChangesPsdbtn;
                }
            }

        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitClient',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitClient',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'colvis',
                columns: [0, 1, 2, 3, 4, 5, 6]
            }
        ],
        columnDefs: [
        {
            targets: -1,
            visible: true
        },
        {
            "targets": [4, 5],
            "visible": false,
            "searchable": false
        },
        ]
    });
    $("#tblDatatable_filter").hide();
    $('#loading').html("").hide();

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Clients';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}
$("#btnsubmit").on("click", function (e) {
    
    $("#submitform").validate({
        rules: {
            ClientName: {
                required: true,
                minlength: 3
            },
            clientwww: {
                required: true,
                url: true
            },
            ClientEmail: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            }
        },
        submitHandler: function (form) {
            var email = $("#ClientEmail").val();
            if (!validateEmail(email)) {
                toastr.error('Error - Enter valid email...!!!');
                $("ClientEmail").val();
                $("ClientEmail").focus();
                return false;
            }
            var txt = $('#ClientWWW').val();
            var re = /^(http: \/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;
            if (!re.test(txt)) {
                toastr.error('Error - Invaild Url...!!!');
                $("#ClientWWW").val('');
                $("#ClientWWW").focus();
                return false;
            }
            $.ajax({
                url: '/Account/SaveClientToUserTbl',
                type: "POST",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                dataType: 'json',
                data: {
                    ClientEmail: $("#ClientEmail").val(),
                    Password: $("#Password").val(),
                    ClientId: null,
                    username: $("#UserName").val()
                },
                success: function (result) {
                    console.log(result);
                    if (result.Errors) {
                        toastr.error(result.Errors[0]);
                    } else {
                        var UserId = result.Data;
                        var formData = $('#submitform').serializeArray();
                        formData.push({ name: "UserId", value: UserId });
                        var params = $.extend({}, doAjax_params_default);
                        params['url'] = BaseUrl + 'odata/Clients';
                        params['requestType'] = "POST";
                        params['data'] = formData;
                        params['successCallbackFunction'] = ajaxSuccess;
                        params['errorCallBackFunction'] = ajaxError;
                        doAjax(params);
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
});
$(document).on("click", ".btndelete", function () {
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Clients(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$(document).on("click", ".btnedit", function () {
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Clients(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function () {
    var email = $("#ClientEmail").val();
    if (!validateEmail(email)) {
        toastr.error('Error - Enter valid email...!!!');
        $("ClientEmail").val();
        $("ClientEmail").focus();
        return false;
    }
    var txt = $('#ClientWWW').val();
    var re = /^(http: \/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;
    if (!re.test(txt)) {
        toastr.error('Error - Invaild Url...!!!');
        $("#ClientWWW").val('');
        $("#ClientWWW").focus();
        return false;
    }
    $("#submitform").validate({
        rules: {
            ClientName: {
                required: true,
                minlength: 3
            },
            //ClientWWW: {
            //    required: true,
            //    url: true
            //},
            ClientEmail: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                minlength: 6,
                equalTo: "#Password"
            }
        },
        submitHandler: function (form) {
            $.ajax({
                url: '/Account/SaveClientToUserTbl',
                type: "POST",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                dataType: 'json',
                data: {
                    ClientEmail: $("#ClientEmail").val(),
                    Password: $("#Password").val(),
                    ClientId: $("#ClientId").val(),
                    username: $("#UserName").val()
                },
                success: function (result) {
                    console.log(result);
                    if (result.Errors) {
                        toastr.error(result.Errors[0]);
                    } else {
                        var UserId = result.Data;
                        var formData = $('#submitform').serializeArray();
                        formData.push({ name: "UserId", value: UserId });
                        var id = parseInt($("#ClientId").val());
                        var params = $.extend({}, doAjax_params_default);
                        params['url'] = BaseUrl + 'odata/Clients(' + id + ')';
                        params['requestType'] = "PUT";
                        params['data'] = formData;
                        params['successCallbackFunction'] = ajaxSuccess;
                        params['errorCallBackFunction'] = ajaxError;
                        doAjax(params);
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

});
$("#btnIn").click(function () {
    ResetClientForm();
    $("#divusername").show();
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").show();
    $("#detailsdata").hide();
    $("#Creatediv").removeClass('hidden');
});
$("#btncancel").click(function () {
    $("#detailsdata").show();
    $("#Creatediv").addClass('hidden');
    $("#chkCmp").prop("checked", false);
    $("#CompanyId").val("").trigger('change');
    $("#DivCompanyName").show();
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
function ResetClientForm() {
    $("#ClientName").val('');
    $("#UserName").val('');
    $("#ClientEmail").val('');
    $("#ClientWWW").val('');
    $("#ClientPhone").val('');
    $("#ClientMobile").val('');
    $("#Password").val('');
    $("#ConfirmPassword").val('');
    $("#Contact_Person").val('');
    $("#Client_PAN_No").val('');
    $("#ClientGST").val('');
    $("#ClientAddress").val('');
    $("#ClientIsactive").val("Select Status").trigger('change');
}