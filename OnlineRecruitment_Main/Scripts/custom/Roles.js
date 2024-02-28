
function ajaxSuccess(data) {
    console.log("Success");
    console.log(data);
    if (data.Message === "Inserted") {
        toastr.success('Success - Role Submitted Successfully...!!!');
        $("#Name").val(" ");
        LoadData();
    } else if (data.Message === "Update") {
        toastr.success('Success - Role Updated Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnRolesubmit").show();
        $("#Name").val(" ");
        LoadData();
    } else if (data.Message === "Delete") {
        toastr.success('Success - Role Deleted Successfully...!!!');
        LoadData();
    }
    else if (data.Message === "Edit") {
        $("#Name").val(data.Name);
        $("#Id").val(data.Id);
        $("#btnUpdate").removeClass('hidden');
        $("#btnRolesubmit").hide();
        LoadData();
    }
    else if (data.Error) {
        console.log("Error");
        toastr.error('Error -' + data.Message);
        $("#Name").val(" ");
        LoadData();
    }
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    $('#tblDatatable').dataTable({
        destroy: true,
        data: data,
        columns: [
            { data: 'Name' },
            { data: 'LastUpdatedBy' },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button"  data-data="' + data.Id + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" title="Delete" data-data="' + data.Id + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    var PermissionBtn = ' | <button class="btn btn-warning  btn-xs btnpermission" type="button" title="Permission" data-rolename="' + data.Name + '"  data-data="' + data.Id + '"><i class="fa fa-users" aria-hidden="true"></i> <span class="bold"></span></button>';
                    return IsEditBtn + PermissionBtn + IsDeleteBtn;
                }
            }
        ],
        dom: "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitRoles',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitRoles',
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
    if ($("#IsPrint").val() === "False") {
        $(".buttons-print").hide();
    }
    if ($("#IsExport").val() === "False") {
        $(".buttons-csv").hide();
    }
}

function ajaxError(data) {
    console.log(data);
    console.log("Error");
    toastr.error('Error - Something is wrong...!!!');
}
//function ajaxInfo(data) {
//    // console.log(data.value);
//    console.log("Info");
//    toastr.error('Error - Something is wrong...!!!');
//}

$("#btnRolesubmit").on("click", function (e) {
    debugger;
    $("#submitRoleData").validate({
        submitHandler: function (form) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = '/Account/CreateRoles';
            params['requestType'] = "POST";
            params['data'] = $('#submitRoleData').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            // params['completeCallbackFunction'] = ajaxInfo;
            doAjax(params);
            return false;
        }
    });

});

function LoadData() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/RolesList';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
}
$(document).ready(function () {
    console.log("ready!");
    $(".Roles").removeClass("active");
    $(".acls_Roles").css("color", "white");
    $(".acls_Roles").mouseover(function () {
        $(".acls_Roles").css("color", "#6a6c6f");
    });
    $(".acls_Roles").mouseout(function () {
        $(".acls_Roles").css("color", "white");
    });
    LoadData();
    $(function () {
        $('#tblpermission').footable();
    });
});
$(document).on("click", ".btnedit", function () {

    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/GetRolById';
    params['requestType'] = "GET";
    params['data'] = { Key: id };
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});

$(document).on("click", ".btndelete", function () {

    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/DeleteRoles';
    params['requestType'] = "GET";
    params['data'] = { Key: id };
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function (e) {
    // $("#submitRoleData").validate({
    //  submitHandler: function (form) {
    // var id = $("#Id").val();
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/UpdateRole';
    params['requestType'] = "POST";
    params['data'] = $('#submitRoleData').serialize();
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
    // }
    //  });
});
$(document).on("click", ".clspermission", function () {
    var AccesssId = $(this).data("id");
    //var Roleid = $("#PermissionRol").val();
    //var EntityId = $(this).data("entityid");
    var Flag = $(this).data("is");
    var value;
    if ($(this).prop("checked") === true) {
        value = true;
    } else {
        value = false;
    }
    $.ajax({
        url: '/Account/UpdateAccessPermisionByRole?AccesssId=' + AccesssId + "&Flag=" + Flag + "&value=" + value,
        type: "GET",
        contentType: false,
        success: function (result) {
            console.log(result);
            if (result.Error === false) {
                $(this).prop('checked', value);
                toastr.success('Success -' + result.Message);
            } else {
                toastr.error('Error -' + result.Message);
            }
            return false;
        },
        error: function (err) {
            toastr.error(err.statusText);
            return false;
        }

    });
});
$(document).on("click", ".btnpermission", function () {
    var id = $(this).data("data");
    var rolename = $(this).data("rolename");
    $("#PermissionRol").val(id);
    $.ajax({
        url: '/Account/GetAccessPermissionByRole?Key=' + id,
        type: "GET",
        contentType: false,
        success: function (result) {
            $("#lbluser").text(rolename);
            // $("#tblpermission tbody").remove();
            console.log(result);
            $.each(result.Data, function (data, value) {
                var IsAddCheck, IsViewCehck, IsEditCheck, IsDeleteCheck, IsPrintCheck, IsExportCheck;
                if (value.IsAdd === true) {
                    IsAddCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsAdd" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsAddCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsAdd" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                if (value.IsView === true) {
                    IsViewCehck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsView" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsViewCehck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsView" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                if (value.IsEdit === true) {
                    IsEditCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsEdit" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsEditCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsEdit" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                if (value.IsDelete === true) {
                    IsDeleteCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsDelete" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsDeleteCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsDelete" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                if (value.IsPrint === true) {
                    IsPrintCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsPrint" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsPrintCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsPrint" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                if (value.IsExport === true) {
                    IsExportCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" checked  data-Is="IsExport" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                } else {
                    IsExportCheck = '<td class="issue-info"><label class="switch" ><input type="checkbox" data-Is="IsExport" data-EntityId="' + value.EntityId + '" data-Id=' + value.Id + ' class="clspermission"><span class="slider round"></span></label></td >';
                }
                var markup = '<tr><td><b> ' + value.Entityname + '</b></td>' + IsAddCheck + '' + IsViewCehck + '' + IsEditCheck + '' + IsDeleteCheck + '' + IsPrintCheck + '' + IsExportCheck + '</tr>';
                $("#tblpermission tbody").append(markup);
                //$("#tfoot").show();
            });
            $("#maincontent").hide();
            $("#Accesscontent").removeClass('hidden');
        },
        error: function (err) {
            debugger;
            toastr.error(err.statusText);
        }
    });
});
$(document).on("click", "#btnaccessclose", function () {
    $("#maincontent").show();
    $("#Accesscontent").addClass('hidden');
    $("#tblpermission tbody tr").remove();
});
