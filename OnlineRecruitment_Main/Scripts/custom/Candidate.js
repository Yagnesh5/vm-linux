$(document).ready(function () {
    console.log("ready!");
    $("#MainTestResultDive").hide();
    CandidateLoadData();
    $(".Candidate").removeClass("active");
    $(".acls_Candidate").css("color", "white");
    $(".acls_Candidate").mouseover(function () {
        $(".acls_Candidate").css("color", "#6a6c6f");
    });
    $(".acls_Candidate").mouseout(function () {
        $(".acls_Candidate").css("color", "white");
    });
    $("#CandidatePhone").mask("(999) 999-9999");
    $("#CandidatePhone2").mask("(999) 999-9999");
    $(".modal a").not(".dropdown-toggle").on("click", function () {
        $(".modal").modal("hide");
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
        setTimeout(function () { $(".details-control").text(''); }, 4000);
    });

    var currentRole = $("#ClientCurrentRole").val();
    if (currentRole == "CLIENT") {
        var id = $("#ClientId").val();
        getJobBasedOnClient(id);
    }
});

function ajaxError(data) {
    console.log("Error");
    setTimeout($.unblockUI, 2000);
    var Data = jQuery.parseJSON(data.responseText);
    console.log(Data.errors[0].Message);
    //debugger;
    if (Data !== null) {
        if (Data.errors[0].Message) {
            toastr.error('Error - ' + Data.errors[0].Message);
        }
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}
function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    setTimeout($.unblockUI, 2000);
    if (data.successlist[0].Message === "Inserted") {
        debugger;
        uploadResume(data.successlist[0].Data);
        toastr.success('Success - Data Inserted Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#detailsdata").show();
        $("#createDiv").addClass('hidden');
        popcandidateInfo();
        insertClientCandidateMapping(data.successlist[0].Data);
        resetform();
        oTable = $('#tblDatatable').DataTable();
        $('#btnSearch').trigger('click');
    } else if (data.successlist[0].Message === "Edit") {
        resetform();
        console.log(data.successlist[0].Data.CandidateId);
        debugger;
        $("#uprestext").hide();
        //$("#editresume").attr("href", "../../UploadedResume/" + data.successlist[0].Data.CandidateResume);
        $("#editresume").attr("href", "https://eqhire.blob.core.windows.net/eqhires/UploadedResume/" + data.successlist[0].Data.CandidateResume);
        $("#editresume").hide();
        var ResumeLinks = "";
        /* creating HTML for resume*/
        if (data.successlist[0].Data.CandidateResumesList.length > 0) {
            for (var i = 0; i < data.successlist[0].Data.CandidateResumesList.length; i++) {
                var element = data.successlist[0].Data.CandidateResumesList[i];
                //if (i == 0) {
                //var url = "../../UploadedResume/" + element.CandidateResume;
                var url = "https://eqhire.blob.core.windows.net/eqhires/UploadedResume/" + element.CandidateResume;
                ResumeLinks = ResumeLinks + "<strong><a href='" + url + "' class=\"clsdownload\" download=\"\" id=\"editresume\"><u>Download Resume!  " + element.CreatedDateString + "</u></a></strong><br/>";
            }
            $("#editresume").hide();
        }
        else {
            //$("#editresume").attr("href", "../../UploadedResume/" + data.successlist[0].Data.CandidateResume);
            $("#editresume").attr("href", "https://eqhire.blob.core.windows.net/eqhires/UploadedResume/" + data.successlist[0].Data.CandidateResume);
            $("#editresume").show();
        }

        var strCanidateList = "";
        if (data.successlist[0].Data.ClientList.length > 0) {
            $("#selectedClientId").empty();
            for (var i = 0; i < data.successlist[0].Data.ClientList.length; i++) {
                var element = data.successlist[0].Data.ClientList[i];
                strCanidateList += "<div class='select7_item'> <div data-option-value='" + element.ClientId + "' class='select7_content'>" + element.ClientName + "</div> ";
                strCanidateList += "<div class='select7_del' onclick='remove_selected_item(this, event);'>✖</div> </div>"
            }
        }
     
        
        $("#divResumeList").html(ResumeLinks);
        $("#selectedClientId").html(strCanidateList);
        //<strong><a href="../../UploadedResume/Candidate_2_ResumeVivekShinde.pdf" class="clsdownload" download="" id="editresume"><u>Download Resume!</u></a></strong>
        $("#editresume").removeClass('hidden');
        $("#CandidateId").val(data.successlist[0].Data.CandidateId);
        $("#CandidateName").val(data.successlist[0].Data.CandidateName);
        $("#CandidateSkill").val(data.successlist[0].Data.CandidateSkill);
        $("#CandidateDOB").val(data.successlist[0].Data.CandidateDOB);
        $("#CandidateExperience").val(data.successlist[0].Data.CandidateExperience);
        $("#CandidateExperienceYears").val(data.successlist[0].Data.CandidateExperienceYears);
        $("#CandidateExperienceMonths").val(data.successlist[0].Data.CandidateExperienceMonths);
        $("#CandidatePhone").val(data.successlist[0].Data.CandidatePhone);
        $("#CandidatePhone2").val(data.successlist[0].Data.CandidatePhone2);
        $("#CandidateEmail").val(data.successlist[0].Data.CandidateEmail);
        $("#CandidateLocation").val(data.successlist[0].Data.CandidateLocation);
        $("#CandidateLastJobChange").val(data.successlist[0].Data.CandidateLastJobChange);
        $("#PreviousDesignation").val(data.successlist[0].Data.PreviousDesignation);
        $("#CurrentOrganisation").val(data.successlist[0].Data.CurrentOrganisation);
        $("#SkypeId").val(data.successlist[0].Data.SkypeId);
        $("#CurrentCTC").val(data.successlist[0].Data.CurrentCTC);
        $("#ExpectedCTC").val(data.successlist[0].Data.ExpectedCTC);
        var buyoptionval = data.successlist[0].Data.BuyoutOption;
        if (buyoptionval === true) {
            $("#BuyoutOptionYes").prop('checked', true);
        } else {
            $("#BuyoutOptionNo").prop('checked', true);
        }
        $("#Education").val(data.successlist[0].Data.Education);
        $("#OffersInHand").val(data.successlist[0].Data.OffersInHand);
        $("#NoticePeriod").val(data.successlist[0].Data.NoticePeriod);
        $("#ClientDesignation").val(data.successlist[0].Data.ClientDesignation);
        $("#Remarks").val(data.successlist[0].Data.Remarks);
        $("#Relevant_Exp").val(data.successlist[0].Data.Relevant_Exp);
        $("#Preferred_Location").val(data.successlist[0].Data.Preferred_Location);
        $("#Last_Job_change_reason").val(data.successlist[0].Data.Last_Job_change_reason);
        $("#createDiv").removeClass('hidden');
        $("#detailsdata").hide();
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
        $("#CandidateResume").prop('required', false);
    } else if (data.successlist[0].Message === "Update") {
        uploadResume($("#CandidateId").val());
        toastr.success('Success - Data Updated Successfully...!!!');
        $("#btnupdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#detailsdata").show();
        $("#createDiv").addClass('hidden');
        popcandidateInfo();
        insertClientCandidateMapping($("#CandidateId").val());
        //CandidateLoadData();
        resetform();
        oTable = $('#tblDatatable').DataTable();
        $('#btnSearch').trigger('click');
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        //CandidateLoadData();
        oTable = $('#tblDatatable').DataTable();
        $('#btnSearch').trigger('click');
    } else if (data.successlist[0].Message === "Send") {
        toastr.success('Success - Activity Assigned Successfully...!!!');
        $('#myModal').modal('hide');
        //reset controls
        $("#ActivityType1").prop('selectedIndex', 0);
        $("#TestName").prop('selectedIndex', 0);
        $("#ClientId").prop('selectedIndex', 0);
        $("#JobId").prop('selectedIndex', 0);
        $("#Activity1").val("");
        //--------------
        CandidateLoadData();
    } else if (data.successlist[0].Message === "Activity") {
        $.each(data.successlist[0].Data.ActivityTypeList, function (data, value) {
            $("#detailsActivityType").append($("<option></option>").val($.trim(value.ActivityType1)).html($.trim(value.ActivityType1)));
        });
        $("#ActivityId").val(data.successlist[0].Data.ActivityId);
        $('#detailsActivityType').val(data.successlist[0].Data.ActivityType).trigger('change');
        $('#Activity').val(data.successlist[0].Data.Activity1);
        $('#ActivityDate').val(data.successlist[0].Data.ActivityDate);
        $('#ActivityRemark').val(data.successlist[0].Data.ActivityRemark);
        $('#ActivityComplete').val(data.successlist[0].Data.ActivityComplete).trigger('change');
        $('#ActivityDetailsModal').modal('toggle');
    } else if (data.successlist[0].Message === "ActivityUpdate") {
        toastr.success('Success - Activity Updated Successfully...!!!');
        $('#ActivityDetailsModal').modal('hide');
        $('#sidebar-right').modal('hide');
    } else {
        //var newList = [];
        //$.each(data.successlist[0].Data.filter(
        //    function (el) {
        //        if (el.CompanyId === parseInt(currentCompanyId)) {
        //            newList.push(el);
        //        }
        //    }),
        //);
        CandidateLoadData();
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
        }
    }

}
function getJobBasedOnClient(id) {
    $.ajax({
        url: '/Master/GetClientJob?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        // data: { key: $("#CanId").val() },
        success: function (result) {
            console.log(result);
            if (result.Data) {
                $("#JobId").empty();
                $("#JobId").append($("<option></option>").val(0).html("Select Job"));
                $.each(result.Data, function (data, value) {
                    $("#JobId").append($("<option></option>").val(value.JobId).html(value.JobName));
                });
            }
            //toastr.success(result);
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
}

function insertClientCandidateMapping(id) {
    if (window.FormData !== undefined) {
        var selectedClientsArray = [];
        
        // an array of objects containing values and text
        //get_selected_items('select-id');

        // gets only values
        var selectedClients = get_selected_items('select7', 'value');

    // gets only text
    //get_selected_items('select-id', 'text');

        //var selectedClients = document.getElementById("chkveg");
        var listLength = selectedClients.length;
        for (var i = 0; i < listLength; i++) {
                selectedClientsArray.push(selectedClients[i]);
        }

        var params = $.extend({}, doAjax_params_default);
        params['url'] = '/Master/CandidateClient?clientId=' + selectedClientsArray.toString() + '&candidateId=' + id;
        params['requestType'] = "POST";
        params['successCallbackFunction'] = ajaxClientGridSuccess;
        params['errorCallBackFunction'] = ajaxClientGridError;
        doAjax(params);
    }
}

function format(d) {
    console.log(d.BuyoutOption);
    var phonenum = d.CandidatePhone2 === null ? "-" : d.CandidatePhone2;
    var Byoption = d.BuyoutOption === true ? "Yes" : "No";
    var Education = d.Education === null ? "-" : d.Education;
    var CandidateSkill = d.CandidateSkill === null ? "-" : d.CandidateSkill;
    var CandidateExperience = d.CandidateExperience === null ? "-" : d.CandidateExperience;
    var CandidateLastJobChange = d.CandidateLastJobChange === null ? "-" : d.CandidateLastJobChange;
    var PreviousDesignation = d.PreviousDesignation === null ? "-" : d.PreviousDesignation;
    var CurrentOrganisation = d.CurrentOrganisation === null ? "-" : d.CurrentOrganisation;
    var CurrentCTC = d.CurrentCTC === null ? "-" : d.CurrentCTC;
    var ExpectedCTC = d.ExpectedCTC === null ? "-" : d.ExpectedCTC;
    var OffersInHand = d.OffersInHand === null ? "-" : d.OffersInHand;
    var NoticePeriod = d.NoticePeriod === null ? "-" : d.NoticePeriod;
    var SkypeId = d.SkypeId === null ? "-" : d.SkypeId;
    var CandidateLocation = d.CandidateLocation === null ? "-" : d.CandidateLocation;
    var Remarks = d.Remarks === null ? "-" : d.Remarks;
    return '<div class="row"> <div class="col-lg-12"><table  class="table table-bordered" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>Education:</td>' +
        '<td>' + Education + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Candidate Skill:</td>' +
        '<td>' + CandidateSkill + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Experience:</td>' +
        '<td>' + CandidateExperience + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Secondary Phone Number:</td>' +
        '<td>' + phonenum + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Last Job:</td>' +
        '<td>' + CandidateLastJobChange + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Last Designation:</td>' +
        '<td>' + PreviousDesignation + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Current Organisation:</td>' +
        '<td>' + CurrentOrganisation + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Current CTC (Lac(s) pa):</td>' +
        '<td>' + CurrentCTC + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Expected CTC (Lac(s) pa):</td>' +
        '<td>' + ExpectedCTC + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Offers In Hand:</td>' +
        '<td>' + OffersInHand + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Notice Period (Days):</td>' +
        '<td>' + NoticePeriod + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Skype Id:</td>' +
        '<td>' + SkypeId + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Buyout Option:</td>' +
        '<td>' + Byoption + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Address:</td>' +
        '<td>' + CandidateLocation + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Remarks:</td>' +
        '<td>' + Remarks + '</td>' +
        '</tr>' +
        '</table></div>';
}
$("#btnIn").click(function () {
    $("#detailsdata").hide();
    $("#createDiv").removeClass('hidden');
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").show();
    resetform();
});
$("#btncancel").click(function () {
    $("#detailsdata").show();
    $("#createDiv").addClass('hidden');
    $("#btnUpdate").addClass('hidden');
    $("#btnsubmit").show();
    resetform();
});

function resetform() {
    $("#CandidateId").val("");
    $("#CandidateName").val("");
    $("#CandidateSkill").val("");
    $("#CandidateDOB").val("");
    $("#CandidateExperienceYears").val("");
    $("#CandidateExperienceMonths").val("");
    $("#CandidatePhone").val("");
    $("#CandidatePhone2").val("");
    $("#CandidateEmail").val("");
    $("#CandidateLocation").val("");
    $("#CandidateLastJobChange").val("");
    $("#PreviousDesignation").val("");
    $("#CurrentOrganisation").val("");
    $("Education").val("");
    $("#CurrentCTC").val("");
    $("#ExpectedCTC").val("");
    $("#OffersInHand").val("");
    $("#ClientDesignation").val("");
    $("#NoticePeriod").val("");
    $("#SkypeId").val("");
    $("#Remarks").val("");
    $("#CandidateResume").val("");
    $("#editresume").addClass('hidden');
    $("#uprestext").show();
}
function CandidateLoadData() {
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    var ClientCurrentRole = $("#ClientCurrentRole").val();
    var strAttachment = "";
    $('#tblDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/CandidateLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "fnDrawCallback": function( oSettings ) {
            $(".details-control").text('');
        },
        "order": [[ 0, "desc" ]],
        "columns": [
            {
                "className": 'details-control',
                "data": "CandidateId",
                "name": "CandidateId",
                "autoWidth": true,
                "defaultContent": ''
            },
            { "data": "CandidateName", "name": "CandidateName", "autoWidth": true },
            { "data": "ClientDesignation", "name": "ClientDesignation", "autoWidth": true },
            { "data": "CandidateEmail", "name": "CandidateEmail", "autoWidth": true },
            { "data": "CandidatePhone", "name": "CandidatePhone", "autoWidth": true },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {                    
                    if (data.CandidateVideo !== null) {
                        strAttachment = '<a href="https://eqhire.blob.core.windows.net/eqhires/UploadedResume/' + data.CandidateResume + '"  data-data="' + data.CandidateResume + '" class="clsdownload" download>Resume</a><br>' +
                            '<div id="light">' +
                            '<a class="boxclose" id="boxclose" onclick="lightbox_close(this);"></a>' +
                            '<video id="VisaChipCardVideo" width="600" controls>' +
                            '<source src="../../UploadedVideo/' + data.CandidateVideo + '" type="video/mp4">' +
                            '</video>' +
                            '</div>' +

                            '<div id="fade" onClick="lightbox_close();"></div>' +

                            '<div>' +
                            '<a href="#" onclick="lightbox_open(this);">Watch video</a>' +
                            '</div>';
                    }
                    else {
                        strAttachment = '<a href="https://eqhire.blob.core.windows.net/eqhires/UploadedResume/' + data.CandidateResume + '"  data-data="' + data.CandidateResume + '" class="clsdownload" download>Resume</a>';
                    }
                    if (data.InterviewId !== null && data.InterviewId !== 0) {
                        strAttachment += '<div>' +
                        '<a href="#" data-data="' + data.InterviewId + '" class="btnInterviewVideo"">Interview Video</a>' +
                        '</div>';
                    }
                    return strAttachment;
                }
            },
            { "data": "LastUpdateDateString", "name": "LastUpdateDate", "autoWidth": true },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? '<button class="btn btn-info btn-xs btnedit" type="button" id="" title="Edit" data-data="' + data.CandidateId + '"><i class="fa fa-edit"></i> </button>' : "";
                    var IsDeleteBtn = IsDelete === "True" ? ' | <button class="btn btn-danger btn-xs btndelete" type="button" id="" title="Detete" data-data="' + data.CandidateId + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "";
                    var AssignJobBtn = ' | <button class="btn btn-success btn-xs btnJob" type="button" id="" title="Assign Activity" data-data="' + data.CandidateId + '"><i class="pe-7s-upload pe-7s-news-paper"></i> <span class="bold"></span></button>';
                    var HistoryBtn = ' | <button type="button" title="View Activity History" data-data="' + data.CandidateId + '" class="btn btn-primary navbar-btn btn-xs btnactivity"><i class="fa fa-history" aria-hidden="true"></i></button>';
                    var CanTestsList = ' | <button type="button" title="Candidate Test List" data-data="' + data.CandidateId + '" class="btn btn-warning navbar-btn btn-xs btncandidateTestlist"><i class="fa fa-id-card" aria-hidden="true"></i></button>';
                    return IsEditBtn + IsDeleteBtn + AssignJobBtn + HistoryBtn + CanTestsList;
                }
            }
        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                extend: 'csv', title: 'CandidateFile',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'CandidateFile',
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

    table = $('#tblDatatable').DataTable();
    $('#tblDatatable tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });
    setTimeout(function () { $(".details-control").text(''); }, 4000);

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/ClientsData?companyId=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientSuccess;
    params['errorCallBackFunction'] = ajaxClientError;
    doAjax(params);

    $.ajax({
        url: '/Master/GetActivityClients',
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            if (result.Data) {
                $("#ClientId").empty();
                if (ClientCurrentRole !== "CLIENT")
                $("#ClientId").append($("<option></option>").val(0).html("Select Client"));
                $.each(result.Data, function (data, value) {
                    $("#ClientId").append($("<option></option>").val(value.ClientId).html(value.ClientName));
                });
            }
            //toastr.success(result);
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Candidates';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}

$(document).on("click", ".btnInterviewVideo", function () {
   
    var id = $(this).data("data");
    window.location.href = "InterviewReport?InterviewId="+ id;
    return false;
});

$("#btnsubmit").on("click", function (e) {
    $("#submitform").validate({
        rules: {
            CandidateName: {
                required: true,
                minlength: 3
            },
            CandidateExperience: {
                required: true,
                number: true
            },
            CandidateExperienceYears: {
                required: true,
                number: true,
                maxlength: 2
            },
            CandidateExperienceMonths: {
                required: true,
                number: true,
                maxlength: 2
            },
            CurrentCTC: {
                required: true,
                number: true
            },
            ExpectedCTC: {
                required: true,
                number: true
            },
            OffersInHand: {
                required: true,
                number: true,
                maxlength: 2
            },
            max: {
                required: true,
                maxlength: 4
            },
            CandidateEmail: {
                required: true,
                email: true
            },
            NoticePeriod: {
                required: true,
                number: true,
                maxlength: 3
            }
        },
        submitHandler: function (form) {
            //if ($("#CandidateResume").val() === "") {
            //    toastr.error('Error - please upload your resume...!!!');
            //    $("#CandidateResume").focus();
            //    return false;
            //}
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
            var filepath = $("#CandidateResume").val();
            var filedata = filepath.split("\\");
            var fileName = filedata[2];
            var formData = $('#submitform').serializeArray();
            formData.push({ name: "CandidateResume", value: fileName });
            var buyoption = $("input:radio[name='BuyoutOption']:checked").val();
            formData.push({ name: "BuyoutOption", value: buyoption });

            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Candidates';
            params['requestType'] = "POST";
            params['data'] = formData;
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
    params['url'] = BaseUrl + 'odata/Candidates(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function (e) {

    $("#submitform").validate({
        rules: {
            CandidateName: {
                required: true,
                minlength: 3
            },
            CandidateExperience: {
                required: true,
                number: true
            },
            CandidateExperienceYears: {
                required: true,
                number: true,
                maxlength: 2
            },
            CandidateExperienceMonths: {
                required: true,
                number: true,
                maxlength: 2
            },
            CurrentCTC: {
                required: true,
                number: true
            },
            ExpectedCTC: {
                required: true,
                number: true
            },
            OffersInHand: {
                required: true,
                number: true,
                maxlength: 2
            },
            CandidateEmail: {
                required: true,
                email: true
            },
            max: {
                required: true,
                maxlength: 4
            },
            NoticePeriod: {
                required: true,
                number: true,
                maxlength: 3
            }
        },
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
            var formData = $('#submitform').serializeArray();
            var filepath = $("#CandidateResume").val();
            if (filepath !== "") {
                var filedata = filepath.split("\\");
                var fileName = filedata[2];
                formData.push({ name: "CandidateResume", value: fileName });
            }
            var buyoption = $("input:radio[name='BuyoutOption']:checked").val();
            formData.push({ name: "BuyoutOption", value: buyoption });
            var id = $("#CandidateId").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Candidates(' + id + ')';
            params['requestType'] = "PUT";
            params['data'] = formData;
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
    params['url'] = BaseUrl + 'odata/Candidates(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
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
    params['url'] = BaseUrl + 'odata/Candidates(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});

$(document).on("click", ".btnJob", function () {
    var id = $(this).data("data");
    $("#CanId").val(id);
    $('#myModal').modal('toggle');
});

$(document).on("change", "#ActivityType1", function () {
    debugger;
    var val = $(this).val();
    if (val === "TEST" || val === "ONLINE INTERVIEW") {
        $("#TestNameDiv").removeClass('hidden');
    } else {
        $("#TestNameDiv").addClass('hidden');

    }

});


$(document).on("change", "#ClientId", function () {

    var id = $(this).val();
    $.ajax({
        url: '/Master/GetClientJob?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        // data: { key: $("#CanId").val() },
        success: function (result) {
            console.log(result);
            if (result.Data) {
                $("#JobId").empty();
                $("#JobId").append($("<option></option>").val(0).html("Select Job"));
                $.each(result.Data, function (data, value) {
                    $("#JobId").append($("<option></option>").val(value.JobId).html(value.JobName));
                });
            }
            //toastr.success(result);
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});
$(document).on("keyup", "#s2id_autogen4_search", function () {
    var searchtext = $(this).val();
    var clientid = $("#ClientId").val();
    if (searchtext.length === 3) {
        $.ajax({
            url: '/master/getclientjob?key=' + clientid + "&searchtext=" + searchtext,
            type: "get",
            contenttype: false,
            processdata: false,
            success: function (result) {
                console.log(result);
                if (result.data) {
                    $("#JobId").empty();
                    $("#JobId").append($("<option></option>").val(0).html("Select Job"));
                    $.each(result.data, function (data, value) {
                        $("#jobid").append($("<option></option>").val(value.jobid).html(value.jobname));
                    });
                }
            },
            error: function (err) {
                toastr.error(err.statustext);
            }
        });
    }
});

$("#btnJobsubmit").on("click", function (e) {
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
    var formData = $('#submitform').serializeArray();
    var CandidateId = $("#CanId").val();
    var ClientId = $("#ClientId").val();
    var JobId = $("#JobId").val();
    var UserId = $("#UserId").val();
    var ActivityType = $("#ActivityType1").val();
    var TestLink = $("#TestName").val();
    var CompanyId = $("#CompanyId").val();
    var Activity = $("#Activity1").val();
    formData.push({ name: "CandId", value: CandidateId });
    formData.push({ name: "ClientId", value: ClientId });
    formData.push({ name: "JobId", value: JobId });
    formData.push({ name: "UserId", value: UserId });
    formData.push({ name: "CompanyId", value: CompanyId });
    formData.push({ name: "ActivityType", value: ActivityType });
    formData.push({ name: "TestLink", value: TestLink });
    formData.push({ name: "Activity1", value: Activity });
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Activities';
    params['requestType'] = "POST";
    params['data'] = formData;
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    
    return false;

});


function uploadResume(id) {

    if (window.FormData !== undefined) {

        var fileUpload = $("#CandidateResume").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        fileData.append('CandidateId', id);
        $.ajax({
            url: '/Master/UploadResume',
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                toastr.success(result);
            },
            error: function (err) {
                toastr.error(err.statusText);
            }
        });
    } else {
        toastr.error('Error - FormData is not supported...!!!');
    }
}

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
            $('.drpActivity').addClass('js-source-states');
            $('#sidebar-right').modal('toggle');
            setTimeout($.unblockUI, 2000);
        },
        error: function (err) {
            toastr.error(err.statusText);
            setTimeout($.unblockUI, 2000);
        }
    });
});
$(document).on("click", ".btnaddUppActivity", function () {
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
    var JobId = $(this).data("jobid");
    // $(".activityTextDiv_" + JobId).removeClass('hidden');
    var drpval = $("#drp_" + JobId).val();
    var ActDate = $("#ActivityDate_" + JobId).val();
    var Activitydata = $("#Activity_" + JobId).val();
    if (drpval === "") {
        toastr.error('Error - Select Activity...!!!');
    } else if (ActDate === "") {
        toastr.error('Error - Select Activity Date...!!!');
    } else if (Activitydata === "") {
        toastr.error('Error - Add Actitviy Deatils..!!!');
    } else {
        var formData = $('#submitform').serializeArray();

        var ClientId = $(this).data("clientid");
        var UserId = $(this).data("userid");
        var CompanyId = $(this).data("companyid");
        var CandidateId = $(this).data("data");
        var ActivityType = drpval;
        formData.push({ name: "CandId", value: CandidateId });
        formData.push({ name: "ClientId", value: ClientId });
        formData.push({ name: "JobId", value: JobId });
        formData.push({ name: "UserId", value: UserId });
        formData.push({ name: "CompanyId", value: CompanyId });
        formData.push({ name: "ActivityType", value: ActivityType });
        formData.push({ name: "ActivityDate", value: ActDate });
        formData.push({ name: "Activity1", value: Activitydata });
        $.ajax({
            url: '/Master/UpdateCandidateActivity',
            type: "GET",
            contentType: false,
            data: formData,
            success: function (result) {
                console.log(result);
                if (result.Error === true) {
                    toastr.error(result.Message);
                } else {
                    toastr.success(result.Message);
                    $('#sidebar-right').modal('hide');
                }
                //$('#myPartialContainer').html(result);
                //$('#sidebar-right').modal('toggle');
                setTimeout($.unblockUI, 2000);
            },
            error: function (err) {
                toastr.error(err.statusText);
                setTimeout($.unblockUI, 2000);
            }
        });
    }
});
$(document).on("click", ".clsActivityDetails", function () {
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
    var id = $(this).attr('id');
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Activities(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$(document).on("click", "#btActivityDetailsnsubmit", function () {
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
    var id = $("#ActivityId").val();
    var formData = $('#submitform').serializeArray();
    formData.push({ name: "ActivityType", value: $("#detailsActivityType").val() });
    formData.push({ name: "Activity1", value: $("#Activity").val() });
    formData.push({ name: "ActivityDate", value: $("#ActivityDate").val() });
    formData.push({ name: "ActivityRemark", value: $("#ActivityRemark").val() });
    formData.push({ name: "ActivityComplete", value: $("#ActivityComplete").val() });
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Activities(' + id + ')';
    params['requestType'] = "PUT";
    params['data'] = formData;
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$(document).on("change", ".drpActivity", function () {
    var id = $(this).data('data');
    // alert(id);
    $(".activityTextDiv_" + id).removeClass('hidden');

});


function popcandidateInfo() {
    var candidatename = $("#CandidateName").val();
    var totalexp = $("#CandidateExperienceYears").val() + '.' + $("#CandidateExperienceMonths").val();
    var Cuntorg = $("#CurrentOrganisation").val();
    var lblcrntdig = $("#ClientDesignation").val();
    var lbleduqua = $("#Education").val();
    var lblcrnctc = $("#CurrentCTC").val();
    var lblexpctc = $("#ExpectedCTC").val();
    var lblnotice = $("#NoticePeriod").val();
    var lblcrnloc = $("#CandidateLocation").val();
    var lblcrnpreloc = $("#Preferred_Location").val();

    $("#lblcandidatename").text(candidatename);
    $("#lbltotalexp").text(totalexp);
    $("#lblcurrntorg").text(Cuntorg);
    $("#lblcrntdig").text(lblcrntdig);
    $("#lbleduqua").text(lbleduqua);
    $("#lblcrnctc").text(lblcrnctc);
    $("#lblexpctc").text(lblexpctc);
    $("#lblnotice").text(lblnotice);
    $("#lblcrnloc").text(lblcrnloc);
    $("#lblcrnpreloc").text(lblcrnpreloc);
    $('#popval').modal('show');
}
$(document).on("click", ".btncandidateTestlist", function () {
    var id = $(this).data("data");
    $.ajax({
        url: '/Master/GetCandidateTestList?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            $('#CandidateTestDataTable').dataTable({
                destroy: true,
                data: result,
                columns: [
                    { data: 'TestName' },
                    { data: 'AssignBy' },
                    { data: 'SubmitDate' },
                    { data: 'TestScore' },
                    {data: 'TestStatus'},
                    {
                        data: null,
                        className: "center",
                        "render": function (data, type, row, meta) {
                            var ActionBtn = '<button class="btn btn-warning btn-xs btngetTestResult" type="button" id="" data-submitdate="' + data.SubmitDate + '" data-candidateid="' + data.CandidateId + '" data-testgroupid="' + data.TestGroupId + '"><i class="fa fa-eye"></i> </button> ';
                            var Reassign = ' | <button type="button" title="Reassign Test" id="" data-ClientId="' + data.ClientId + '" data-JobId="' + data.JobId + '" data-CompanyId="' + data.CompanyId + '" data-TestLink="' + data.TestName + '" data-UserId="' + data.UserId + '" data-submitdate="' + data.SubmitDate + '" data-candidateid="' + data.CandidateId + '" data-testgroupid="' + data.TestGroupId + '" class="btn btn-primary navbar-btn btn-xs btnreassign"><i class="fa fa-history" aria-hidden="true"></i></button>';
                            return ActionBtn + Reassign;
                        }
                    }
                ]
            });
            $('#canddatetestpopval').modal('show');
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});

$(document).on("click", ".btngetTestResult", function () {
    var submitdate = $(this).data("submitdate");
    var candidateid = $(this).data("candidateid");
    var testgroupid = $(this).data("testgroupid");
    $.ajax({
        url: '/Master/getTestResult?submitdate=' + submitdate + "&candidateid=" + candidateid + "&testgroupid=" + testgroupid,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            $('#canddatetestpopval').modal('hide');
            $("#TestResultDiv").html(result);
            $("#detailsdata").hide();
            $("#MainTestResultDive").show();
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});
$(document).on("click", ".clsCloseTestResult", function () {
    $("#detailsdata").show();
    $("#MainTestResultDive").hide();
});


$(document).on("click", ".btnreassign", function () {
    //debugger;

    var formData = $('#submitform').serializeArray();
    var CandidateId = $(this).data("candidateid");
    var ClientId = "";
    var JobId = 5;
    var UserId = "4c7ded26-eea5-433a-ada0-5c9d314a09d9";
    var ActivityType = "Reassign";
    var TestLink = $(this).data("testlink");
    var CompanyId = "";
    var Activity = "Re-Assigned Test";
    formData.push({ name: "CandId", value: CandidateId });
    formData.push({ name: "ClientId", value: ClientId });
    formData.push({ name: "JobId", value: JobId });
    formData.push({ name: "UserId", value: UserId });
    formData.push({ name: "CompanyId", value: CompanyId });
    formData.push({ name: "ActivityType", value: ActivityType });
    formData.push({ name: "TestLink", value: TestLink });
    formData.push({ name: "Activity1", value: Activity });
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Activities';
    params['requestType'] = "POST";
    params['data'] = formData;
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
    //$.ajax({
    //    url: '/Master/getTestResult?submitdate=' + submitdate + "&candidateid=" + candidateid + "&testgroupid=" + testgroupid,
    //    type: "GET",
    //    contentType: false,
    //    processData: false,
    //    success: function (result) {
    //        $('#canddatetestpopval').modal('hide');
    //        $("#TestResultDiv").html(result);
    //        $("#detailsdata").hide();
    //        $("#MainTestResultDive").show();
    //    },
    //    error: function (err) {
    //        toastr.error(err.statusText);
    //    }
    //});
});



    $('#btnget').click(function () {

        alert($('#chkveg').val());

    });

function ajaxClientSuccess(data) {
    console.log(data);
    console.log("Success");
    $("#chkveg").empty();
    var ddlClients = document.getElementById("chkveg");
    if (data.length >= 0 && data.length !== undefined) {

        var blankoption = document.createElement("OPTION");

        //Set Customer Name in Text part.
        blankoption.innerHTML = "";
        blankoption.className = "select7_hide";

        //Set CustomerId in Value part.
        blankoption.value = "";

        //Add the Option element to DropDownList.
        ddlClients.options.add(blankoption);
        //Add the Options to the DropDownList.
        for (var i = 0; i < data.length; i++) {
            var option = document.createElement("OPTION");

            //Set Customer Name in Text part.
            option.innerHTML = data[i].ClientName;

            //Set CustomerId in Value part.
            option.value = data[i].ClientId;

            //Add the Option element to DropDownList.
            ddlClients.options.add(option);
        }
    }
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxClientError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}


function ajaxClientGridSuccess(data) {
    console.log(data);
    console.log("Success");
    
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxClientGridError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}

