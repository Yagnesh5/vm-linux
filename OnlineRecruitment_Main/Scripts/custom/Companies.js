function ajaxError(data) {
    console.log(data);
    console.log("Error");
    //toastr.error('Error - Something is wrong...!!!');
}
function CmpLoadData() {
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    $('#tblDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/CompanyLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "CompanyName", "name": "CompanyName" },
            { "data": "UserID", "name": "UserID" },
            { "data": "CompanyEmail", "name": "CompanyEmail"},
            { "data": "PhoneNumber", "name": "PhoneNumber" },
            { "data": "City", "name": "City" },
            { "data": "LastUpdateDateString", "name" :"Last Update Date"},
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    return IsEditBtn = data.IsActive === true ? '<lable class="label label-success">Active</lable>' : '<lable class="label label-danger">Deactive</lable>';
                }
            },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button" id="" data-data="' + data.ID + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" title="Delete" id="" data-data="' + data.ID + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    return IsEditBtn + IsDeleteBtn;
                }
            }

        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitCompany',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitCompany',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'colvis',
                columns: [0, 1, 2, 3, 4, 5, 6]
            }
        ],
        columnDefs: [{
            targets: -1,
            visible: true
        }]
    });
    $("#tblDatatable_filter").hide();
    $('#loading').html("").hide();

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Companies';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}
function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Message === "Inserted") {
        toastr.success('Success - Data Inserted Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#detailsdata").show();
        $("#creatediv").addClass('hidden');
        CmpLoadData();
        clear();
    } else if (data.successlist[0].Message === "Edit") {
        getcity(data.successlist[0].Data.State);
        setTimeout(function () {
            $("#State").val(data.successlist[0].Data.State).trigger('change');
        }, 1000);
        setTimeout(function () {
            $("#City").val(data.successlist[0].Data.City).trigger('change');
        }, 2000);
        $("#Country").val(data.successlist[0].Data.Country).trigger('change');
        $("#IsActive").val(data.successlist[0].Data.IsActive).trigger('change');
        $("#Id").val(data.successlist[0].Data.ID);
        $("#CompanyName").val(data.successlist[0].Data.CompanyName);
        $("#CompanyEmail").val(data.successlist[0].Data.CompanyEmail);
        $("#PhoneNumber").val(data.successlist[0].Data.PhoneNumber);
        $("#CompanyAddress").val(data.successlist[0].Data.CompanyAddress);
        $("#Pincode").val(data.successlist[0].Data.Pincode);
        $("#UserID").val(data.successlist[0].Data.UserID);
        $("#detailsdata").hide();
        $("#creatediv").removeClass('hidden');
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
    } else if (data.successlist[0].Message === "Update") {
        toastr.success('Success - Data Updated Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#detailsdata").show();
        $("#creatediv").addClass('hidden');
        CmpLoadData();
        clear();
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        CmpLoadData();
    } else if (data.successlist[0].Message === "County") {
        $.each(data.successlist[0].Data, function (data, value) {
            $("#Country").append($("<option></option>").val(value.Id).html(value.County));
        });
    } else if (data.successlist[0].Message === "State") {
        $("#State").empty();
         $("#State").append($("<option></option>").val(0).html("Select State"));
        $.each(data.successlist[0].Data, function (data, value) {
            $("#State").append($("<option></option>").val(value.Id).html(value.State));
        });
    } else if (data.successlist[0].Message === "City") {
        $("#City").empty();
        $("#City").append($("<option></option>").val(0).html("Select City"));
        $.each(data.successlist[0].Data, function (data, value) {
            $("#City").append($("<option></option>").val(value.Id).html(value.City));
        });
    } 
    else {
      
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
        }
    }
}
$("#btnIn").click(function () {
    $("#detailsdata").hide();
    $("#creatediv").removeClass('hidden');
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").show();
    clear();
    fillCuontriesData();
    return false;

});
$("#btncancel").click(function () {
    $("#detailsdata").show();
    $("#creatediv").addClass('hidden');
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").removeClass('hidden');
});
$("#btnsubmit").on("click", function (e) {
    var email = $("#CompanyEmail").val();
    //if (!validateEmail(email)) {
    //    toastr.error('Error - Enter valid email...!!!');
    //    $("#CompanyEmail").val();
    //    $("#CompanyEmail").focus();
    //    return false;
    //}
    $("#submitform").validate({
        rules: {
            CompanyName: {
                required: true,
                minlength: 3
            },
            CompanyEmail: {
                required: true,
                email: true
            }
        },
        submitHandler: function (form) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Companies';
            params['requestType'] = "POST";
            params['data'] = $('#submitform').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            doAjax(params);
            return false;
        }
    });
});
$(document).on("click", ".btnedit", function () {
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl +'odata/Companies(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;    
});

