$(document).ready(function () {
    console.log("ready!");
    $(".QuestionGroup").removeClass("active");
    $(".acls_QuestionGroup").css("color", "white");
    $(".acls_QuestionGroup").mouseover(function () {
        $(".acls_QuestionGroup").css("color", "#6a6c6f");
    });
    $(".acls_QuestionGroup").mouseout(function () {
        $(".acls_QuestionGroup").css("color", "white");
    });
    QueLoadData();
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

});

function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Code === "0") {
        toastr.error('Error - ' + data.successlist[0].Message);
    }
    if (data.successlist[0].Message === "Inserted") {
        toastr.success('Success - Data Inserted Successfully...!!!');
        QueLoadData();
        resetwuegroupform();
        location.reload();
    } else if (data.successlist[0].Message === "Edit") {
        resetwuegroupform();
        $("#ID").val(data.successlist[0].Data.ID);
        $("#GroupName").val(data.successlist[0].Data.GroupName);
        if (comCurrentRole === "SUPER ADMIN") {
            if (data.successlist[0].Data.CompanyId) {
                $("#chkCmp").prop("checked", true);
                $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');
                $("#DivCompanyName").show();
            }
        } else {
            $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');;
        }
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
    } else if (data.successlist[0].Message === "Update") {
        toastr.success('Success - Data Updated Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        QueLoadData();
        resetwuegroupform();
        location.reload();
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        QueLoadData();
    } else if (data.successlist[0].Message === "BindDrp") {
        $.each(data.successlist[0].Data, function (data, value) {
            $("#CompanyId").append($("<option></option>").val(value.ID).html(value.CompanyName));
        });
    } else {
        console.log(data.successlist[0].Data);
        var IsEdit = $("#IsEdit").val();
        var IsDelete = $("#IsDelete").val();
        if (comCurrentRole === "SUPER ADMIN") {
            $('#tblDatatable').dataTable({
                destroy: true,
                data: data.successlist[0].Data,
                columns: [
                    { data: 'GroupName' },
                    { data: 'CompanyName' },
                    { data: 'LastUpdateBy' },
                    {
                        data: null,
                        className: "center",
                        "render": function (data, type, row, meta) {
                            var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button" id="" data-data="' + data.ID + '"><i class="fa fa-edit"></i> </button>' : "";
                            var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" id="" title="Delete" data-data="' + data.ID + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                            return IsEditBtn + IsDeleteBtn;
                        }
                    }
                ],
                dom: "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>tp",
                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                buttons: [
                    {
                        extend: 'csv', title: 'VrecruitQuestionGroup',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print', title: 'VrecruitQuestionGroup',
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

        } else {
            var newList = [];
            $.each(data.successlist[0].Data.filter(
                function (el) {
                    if (el.CompanyId === parseInt(currentCompanyId)) {
                        newList.push(el);
                    }
                }),
            );
            $('#tblDatatable').dataTable({
                destroy: true,
                data: newList,
                columns: [
                    { data: 'GroupName' },
                    { data: 'LastUpdateBy' },
                    {
                        data: null,
                        className: "center",
                        "render": function (data, type, row, meta) {

                            var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button" id="" data-data="' + data.ID + '"><i class="fa fa-edit"></i> </button>' : "";
                            var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" id="" title="Delete" data-data="' + data.ID + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                            return IsEditBtn + IsDeleteBtn;
                        }
                    }
                ],
                dom: "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>tp",
                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                buttons: [
                    {
                        extend: 'csv', title: 'VrecruitQuestionGroup',
                        exportOptions: {
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'print', title: 'VrecruitQuestionGroup',
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
    }
}
function ajaxError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}
function QueLoadData() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/QuestionGroups';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
}
$("#btnsubmit").on("click", function (e) {
    $("#submitform").validate({
        submitHandler: function (form) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/QuestionGroups';
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
    params['url'] = BaseUrl + 'odata/QuestionGroups(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function () {
    $("#submitform").validate({
        submitHandler: function (form) {
            var id = parseInt($("#ID").val());
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/QuestionGroups(' + id + ')';
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
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/QuestionGroups(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
function resetwuegroupform() {
    $("#GroupName").val('');
    $("#chkCmp").prop("checked", false);
    $("#CompanyId").val('').trigger('change');
    $("#DivCompanyName").hide();
    $("#ID").val('');
}