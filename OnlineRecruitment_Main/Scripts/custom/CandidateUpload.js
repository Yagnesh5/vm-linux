$(document).ready(function () {
    console.log("ready!");
});

//$("#btnsubmit").on("click", function (e) {
//    if (window.FormData !== undefined) {
//        if (!recordRTC) return alert('No recording found.');
//        var file = new File([recordRTC.getBlob()], fileName, {
//            type: mimeType
//        });
//        var fileData = new FormData();
//        var myParam = location.search.split('CanId=')[1];
//        fileData.append('CandidateId', myParam);
//        fileData.append(file.name, file);
//        $.ajax({
//            url: '/Master/UploadCandVideo',
//            type: "POST",
//            contentType: false, // Not to set any content header  
//            processData: false, // Not to process data  
//            data: fileData,
//            success: function (result) {
//                toastr.success(result);
//            },
//            error: function (err) {
//                toastr.error(err.statusText);
//            }
//        });
//    } else {
//        toastr.error('Error - FormData is not supported...!!!');
//    }
//});

//function ajaxError(data) {
//    console.log("Error");
//    setTimeout($.unblockUI, 2000);
//    var Data = jQuery.parseJSON(data.responseText);
//    console.log(Data.errors[0].Message);
//    debugger;
//    if (Data !== null) {
//        if (Data.errors[0].Message) {
//            toastr.error('Error - ' + Data.errors[0].Message);
//        }
//    } else {
//        toastr.error('Error - Something is wrong...!!!');
//    }
//}

//function ajaxSuccess(data) {
//    console.log(data);
//    console.log("Success");
//    setTimeout($.unblockUI, 2000);
//    if (data.successlist[0].Message === "Inserted") {
//        debugger;
//        uploadResume(data.successlist[0].Data);
//        toastr.success('Success - Data Inserted Successfully...!!!');
//        $("#btnUpdate").addClass('hidden');
//        $("#btnsubmit").show();
//        $("#detailsdata").show();
//        $("#createDiv").addClass('hidden');
//        popcandidateInfo();
//        insertClientCandidateMapping(data.successlist[0].Data);
//        resetform();
//        oTable = $('#tblDatatable').DataTable();
//        $('#btnSearch').trigger('click');
//    } else if (data.successlist[0].Message === "Edit") {
//        resetform();
//        console.log(data.successlist[0].Data.CandidateId);
//        debugger;
//        $("#uprestext").hide();
//        $("#editresume").attr("href", "../../UploadedResume/" + data.successlist[0].Data.CandidateResume);
//        $("#editresume").hide();
//        var ResumeLinks = "";
//        /* creating HTML for resume*/
//        if (data.successlist[0].Data.CandidateResumesList.length > 0) {
//            for (var i = 0; i < data.successlist[0].Data.CandidateResumesList.length; i++) {
//                var element = data.successlist[0].Data.CandidateResumesList[i];
//                //if (i == 0) {
//                var url = "../../UploadedResume/" + element.CandidateResume;
//                ResumeLinks = ResumeLinks + "<strong><a href='" + url + "' class=\"clsdownload\" download=\"\" id=\"editresume\"><u>Download Resume!  " + element.CreatedDateString + "</u></a></strong><br/>";
//            }
//            $("#editresume").hide();
//        }
//        else {
//            $("#editresume").attr("href", "../../UploadedResume/" + data.successlist[0].Data.CandidateResume);
//            $("#editresume").show();
//        }