function fillCuontriesData() {
    $.ajax({
        type: "POST",
        url: "/Master/GetCountries",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            debugger;
            var arr = $.makeArray(response.data);
            for (var i = 0; i < arr.length; i++)
                $('#Country').append(new Option(arr[i].Country1, arr[i].Id))

        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });

}
$('#State').change(function () { 
    $.ajax({
        type: "POST",
        url: "/Master/GetStats/1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var arr = $.makeArray(response.data);
            for (var i = 0; i < arr.length; i++)
                $('#State').append(new Option(arr[i].State1, arr[i].Id))
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$('#City').change(function () {
    $.ajax({
        type: "POST",
        url: "/Master/GetCities/1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            debugger;
            var arr = $.makeArray(response.data);
            for (var i = 0; i < arr.length; i++)
                $('#City').append(new Option(arr[i].City1, arr[i].Id))

        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});

$("#btnUpdate").on("click", function (e) {
    var email = $("#CompanyEmail").val();
    //if (!validateEmail(email)) {
    //    toastr.error('Error - Enter valid email...!!!');
    //    $("#CompanyEmail").val();
    //    $("#CompanyEmail").focus();
    //    return false;
    //}
    $("#submitform").validate({
        rules: {
            CompanyName: {
                required: true,
                minlength: 3
            },
            CompanyEmail: {
                required: true,
                email: true
            }
        },
        submitHandler: function (form) {
            var id = $("#Id").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Companies(' + id + ')';
            params['requestType'] = "PUT";
            params['data'] = $('#submitform').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            doAjax(params);
            return false;
        }
    });
});
$(document).on("click", ".btndelete", function () {
    var id = $(this).data("data");
    swal({
        title: "Are you sure?",
        text: "Your company and relevant data will also be deleted",
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
                params['url'] = BaseUrl + 'odata/Companies(' + id + ')';
                params['requestType'] = "DELETE";
                params['successCallbackFunction'] = ajaxSuccess;
                params['errorCallBackFunction'] = ajaxError;
                doAjax(params);
                swal("Deleted!", "Your company and relevant data has been deleted.", "success");
                return false;
            } else {
                swal("Cancelled", "Your company and relevant data is safe :)", "error");
            }
        });
});

$(document).ready(function () {
    console.log("ready!");
    CmpLoadData();
    GetCounty();
    $("#PhoneNumber").mask("(999) 999-9999");
    $("#Pincode").mask("999999");
    $(".Companies").removeClass("active");
    $(".acls_Companies").css("color", "white");
    $(".acls_Companies").mouseover(function () {
        $(".acls_Companies").css("color", "#6a6c6f");
    });
    $(".acls_Companies").mouseout(function () {
        $(".acls_Companies").css("color", "white");
    });
    oTable = $('#tblDatatable').DataTable();

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

function GetCounty() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Countries';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
}

$(document).on("change", "#Country", function () {
    $("#State").empty();
    $("#State").append($("<option></option>").val(0).html("Select State"));
    $("#City").empty();
    $("#City").append($("<option></option>").val(0).html("Select City"));
    $("#State").val("0").trigger('change');
    $("#City").val("0").trigger('change');
    if ($("#Country").val() !== "0") {
        getstate($(this).val());
    }
    return false;
});
$(document).on("change", "#State", function () {
    $("#City").empty();
    $("#City").append($("<option></option>").val(0).html("Select City"));
    $("#City").val("0").trigger('change');
    if ($("#State").val() !== "0") {
        getcity($(this).val());
    }
    return false;
});
function getstate(id) {

    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/States(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
}

function getcity(id) {

    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Cities(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
}
function clear() {
    $("#CompanyName").val('');
    $("#CompanyEmail").val('');
    $("#PhoneNumber").val('');
    $("#Country").val("0").trigger('change');
    $("#State").val("0").trigger('change');
    $("#City").val("0").trigger('change');
    $("#Pincode").val('');
    $("#CompanyAddress").val('');

}
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}