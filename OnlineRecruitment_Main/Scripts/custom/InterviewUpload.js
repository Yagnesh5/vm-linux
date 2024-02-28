var seconds = 0;
var countdownTimer = "";
const testvideo = document.getElementById('testVideo');
var localstream = null;
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
        $('#CheckEmotionModal').modal('show');
        startVideo();
    } else {
        alert("Please checked all the instructions.");
    }
});

function startVideo() {
    navigator.getUserMedia({ video: {} }, function(stream) {
        localstream = stream;
        testvideo.srcObject = stream;
        stream.play();
    }, err => console.error(err));
}

testvideo.addEventListener('play', () => {

    const displaySize = { width: 720, height: 560 }
    //faceapi.matchDimensions(canvas, displaySize)
    setInterval(async () => {
        const detections = await faceapi.detectSingleFace(testvideo, new faceapi.TinyFaceDetectorOptions()).withFaceLandmarks().withFaceExpressions();
        if (detections) {
            const resizedDetections = faceapi.resizeResults(detections, displaySize)

            if (resizedDetections && Object.keys(resizedDetections).length > 0) {
                const expressions = resizedDetections.expressions;
                
                var btncontinue = document.getElementById("btnOK");
                btncontinue.disabled = false;
            }
        }
    }, 3000);
})

$("#btnOK").click(function (e) {
    $('#CheckEmotionModal').modal('hide');
    $("#Instruction").hide();
    $("#quedone").removeClass('hidden');
    $("#preStartInterView").removeClass('hidden');
    preStartInterView
    $("#videoRecord").removeClass('hidden');
    localstream.stop();
})

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

$(".btnNext").click(function () {
    var queId;
    var id = $(this).attr('id');
    var idinc = parseInt(id) + 1;
    var countq = $("#count").val();
    if (parseInt(id) <= parseInt(countq)) {
        //$(".btnPrevious").removeClass('hidden');
        var clas = 'someClass_' + id;
        var type = $("." + clas).attr('type');

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
        }
        if (idinc == countq) {
            $(".btnNext").hide();
            $(".btnFinish").removeClass('hidden');
        }
    }
    var selecteditems = [];
    $("#" + id).find("input:checked").each(function (i, ob) {
        selecteditems.push($(ob).val());
    });  
    //SaveCandidateResult();
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
    //if (idinc == 1) {
    //    $(".btnPrevious").addClass('hidden');
    //}
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
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: "POST",
        url: '/CommonViews/InterviewSubmit?canEmail=' + $("#canEmail").val() + "&useremail=" + $("#useremail").val() ,
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
