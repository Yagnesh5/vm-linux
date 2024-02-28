$(document).ready(function () {
    console.log("ready!");
    $(".Skills").removeClass("active");
    $(".acls_Skills").css("color", "white");
    $(".acls_Skills").mouseover(function () {
        $(".acls_Skills").css("color", "#6a6c6f");
    });
    $(".acls_Skills").mouseout(function () {
        $(".acls_Skills").css("color", "white");
    });

    $("#JobCount").TouchSpin({
        verticalbuttons: true
    });
    LoadData();
    oTable = $('#JobDatatabledetails').DataTable();
    $('#btnSearch').click(function () {
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.draw();
    });
});
$("#btnIn").click(function () {
    $("#detailsdata").hide();
    $("#creatediv").removeClass('hidden');
    resetform();
});
$("#btncancel").click(function () {
    $("#detailsdata").show();
    $("#creatediv").addClass('hidden');
});

function ajaxError(data) {
    setTimeout($.unblockUI, 2000);
    console.log(data);
    console.log("Error");
    resetform();
    if (data.statusText) {
        debugger;
        toastr.error(data.responseJSON.errors[0].Message);
    }
    $("#myInputDate").val('');
}
function ajaxSuccess(data) {
    setTimeout($.unblockUI, 2000);
    resetform();
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Message === "Inserted") {
        toastr.success('Success - Data Inserted Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        location.reload();
    } else if (data.successlist[0].Message === "Edit") {
        $("#SkillId").val(data.successlist[0].Data.SkillId).trigger('change');
        $("#SkillId").val(data.successlist[0].Data.SkillId);
        $("#SkillName").val(data.successlist[0].Data.SkillName);
        var Jstatus = (data.successlist[0].Data.Active).toString();
        $("#Active").val(Jstatus).trigger('change');
        $("#creatediv").removeClass('hidden');
        $("#detailsdata").hide();
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
    } else if (data.successlist[0].Message === "Update") {
        toastr.success('Success - Data Updated Successfully...!!!');
        location.reload();
        //QueansLoadData();
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        location.reload();
    } else {
        LoadData();
        //var newList = [];
        //$.each(data.successlist[0].Data.filter(
        //    function (el) {
        //        if (el.CompanyId === parseInt(currentCompanyId)) {
        //            newList.push(el);
        //        }
        //    }),
        //);
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
        }
    }
}
$("#btnsubmit").on("click", function (e) {
    $("#submitform").validate({
        submitHandler: function (form) {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                }
            });
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/SkillMasters';
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
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/SkillMasters(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function (e) {
    $("#submitform").validate({
        submitHandler: function (form) {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                }
            });
            var id = $("#SkillId").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/SkillMasters(' + id + ')';
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
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });

    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/SkillMasters(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    LoadData();
    return false;
});



function LoadData() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    var IsAdd = $("#IsAdd").val();
    var IsView = $("#IsView").val();
    $('#JobDatatabledetails').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/GetSkillData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "SkillName", "name": "SkillName" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    return Active = data.Active === true ? '<lable class="label label-success">Active</lable>' : '<lable class="label label-danger">In-Active</lable>';
                }
            },
            { "data": "LastUpdateDateString", "name": "LastUpdateDate" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '| <button class="btn btn-info btn-xs btnedit" type="button" title="Edit Skill" id="" data-data="' + data.SkillId + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" title="Delete" id="" data-data="' + data.SkillId + '"><i class="fa fa-trash-o"></i></button>' : "";
                    var IsClientjob = comCurrentRole === "CLIENT" ? ' | <button class="btn btn-warning btn-xs btnGetCandidateByjob" title="Candidate Activity" type="button" id="" data-data="' + data.SkillId + '"><i class="fa fa-address-book-o" aria-hidden="true"></i></button>' : "";
                    return  IsEditBtn + IsDeleteBtn + IsClientjob;
                }
            }

        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'VrecruitJobs',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'VrecruitJobs',
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
    //$("#JobDatatabledetails_filter").hide();
    $('#loading').html("").hide();

    setTimeout($.unblockUI, 2000);
}

function resetform() {
    $("#SkillId").val("").trigger('change');
    $("#SkillName").val("");
    $("#Active").val("").trigger('change');

}


