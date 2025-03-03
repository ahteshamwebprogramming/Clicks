
let croppedImageBase64;
let milestoneIndex = 0;

$(document).ready(function () {

    let SerializedMilestones = $("#SerializedMilestones").val();

    if ($.trim(SerializedMilestones) != "") {
        arrMilestones = JSON.parse(SerializedMilestones);
    }
    let SerializedDeliverables = $("#SerializedDeliverables").val();

    //if ($.trim(SerializedDeliverables) != "") {
    //    arrDeliverables = JSON.parse(SerializedDeliverables);
    //}

    var $select = $('.select2').select2();
    //console.log($select);
    $select.each(function (i, item) {
        //console.log(item);
        $(item).select2("destroy");
    });



    $("#AddProjectForm").find("[name='Project.CategoryID']").select2();
    $("#AddProjectForm").find("[name='Project.StatusID']").select2();
    $("#AddProjectForm").find("[name='Project.PriorityId']").select2();
    $(".approvers").select2();
    $(".collaborators").select2();
    //$("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val('[{"value":"Atesham"},{"value":"Adil"},{"value":"Test"}]')


    //Dropzone



    // Initialize Dropzone with custom options
    //var myDropzone = new Dropzone("#dropz", {
    //    url: '/EmployeeAccouncement/UploadResume', // Specify the URL where files should be uploaded
    //    paramName: "file", // Name of the file parameter
    //    //maxFilesize: 5, // Maximum file size in MB
    //    //maxFiles: 5, // Maximum number of files allowed to upload
    //    //acceptedFiles: ".jpg, .jpeg, .png, .gif", // Specify accepted file types
    //    addRemoveLinks: true, // Show remove button for uploaded files
    //    dictDefaultMessage: "Drop files here or click to upload", // Default message
    //    dictRemoveFile: "Remove file", // Text for the remove file link
    //    dictCancelUpload: "Cancel upload", // Text for the cancel upload link
    //    // Add additional options as needed
    //});

    //// Event listeners for Dropzone events
    //myDropzone.on("addedfile", function (file) {
    //    // Handle when a file is added
    //});

    //myDropzone.on("removedfile", function (file) {
    //    // Handle when a file is removed
    //});

    //myDropzone.on("success", function (file, response) {
    //    // Handle when a file is successfully uploaded
    //});

    //myDropzone.on("error", function (file, errorMessage, xhr) {
    //    // Handle when an error occurs during file upload
    //});

    // Add more event listeners as needed
    $("#AddProjectForm").find("[required]").on("focus blur click change", function () {

        let currObj = $(this);
        if (currObj[0].tagName == "SELECT") {
            currObj.parent().find(".text-danger").text("");
            currObj.parent().find(".text-danger").hide();
        }
        else {
            currObj.parent().find(".text-danger").text("");
            currObj.parent().find(".text-danger").hide();
        }
    });
    //$("#myInput").on("focus blur", function () {
    //    // Your code here
    //    if ($(this).is(":focus")) {
    //        console.log("Input is focused");
    //        // Do something when input is focused
    //    } else {
    //        console.log("Input is blurred");
    //        // Do something when input is blurred
    //    }
    //});

    var e = document.querySelector("#flatpickr-date-StartDate")
        , f = document.querySelector("#flatpickr-date-EndDate")
        , t = document.querySelector("#flatpickr-time");
    e && e.flatpickr({
        dateFormat: "d-M-Y"
    }),
        f && f.flatpickr({
            dateFormat: "d-M-Y"
        }),
        t && t.flatpickr({
            enableTime: !0,
            noCalendar: !0
        }) && (
            e = new Date(Date.now() - 1728e5),
            f = new Date(Date.now() - 1728e5),
            t = new Date(Date.now() + 1728e5)
        )
});