//        $("#divResumeList").html(ResumeLinks);
//        //<strong><a href="../../UploadedResume/Candidate_2_ResumeVivekShinde.pdf" class="clsdownload" download="" id="editresume"><u>Download Resume!</u></a></strong>
//        $("#editresume").removeClass('hidden');
//        $("#CandidateId").val(data.successlist[0].Data.CandidateId);
//        $("#CandidateName").val(data.successlist[0].Data.CandidateName);
//        $("#CandidateSkill").val(data.successlist[0].Data.CandidateSkill);
//        $("#CandidateDOB").val(data.successlist[0].Data.CandidateDOB);
//        $("#CandidateExperience").val(data.successlist[0].Data.CandidateExperience);
//        $("#CandidatePhone").val(data.successlist[0].Data.CandidatePhone);
//        $("#CandidatePhone2").val(data.successlist[0].Data.CandidatePhone2);
//        $("#CandidateEmail").val(data.successlist[0].Data.CandidateEmail);
//        $("#CandidateLocation").val(data.successlist[0].Data.CandidateLocation);
//        $("#CandidateLastJobChange").val(data.successlist[0].Data.CandidateLastJobChange);
//        $("#CurrentOrganisation").val(data.successlist[0].Data.CurrentOrganisation);
//        $("#SkypeId").val(data.successlist[0].Data.SkypeId);
//        $("#CurrentCTC").val(data.successlist[0].Data.CurrentCTC);
//        $("#ExpectedCTC").val(data.successlist[0].Data.ExpectedCTC);
//        var buyoptionval = data.successlist[0].Data.BuyoutOption;
//        if (buyoptionval === true) {
//            $("#BuyoutOptionYes").prop('checked', true);
//        } else {
//            $("#BuyoutOptionNo").prop('checked', true);
//        }
//        $("#Education").val(data.successlist[0].Data.Education);
//        $("#OffersInHand").val(data.successlist[0].Data.OffersInHand);
//        $("#NoticePeriod").val(data.successlist[0].Data.NoticePeriod);
//        $("#ClientDesignation").val(data.successlist[0].Data.ClientDesignation);
//        $("#Remarks").val(data.successlist[0].Data.Remarks);
//        $("#Relevant_Exp").val(data.successlist[0].Data.Relevant_Exp);
//        $("#Preferred_Location").val(data.successlist[0].Data.Preferred_Location);
//        $("#Last_Job_change_reason").val(data.successlist[0].Data.Last_Job_change_reason);
//        $("#createDiv").removeClass('hidden');
//        $("#detailsdata").hide();
//        $("#btnUpdate").removeClass('hidden');
//        $("#btnsubmit").hide();
//        $("#CandidateResume").prop('required', false);
//    } else if (data.successlist[0].Message === "Update") {
//        uploadResume($("#CandidateId").val());
//        toastr.success('Success - Data Updated Successfully...!!!');
//        $("#btnupdate").addClass('hidden');
//        $("#btnsubmit").show();
//        $("#detailsdata").show();
//        $("#createDiv").addClass('hidden');
//        popcandidateInfo();
//        //CandidateLoadData();
//        resetform();
//        oTable = $('#tblDatatable').DataTable();
//        $('#btnSearch').trigger('click');
//    } else if (data.successlist[0].Message === "Delete") {
//        toastr.success('Success - Data Deleted Successfully...!!!');
//        //CandidateLoadData();
//        oTable = $('#tblDatatable').DataTable();
//        $('#btnSearch').trigger('click');
//    } else if (data.successlist[0].Message === "Send") {
//        toastr.success('Success - Job Assign Successfully...!!!');
//        $('#myModal').modal('hide');
//        CandidateLoadData();
//    } else if (data.successlist[0].Message === "Activity") {
//        $.each(data.successlist[0].Data.ActivityTypeList, function (data, value) {
//            $("#detailsActivityType").append($("<option></option>").val($.trim(value.ActivityType1)).html($.trim(value.ActivityType1)));
//        });
//        $("#ActivityId").val(data.successlist[0].Data.ActivityId);
//        $('#detailsActivityType').val(data.successlist[0].Data.ActivityType).trigger('change');
//        $('#Activity').val(data.successlist[0].Data.Activity1);
//        $('#ActivityDate').val(data.successlist[0].Data.ActivityDate);
//        $('#ActivityRemark').val(data.successlist[0].Data.ActivityRemark);
//        $('#ActivityComplete').val(data.successlist[0].Data.ActivityComplete).trigger('change');
//        $('#ActivityDetailsModal').modal('toggle');
//    } else if (data.successlist[0].Message === "ActivityUpdate") {
//        toastr.success('Success - Activity Updated Successfully...!!!');
//        $('#ActivityDetailsModal').modal('hide');
//        $('#sidebar-right').modal('hide');
//    } else {
//        //var newList = [];
//        //$.each(data.successlist[0].Data.filter(
//        //    function (el) {
//        //        if (el.CompanyId === parseInt(currentCompanyId)) {
//        //            newList.push(el);
//        //        }
//        //    }),
//        //);
//        CandidateLoadData();
//        if ($("#IsPrint").val() === "False") {
//            $(".buttons-print").hide();
//        }
//        if ($("#IsExport").val() === "False") {
//            $(".buttons-csv").hide();
//        }
//    }

//}
