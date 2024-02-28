var seconds = 0;
var countdownTimer = "";
$("#btnStart").click(function () {
    var checklength = $('.clsinstruction').length;
    var TruecheckLength = 0;
    $(".clsinstruction").each(function (index, element) {
        if ($(this).prop("checked") === false) {
            return false;
        } else {
            TruecheckLength = parseInt(TruecheckLength) + 1;
        }
    });
    console.log("TruecheckLength : " + TruecheckLength);
    console.log("checklength : " + checklength);
    if (parseInt(checklength) === parseInt(TruecheckLength)) {
        $("#1").removeClass('hidden');
        $("#Instruction").hide();
        $("#quedone").removeClass('hidden');
        $("#qesheader").removeClass('hidden');
        var upgradeTime = $("#second").val();
        seconds = upgradeTime;
        countdownTimer = setInterval('stratexam()', 1000);
    } else {
        alert("Please checked all the instructions.");
    }
});
function stratexam() {
    if (seconds > 0) {
        d = Number(seconds);
        var h = Math.floor(d / 3600);
        var m = Math.floor(d % 3600 / 60);
        var s = Math.floor(d % 3600 % 60);
        var hDisplay = h > 0 ? h + (h === 1 ? " hours " : " hours ") : "00 hour ";
        var mDisplay = m > 0 ? m + (m === 1 ? " minute " : " minutes ") : "00 minute ";
        var sDisplay = s > 0 ? s + (s === 1 ? " second" : " seconds") : "00 second";
        document.getElementById('countdown').innerHTML = hDisplay + ":" + mDisplay + ":" + sDisplay;
        seconds--;
        document.getElementById("countdown").style.color = "#62cb31";
    }
    if (seconds === 300) {
        document.getElementById("countdown").style.color = "red";
        swal({
            title: "Time Expiration!",
            text: "Yor answers will be submitted automatically after 5 minutes",
            type: "success",
            timer: 3000
        });
    }
    if (seconds === 0) {
        clearInterval(countdownTimer);
        document.getElementById('countdown').innerHTML = "Completed";
        Submit();
    }
}
var TestData = [];
var SingleTestData = [];
$(".btnNext").click(function () {
    SingleTestData = [];
    var queId;
    var id = $(this).attr('id');
    var idinc = parseInt(id) + 1;
    var countq = $("#count").val();
    if (parseInt(id) <= parseInt(countq)) {
        var clas = 'someClass_' + id;
        var type = $("." + clas).attr('type');
        if (type === "radio") {
            $('input:radio[class=' + clas + ']').each(function () {
                var AnsId = [];
                AnsId.push(parseInt($(this).attr('id')));
                queId = $(this).data('queid');
                if ($(this).prop("checked") === true) {
                    var isnew = true;
                    $.each(TestData, function (i, item) {
                        if (item.queId === queId) {
                            item.AnsId = AnsId;
                            isnew = false;
                        }
                        //break;
                    });
                    if (isnew) {
                        TestData.push({
                            AnsId: AnsId,
                            QueId: queId
                        });
                    }
                    SingleTestData = [];
                    SingleTestData.push({
                        AnsId: AnsId,
                        QueId: queId
                    });
                }
            });
            console.log(TestData);
            var radiocheck = $("input:radio." + clas + ":checked").val();
            if (radiocheck === "on") {
                document.getElementById("show_" + id).checked = true;
            }
        }
        if (type === "checkbox") {
            var AnsId = [], isselected = false;
            $('input:checkbox[class=' + clas + ']').each(function () {
                var attrAnsId = $(this).attr('id');
                queId = $(this).data('queid');
                if ($(this).prop("checked") === true) {
                    isselected = true;
                    AnsId.push(parseInt(attrAnsId));
                }
            });
            if (isselected) {
                var isnew = true;
                $.each(TestData, function (i, item) {
                    if (item.queId === queId) {
                        item.AnsId = AnsId;
                        isnew = false;
                    }
                });
                if (isnew) {
                    TestData.push({
                        AnsId: AnsId,
                        QueId: queId,
                        isMulti: true
                    });
                    SingleTestData = [];
                    SingleTestData.push({
                        AnsId: AnsId,
                        QueId: queId,
                        isMulti: true
                    });
                }
            }
            var checkboxcheck = $("input:checkbox." + clas + ":checked").val();
            if (checkboxcheck === "on") {
                document.getElementById("show_" + id).checked = true;
            }
        }
        console.log("Present ID :" + id);
        console.log("Next ID :" + countq);
        if (parseInt(id) < parseInt(countq)) {
            console.log("Adding Class: " + id);
            console.log("removing class: " + idinc);
            $("#" + id).addClass('hidden');
            $("#" + idinc).removeClass('hidden');
            $("#quescnt").text('');
            $("#quescnt").text(idinc);
            RemovePriviousDivs(id);
        } else {
            $(".btnNext").hide();
        }
    }
    var selecteditems = [];
    $("#" + id).find("input:checked").each(function (i, ob) {
        selecteditems.push($(ob).val());
    });

    if (SingleTestData.length <= 0) {
        SingleTestData = [];
        SingleTestData.push({
            AnsId: 0,
            QueId: queId
        });
    }

    SaveCandidateResult();
});
$(".btnPrevious").click(function () {
    $(".btnNext").show();
    var id = $(this).attr('id');
    var idinc = parseInt(id) - 1;
    if (parseInt(id) > 1) {
        $("#" + id).addClass('hidden');
        $("#" + idinc).removeClass('hidden');
        $("#quescnt").text('');
        $("#quescnt").text(idinc);
    }
});

