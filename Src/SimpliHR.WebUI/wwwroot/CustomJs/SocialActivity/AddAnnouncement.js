Dropzone.autoDiscover = false; // Disable auto discovery of dropzone elements

let croppedImageBase64;
$(document).ready(function () {
    var cropper;


    const fullToolbar = [
        [
            {
                font: []
            },
            {
                size: []
            }
        ],
        ['bold', 'italic', 'underline', 'strike'],
        [
            {
                color: []
            },
            {
                background: []
            }
        ],
        [
            {
                script: 'super'
            },
            {
                script: 'sub'
            }
        ],
        [
            {
                header: '1'
            },
            {
                header: '2'
            },
            'blockquote',
            'code-block'
        ],
        [
            {
                list: 'ordered'
            },
            {
                list: 'bullet'
            },
            {
                indent: '-1'
            },
            {
                indent: '+1'
            }
        ],
        [{ direction: 'rtl' }],
        ['link', 'image', 'video', 'formula'],
        ['clean']
    ];

    const fullEditor1 = new Quill('#full-editor1', {
        bounds: '#full-editor1',
        placeholder: 'Type Something...',
        modules: {
            formula: true,
            toolbar: fullToolbar
        },
        theme: 'snow'
    });

    $("#uploads").change(function (e) {
        $("#btnModalImageCrop").click();
        var fileReader = new FileReader();
        fileReader.onload = function (event) {
            $('#croppedImage').attr('src', event.target.result);
            if (cropper) {
                croppedImageBase64 = "";
                cropper.destroy();
            }
            cropper = new Cropper($('#croppedImage')[0], {
                aspectRatio: 1 / 1,
                viewMode: 1,
                minContainerWidth: 600,
                minContainerHeight: 600,

            });
        };
        fileReader.readAsDataURL(e.target.files[0]);
    });
    $("#backDropModal").find(".btn-close").click(function () {
        var croppedCanvas = cropper.getCroppedCanvas();
        croppedImageBase64 = croppedCanvas.toDataURL();
        console.log(croppedImageBase64);
    });
    $("#cropButton").click(function () {
        var croppedCanvas = cropper.getCroppedCanvas();
        croppedImageBase64 = croppedCanvas.toDataURL();
        console.log(croppedImageBase64);
        $("#backDropModal").find(".btn-close").click();
    });
});

$(document).ready(function () {

    let SerializedSurveyPolls = $("#SerializedSurveyPolls").val();

    if ($.trim(SerializedSurveyPolls) != "") {
        SurveyPolls = JSON.parse(SerializedSurveyPolls);
    }

    var $select = $('.select2').select2();
    //console.log($select);
    $select.each(function (i, item) {
        //console.log(item);
        $(item).select2("destroy");
    });


    $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.AnnouncementType']").select2();
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
    $("#AnnoucementForm").find("[required]").on("focus blur click change", function () {

        let currObj = $(this);
        if (currObj[0].tagName == "SELECT") {
            currObj.parent().parent().find(".text-danger").text("");
            currObj.parent().parent().find(".text-danger").hide();
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

    var e = document.querySelector("#flatpickr-date")
        , t = document.querySelector("#flatpickr-time");
    e && e.flatpickr({
        dateFormat: "d-M-Y"
    }),
        t && t.flatpickr({
            enableTime: !0,
            noCalendar: !0
        }) && (
            e = new Date(Date.now() - 1728e5),
            t = new Date(Date.now() + 1728e5)
        )
});

function isValidateForm() {
    let res = true;

    let AnnouncementType = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.AnnouncementType']").val();
    let Title = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").val();
    let Departments = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").val();
    let Keywords = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val();

    let keywordsRaw = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val();
    let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw)

    //Thumbnail Validation
    var filesUpload = jQuery("#uploads")[0].files;
    var filesUploaded = $("#uploads").parent().find("[name^=attachmentTag_]");

    if (!(AnnouncementType != null && AnnouncementType != undefined && $.trim(AnnouncementType) != 0)) {
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.AnnouncementType']").parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.AnnouncementType']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(Title != null && Title != undefined && $.trim(Title) != "")) {
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(Departments != null && Departments != undefined && Departments.length != 0)) {
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").parent().parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").parent().parent().find(".text-danger").show();
        res = false;
    }
    if (!(keywordsJson != null && keywordsJson != undefined && keywordsJson.length != 0)) {
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").parent().find(".text-danger").show();
        res = false;
    }

    if (!(filesUpload != null && filesUpload != undefined && filesUpload.length != 0 && croppedImageBase64 != undefined && croppedImageBase64 != "") && (filesUploaded.length == 0)) {
        $("#AnnoucementForm").find("[id='uploads']").parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementForm").find("[id='uploads']").parent().find(".text-danger").show();
        res = false;
    }
    return res;
}

