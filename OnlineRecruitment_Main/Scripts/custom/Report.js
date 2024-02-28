$(document).ready(function () {
    console.log("ready!");
    $(".Report").removeClass("active");
    $(".acls_Report").css("color", "white");
    $(".acls_Report").mouseover(function () {
        $(".acls_Report").css("color", "#6a6c6f");
    });
    $(".acls_Report").mouseout(function () {
        //$(".acls_Report").css("color", "white");
    });
    BlocksLoadData();
    //$("#DivCompanyName").hide();
    //$(function () {
    //    $("#chkCmp").click(function () {
    //        if ($(this).is(":checked")) {
    //            $("#DivCompanyName").show();
    //        } else {
    //            $("#DivCompanyName").hide();
    //            $("#CompanyId").val("").trigger('change');
    //        }
    //    });
    //});

});


function BlocksLoadData() {
    var params = $.extend({}, doAjax_params_default);

    params['url'] = '/Master/CompanyData';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxCompanySuccess;
    params['errorCallBackFunction'] = ajaxCompanyError;
    doAjax(params);

    params['url'] = BaseUrl + 'odata/Candidates';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);

    params['url'] = BaseUrl + 'odata/Jobs';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxJobSuccess;
    params['errorCallBackFunction'] = ajaxJobError;
    doAjax(params);
    
    $("#ddlDuration option:selected").prop("selected", false);

    $("#ddlActive").empty();

    // Fill Data For Active / In Active Drop Down
    var ddlActive = document.getElementById("ddlActive");
    var optionActive = document.createElement("OPTION");
    optionActive.innerHTML = "Active";
    optionActive.value = 1;

    var optionInActive = document.createElement("OPTION");
    optionInActive.innerHTML = "In Active";
    optionInActive.value = 0;

    //Add the Option element to DropDownList.
    ddlActive.options.add(optionActive);
    ddlActive.options.add(optionInActive);

    params['url'] = '/Master/CompanyUserData?companyId=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxUserSuccess;
    params['errorCallBackFunction'] = ajaxUserError;
    doAjax(params);

    params['url'] = '/Master/ClientsData?companyId=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientSuccess;
    params['errorCallBackFunction'] = ajaxClientError;
    doAjax(params);

    var submitdate = Date.now();
    params['url'] = '/Master/ClientGridData?submitdate=' + submitdate + '&companyId=' + '' + '&clientId=' + '' + '&activeId=' + '' + '&updatedBy=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientGridSuccess;
    params['errorCallBackFunction'] = ajaxClientGridError;
    doAjax(params);

    params['url'] = '/Master/CandidateGridData?clientId=' + '' + '&companyId=' + '' + '&userId=' + '' + '&activeId=' + '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxCandidateGridSuccess;
    params['errorCallBackFunction'] = ajaxCandidateGridError;
    doAjax(params);

    params['url'] = '/Master/MCallerLoadData?clientId=' + '' + '&companyId=' + '' + '&userId=' + '' + '&daysId='+ '';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxMCallerSuccess;
    params['errorCallBackFunction'] = ajaxMCallerError;
    doAjax(params);

    $('#tblCandDatatable').DataTable({
        "destroy": true,
        "processing": true,
        "serverSide": true,
        "orderMulti": false,
        "ajax": {
            "url": "/Master/CandidateLoadData",
            "type": "POST",
            "datatype": "json"
        },
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
                    return '<a href="../../UploadedResume/' + data.CandidateResume + '"  data-data="' + data.CandidateResume + '" class="clsdownload" download>Download</a>';
                }
            },
            { "data": "LastUpdateDateString", "name": "LastUpdateDate", "autoWidth": true }
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
            }
        ],
        columnDefs: [{
            targets: -1,
            visible: true
        }]
    });

    return false;
}

function CandidateLoadData() {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = BaseUrl + 'odata/Candidates';
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxSuccess;
    params['errorCallBackFunction'] = ajaxError;
    doAjax(params);
    return false;
}


function ajaxCompanySuccess(data) {
    console.log(data);
    console.log("Success");
    $("#ddlCompany").empty();
    var ddlCompany = document.getElementById("ddlCompany");
    if (data.length >= 0 && data.length !== undefined) {

        //Add the Options to the DropDownList.
        for (var i = 0; i < data.length; i++) {
            var option = document.createElement("OPTION");

            //Set Customer Name in Text part.
            option.innerHTML = data[i].CompanyName;

            //Set CustomerId in Value part.
            option.value = data[i].ID;

            //Add the Option element to DropDownList.
            ddlCompany.options.add(option);
        }
    }
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxCompanyError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}

