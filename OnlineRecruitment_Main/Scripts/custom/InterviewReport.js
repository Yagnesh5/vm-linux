$(document).ready(function () {
    console.log("ready!");
    $(".InterviewReport").removeClass("footer");
    $(".InterviewReport").removeClass("active");
    $(".acls_InterviewReport").css("color", "white");
    $(".acls_InterviewReport").mouseover(function () {
        $(".acls_InterviewReport").css("color", "#6a6c6f");
    });
    $(".acls_InterviewReport").mouseout(function () {
    });

    var urlParams = new URLSearchParams(window.location.search);
    var InterviewId = urlParams.get('InterviewId');
    if (InterviewId !== null) {
        LoadInterviewData(InterviewId);
        $("#ddlClients").prop("disabled", true);
        $("#ddlJobs").prop("disabled", true);
        $("#btnSubmit").prop("disabled", true);
        $("#divCandidate").hide();
    }
    else {
        BlocksLoadData();
    }

});

$(document).on("change", "#ddlClients", function () {
    $("#ddlJobs").empty();
    var id = $(this).val();
    $.ajax({
        url: '/Master/GetClientJob?key=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            if (result.Data) {
                $("#ddlJobs").empty();
                $("#ddlJobs").append($("<option></option>").val(0).html("Select Job"));
                $.each(result.Data, function (data, value) {
                    $("#ddlJobs").append($("<option></option>").val(value.JobId).html(value.JobName));
                });
            }
            //toastr.success(result);
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});

function BlocksLoadData() {
    var params = $.extend({}, doAjax_params_default);

    params['url'] = '/Master/ClientsData?companyId=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientSuccess;
    params['errorCallBackFunction'] = ajaxClientError;
    doAjax(params);
    return false;
}

function ajaxClientSuccess(data) {
    console.log(data);
    console.log("Success");
    $("#ddlClients").empty();
    var ddlClients = document.getElementById("ddlClients");
    if (data.length >= 0 && data.length !== undefined) {

        var option = document.createElement("OPTION");
        option.innerHTML = "Select Client";
        option.value = 0;
        //option.value = "Select Client";
        ddlClients.options.add(option);
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

$("#btnSubmit").on("click", function (e) {
    var selectedClient = $('#ddlClients').val();
    var selectedJob = $('#ddlJobs').val();

    var params = $.extend({}, doAjax_params_default);

    params['url'] = '/Master/InterviewCandidateData?clientId=' + selectedClient + '&jobId=' + selectedJob;
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxCandidateGridSuccess;
    params['errorCallBackFunction'] = ajaxCandidateGridError;
    doAjax(params);

    return false;
});

function ajaxCandidateGridSuccess(data) {
    console.log(data);
    console.log("Success");
    $('#tblCandidateData').DataTable({
        data: data,
        "order": [[0, "desc"]],
        //"iDisplayLength": 5,
        columns: [
            { "data": "CandidateName", "name": "CandidateName" },
            { "data": "TestName", "name": "TestName" },
             {
                 data: null,
                 className: "center",
                 "render": function (data, type, row, meta) {
                     return '<div>' +
                         '<a href="#" data-data="' + data.InterviewId + '" class="btnWatchVideo"">Watch video</a>' +
                         '</div>';

                 }
             },
        ],
        dom: 'Bfrtip',
        buttons: [
        ],
        destroy: true,
        'columnDefs': [
         {
             "targets": 0,
             "className": "text-left",
         },
        {
            "targets": 1,
            "className": "text-left",
        }]
    });
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxCandidateGridError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}
var InterviewId = 0;

$(document).on("click", ".btnWatchVideo", function () {
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
    InterviewId = $(this).data("data");
    LoadInterviewData(id);
    $(".InterviewReport").removeClass("footer");
    return false;
});

function LoadInterviewData(id) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Master/GetInterviewData?Interviewid=' + id;
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxInterviewDataSuccess;
    params['errorCallBackFunction'] = ajaxInterviewDataError;
    doAjax(params);
}

var video = document.getElementsByTagName("video")[0];
var interviewEmotiontime;
var interviewQuetionTime;
var emtionCount = 0;
var InterviewQuestionCount = 1;
var CorrectQueCount = 0;
var displayEmotionTime = "";
var displayEmotionName = "";
var emotions = "";
function ajaxInterviewDataSuccess(data) {
    setTimeout($.unblockUI, 2000);

    emtionCount = 0;
    InterviewQuestionCount = 1;
    CorrectQueCount = 0;
    emotions = "";
    $("#tblEmotions").html(emotions);
    $("#divEmotionTime").html('');
    PlayVideo(data.InterviewData.InterviewVideo);

    interviewEmotiontime = data.InterviewData.lstEmotions;
    interviewQuetionTime = data.InterviewData.lstQuestions;
    BindQuestions(data.InterviewData.lstQuestions);
    BindComments(data.InterviewData.lstComments);
}

function ajaxInterviewDataError(data) {
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

function BindQuestions(lstQuestions) {
    $("#queNums").html("");
    $("#queNums").append('<label id="lblQue" class="col-form-label text-left">Questions : </label> &nbsp;');

    for (var i = 0; i < lstQuestions.length; i++) {
        $("#queNums").append('<a id="qCount' + i + '" class="btnQuetion" data-data="' + lstQuestions[i].QuestionName + '" data-time="' + lstQuestions[i].QuestionTime + '" data-intqueid="' + lstQuestions[i].Id + '" data-isAnsCorrect="' + lstQuestions[i].IsAnswerCorrect + '" href="#" ;>' + (i + 1) + '</a> &nbsp;');
        if (lstQuestions[i].IsAnswerCorrect === true) {
            CorrectQueCount += 1;
        }
        //$("#radioBtnQue").html('<input id="QueAns" name="QueAns" value="1" type="radio"> Correct&nbsp;&nbsp;');
        //$("#radioBtnQue").append('<input id="QueAns" name="QueAns" value="0" type="radio"> Incorrect');
    }

    var intResult = ((CorrectQueCount * 100) / (lstQuestions.length));
    $("#lblIntResult").html("Result : " + intResult + "% <br /> No. of Correct Answers : " + CorrectQueCount);
    $(".btnQuetion").css({ "font-size": "13px", "font-style": "", "font-weight": "", "text-decoration": "" });
    $("#qCount" + 0).css({ "fontSize": "16px", "font-style": "italic", "font-weight": "bold", "text-decoration": "underline" });
    $("#lblQueText").html(lstQuestions[0].QuestionName);
    $("#lblIntQueId").html(lstQuestions[0].Id);

    $("#radioBtnQue input:radio").attr('checked', false);

    if (lstQuestions[0].IsAnswerCorrect === true) {
        $("#isAnsCorrect").prop("checked", true);
    }
    else if (lstQuestions[0].IsAnswerCorrect === false) {
        $("#isAnsInCorrect").prop("checked", true);
    }
}

function BindComments(lstComments) {
    $("#tblCommentBody").empty();
    $.each(lstComments, function (i, value) {
        $("#tblCommentBody").append("<tr><td>" + value.Comment + "</td><td>" + value.UpdatedByName + "</td><td>" + value.CommentDate + "</td></tr>");
    });
}

$(document).on("click", ".btnQuetion", function () {
    emtionCount = parseInt($(this).text()) - 1;
    InterviewQuestionCount = parseInt($(this).text()) - 1;
    var QuestionName = $(this).data("data");
    $("#lblQueText").html(QuestionName);
    var IntQueId = $(this).data("intqueid");
    $("#lblIntQueId").html(IntQueId);    
    $(".btnQuetion").css({ "font-size": "13px", "font-style": "", "font-weight": "", "text-decoration": "" });
    $(this).css({ "fontSize": "16px", "font-style": "italic", "font-weight": "bold", "text-decoration": "underline" });
    video.currentTime = Math.round($(this).data("time"));
    var IsAnsCorrect = $(this).data("isanscorrect");

    $("#radioBtnQue input:radio").attr('checked', false);
    if (IsAnsCorrect === true) {
        $("#isAnsCorrect").prop("checked", true);
    }
    else if (IsAnsCorrect === false) {
        $("#isAnsInCorrect").prop("checked", true);
    }
    return false;
});

video.addEventListener('timeupdate', function () {

    var videoCurrentTime = Math.round(video.currentTime);
    $("#videoTime").html('<h2 class="font-light m-b-xs">Timer : ' + videoCurrentTime + '</h2>');

    for (i = emtionCount ; i < interviewEmotiontime.length ; i++) {
        var isMatch = 0;

        if (Math.round(interviewEmotiontime[emtionCount].EmotionTime) == videoCurrentTime) {

            displayEmotionTime += "," + Math.round(interviewEmotiontime[emtionCount].EmotionTime);
            displayEmotionName += "," + interviewEmotiontime[emtionCount].EmotionName;
            emotions += '<tr><td>&nbsp;' + interviewEmotiontime[emtionCount].EmotionName + '</td><td>&nbsp;' + interviewEmotiontime[emtionCount].EmotionScore + '</td></tr>';
            $("#divEmotionTime").html('Emotion Time : ' + Math.round(interviewEmotiontime[emtionCount].EmotionTime));
            isMatch = 1;
            emtionCount++;

            $("#tblEmotions").html('<tr><th>&nbsp;Emotion Name</th><th>&nbsp;Emotion Score</th></tr>');
            $("#tblEmotions").append(emotions);
        }

        if (isMatch == 0) {
            emotions = "";
            break;
        }
    }

    if (Math.round(interviewQuetionTime[InterviewQuestionCount].QuestionTime) + 2 == videoCurrentTime) {
        $(".btnQuetion").css({ "font-size": "13px", "font-style": "", "font-weight": "", "text-decoration": "" });
        $("#qCount" + InterviewQuestionCount).css({ "fontSize": "16px", "font-style": "italic", "font-weight": "bold", "text-decoration": "underline" });
        $("#lblQueText").html(interviewQuetionTime[InterviewQuestionCount].QuestionName);
        $("#lblIntQueId").html(interviewQuetionTime[InterviewQuestionCount].Id);

        $("#radioBtnQue input:radio").attr('checked', false);

        if (interviewQuetionTime[InterviewQuestionCount].IsAnswerCorrect === true) {
            $("#isAnsCorrect").prop("checked", true);
        }
        else if (interviewQuetionTime[InterviewQuestionCount].IsAnswerCorrect === false) {
            $("#isAnsInCorrect").prop("checked", true);
        }
       
        InterviewQuestionCount++;
    }
}, false);


function PlayVideo(VideoURL) {
    $('#interviewVideo').trigger('pause');
    $("#interviewVideo").html('');
    $("#interviewVideo").html('<source src="../../InterviewVideo/' + VideoURL + '" type="video/webm">');
    $('#interviewVideo').trigger('load');
    $('#interviewVideo').trigger('play');
}

$("#btnComment").click(function () {

    var comment = $("#txtComment").val();

    var dataToSend = JSON.stringify({
        'interviewFeedback': {
            InterviewId: InterviewId,
            Comment: comment
        }
    });
    $.ajax({
        url: "/CommonViews/SaveInterviewFeedback/",
        type: "POST",
        contentType: "application/json;charset-utf=8",
        data: dataToSend,
        dataType: "json",
        async: false,
        success: function (data) {
            $("#txtComment").val("");
            $.ajax({
                url: "/CommonViews/GetComments/?interviewId=" + InterviewId,
                type: "POST",
                contentType: "application/json;charset-utf=8",
                dataType: "json",
                async: false,
                success: function (data) {
                    BindComments(data);
                },
                error: function (err) {
                    alert("No Record Found");
                }
            });
            if (data.Message == "Success")
                alert("Comment Saved Successfully!");
        },
        error: function (err) {
            alert("No Record Found");
        }
    })
});

$("#radioBtnQue input:radio").on("click", function () {
    var isAnsCorrect = $(this).val();
    var intQueId = $("#lblIntQueId").text();
    var queNum = $("#qCount" + InterviewQuestionCount).text();
    var dataToSend = JSON.stringify({
        Id: intQueId,
        IsAnswerCorrect: isAnsCorrect
    });
    $.ajax({
        url: "/CommonViews/SaveInterviewResult/",
        type: "POST",
        contentType: "application/json;charset-utf=8",
        data: dataToSend,
        dataType: "json",
        async: false,
        success: function (data) {
            //$("#radioBtnQue input:radio").attr('checked', false);
            toastr.success("Result of que-" + queNum + " saved sucessfully..");
        },
        error: function (err) {
            alert("No Record Found");
        }
    })
});


//$("#btnReset").on("click", function (e) {
//    BlocksLoadData();
//    return false;
//});