function SaveAnnouncement(type) {
    BlockUI();

    if (!isValidateForm()) {
        UnblockUI();
        return;
    }

    let keywordsRaw = $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Keywords']").val();
    let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw)
    var keywordscommaSeparated = '';
    if (keywordsRaw != null && $.trim(keywordsRaw) != "") {
        $.each(keywordsJson, function (k, v) {
            keywordscommaSeparated += v.value + ', ';
        });
        keywordscommaSeparated = keywordscommaSeparated.slice(0, -2);
    }

    let dataVM = new FormData();

    var filesAttachment = jQuery("#attachments")[0].files;
    for (var i = 0; i < filesAttachment.length; i++) {
        dataVM.append("Attachment", filesAttachment[i]);
    }
    var filesUpload = jQuery("#uploads")[0].files;
    for (var i = 0; i < filesUpload.length; i++) {
        dataVM.append("Upload", filesUpload[i]);
    }

    var filesUploaded = $("#uploads").parent().find("[name^=attachmentTag_]");
    if (filesUploaded.length > 0) {
        dataVM.append("ExistingThumbnail", true);
    }
    else {
        dataVM.append("ExistingThumbnail", false);
    }
    dataVM.append("ThumbnailImage", croppedImageBase64);

    dataVM.append("employeeAnnouncementDTO.AnnouncementType", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.AnnouncementType']").val());
    dataVM.append("employeeAnnouncementDTO.Title", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Title']").val());
    dataVM.append("employeeAnnouncementDTO.Departments", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.Departments']").val().join(','));
    dataVM.append("employeeAnnouncementDTO.KeywordsRaw", keywordsRaw);
    dataVM.append("employeeAnnouncementDTO.Keywords", keywordscommaSeparated);
    dataVM.append("employeeAnnouncementDTO.Description", $("#full-editor").find(".ql-editor").html());
    dataVM.append("employeeAnnouncementDTO.encEmployeeAnnouncementId", $("#AnnoucementForm").find("[name='employeeAnnouncementDTO.encEmployeeAnnouncementId']").val());
    dataVM.append("employeeAnnouncementDTO.Type", type);
    dataVM.append("SerializedSurveyPolls", JSON.stringify(SurveyPolls));

    if (type == "Schedule") {
        dataVM.append("employeeAnnouncementDTO.PublishDate", $("#ScheduleModal").find("[name='PublishDate']").val());
        dataVM.append("employeeAnnouncementDTO.PublishTime", $("#ScheduleModal").find("[name='PublishTime']").val());
    }

    $("#ScheduleModal").find(".btn-close").click();

    $.ajax({
        url: '/EmployeeAccouncement/SaveAnnouncement',
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
                window.location.href = "/EmployeeAccouncement/EmployeeAnnouncements";
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

function addOption(button) {
    let options = $("#SurveyPollModal").find(".optionsDiv").find("[name^=Options_]");

    let html = OptionHTML(options.length, null, undefined);

    //let html = '<div class="form-group col-md-12 mb-3 singleoptionDiv">' +
    //    '<label for="defaultFormControlInput" class="form-label required" required> Option ' + (options.length + 1) + '</label>' +
    //    '<div class="input-group"><input type="text" class="form-control" name="Options_' + (options.length + 1) + '" placeholder="Option ' + (options.length + 1) + '" required><button class="btn btn-outline-danger delete-option-btn" type="button" onclick="deleteOption(this)"><i class="fas fa-trash-alt"></i></button></div>' +
    //    '<span class="text-danger validation-error" style="display:none"></span>' +
    //    '                   </div>';

    //let html = '<div class="form-group col-md-12 mb-3"><label for="defaultFormControlInput" class="form-label required">Option ' + (options.length + 1) + '</label><input type="text" class="form-control"  name="Options_' + (options.length + 1) + '"  placeholder="Option ' + (options.length + 1) + '" required><span class="text-danger validation-error" style="display:none"></span></div>';
    const optionDiv = document.createElement('div');
    optionDiv.innerHTML = html;
    $("#SurveyPollModal").find(".optionsDiv").append(html);
}

function isValidateSurveyPollForm() {
    intResetError();
    let res = true;
    let Question = $("#SurveyPollModal").find("[name='Question']").val();
    let Options = $("#SurveyPollModal").find(".optionsDiv").find("input[required]");
    if (!(Question != null && Question != undefined && $.trim(Question) != "")) {
        $("#SurveyPollModal").find("[name='Question']").parent().find(".text-danger").text("This field is mandatory");
        $("#SurveyPollModal").find("[name='Question']").parent().find(".text-danger").show();
        res = false;
    }
    Options.each((i, v) => {
        let option = $(v).val();
        if (!(option != null && option != undefined && $.trim(option) != "")) {
            $(v).parent().parent().find(".text-danger").text("This field is mandatory");
            $(v).parent().parent().find(".text-danger").show();
            res = false;
        }
    });
    return res;
}

let SurveyPolls = [];

function SaveSurveyPoll() {
    BlockUI();
    if (!isValidateSurveyPollForm()) {
        UnblockUI();
        return;
    }

    let SurveyPollViewModel = {};
    let PollQuestion = {};

    let PollOptions = [];


    let index = $("#SurveyPollModal").find("[name='Question']").attr("index");
    if (index == -1) {
        PollQuestion.Question = $("#SurveyPollModal").find("[name='Question']").val();
        PollQuestion.IsActive = true;

        let options = $("#SurveyPollModal").find(".optionsDiv").find("[name^=Options_]");
        options.each((i, v) => {
            let SurveyPollOptionDTO = {};
            SurveyPollOptionDTO.OptionName = $(v).val();
            SurveyPollOptionDTO.IsActive = true;
            SurveyPollOptionDTO.SurveyPollOptionId = 0;
            PollOptions.push(SurveyPollOptionDTO);
        });
        SurveyPollViewModel.PollQuestion = PollQuestion;
        SurveyPollViewModel.PollOptions = PollOptions;

        SurveyPolls.push(SurveyPollViewModel);
    }
    else {
        let surveyPollQuestion = SurveyPolls[index].PollQuestion;
        let surveyPollOption = SurveyPolls[index].PollOptions;
        surveyPollQuestion.Question = $("#SurveyPollModal").find("[name='Question']").val();

        let options = $("#SurveyPollModal").find(".optionsDiv").find("[name^=Options_]");
        options.each((i, v) => {
            let optionIndex = $(v).attr("index");
            if (optionIndex == -1) {
                let SurveyPollOptionDTO = {};
                SurveyPollOptionDTO.OptionName = $(v).val();
                SurveyPollOptionDTO.IsActive = true;
                SurveyPollOptionDTO.SurveyPollOptionId = 0;
                SurveyPolls[index].PollOptions.push(SurveyPollOptionDTO);
                //PollOptions.push(SurveyPollOptionDTO);
            }
            else {
                surveyPollOption[optionIndex].OptionName = $(v).val();
            }

        });

    }






    showSurveyPoll();
    $("#SurveyPollAddQuestionDiv").empty();
    $("#btnSurveyPollAddQuestion").show();
    UnblockUI();
}


function CreatePollPopup() {
    showSurveyPoll();
}

function showSurveyPoll() {
    $("#tblSurveyPolls").empty();
    if (SurveyPolls.length > 0) {
        $("#tblSurveyPolls").append('<div class="table-responsive text-nowrap"><table class="datatables-ajax table List"><thead><tr><th>Question</th><th>Action</th></tr></thead><tbody></tbody></table></div>');
    }
    SurveyPolls.forEach((v, i) => {
        $("#tblSurveyPolls").find("table tbody").append('<tr><td>' + v.PollQuestion.Question + '</td><td><a class="dropdown-item d-inline" href="#" onclick="editQuestion(' + i + ')"><i class="bx bx-edit-alt me-1"></i></a><a class="dropdown-item d-inline" onclick="deleteQuestion(' + i + ')" href="#" ><i class="bx bx-trash me-1"></i></a ></td></tr>');
    });
}

function AddQuestion() {
    $("#SurveyPollAddQuestionDiv").append('<div class="form-group col-md-12 mb-3">' +
        '<label for="defaultFormControlInput" class= "form-label required" required> Question Name</label>' +
        '<input type="text" class= "form-control" placeholder = "Question" name = "Question" index="-1" required >' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="optionsDiv"> ' +

        OptionHTML(0, null, undefined) +

        //'<div class="form-group col-md-12 mb-3 singleoptionDiv">' +
        //'<label for="defaultFormControlInput" class="form-label required" required> Option 1</label>' +
        //'<div class="input-group"><input type="text" class="form-control" name="Options_1" placeholder="Option 1" required><button class="btn btn-outline-danger delete-option-btn" type="button" onclick="deleteOption(this)"><i class="fas fa-trash-alt"></i></button></div>' +
        //'<span class="text-danger validation-error" style="display:none"></span>' +
        //'                   </div>' +
        '              </div> ' +
        '<div class="form-group col-md-12 mb-3">' +
        '<a href="javascript:void(0)" onclick="addOption(this)">Add Option</a>' +
        '     </div> ' +
        '<div class="form-group col-md-12 mb-3">' +
        ' <button type="button" class="btn btn-primary btn-sm" onclick="SaveSurveyPoll()">Save Poll</button>' +
        ' <button type="button" class="btn btn-secondary btn-sm" onclick="CancelSurveyPoll()">Cancel</button>      ' +
        '     </div> ');
    $("#btnSurveyPollAddQuestion").hide();
}

function CancelSurveyPoll() {
    $("#SurveyPollAddQuestionDiv").empty();
    $("#btnSurveyPollAddQuestion").show();
}

function intResetError() {
    $("#SurveyPollModal").find("[required]").on("focus blur click change", function () {
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

function OptionHTML(index, optionObj, questionindex) {
    let option = '<div class="form-group col-md-12 mb-3 singleoptionDiv ' + ((optionObj != null && optionObj.IsActive == false) ? "d-none" : "") + ' ">' +
        '<label for="defaultFormControlInput" class="form-label required" required> Option</label>' +
        '<div class="input-group"><input type="text" class="form-control" index="' + ((optionObj != null) ? index : -1) + '" SurveyPollOptionId="' + ((optionObj != null) ? optionObj.SurveyPollOptionId : 0) + '" name="Options_' + (index + 1) + '" placeholder="Option" value="' + (optionObj == null ? "" : optionObj.OptionName) + '" required><button class="btn btn-outline-danger delete-option-btn" type="button" attroptionindex="' + index + '" attrquestionindex="' + questionindex + '" onclick="deleteOption(this)"><i class="fas fa-trash-alt"></i></button></div>' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '                   </div>';
    return option;
}

function editQuestion(index) {
    $("#SurveyPollAddQuestionDiv").empty();
    let sp = SurveyPolls[index];

    let PollOptions = sp.PollOptions;
    let options = '';
    PollOptions.forEach((v, i) => {
        options += OptionHTML(i, v, index);
        //options += '<div class="form-group col-md-12 mb-3 singleoptionDiv">' +
        //    '<label for="defaultFormControlInput" class="form-label required" required> Option ' + (i + 1) + '</label>' +
        //    '<div class="input-group"><input type="text" class="form-control" name="Options_' + (i + 1) + '" placeholder="Option ' + (i + 1) + '" value="' + v.OptionName + '" required><button class="btn btn-outline-danger delete-option-btn" type="button" attroptionindex="' + i + '" attrquestionindex="' + index + '" onclick="deleteOption(this)"><i class="fas fa-trash-alt"></i></button></div>' +
        //    '<span class="text-danger validation-error" style="display:none"></span>' +
        //    '                   </div>';
    });

    $("#SurveyPollAddQuestionDiv").append('<div class="form-group col-md-12 mb-3">' +
        '<label for="defaultFormControlInput" class= "form-label required" required> Question Name</label>' +
        '<input type="text" class="form-control" placeholder = "Question" name="Question" index="' + index + '" required value="' + sp.PollQuestion.Question + '" >' +
        '<span class="text-danger validation-error" style="display:none"></span>' +
        '</div>' +
        '<div class="optionsDiv"> ' +
        options +
        '              </div> ' +
        '<div class="form-group col-md-12 mb-3">' +
        '<a href="javascript:void(0)" onclick="addOption(this)">Add Option</a>' +
        '               </div> ');
    $("#btnSurveyPollAddQuestion").hide();
}
function deleteQuestion(index) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let sp = SurveyPolls[index];

            let question = sp.PollQuestion;
            let SurveyPollQuestionId = question.SurveyPollQuestionId;

            if (SurveyPollQuestionId != undefined && SurveyPollQuestionId != null && SurveyPollQuestionId > 0) {
                DeleteQuestionFromDatabase(SurveyPollQuestionId);
            }
            SurveyPolls.splice(index, 1);
            showSurveyPoll();
        }
    });
}
function deleteOption(currObj) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let attrquestionindex = $(currObj).attr("attrquestionindex");
            let attroptionindex = $(currObj).attr("attroptionindex");
            if (attrquestionindex == undefined || attrquestionindex == "undefined" || attroptionindex == undefined) {

            }
            else {
                let sp = SurveyPolls[attrquestionindex];
                let PollOptions = sp.PollOptions;

                let pollOption = PollOptions[attroptionindex];
                if (pollOption.SurveyPollOptionId > 0) {
                    DeleteOptionFromDatabase(pollOption.SurveyPollOptionId);
                    PollOptions.splice(attroptionindex, 1);
                }
                else {
                    PollOptions.splice(attroptionindex, 1);
                }
            }
            $(currObj).closest(".singleoptionDiv").remove();
        }
    });
}

function DeleteOptionFromDatabase(SurveyPollOptionId) {
    BlockUI();
    var inputDTO = {
        "SurveyPollOptionId": SurveyPollOptionId
    };
    $.ajax({
        type: "POST",
        url: "/EmployeeAccouncement/DeleteOption",
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
function DeleteQuestionFromDatabase(SurveyPollQuestionId) {
    BlockUI();
    var inputDTO = {
        "SurveyPollQuestionId": SurveyPollQuestionId
    };
    $.ajax({
        type: "POST",
        url: "/EmployeeAccouncement/DeleteQuestion",
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