function ajaxSuccess(data) {
    console.log(data);
    console.log("Success");
    console.log("Current USerID:" + comCurrentUserId);
    if (data.successlist[0].Code === "1") {
        var totalCandidate = data.successlist[0].Data.length;
       // document.getElementById("TotalCandidate").innerHTML = totalCandidate;
    }
    if (data.successlist[0].Code === "0") {
        toastr.error('Error - ' + data.successlist[0].Message);
    }
    if (comCurrentRole === "SUPER ADMIN") {
        if (data.successlist[0].Data.CompanyId) {
            $("#chkCmp").prop("checked", true);
            $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');
            $("#DivCompanyName").show();
        }
    } else {
        $("#CompanyId").val(data.successlist[0].Data.CompanyId).trigger('change');
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


function ajaxJobSuccess(data) {
    console.log(data);
    console.log("Success");
    if (data.successlist[0].Code === "1") {
        var totalJobs = data.successlist[0].Data.length;
       // document.getElementById("TotalJobs").innerHTML = totalJobs;
    }
    if (data.successlist[0].Code === "0") {
        toastr.error('Error - ' + data.successlist[0].Message);
    }
}

function ajaxJobError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}


function ajaxUserSuccess(data) {
    console.log(data);
    console.log("Success");
    $("#ddlUsers").empty();
    var ddlUsers = document.getElementById("ddlUsers");
    if (data.length >= 0 && data.length !== undefined) {

        if (comCurrentRole === "CLIENT") {
            //Add the Options to the DropDownList.
            for (var i = 0; i < data.length; i++) {
                console.log(data);
                if (comCurrentUserId === data[i].Id) {
                    var option = document.createElement("OPTION");
                    //Set Customer Name in Text part.
                    option.innerHTML = data[i].UserName;
                    //Set CustomerId in Value part.
                    option.value = data[i].Id;
                    //Add the Option element to DropDownList.
                    ddlUsers.options.add(option);
                }
            }
        }
        else {
            //Add the Options to the DropDownList.
            for (var i = 0; i < data.length; i++) {
                console.log(data);
                var option = document.createElement("OPTION");
                //Set Customer Name in Text part.
                option.innerHTML = data[i].UserName;
                //Set CustomerId in Value part.
                option.value = data[i].Id;
                //Add the Option element to DropDownList.
                ddlUsers.options.add(option);
            }
        }

    }
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxUserError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}


function ajaxClientSuccess(data) {
    console.log(data);
    console.log("Success");
    $("#ddlClients").empty();
    var ddlClients = document.getElementById("ddlClients");
    if (data.length >= 0 && data.length !== undefined) {

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

    var totalCandidate = data.ID;
    document.getElementById("TotalCandidate").innerHTML = totalCandidate;

    var totalClients = data.totalClients;
    document.getElementById("TotalCliets").innerHTML = totalClients;

    var ShortListedCount = data.ShortListedCount;
    document.getElementById("ShortListedCount").innerHTML = ShortListedCount;

    var interviewedCount = data.interviewedCount;
    document.getElementById("interviewedCount").innerHTML = interviewedCount;

    var joinedCount = data.joinedCount;
    document.getElementById("joinedCount").innerHTML = joinedCount;

    var rejectedCount = data.rejectedCount;
    document.getElementById("rejectedCount").innerHTML = rejectedCount;

    var totalJobs = data.Job.length;
    document.getElementById("TotalJobs").innerHTML = totalJobs;

    var totalPositions = data.totalPositions;
    document.getElementById("TotalPositions").innerHTML = totalPositions;
   
   
    $('#tblDatatable').DataTable({
        data: data.Job,
        "iDisplayLength": 5,
        columns: [
            { "data": "ClientName", "name": "ClientName" },
            { "data": "JobCount", "name": "JobCount" },
            { "data": "JobName", "name": "JobName" },
            { "data": "JobTargetDate", "name": "JobTargetDate" },
            { "data": "JobStatusString", "name": "JobStatusString" },
            { "data": "LastUpdatedByName", "name": "LastUpdatedByName" }
        ],
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copy'
            }, {
                extend: 'pdfHtml5',
                text: 'PDF'
            }, {
                extend: 'csvHtml5',
                text: 'CSV / EXCEL'
            }
            //'copyHtml5', 'pdfHtml5', 'csvHtml5'
        ],
        'processing': true,
        //'serverSide': true,
        //'language': {
        //    'loadingRecords': '&nbsp;',
        //    'processing': 'Loading...'
        //},    
        destroy: true,
        'columnDefs': [
          {
              "targets": 0,
              "className": "text-left",
          },
         {
             "targets": 1,
             "className": "text-left",
         },
         {
             "targets": 2,
             "className": "text-left",
         },
         {
             "targets": 3,
             "className": "text-left",
         },
         {
            "targets": 4,
            "className": "text-left",
         },
         {
             "targets": 5,
             "className": "text-left",
         }]
    });
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


function ajaxJobSuccessSecond(data) {
    console.log(data);
    console.log("Success");
    var totalJobs = data.length;
    document.getElementById("TotalJobs").innerHTML = totalJobs;
}

function ajaxJobErrorSecond(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}

function ajaxCandidateGridSuccess(data) {
    console.log(data);
    console.log("Success");

    $('#tblCandDatatable').DataTable({
        data: data,
        "order": [[4, "desc"]],
        "iDisplayLength": 5,
        columns: [
            { "data": "CandidateName", "name": "CandidateName" },
            { "data": "ClientDesignation", "name": "ClientDesignation" },
            { "data": "CandidateEmail", "name": "CandidateEmail", "autoWidth": true },
            { "data": "CandidatePhone", "name": "CandidatePhone", "autoWidth": true },
            {
                "data": "LastUpdateDate", "name": "LastUpdateDate",
                "type": "date ",
                "render": function (value) {
                    if (value === null) return "";

                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));

                    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                }, "autoWidth": true
            }
        ],
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copy'
            }, {
                extend: 'pdfHtml5',
                text: 'PDF'
            }, {
                extend: 'csvHtml5',
                text: 'CSV / EXCEL'
            }
            //'copyHtml5', 'pdfHtml5', 'csvHtml5'
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
        },
        {
            "targets": 2,
            "className": "text-left",
        },
        {
            "targets": 3,
            "className": "text-left",
        },
        {
            "targets": 4,
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


function companyChange() {
    $("#btnSubmit").click();
}

$("#btnSubmit").on("click", function (e) {
    debugger;
    var companyDDL = document.getElementById("ddlCompany");
    var selectedCompany = companyDDL.options[companyDDL.selectedIndex].value;

    var selectedClientsArray = [];
    var selectedClients = document.getElementById("ddlClients");
    var listLength = selectedClients.options.length;
    for (var i = 0; i < listLength; i++) {
        if (selectedClients.options[i].selected)
            selectedClientsArray.push(selectedClients.options[i].value);
    }

    var selectedUsersArray = [];
    var selectedUsers = document.getElementById("ddlUsers");
    var usersListLength = selectedUsers.options.length;
    for (var j = 0; j < usersListLength; j++) {
        if (selectedUsers.options[j].selected)
            selectedUsersArray.push(selectedUsers.options[j].value);
    }

    var submitdate = Date.now();
    var params = $.extend({}, doAjax_params_default);

    //params['url'] = '/Master/ReportJobData?submitdate=' + submitdate + "&clientId=" + selectedClientsArray.toString();
    //params['requestType'] = "GET";
    //params['successCallbackFunction'] = ajaxClientGridSuccess;
    //params['errorCallBackFunction'] = ajaxJobErrorSecond;
    //doAjax(params);

    params['url'] = '/Master/ClientGridData?submitdate=' + submitdate + '&companyId=' + selectedCompany + '&clientId=' + selectedClientsArray.toString() + '&activeId=' + $("#ddlActive option:selected").val() + '&updatedBy=' + selectedUsersArray.toString();
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientGridSuccess;
    params['errorCallBackFunction'] = ajaxClientGridError;
    doAjax(params);

    params['url'] = '/Master/CompanyUserData?companyId=' + selectedCompany;
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxUserSuccess;
    params['errorCallBackFunction'] = ajaxUserError;
    doAjax(params);

    params['url'] = '/Master/ClientsData?companyId=' + selectedCompany;
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxClientSuccess;
    params['errorCallBackFunction'] = ajaxClientError;
    doAjax(params);

    var clientId = selectedClientsArray.toString();
    params['url'] = '/Master/CandidateGridData?clientId=' + clientId + '&companyId=' + selectedCompany + '&userId=' + selectedUsersArray.toString() + '&activeId=' + $("#ddlActive option:selected").val() ;
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxCandidateGridSuccess;
    params['errorCallBackFunction'] = ajaxCandidateGridError;
    doAjax(params);

    params['url'] = '/Master/MCallerLoadData?clientId=' + clientId + '&companyId=' + selectedCompany + '&userId=' + selectedUsersArray.toString() + '&daysId=' + $("#ddlDuration option:selected").val();
    params['requestType'] = "GET";
    params['successCallbackFunction'] = ajaxMCallerSuccess;
    params['errorCallBackFunction'] = ajaxMCallerError;
    doAjax(params);

    $("#ddlActive").empty();

    // Fill Data For Active In Active Drop Down
    var ddlActive = document.getElementById("ddlActive");
    var optionActive = document.createElement("OPTION");
    optionActive.innerHTML = "Active";
    optionActive.value = 1;

    var optionInActive = document.createElement("OPTION");
    optionInActive.innerHTML = "In Active";
    optionInActive.value = 0;

    //Add the Option element to DropDownList.
    ddlActive.options.add(optionActive);
    ddlActive.options.add(optionInActive);


    $("#ddlDuration option:selected").prop("selected", false);

    //$.ajax({
    //    url: '/Master/CandidateGridData?clientId=' + id,
    //    type: "GET",
    //    contentType: false,
    //    processData: false,
    //    success: function (result) {
    //        console.log(result);
    //        if (result.data) {
    //            var data = result.data;
    //            $('#tblCandDatatable').DataTable({
    //                "destroy": true,
    //                "processing": true,
    //                "serverSide": true,
    //                "orderMulti": false,
    //                "ajax": {
    //                    "url": "/Master/CandidateLoadData",
    //                    "type": "POST",
    //                    "datatype": "json"
    //                },
    //                "columns": [
    //                    {
    //                        "className": 'details-control',
    //                        "data": "CandidateId",
    //                        "name": "CandidateId",
    //                        "autoWidth": true,
    //                        "defaultContent": ''
    //                    },
    //                    { "data": "CandidateName", "name": "CandidateName", "autoWidth": true },
    //                    { "data": "ClientDesignation", "name": "ClientDesignation", "autoWidth": true },
    //                    { "data": "CandidateEmail", "name": "CandidateEmail", "autoWidth": true },
    //                    { "data": "CandidatePhone", "name": "CandidatePhone", "autoWidth": true },
    //                    {
    //                        data: null,
    //                        className: "center",
    //                        "render": function (data, type, row, meta) {
    //                            return '<a href="../../UploadedResume/' + data.CandidateResume + '"  data-data="' + data.CandidateResume + '" class="clsdownload" download>Download</a>';
    //                        }
    //                    },
    //                    { "data": "LastUpdateDateString", "name": "LastUpdateDate", "autoWidth": true }
    //                ],
    //                "dom": "<'row'<'col-sm-4'l><'col-sm-4 text-center'B><'col-sm-4'f>>tp",
    //                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
    //                buttons: [
    //                    {
    //                        extend: 'csv', title: 'CandidateFile',
    //                        exportOptions: {
    //                            columns: ':visible'
    //                        }
    //                    },
    //                    {
    //                        extend: 'print', title: 'CandidateFile',
    //                        exportOptions: {
    //                            columns: ':visible'
    //                        }
    //                    }
    //                ],
    //                destroy: true,
    //                columnDefs: [{
    //                    targets: -1,
    //                    visible: true
    //                }]
    //            });
    //        }
    //        //toastr.success(result);
    //    },
    //    error: function (err) {
    //        toastr.error(err.statusText);
    //    }
    //});
    return false;
});

function ajaxMCallerSuccess(data) {
    console.log(data);
    console.log("Success");

    var totalCalls = data.totalCalls;
    document.getElementById("TotalCalls").innerHTML = totalCalls;

    $('#tblMCallerDatatable').DataTable({
        data: data.MCallerData,
        "order": [[4, "desc"]],
        "iDisplayLength": 5,
        columns: [
            { "data": "Name", "name": "Name" },
            { "data": "ContactType", "name": "Contact Type" },
            { "data": "Mobile", "name": "Mobile" },
            { "data": "CallType", "name": "Call Type" },
            { "data": "DateTime", "name": "Date TIme" },
            { "data": "Stauts", "name": "Status" },
            { "data": "CallDuration", "name": "Call Duration" },
            { "data": "Remarks", "name": "Remarks" },
            { "data": "NextAction", "name": "Next Action" },
            { "data": "CalledBy", "name": "Called By" },
            
        ],
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copy'
            }, {
                extend: 'pdfHtml5',
                text: 'PDF'
            }, {
                extend: 'csvHtml5',
                text: 'CSV / EXCEL'
            }
            //'copyHtml5', 'pdfHtml5', 'csvHtml5'
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
        },
        {
            "targets": 2,
            "className": "text-left",
        },
        {
            "targets": 3,
            "className": "text-left",
        },
        {
            "targets": 4,
            "className": "text-left",
        },
        {
            "targets": 5,
            "className": "text-left",
        },
        {
            "targets": 6,
            "className": "text-left",
        },
        {
            "targets": 7,
            "className": "text-left",
        },
        {
            "targets": 8,
            "className": "text-left",
        },
        {
            "targets": 9,
            "className": "text-left",
        }]
    });
    if (data.length <= 0 && data.length === undefined) {
        toastr.error('Error - ' + data);
    }
}

function ajaxMCallerError(data) {
    console.log("Error");
    console.log(data.responseText);
    var Errorobj = jQuery.parseJSON(data.responseText);
    if (Errorobj.errors[0].Code === "0") {
        toastr.error('Error - ' + Errorobj.errors[0].Message);
    } else {
        toastr.error('Error - Something is wrong...!!!');
    }
}

$("#btnReset").on("click", function (e) {
    BlocksLoadData();
    return false;
});