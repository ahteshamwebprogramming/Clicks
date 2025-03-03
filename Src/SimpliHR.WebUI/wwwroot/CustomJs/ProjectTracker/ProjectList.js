function ViewProject(encProjectID) {
    var inputDTO = {};
    inputDTO.encProjectID = encProjectID;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ProjectTracker/GetProjectWithChildById',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewProjectDetailsPartial').html(data);
            let autoresize = $(".auto-resize");
            autoresize.each((i, v) => {
                $(v)[0].style.height = "auto";
                $(v)[0].style.height = $(v)[0].scrollHeight + "px";
            });

            $("#SaveComment").click(function (e) {
                e.preventDefault();
                SaveComment();
            });

        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function remove(currDiv) {
    $(currDiv).parent().parent().parent().remove();
}
function SaveComment() {
    let dataVM = new FormData();

    let comment = $("#CommentForm").find("[name='CommentText']").val();
    var filesAttachment = $("#attach-doc")[0].files;

    if (comment == "" && filesAttachment.length == 0) {
        return;
    }

    for (var i = 0; i < filesAttachment.length; i++) {
        dataVM.append("Attachment", filesAttachment[i]);
    }
    dataVM.append("Attachment", filesAttachment[i]);
    dataVM.append("Comments.encProjectID", $("#CommentForm").find("[name='ProjectWithChild.encProjectID']").val());
    dataVM.append("Comments.CommentText", comment);

    $(".chat-history").append('<li class="chat-message chat-message-right"><div class="d-flex overflow-hidden"><div class="chat-message-wrapper flex-grow-1 w-50"><div class="chat-message-text"><p class="mb-0">' + comment + '</p></div><div class="text-end text-muted mt-1"><small>' + getCurrentTime() + '</small></div></div><div class="user-avatar flex-shrink-0 ms-3"><div class="avatar avatar-sm"><img src="' + $('.loggedinUserAvatar').attr('src') + '" alt="Avatar" class="rounded-circle"></div></div></div></li>');


    BlockUI();
    $.ajax({
        url: '/ProjectTracker/SaveComments',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $("#CommentForm").find("[name='CommentText']").val("");
            UnblockUI();

        },
        error: function (xhr, ajaxOptions, error) {
            $("#CommentForm").find("[name='CommentText']").val("");
            UnblockUI();
        }
    });
}


function getCurrentTime() {
    var now = new Date();
    var hours = now.getHours();
    var minutes = now.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function MarkMilestoneCompleted(MilestoneId, currObj) {



    Swal.fire({ title: 'Are you sure?', text: "You want to mark this milestone as completed!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "MilestoneId": MilestoneId
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTracker/MarkMilestoneCompleted",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Transaction Successful!");
                    UnblockUI();
                    if (data == 'Approval Sent') {
                        $(currObj).parent().html("Sent for Approval")
                    }
                    else if (data == 'Completed') {
                        $(currObj).closest('tr').css("text-decoration", "line-through");
                        $(currObj).parent().html("");
                    }
                    else {

                    }
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}

function ActionOnProject(encProjectID, action) {

    let swalAction = action == "Approve" ? "This will get approved permanently!" : action == "Reject" ? "This will get rejected permanently!" : action == "NeedCorrection" ? "You want to mark this as Need Correction" : "";

    Swal.fire({ title: 'Are you sure?', text: swalAction, icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encProjectID": encProjectID,
                "Description": action
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTracker/ActionOnProject",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Transaction Successful!");
                    UnblockUI();
                    ViewProject(encProjectID);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}