let croppedImageBase64;
$(document).ready(function () {
    var cropper;
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
        //console.log(croppedImageBase64);
        // Send croppedImageBase64 to server for saving
        //$.ajax({
        //    url: '/YourController/SaveImage',
        //    method: 'POST',
        //    data: { image: croppedImageBase64 },
        //    success: function (response) {
        //        // Handle success
        //    },
        //    error: function (xhr, status, error) {
        //        // Handle error
        //    }
        //});
    });
});


$(document).ready(function () {
    const tagifyBasicElAuthorsName = document.querySelector('#TagifyBasicAuthorsName');
    const TagifyBasicAuthorsName = new Tagify(tagifyBasicElAuthorsName);

    //const tagifyBasicElTagging = document.querySelector('#TagifyBasicTagging');
    //const TagifyBasicTagging = new Tagify(tagifyBasicElTagging);

    const tagifyBasicElKeywords = document.querySelector('#TagifyBasicKeywords');
    const TagifyBasicKeywords = new Tagify(tagifyBasicElKeywords);

    var $select = $('.select2').select2();
    $select.each(function (i, item) {
        $(item).select2("destroy");
    });
    $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").select2();
    $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").select2();
    $("#NewsForm").find("[required]").on("focus blur click change", function () {
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
    $("#full-editor").on("focus blur click change", function () {
        let currObj = $(this);
        currObj.parent().find(".text-danger").text("");
        currObj.parent().find(".text-danger").hide();

    });



    initArticleType();

});

function initArticleType() {
    let articleType = $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").val();
    if (articleType == 1) {
        $(".article-link").show();
        $(".external-article").hide();
    }
    else if (articleType == 2) {
        $(".article-link").hide();
        $(".external-article").show();
    }
    else {
        $(".article-link").hide();
        $(".external-article").hide();
    }
}

function isValidateForm() {
    let res = true;

    let Title = $("#NewsForm").find("[name='employeeNewsDTO.Title']").val();
    let ArticleType = $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").val()
    let ArticleLink = $("#NewsForm").find("[name='employeeNewsDTO.ArticleLink']").val();
    let Description = $("#NewsForm").find("[name='employeeNewsDTO.Description']").val();
    let AuthorsName = $("#NewsForm").find("[name='employeeNewsDTO.AuthorsName']").val();
    let PublicationName = $("#NewsForm").find("[name='employeeNewsDTO.PublicationName']").val();
    let Tagging = $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").val();
    let Keywords = $("#NewsForm").find("[name='employeeNewsDTO.Keywords']").val();

    let Article = $("#full-editor").find(".ql-editor").text();


    let authorsNameRaw = $("#NewsForm").find("[name='employeeNewsDTO.AuthorsName']").val();
    let authorsNameJson = authorsNameRaw == null ? "" : $.trim(authorsNameRaw) == "" ? "" : $.parseJSON(authorsNameRaw)

    //let taggingRaw = $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").val();
    //let taggingJson = taggingRaw == null ? "" : $.trim(taggingRaw) == "" ? "" : $.parseJSON(taggingRaw)

    let keywordsRaw = $("#NewsForm").find("[name='employeeNewsDTO.Keywords']").val();
    let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw);

    var filesUpload = jQuery("#uploads")[0].files;
    var filesUploaded = $("#uploads").parent().find("[name^=attachmentTag_]");

    if (!(Title != null && Title != undefined && $.trim(Title) != "")) {
        $("#NewsForm").find("[name='employeeNewsDTO.Title']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.Title']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(ArticleType != null && ArticleType != undefined && ArticleType != 0)) {
        $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").parent().find(".text-danger").show();
        res = false;
    }

    if (ArticleType == 1) {
        if (!(ArticleLink != null && ArticleLink != undefined && $.trim(ArticleLink) != "")) {
            $("#NewsForm").find("[name='employeeNewsDTO.ArticleLink']").parent().find(".text-danger").text("This field is mandatory");
            $("#NewsForm").find("[name='employeeNewsDTO.ArticleLink']").parent().find(".text-danger").show();
            res = false;
        }
        if (!(Description != null && Description != undefined && $.trim(Description) != "")) {
            $("#NewsForm").find("[name='employeeNewsDTO.Description']").parent().find(".text-danger").text("This field is mandatory");
            $("#NewsForm").find("[name='employeeNewsDTO.Description']").parent().find(".text-danger").show();
            res = false;
        }
    }
    else {
        if (!(Article != null && Article != undefined && $.trim(Article) != "")) {
            $("#NewsForm").find("[id='full-editor']").parent().find(".text-danger").text("This field is mandatory");
            $("#NewsForm").find("[id='full-editor']").parent().find(".text-danger").show();
            res = false;
        }
    }
    if (!(authorsNameJson != null && authorsNameJson != undefined && authorsNameJson.length != 0)) {
        $("#NewsForm").find("[name='employeeNewsDTO.AuthorsName']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.AuthorsName']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(PublicationName != null && PublicationName != undefined && $.trim(PublicationName) != "")) {
        $("#NewsForm").find("[name='employeeNewsDTO.PublicationName']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.PublicationName']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(Tagging != null && Tagging != undefined && Tagging.length != 0)) {
        $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").parent().find(".text-danger").show();
        res = false;
    }
    //if (!(taggingJson != null && taggingJson != undefined && taggingJson.length != 0)) {
    //    $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").parent().find(".text-danger").text("This field is mandatory");
    //    $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").parent().find(".text-danger").show();
    //    res = false;
    //}
    if (!(keywordsJson != null && keywordsJson != undefined && keywordsJson.length != 0)) {
        $("#NewsForm").find("[name='employeeNewsDTO.Keywords']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[name='employeeNewsDTO.Keywords']").parent().find(".text-danger").show();
        res = false;
    }
    if (!(filesUpload != null && filesUpload != undefined && filesUpload.length != 0 && croppedImageBase64 != undefined && croppedImageBase64 != "") && (filesUploaded.length == 0)) {
        $("#NewsForm").find("[id='uploads']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsForm").find("[id='uploads']").parent().find(".text-danger").show();
        res = false;
    }
    return res;
}

function SaveNews(type) {
    BlockUI();
    if (!isValidateForm()) {
        UnblockUI();
        return;
    }

    let authorsNameRaw = $("#NewsForm").find("[name='employeeNewsDTO.AuthorsName']").val();
    let authorsNameJson = authorsNameRaw == null ? "" : $.trim(authorsNameRaw) == "" ? "" : $.parseJSON(authorsNameRaw)

    //let taggingRaw = $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").val();
    //let taggingJson = taggingRaw == null ? "" : $.trim(taggingRaw) == "" ? "" : $.parseJSON(taggingRaw)

    let keywordsRaw = $("#NewsForm").find("[name='employeeNewsDTO.Keywords']").val();
    let keywordsJson = keywordsRaw == null ? "" : $.trim(keywordsRaw) == "" ? "" : $.parseJSON(keywordsRaw)


    var authorsNamecommaSeparated = '';
    var taggingcommaSeparated = '';
    var keywordscommaSeparated = '';

    if (authorsNameRaw != null && $.trim(authorsNameRaw) != "") {
        $.each(authorsNameJson, function (k, v) {
            authorsNamecommaSeparated += v.value + ', ';
        });
        authorsNamecommaSeparated = authorsNamecommaSeparated.slice(0, -2);
    }
    //if (taggingRaw != null && $.trim(taggingRaw) != "") {
    //    $.each(taggingJson, function (k, v) {
    //        taggingcommaSeparated += v.value + ', ';
    //    });
    //    taggingcommaSeparated = taggingcommaSeparated.slice(0, -2);
    //}
    if (keywordsRaw != null && $.trim(keywordsRaw) != "") {
        $.each(keywordsJson, function (k, v) {
            keywordscommaSeparated += v.value + ', ';
        });
        keywordscommaSeparated = keywordscommaSeparated.slice(0, -2);
    }

    let dataVM = new FormData();

    //var filesAttachment = jQuery("#attachments")[0].files;
    //for (var i = 0; i < filesAttachment.length; i++) {
    //    dataVM.append("Attachment", filesAttachment[i]);
    //}

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


    dataVM.append("employeeNewsDTO.Tagging", $("#NewsForm").find("[name='employeeNewsDTO.Tagging']").val().join(','));
    dataVM.append("employeeNewsDTO.Title", $("#NewsForm").find("[name='employeeNewsDTO.Title']").val());
    dataVM.append("employeeNewsDTO.ArticleType", $("#NewsForm").find("[name='employeeNewsDTO.ArticleType']").val());
    dataVM.append("employeeNewsDTO.Article", $("#full-editor").find(".ql-editor").html());
    dataVM.append("employeeNewsDTO.ArticleLink", $("#NewsForm").find("[name='employeeNewsDTO.ArticleLink']").val());
    dataVM.append("employeeNewsDTO.Description", $("#NewsForm").find("[name='employeeNewsDTO.Description']").val());
    dataVM.append("employeeNewsDTO.PublicationName", $("#NewsForm").find("[name='employeeNewsDTO.PublicationName']").val());
    dataVM.append("employeeNewsDTO.AuthorsNameRaw", authorsNameRaw);
    dataVM.append("employeeNewsDTO.AuthorsName", authorsNamecommaSeparated);
    //dataVM.append("employeeNewsDTO.TaggingRaw", taggingRaw);

    dataVM.append("employeeNewsDTO.KeywordsRaw", keywordsRaw);
    dataVM.append("employeeNewsDTO.Keywords", keywordscommaSeparated);

    dataVM.append("employeeNewsDTO.encEmployeeNewsId", $("#NewsForm").find("[name='employeeNewsDTO.encEmployeeNewsId']").val());
    //dataVM.append("employeeNewsDTO.Type", type);
    //if (type == "Schedule") {
    //    dataVM.append("employeeNewsDTO.PublishDate", $("#ScheduleModal").find("[name='PublishDate']").val());
    //    dataVM.append("employeeNewsDTO.PublishTime", $("#ScheduleModal").find("[name='PublishTime']").val());
    //}

    //$("#ScheduleModal").find(".btn-close").click();

    $.ajax({
        url: '/EmployeeNews/SaveNews',
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
                window.location.href = "/EmployeeNews/EmployeeNews";
            }, 2000);
        },
        error: function (xhr, ajaxOptions, error) {
            $erroralert("Transaction Failed!", xhr.responseText + '!');
            UnblockUI();
        }
    });

}



function deleteUploadedFile(EmployeeNewsFileUploadsId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "EmployeeNewsFileUploadsId": EmployeeNewsFileUploadsId
            };
            $.ajax({
                type: "POST",
                url: "/EmployeeNews/DeleteUploadedFile",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Transaction Successful!");
                    $("[name='attachmentTag_" + EmployeeNewsFileUploadsId + "']").remove();
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