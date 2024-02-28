$(document).ready(function () {
    console.log("ready!");
    $(".Job").removeClass("active");
    $(".acls_Job").css("color", "white");
    $(".acls_Job").mouseover(function () {
        $(".acls_Job").css("color", "#6a6c6f");
    });
    $(".acls_Job").mouseout(function () {
        $(".acls_Job").css("color", "white");
    });

    $("#JobCount").TouchSpin({
        verticalbuttons: true
    });
    LoadData();
    oTable = $('#JobDatatabledetails').DataTable();
    oTable.columns(4).search($('#SearchJobStatus').val().trim());
    oTable.draw();
    
    var input = document.getElementById("txtSearch");
    input.addEventListener("keyup", function (event) {
        event.preventDefault();
        if (event.keyCode === 13) {
            document.getElementById("btnSearch").click();
        }
    });


    var inputbtn = document.getElementById("btnsubmit");
    inputbtn.addEventListener("keyup", function (eventbtn) {
        if (eventbtn.keyCode === 13) {
            eventbtn.preventDefault(); 
        }
    });

    $('#btnSearch').click(function () {
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.columns(4).search($('#SearchJobStatus').val().trim());
        oTable.draw();
        setTimeout($.unblockUI, 2000);
        setTimeout(function () { $(".details-control").text(''); }, 4000);
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
        toastr.error('Error - Something is wrong...!!!');
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
        $("#JM_ClientId").val(data.successlist[0].Data.JM.ClientId).trigger('change');
        $("#JobId").val(data.successlist[0].Data.JM.JobId);
        $("#JM_JobName").val(data.successlist[0].Data.JM.JobName);
        $("#JM_JobDescription").val(data.successlist[0].Data.JM.JobDescription);
        $("#JobCount").val(data.successlist[0].Data.JM.JobCount);
        $("#JM_JobTargetDate").val(data.successlist[0].Data.JM.JobTargetDate);
        $("#Years_of_Exp").val(data.successlist[0].Data.JM.Years_of_Exp);
        $("#Location").val(data.successlist[0].Data.JM.Location);
        $("#Contact_Person").val(data.successlist[0].Data.JM.Contact_Person);
        var Jstatus = (data.successlist[0].Data.JM.JobStatus).toString();
        $("#JobStatus").val(Jstatus).trigger('change');
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
    if (e.type === 13) {
        e.preventDefault();
        return false;
    }
   
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
            params['url'] = BaseUrl + 'odata/Jobs';
            params['requestType'] = "POST";
            params['data'] = $('#submitform').serialize();
            params['successCallbackFunction'] = ajaxSuccess;
            params['errorCallBackFunction'] = ajaxError;
            doAjax(params);
            return false;
        }
    });
    setTimeout($.unblockUI, 2000);
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
    params['url'] = BaseUrl + 'odata/Jobs(' + id + ')';
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
            var id = $("#JobId").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Jobs(' + id + ')';
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
    params['url'] = BaseUrl + 'odata/Jobs(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$(document).on("click", ".btnclone", function () {
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
    params['url'] = BaseUrl + 'odata/Jobs(' + id + ')';
    params['requestType'] = "PATCH";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$(document).on("click", ".btnGetCandidateByjob", function () {
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
    $.ajax({
        url: '/Master/GetCandidateByJob?Jobid=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.Error === true) {
                toastr.error('Error - ' + result.Message);
            } else {
                $("#detailsdatatbl").removeClass('hidden');
                $("#detailsdata").hide();
                $("#CreateJobdiv").hide();
                $('#tblDatatable').dataTable({
                    destroy: true,
                    data: result.Data,
                    columns: [
                        { data: 'CandidateName' },
                        { data: 'ClientDesignation' },
                        { data: 'CandidateExperience' },
                        { data: 'Education' },
                        { data: 'CandidateDOB' },
                        { data: 'CandidateLocation' },
                        {
                            data: null,
                            className: "center",
                            "render": function (data, type, row, meta) {
                                return '<button type="button" data-data="' + data.CandidateId + '" title="Activity" class="btn btn-primary navbar-btn btn-xs btnactivity"><i class="fa fa-history" aria-hidden="true"></i></button>';
                            }
                        }
                    ],
                    dom: "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
                    "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                    buttons: [
                    ]
                });
            }
            setTimeout($.unblockUI, 2000);
            return false;
        },
        error: function (err) {
            toastr.error(err.statusText);
            setTimeout($.unblockUI, 2000);
            return false;
        }
    });
});
$(document).on("click", ".btnactivity", function () {
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
    $.ajax({
        url: '/Master/GetCandidateActivity?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            $('#myPartialContainer').html(result);
            $(".clsUpdateActDiv").hide();
            $('#sidebar-right').modal('toggle');
            setTimeout($.unblockUI, 2000);
        },
        error: function (err) {
            toastr.error(err.statusText);
            setTimeout($.unblockUI, 2000);
        }
    });
});
$("#btnInjobist").click(function () {
    $("#detailsdatatbl").addClass('hidden');
    $("#detailsdata").show();
    $("#CreateJobdiv").show();
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
            "url": "/Master/JobLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "JobName", "name": "JobName" },
            { "data": "JobCount", "name": "JobCount" },
            { "data": "CompanyName", "name": "CompanyName" },
            { "data": "ClientName", "name": "ClientName" },
            { "data": "uniqueJobid", "name": "uniqueJobid" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    return JobStatus = data.JobStatus === true ? '<lable class="label label-success">Active</lable>' : '<lable class="label label-danger">In-Active</lable>';
                }
            },
            { "data": "LastUpdateDateString", "name": "LastUpdateDate" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsViewBtn = IsView === "True" ? '<button class="btn btn-warning btn-xs btnjobView" title="View job" type="button" id="" data-data="' + data.JobId + '"><i class="fa fa-eye" aria-hidden="true"></i></button>' : "";
                    var IsAddBtn = IsAdd === "True" ? '| <button class="btn btn-success btn-xs btnclone" title="clone job" type="button" id="" data-data="' + data.JobId + '"><i class="fa fa-clone" aria-hidden="true"></i></button>' : "";
                    var IsEditBtn = IsEdit === "True" ? '| <button class="btn btn-info btn-xs btnedit" type="button" title="Edit" id="" data-data="' + data.JobId + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" title="Delete" id="" data-data="' + data.JobId + '"><i class="fa fa-trash-o"></i></button>' : "";
                    var IsClientjob = comCurrentRole === "CLIENT" ? ' | <button class="btn btn-warning btn-xs btnGetCandidateByjob" title="Job wise Candidate" type="button" id="" data-data="' + data.JobId + '"><i class="fa fa-address-book-o" aria-hidden="true"></i></button>' : "";
                    return IsViewBtn + IsAddBtn + IsEditBtn + IsDeleteBtn + IsClientjob;
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
    $("#JobDatatabledetails_filter").hide();
    $('#loading').html("").hide();

    setTimeout($.unblockUI, 2000);

    //$.ajax({
    //    url: '/Master/GetActivityClients',
    //    type: "GET",
    //    contentType: false,
    //    processData: false,
    //    success: function (result) {
    //        console.log(result);
    //        if (result.Data) {
    //            $("#ClientId").empty();
    //            $("#ClientId").append($("<option></option>").val(0).html("Select Client"));
    //            $.each(result.Data, function (data, value) {
    //                $("#ClientId").append($("<option></option>").val(value.ClientId).html(value.ClientName));
    //            });
    //        }
    //        //toastr.success(result);
    //    },
    //    error: function (err) {
    //        toastr.error(err.statusText);
    //    }
    //});

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Jobs';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}

function resetform() {
    $("#JM_ClientId").val("").trigger('change');
    $("#JM_JobName").val("");
    $("#JM_JobDescription").val("");
    $("#Years_of_Exp").val("");
    $("#JobCount").val("");
    $("#Location").val("");
    $("#Contact_Person").val("");
    $("#JM_JobTargetDate").val("");
    $("#JobStatus").val("").trigger('change');

}
$(document).on("click", ".btnjobView", function () {
    var id = $(this).data('data');
    $.ajax({
        url: '/Master/JobView?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result.Data);
            if (result.Data) {
                $("#lbljobtitle").text(result.Data.JobName);
                $("#lbljobdec").text(result.Data.JobDescription);
                $("#lbljobcnt").text(result.Data.JobCount);
                if (result.Data.JobStatus === true) {
                    $("#lbljobstatus").text("Active");
                } else {
                    $("#lbljobstatus").text("In-Active");
                }
                $("#lbllocation").text(result.Data.Location);
                $("#lblContactPerson").text(result.Data.Contact_Person);
                $("#lbljobdate").text(result.Data.JobTargetDate);
                $("#jobuniqid").text(result.Data.uniqueJobid);
                $("#lbljobclient").text(result.Data.ClientName);
                $('#JobView').modal('show');
            }
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});

