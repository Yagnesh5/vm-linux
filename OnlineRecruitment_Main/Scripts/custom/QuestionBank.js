$(document).ready(function () {
    console.log("ready!");
    $(".Question").removeClass("active");
    $(".acls_Question").css("color", "white");
    $(".acls_Question").mouseover(function () {
        $(".acls_Question").css("color", "#6a6c6f");
    });
    $(".acls_Question").mouseout(function () {
        $(".acls_Question").css("color", "white");
    });
    $("#DivCompanyName").hide();
    $(function () {
        $("#chkCmp").click(function () {
            if ($(this).is(":checked")) {
                $("#DivCompanyName").show();
            } else {
                $("#DivCompanyName").hide();
            }
        });
    });
    $("#btnIn").click(function () {
        resetQueBank();
        $("#ListDiv").hide();
        $("#DetailsDiv").removeClass('hidden');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
    });
    $("#btncancel").click(function () {
        $("#ListDiv").show();
        $("#DetailsDiv").addClass('hidden');
    });
    $(".addCF").click(function () {
        var rowCount = ($('#customFields tr').length);
        var num = rowCount + 1;
        $("#customFields").append('<tr>'
            + '<td>'
            + '<div class="form-group">'
            + '<div class="input-group m-b"><span class="input-group-addon"> <label>Option ' + num + '</label><input type="checkbox" class="Ansoption" id="AnsChk' + num + '"></span> <input type="text" class="form-control optionAns" id="Option' + num + '" placeholder = "Enter Option"><span class="input-group-btn"><button class="btn btn-default bootstrap-touchspin-up remCF" type="button">-</button></span></div>'
            + '</div>'
            + '</td>'
            + '</tr>');
        $("#option" + num).prop('required', true);
        $('#QuestionType').trigger('change');
    });
    $("#customFields").on('click', '.remCF', function () {
        $(this).parents('tr').remove();
    });

    $(function () {
        $('.summernote').summernote();
        $('.summernote1').summernote({
            toolbar: [
                ['headline', ['style']],
                ['style', ['bold', 'italic', 'underline', 'superscript', 'subscript', 'strikethrough', 'clear']],
                ['textsize', ['fontsize']],
                ['alignment', ['ul', 'ol', 'paragraph', 'lineheight']],
            ]
        });
        $('.summernote2').summernote({
            airMode: true
        });
    });
    QueansLoadData();
    oTable = $('#tblDatatable').DataTable();
    //Apply Custom search on jQuery DataTables here
    $('#btnSearch').click(function () {
        //Apply search for Employee Name // DataTable column index 0
        oTable.columns(0).search($('#txtEmployeeName').val().trim());
        //Apply search for Country // DataTable column index 2
        oTable.columns(2).search($('#SearchQuestionType').val().trim());
        //hit search on server
        oTable.draw();
    });
});


