$(document).ready(function () {
    $(".Test").removeClass("active");
    $(".acls_Test").css("color", "white");
    $(".acls_Test").mouseover(function () {
        $(".acls_Test").css("color", "#6a6c6f");
    });
    $(".acls_Test").mouseout(function () {
        $(".acls_Test").css("color", "white");
    });
    testloadData();
    oTable = $('#tblDatatable').DataTable();
    $('#btnSearch').click(function () {
        oTable.columns(0).search($('#txtSearch').val().trim());
        oTable.columns(1).search($('#QuestionSkill').val().trim());
        oTable.draw();
    });
});
$(document).on("click", "#btnsubmit", function () {
    checknum();
    var hr = $("#txthr").val();
    var min = $("#txtmin").val();
    if (hr != '' && min != '') {

        if ($("#QuestionGroupId").val() === "") {
            toastr.error('Error - Select Question Group');
        } else {
            var questionList = [];
            var groupId = $("#QuestionGroupId").val();
            var cmpId = $("#CompanyId").val();
            $('#sortable1 .clsqustion').each(function () {
                questionList.push({
                    GroupId: groupId, QueId: $(this).attr('id'), TestName: $("#TestName").val(), CompanyId: cmpId
                });
            });
            var dataToSend = JSON.stringify({ 'list': questionList });
            console.log(questionList);
            console.log(dataToSend);
            $.ajax({
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: "POST",
                url: '/Master/CreateTest?hr=' + hr + '&min=' + min,
                data: dataToSend,
                success: function (result) {
                    if (result.Error === true) {
                        toastr.error('Error - ' + result.Message);
                    } else {
                        $("#sortable1 .clsqustion").remove();
                        $.each(result.Data, function (key, val) {
                            $("#sortable1").append('<li class="ui-state-default clsqustion" id="' + val.QuestionId + '">' + val.Question1 + '</li>');
                        });
                        toastr.success('Success - Data Inserted Successfully...!!!');
                        swal({
                            title: "You have submitted test successfully !!",
                            text: "Next, Go and click on Candidate menu next select candidates and assign activity.",
                            type: "success"
                        });
                    }
                    $("html, body").animate({ scrollTop: 0 }, 600);
                },
                error: function (err) {
                    toastr.error(err.statusText);
                }
            });
        }
    }
    else {
        toastr.error('Please - Inserte Time...!!!');
    }
});

$(document).on("change", "#QuestionGroupId", function () {
    var grpId = $(this).val();
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: "GET",
        url: '/Master/getTest',
        data: { Key: grpId },
        success: function (result) {
            console.log(result);
            if (result.Error === true) {
                toastr.error(result.Message);
                $('#sortable1').empty();
            }
            else {
                $('#sortable1').empty();
                $.each(result.Data, function (key, val) {

                    var ul = document.getElementById("sortable1");
                    var li = document.createElement("li");
                    li.appendChild(document.createTextNode(val.Question1));
                    if ($("#IsDelete").val() === "True") {
                        var span = document.createElement("SPAN");
                        var txt = document.createTextNode("\u00D7");
                        span.className = "close";
                        span.appendChild(txt);
                        li.appendChild(span);
                    }
                    li.setAttribute("id", val.QuestionId); // added line
                    li.setAttribute("class", "clsqustion");// added line
                    ul.appendChild(li);
                });
                $("#TestName").val(result.Data[0].TestName);
            }
        },
        error: function (err) {
            toastr.error(err.statusText);
        }
    });
});

function checknum() {
    var hr = $("#txthr").val();
    var min = $("#txtmin").val();
    if (isNaN(hr)) {
        $("#txthr").val('');
        $("#txthr").focus();
    }
    if (isNaN(min)) {
        $("#txtmin").val('');
        $("#txtmin").focus();
    }
}
function ajaxSuccess(data) {
    testloadData();
};
function ajaxError(data) {
    console.log("Error");
    console.log(data.responseText);
    var obj = jQuery.parseJSON(data.responseText);
    console.log(obj);
    if (obj.errors[0].Code === "0") {
        toastr.error('Error - ' + obj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }

}
function testloadData() {
    var IsAdd = $("#IsAdd").val();
    var IsDelete = $("#IsDelete").val();

    $('#tblDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        //"rowReorder": true,
       // "dragHandle": 'tr',
        //"maxMovingRows": 1,
        //"excludeFooter": true,
        "ajax": {
            "url": "/Master/TestLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "Question1", "name": "Question1" },
            {
                data: null,
                className: "center",
                "render": function (data, type, row, meta) {
                    return IsAdd === "True" ? '<a> <span class="btselect"  id="" title="Select" data-id="' + data.QuestionId + '" data-text="' + data.Question1 + '"><i class="fa fa-arrow-right" aria-hidden="true"></i></span></a>' : "";
                }
            }

        ],
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });

    var el = $('.col-sm-7');
    el.addClass('col-sm-12');
    el.removeClass('col-sm-7');
    $(".dataTables_filter").hide();
    $('#loading').html("").hide();

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Tests';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}
$(document).on("click", ".btselect", function (e) {
    debugger;
    var text = e.currentTarget.dataset.text;
    var id = e.currentTarget.dataset.id;

    var ul = document.getElementById("sortable1");

    if ($("#sortable1").find("#" + id).length == 0) {
        var li = document.createElement("li");
        li.appendChild(document.createTextNode(text));

        if ($("#IsDelete").val() === "True") {
            var span = document.createElement("SPAN");
            var txt = document.createTextNode("\u00D7");
            span.className = "close";
            span.appendChild(txt);
            li.appendChild(span);
        }
        li.setAttribute("id", id);
        li.setAttribute("class", "clsqustion");// added line
        ul.appendChild(li);
    }
    else {
        toastr.error('Error - Question already exist..!');
    }

});
$(document).on("click", ".close", function (e) {
    $(this)[0].parentNode.parentNode.removeChild($(this)[0].parentNode);
});