function isValidateForm() {
    let res = true;

    let ProjectName = $("#AddProjectForm").find("[name='Project.ProjectName']").val();
    let CategoryID = $("#AddProjectForm").find("[name='Project.CategoryID']").val();
    let Description = $("#AddProjectForm").find("[name='Project.Description']").val();
    let StartDate = $("#AddProjectForm").find("[name='Project.StartDate']").val();
    let EndDate = $("#AddProjectForm").find("[name='Project.EndDate']").val();
    let PriorityId = $("#AddProjectForm").find("[name='Project.PriorityId']").val();
    let StatusID = $("#AddProjectForm").find("[name='Project.StatusID']").val();


    //let Title = $("#AddProjectForm").find("[name='employeeAnnouncementDTO.Title']").val();
    //let Departments = $("#AddProjectForm").find("[name='employeeAnnouncementDTO.Departments']").val();
    //let Keywords = $("#AddProjectForm").find("[name='employeeAnnouncementDTO.Keywords']").val();

    //let keywordsRaw = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val();
    //let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw)

    //Thumbnail Validation
    //var filesUpload = jQuery("#uploads")[0].files;
    //var filesUploaded = $("#uploads").parent().find("[name^=attachmentTag_]");
    if (!(ProjectName != null && ProjectName != undefined && $.trim(ProjectName) != "")) {
        $("#AddProjectForm").find("[name='Project.ProjectName']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.ProjectName']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(CategoryID != null && CategoryID != undefined && $.trim(CategoryID) != 0)) {
        $("#AddProjectForm").find("[name='Project.CategoryID']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.CategoryID']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(StartDate != null && StartDate != undefined && $.trim(StartDate) != "")) {
        $("#AddProjectForm").find("[name='Project.StartDate']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.StartDate']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(EndDate != null && EndDate != undefined && $.trim(EndDate) != "")) {
        $("#AddProjectForm").find("[name='Project.EndDate']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.EndDate']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(PriorityId != null && PriorityId != undefined && $.trim(PriorityId) != "")) {
        $("#AddProjectForm").find("[name='Project.PriorityId']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.PriorityId']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(StatusID != null && StatusID != undefined && $.trim(StatusID) != 0)) {
        $("#AddProjectForm").find("[name='Project.StatusID']").parent().find(".text-danger").text("This field is mandatory");
        $("#AddProjectForm").find("[name='Project.StatusID']").parent().find(".text-danger").show();
        res = false;
    }
    //if (!(Title != null && Title != undefined && $.trim(Title) != "")) {
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").parent().find(".text-danger").text("This field is mandatory");
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").parent().find(".text-danger").show();
    //    res = false;
    //}
    //if (!(Departments != null && Departments != undefined && Departments.length != 0)) {
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").parent().parent().find(".text-danger").text("This field is mandatory");
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").parent().parent().find(".text-danger").show();
    //    res = false;
    //}
    //if (!(keywordsJson != null && keywordsJson != undefined && keywordsJson.length != 0)) {
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").parent().find(".text-danger").text("This field is mandatory");
    //    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").parent().find(".text-danger").show();
    //    res = false;
    //}

    //if (!(filesUpload != null && filesUpload != undefined && filesUpload.length != 0 && croppedImageBase64 != undefined && croppedImageBase64 != "") && (filesUploaded.length == 0)) {
    //    $("#AnnoucementForm").find("[id='uploads']").parent().find(".text-danger").text("This field is mandatory");
    //    $("#AnnoucementForm").find("[id='uploads']").parent().find(".text-danger").show();
    //    res = false;
    //}
    return res;
}

function SaveProject() {
    BlockUI();

    if (!isValidateForm()) {
        UnblockUI();
        return;
    }

    //let keywordsRaw = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val();
    //let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw)
    //var keywordscommaSeparated = '';
    //if (keywordsRaw != null && $.trim(keywordsRaw) != "") {
    //    $.each(keywordsJson, function (k, v) {
    //        keywordscommaSeparated += v.value + ', ';
    //    });
    //    keywordscommaSeparated = keywordscommaSeparated.slice(0, -2);
    //}

    let dataVM = new FormData();

    //var filesAttachment = jQuery("#attachments")[0].files;
    //for (var i = 0; i < filesAttachment.length; i++) {
    //    dataVM.append("Attachment", filesAttachment[i]);
    //}
    //var filesUpload = jQuery("#uploads")[0].files;
    //for (var i = 0; i < filesUpload.length; i++) {
    //    dataVM.append("Upload", filesUpload[i]);
    //}

    //var filesUploaded = $("#uploads").parent().find("[name^=attachmentTag_]");
    //if (filesUploaded.length > 0) {
    //    dataVM.append("ExistingThumbnail", true);
    //}
    //else {
    //    dataVM.append("ExistingThumbnail", false);
    //}
    //dataVM.append("ThumbnailImage", croppedImageBase64);

    dataVM.append("Project.ProjectName", $("#AddProjectForm").find("[name='Project.ProjectName']").val());
    dataVM.append("Project.CategoryID", $("#AddProjectForm").find("[name='Project.CategoryID']").val());
    dataVM.append("Project.Description", $("#AddProjectForm").find("[name='Project.Description']").val());
    dataVM.append("Project.StartDate", moment(moment($("#AddProjectForm").find("[name='Project.StartDate']").val()).format("DD-MMM-YYYY")).format("YYYY-MM-DD"));
    dataVM.append("Project.EndDate", moment(moment($("#AddProjectForm").find("[name='Project.EndDate']").val()).format("DD-MMM-YYYY")).format("YYYY-MM-DD"));
    dataVM.append("Project.PriorityId", $("#AddProjectForm").find("[name='Project.PriorityId']").val());
    dataVM.append("Project.StatusID", $("#AddProjectForm").find("[name='Project.StatusID']").val());



    //dataVM.append("employeeAnnouncementDTO.Title", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").val());
    //dataVM.append("employeeAnnouncementDTO.Departments", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").val().join(','));
    //dataVM.append("employeeAnnouncementDTO.KeywordsRaw", keywordsRaw);
    //dataVM.append("employeeAnnouncementDTO.Keywords", keywordscommaSeparated);
    //dataVM.append("employeeAnnouncementDTO.Description", $("#full-editor").find(".ql-editor").html());
    dataVM.append("Project.encProjectID", $("#AddProjectForm").find("[name='Project.encProjectID']").val());
    //dataVM.append("employeeAnnouncementDTO.Type", type);

    dataVM.append("SerializedMilestones", JSON.stringify(arrMilestones));
    //dataVM.append("SerializedDeliverables", JSON.stringify(arrDeliverables));
    dataVM.append("ApproverId", $("#AddProjectForm").find("[name='ApproverId']").val());
    dataVM.append("CollaboratorIds", $("#AddProjectForm").find("[name='CollaboratorIds']").val());

    let approverId = $("#AddProjectForm").find("[name='ApproverId']").val();
    if (approverId != undefined && approverId != null && approverId > 0) {
        dataVM.append("Project.ApprovalNeeded", true);
    }
    else {
        dataVM.append("Project.ApprovalNeeded", false);
    }

    //if (type == "Schedule") {
    //    dataVM.append("employeeAnnouncementDTO.PublishDate", $("#ScheduleModal").find("[name='PublishDate']").val());
    //    dataVM.append("employeeAnnouncementDTO.PublishTime", $("#ScheduleModal").find("[name='PublishTime']").val());
    //}

    //$("#ScheduleModal").find(".btn-close").click();

    //let approvers = $("[name='Approvers']").val();
    //let collaborators = $("[name='Collaborators']").val();
    //alert(approvers);
    //alert(collaborators);

    //UnblockUI();
    //return;
    BlockUI();
    $.ajax({
        url: '/ProjectTracker/SaveProject',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $successalert("", "Transaction Successful!");
            UnblockUI();
            setTimeout(function () {
                window.location.href = "/ProjectTracker/ProjectList1";
            }, 2000);
        },
        error: function (xhr, ajaxOptions, error) {
            $erroralert("Transaction Failed!", xhr.responseText + '!');
            UnblockUI();
        }
    });

}



function deleteUploadedFile(EmployeeAnnouncementFileUploadsId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "EmployeeAnnouncementFileUploadsId": EmployeeAnnouncementFileUploadsId
            };
            $.ajax({
                type: "POST",
                url: "/EmployeeAccouncement/DeleteUploadedFile",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Transaction Successful!");
                    $("[name='attachmentTag_" + EmployeeAnnouncementFileUploadsId + "']").remove();
                    UnblockUI();
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}





//Survey Poll

function isValidateMilestoneForm() {
    intResetError();
    let res = true;
    let SNo = $("#MilestonesDiv").find("[name='SNo']").val();
    let MilestoneName = $("#MilestonesDiv").find("[name='MilestoneName']").val();
    let Deliverables = $("#MilestonesDiv").find("[name='Deliverables']").val();

    if (!(SNo != null && SNo != undefined && $.trim(SNo) != "")) {
        $("#MilestonesDiv").find("[name='SNo']").parent().find(".text-danger").text("This field is mandatory");
        $("#MilestonesDiv").find("[name='SNo']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(MilestoneName != null && MilestoneName != undefined && $.trim(MilestoneName) != "")) {
        $("#MilestonesDiv").find("[name='MilestoneName']").parent().find(".text-danger").text("This field is mandatory");
        $("#MilestonesDiv").find("[name='MilestoneName']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(Deliverables != null && Deliverables != undefined && $.trim(Deliverables) != "")) {
        $("#MilestonesDiv").find("[name='Deliverables']").parent().find(".text-danger").text("This field is mandatory");
        $("#MilestonesDiv").find("[name='Deliverables']").parent().find(".text-danger").show();
        res = false;
    }

    return res;
}


let arrMilestones = [];


function SaveMilestones() {
    BlockUI();
    if (!isValidateMilestoneForm()) {
        UnblockUI();
        return;
    }

    let MilestonesViewModel = {};

    let index = $("#MilestonesModal").find("[name='MilestoneName']").attr("index");
    if (index == -1) {
        MilestonesViewModel.MilestoneName = $("#MilestonesModal").find("[name='MilestoneName']").val();
        MilestonesViewModel.Deliverables = $("#MilestonesModal").find("[name='Deliverables']").val();
        MilestonesViewModel.SNo = $("#MilestonesModal").find("[name='SNo']").val();
        MilestonesViewModel.IsActive = 1;

        arrMilestones.push(MilestonesViewModel);
    }
    else {

        arrMilestones[index].MilestoneName = $("#MilestonesModal").find("[name='MilestoneName']").val();
        arrMilestones[index].Deliverables = $("#MilestonesModal").find("[name='Deliverables']").val();
        arrMilestones[index].SNo = $("#MilestonesModal").find("[name='SNo']").val();
    }

    showMilestones();
    $("#MilestonesDiv").empty();
    $("#btnAddMilestones").show();
    UnblockUI();
}



function DefineMilestonesPopup() {
    showMilestones();
}

function showMilestones() {
    $("#tblMilestones").empty();
    if (arrMilestones.length > 0) {
        $("#tblMilestones").append('<div class="table-responsive text-nowrap"><table class="datatables-ajax table List"><thead><tr><th>SNo</th><th>Milestone</th><th>Deliverables</th><th>Action</th></tr></thead><tbody></tbody></table></div>');
    }
    arrMilestones.forEach((v, i) => {
        $("#tblMilestones").find("table tbody").append('<tr><td>' + v.SNo + '</td><td>' + v.MilestoneName + '</td><td>' + v.Deliverables + '</td><td><a class="dropdown-item d-inline" href="#" onclick="editMilestone(' + i + ')"><i class="bx bx-edit-alt me-1"></i></a><a class="dropdown-item d-inline" onclick="deleteMilestone(' + i + ')" href="#" ><i class="bx bx-trash me-1"></i></a ></td></tr>');
    });
}

function AddMilestones() {
    $("#MilestonesDiv").append('<div class="form-group col-md-3 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required=""> SNo</label>' +
        '<input type="number" class="form-control" placeholder="SNo" name="SNo" index="-1" required="">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="form-group col-md-9 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required=""> Milestone Name</label>' +
        '<input type="text" class="form-control" placeholder="Milestone Name" name="MilestoneName" index="-1" required="">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="form-group col-md-12 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required="">Deliverables</label>' +
        '<input type="text" class="form-control" placeholder="Deliverables" name="Deliverables" index="-1" required="">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '</div>' +


        ' <div class="form-group col-md-12 mb-3">' +
        ' <button type="button" class="btn btn-primary btn-sm" onclick="SaveMilestones()">Save Milestone</button>' +
        ' <button type="button" class="btn btn-secondary btn-sm" onclick="CancelMilestones()">Cancel</button>      ' +
        ' </div> ');
    $("#btnAddMilestones").hide();
}


function CancelMilestones() {
    $("#MilestonesDiv").empty();
    $("#btnAddMilestones").show();
}


function intResetError() {
    $("#MilestonesDiv").find("[required]").on("focus blur click change", function () {
        let currObj = $(this);
        if (currObj[0].tagName == "SELECT") {
            currObj.parent().parent().find(".text-danger").text("");
            currObj.parent().parent().find(".text-danger").hide();
        }
        else if (currObj.parent().attr("class") == "input-group") {
            currObj.parent().parent().find(".text-danger").text("");
            currObj.parent().parent().find(".text-danger").hide();
        }
        else {
            currObj.parent().find(".text-danger").text("");
            currObj.parent().find(".text-danger").hide();
        }
    });
}


function editMilestone(index) {
    $("#MilestonesDiv").empty();
    let sp = arrMilestones[index];

    let PollOptions = sp.PollOptions;
    let options = '';

    $("#MilestonesDiv").append('<div class="form-group col-md-3 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required=""> SNo</label>' +
        '<input type="number" class="form-control" placeholder="SNo" name="SNo" index="' + index + '" required="" value="' + sp.SNo + '">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="form-group col-md-9 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required=""> Milestone Name</label>' +
        '<input type="text" class="form-control" placeholder="Milestone Name" name="MilestoneName" index="' + index + '" required="" value="' + sp.MilestoneName + '">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="form-group col-md-12 mb-3">' +
        '<label for="defaultFormControlInput" class="form-label required" required=""> Deliverables</label>' +
        '<input type="text" class="form-control" placeholder="Deliverables" name="Deliverables" index="' + index + '" required="" value="' + sp.Deliverables + '">' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '</div>' +


        ' <div class="form-group col-md-12 mb-3">' +
        ' <button type="button" class="btn btn-primary btn-sm" onclick="SaveMilestones()">Save Milestone</button>' +
        ' <button type="button" class="btn btn-secondary btn-sm" onclick="CancelMilestones()">Cancel</button>      ' +
        ' </div> ');

    $("#btnAddMilestones").hide();
}
function deleteMilestone(index) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let sp = arrMilestones[index];

            let milestoneName = sp.MilestoneName;
            let MilestoneId = sp.MilestoneId;

            if (MilestoneId != undefined && MilestoneId != null && MilestoneId > 0) {
                DeleteMilestoneFromDatabase(MilestoneId);
            }
            arrMilestones.splice(index, 1);
            showMilestones();
        }
    });
}






function DeleteMilestoneFromDatabase(MilestoneId) {
    BlockUI();
    var inputDTO = {
        "MilestoneId": MilestoneId
    };
    $.ajax({
        type: "POST",
        url: "/ProjectTracker/DeleteMilestone",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            $successalert("", "Transaction Successful!");
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}