function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Code === "0") {
        toastr.error('Error - ' + data.successlist[0].Message);
    }
    if (data.successlist[0].Message === "Inserted") {
        uploadImage(data.successlist[0].Data);
        toastr.success('Success - Data Inserted Successfully...!!!');
        resetQueBank();
        //QueansLoadData();
        $("#ListDiv").show();
        $("#DetailsDiv").addClass('hidden');
        location.reload();
    } else if (data.successlist[0].Message === "Edit") {
        resetQueBank();
        console.log(data.successlist[0].Data);
        $("#QuestionSkill").val(data.successlist[0].Data.QuestionSkill);
        var imglnght = data.successlist[0].Data.QuestionImagelist.length;

        for (var i = 0; i < imglnght; i++) {
            var c = 0;
            $('.summernote').summernote('insertImage', data.successlist[0].Data.QuestionImagelist[i].Images, function ($image) {
                $image.css('width', $image.width() / 3);
                $image.attr('class', data.successlist[0].Data.QuestionImagelist[c].Id);
                $image.attr('data-filename', 'retriever');
                c = c + 1;
            });
        }
        $("#QuestionId").val(data.successlist[0].Data.QuestionId);
        $("#AnswerId").val(data.successlist[0].Data.AnswerId);
        $(".note-editable").text(data.successlist[0].Data.Question);
        $('#QuestionGroupId').val(data.successlist[0].Data.QuestionGroupId).trigger('change');
        if (comCurrentRole === "SUPER ADMIN") {
            if (data.successlist[0].Data.CompanyId !== null) {
                $('#CompanyId').val(data.successlist[0].Data.CompanyId).trigger('change');
                $("#DivCompanyName").show();
                $("#chkCmp").prop('checked', true);
            }
        } else {
            $('#CompanyId').val(data.successlist[0].Data.CompanyId).trigger('change');
        }
        $("#QuestionSkill").val(data.successlist[0].Data.QuestionSkill);
        for (var x = 0; x < data.successlist[0].Data.optionAnswersliist.length - 2; x++) {
            var rowCount = ($('#customFields tr').length);
            var num = rowCount + 1;
            $("#customFields").append('<tr>'
                + '<td>'
                + '<div class="form-group">'
                + '<div class="input-group m-b"><span class="input-group-addon"> <label>Option ' + num + '</label><input type="checkbox" class="Ansoption" id="AnsChk' + num + '"></span> <input type="text" class="form-control optionAns" id="Option' + num + '" placeholder = "Enter Option"><span class="input-group-btn"><button class="btn btn-default bootstrap-touchspin-up remCF" type="button">-</button></span></div>'
                + '</div>'
                + '</td>'
                + '</tr>');
            $("#Option" + num).prop('required', true);
        }
        for (var i = 0, l = data.successlist[0].Data.optionAnswersliist.length; i < l; i++) {
            var op = i + 1;
            $("#Option" + op).val(data.successlist[0].Data.optionAnswersliist[i].Answer1);
            $("#AnsChk" + op).prop('checked', data.successlist[0].Data.optionAnswersliist[i].AnswerCorrect);
        }
        $('#QuestionType').val(data.successlist[0].Data.QuestionType).trigger('change');
        $("#ListDiv").hide();
        $("#DetailsDiv").removeClass('hidden');
        $("#btnUpdate").removeClass('hidden');
        $("#btnsubmit").hide();
    } else if (data.successlist[0].Message === "Update") {
        uploadImage(data.successlist[0].Data);
        toastr.success('Success - Data Updated Successfully...!!!');
        $("#btnUpdate").addClass('hidden');
        $("#btnsubmit").show();
        $("#ListDiv").show();
        $("#DetailsDiv").addClass('hidden');
        //QueansLoadData();
        resetQueBank();
        location.reload();
    } else if (data.successlist[0].Message === "Delete") {
        toastr.success('Success - Data Deleted Successfully...!!!');
        QueansLoadData();
    }
    else {
        QueansLoadData();
        if ($("#IsPrint").val() === "False") {
            $(".buttons-print").hide();
        }
        if ($("#IsExport").val() === "False") {
            $(".buttons-csv").hide();
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

function QueansLoadData() {
    var IsEdit = $("#IsEdit").val();
    var IsDelete = $("#IsDelete").val();
    //jQuery DataTables initialization
    $('#tblDatatable').DataTable({
        "destroy": true,
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order
       // "dom": '<"top"i>rt<"bottom"lp><"clear"> ', // for hide default global search box // little confusion? don't worry I explained in the tutorial website
        "ajax": {
            "url": "/Master/QuestionLoadData",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "Question1", "name": "Question1", "autoWidth": true },
            {
                "data": null,
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    return data.QuestionType === "1" ? "Single Answer" : "Multi Answer";
                }
            },
            {
                "data": null,
                className: "center",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    var IsEditBtn = IsEdit === "True" ? data.LastUpdateBy === comCurrentUserId ? '<button class="btn btn-info btn-xs btnedit" title="Edit" type="button" id="" data-data="' + data.QuestionId + '"><i class="fa fa-edit"></i> </button> ' : "" : "";
                    var IsDeleteBtn = IsDelete === "True" ? data.LastUpdateBy === comCurrentUserId ? ' |  <button class="btn btn-danger btn-xs btndelete" type="button" id=""  title="Detete" data-data="' + data.QuestionId + '"><i class="fa fa-trash-o"></i> <span class="bold"></span></button>' : "" : "";
                    return IsEditBtn + IsDeleteBtn;
                }
            }

        ],
        "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
               buttons: [
            {
                       extend: 'csv', title: 'QuestionFile',
                exportOptions: {
                    columns: ':visible'
                }
            },
            {
                extend: 'print', title: 'QuestionFile',
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
    $("#tblDatatable_filter").hide();
    $('#loading').html("").hide();

    //var params = $.extend({}, doAjax_params_default);
    //params['url'] = BaseUrl + 'odata/Questions';
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxSuccess;
    //params['errorCallBackFunction'] = ajaxError;
    //doAjax(params);
    //return false;
}
$("#btnsubmit").on("click", function (e) {
    debugger;
    if ($(".note-editable").text() === "") {
        toastr.error('Error - Please Enter Question...!!!');
        $(".note-editable").focus();
        return false;
    }
    if ($("#QuestionSkill").val() === "") {
        toastr.error('Error - Please select Question Skill...!!!');
        $(".note-editable").focus();
        return false;
    }
    var checkans = checkonecheckboxcheck();
    if (checkans === false) {
        return false;
    }
    var formData = $('#submitform').serializeArray();
    var Answerlst = [];
    $('#customFields .optionAns').each(function () {
        formData.push({ name: $(this).attr('id'), value: $(this).val() });
        var Id = $(this).attr('id');
        var val = $(this).val();
        formData.push({ name: Id, value: val });
    });
    $('#customFields .Ansoption').each(function () {
        var Id = $(this).attr('id');
        var val = $(this).is(":checked");
        formData.push({ name: Id, value: val });
        console.log(val);
    });
    var QueVal = $(".note-editable").text();
    var compnyId = $("#CompanyId").val();
    if (compnyId === null) {
        formData.push({ name: "CompanyId", value: comcurrentCompanyId });
    }
    formData.push({ name: "Question", value: QueVal });
    $("#submitform").validate({
        submitHandler: function (form) {
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Questions';
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
    var id = $(this).data("data");
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Questions(' + id + ')';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
$("#btnUpdate").on("click", function (e) {
    if ($(".note-editable").text() === "") {
        toastr.error('Error - Please Enter Question...!!!');
        $(".note-editable").focus();
        return false;
    }
    var checkans = checkonecheckboxcheck();
    if (checkans === false) {
        return false;
    }
    $("#submitform").validate({
        submitHandler: function (form) {
            var formData = $('#submitform').serializeArray();
            $('#customFields .optionAns').each(function () {
                formData.push({ name: $(this).attr('id'), value: $(this).val() });
                var Id = $(this).attr('id');
                var val = $(this).val();
                formData.push({ name: Id, value: val });
            });
            $('#customFields .Ansoption').each(function () {
                var Id = $(this).attr('id');
                var val = $(this).is(":checked");
                formData.push({ name: Id, value: val });
                console.log(Id);
                console.log(val);
            });
            var QueVal = $(".note-editable").text();
            var compnyId = $("#CompanyId").val();
            if (compnyId === null) {
                formData.push({ name: "CompanyId", value: comcurrentCompanyId });
            }
            formData.push({ name: "Question", value: QueVal });
            var id = $("#QuestionId").val();
            var params = $.extend({}, doAjax_params_default);
            params['url'] = BaseUrl + 'odata/Questions(' + id + ')';
            params['requestType'] = "PUT";
            params['data'] = formData; //$('#submitform').serialize();
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
    params['url'] = BaseUrl + 'odata/Questions(' + id + ')';
    params['requestType'] = "DELETE";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
});
function uploadImage(id) {

    var Images = [];
    var img = $("p img");

    for (var i = 0; i < img.length; i++) {
        var url = img[i].currentSrc;
        Images.push({
            Images: url,
            QuestionId: id
        });

    }
    var dataToSend = JSON.stringify({ 'objimg': Images });
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        type: "POST",
        url: '/Master/ImageUpload?quid=' + id,
        data: dataToSend,
        success: function (result) {
            toastr.success(result);
        },
        error: function (err) {
            // toastr.error(err.statusText);
        }
    });

}
function resetQueBank() {
    $("#QuestionId").val('');
    $(".note-editable").text('');
    $("#chkCmp").prop("checked", false);
    $("#CompanyId").val('').trigger('change');
    $("#QuestionSkill").val('');
    $("#QuestionType").val('').trigger('change');
    $(".optionAns").val('');
    $(".Ansoption").prop("checked", false);
}
function checkonecheckboxcheck() {
    if ($('.Ansoption').filter(':checked').length < 1) {
        toastr.error('Error - please check at least one checkbox for answer...!!!');
        return false;
    } else {
        return true;
    }
}
$(document).on("change", "#QuestionType", function () {
    var anstype = $("#QuestionType").val();
    if (anstype === "1") {
        $('.Ansoption').attr('type', 'radio');
        // $('.Ansoption').attr('name', 'AnsChk');
        $('.Ansoption').addClass('radio');
    } else {
        $('.Ansoption').attr('type', 'checkbox');
    }
});
$(document).on("change", ".radio", function () {
    $('.radio').not(this).prop('checked', false);
});
//$(document).on("click", ".paginate_button", function () {
//    var data = $('a', this).attr('data-dt-idx');
//    var pageNo = parseInt($('a', this).text());

//    if (data === "7") {
//        var params = $.extend({}, doAjax_params_default);
//        params['url'] = BaseUrl + 'odata/Questions(' + pageNo + ')';
//        params['requestType'] = "PATCH";
//        params['successCallbackFunction'] = ajaxSuccess;
//        params['errorCallBackFunction'] = ajaxError;
//        doAjax(params);
//        return false;
//    }
//});