function RemovePriviousDivs(no) {
    var letestDIV = parseInt(no);
    for (i = letestDIV; i > 1; i--) {
        $('.print_' + i).addClass("hidden");
    }
    var newNo = letestDIV + 1;
    $('.print_' + newNo).removeClass('hidden');
};

function Submit() {
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
    var urlParams = new URLSearchParams(window.location.search);
    var testName = urlParams.get('Testname');
    var dataToSend = JSON.stringify({ 'list': TestData });
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: "POST",
        url: '/CommonViews/SubmitTest?canEmail=' + $("#canEmail").val() + "&useremail=" + $("#useremail").val() + "&TotalCount=" + $("#count").val() + "&TestName=" + testName,
        data: dataToSend,
        success: function (result) {
            console.log("result: " + result);
            if (result.Error === false) {
                $("#MainDiv").hide();
                $("#subTestDiv").removeClass('hidden');

            } else {
                alert(result.Message);
            }
            setTimeout($.unblockUI, 2000);
        },
        error: function (err) {
            setTimeout($.unblockUI, 2000);
            alert(err);
        }
    });
}
$(document).on("click", ".Submit", function () {
    Submit();
});
//$(document).on("click", ".clsmovetoquestion", function () {
//    var id = $(this).data('data');
//    $(".clsqueDiv").addClass('hidden');
//    $("#" + id).removeClass('hidden');
//});
$(document).ready(function () {
    function preventBack() { window.history.forward(); }
    setTimeout("preventBack()", 0);
    window.onunload = function () { null };


});
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
function SaveCandidateResult() {
    var Testname = getUrlVars()["Testname"];
    var AssignBy = getUrlVars()["email"];
    var CanId = getUrlVars()["CanId"];
    var dataToSend = JSON.stringify({ 'list': SingleTestData });
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: "POST",
        url: '/CommonViews/SaveCandidateResult?Testname=' + Testname + "&CanId=" + CanId + "&AssignBy=" + AssignBy,
        data: dataToSend,
        success: function (result) {
            console.log(result);
            if (result.Error === false) {
                //$("#MainDiv").hide();
                //$("#subTestDiv").removeClass('hidden');

            } else {
                alert(result.Message);
            }

        },
        error: function (err) {
            alert(err);
        }
    });
}



if (document.layers) {
    //Capture the MouseDown event.
    document.captureEvents(Event.MOUSEDOWN);

    //Disable the OnMouseDown event handler.
    document.onmousedown = function () {
        return false;
    };
}
else {
    //Disable the OnMouseUp event handler.
    document.onmouseup = function (e) {
        if (e != null && e.type == "mouseup") {
            //Check the Mouse Button which is clicked.
            if (e.which == 1 || e.which == 2 || e.which == 3) {
                //If the Button is middle or right then disable.
                return false;
            }
        }
    };
}

//Disable the Context Menu event.
document.oncontextmenu = function () {
    return false;
};
