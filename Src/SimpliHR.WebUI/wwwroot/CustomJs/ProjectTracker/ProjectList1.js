function ProjectListPartialView(PageNumber = 1) {
    $("#CloseDetailedProjectViewSection").click();
    let ProjectStatusType = $("#projectNavValue").val();
    if (ProjectStatusType == "") {
        ProjectStatusType = "ApprovalPending";
    }
    let SearchKeyword = $("#ProjectSearchKeyword").val();
    var inputDTO = {};
    inputDTO.ProjectStatusType = ProjectStatusType;
    inputDTO.PageSize = 6;
    inputDTO.PageNumber = PageNumber;
    inputDTO.SearchKeyword = SearchKeyword;
    inputDTO.Source = $("[name='Source']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ProjectTracker/ProjectListPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewProjectListPartial').html(data);
            //let autoresize = $(".auto-resize");
            //autoresize.each((i, v) => {
            //    $(v)[0].style.height = "auto";
            //    $(v)[0].style.height = $(v)[0].scrollHeight + "px";
            //});

            //$("#SaveComment").click(function (e) {
            //    e.preventDefault();
            //    SaveComment();
            //});


        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function remove(currDiv) {
    $(currDiv).closest(".card").remove();
}
function SaveComment() {
    let dataVM = new FormData();
    let encProjectID = $("#CommentForm").find("[name='ProjectWithChild.encProjectID']").val();
    let comment = $("#CommentForm").find("[name='CommentText']").val();
    var filesAttachment = $("#attach-doc")[0].files;

    if (comment == "" && filesAttachment.length == 0) {
        return;
    }

    for (var i = 0; i < filesAttachment.length; i++) {
        dataVM.append("Attachment", filesAttachment[i]);
    }
    dataVM.append("Attachment", filesAttachment[i]);
    dataVM.append("Comments.encProjectID", encProjectID);
    dataVM.append("Comments.CommentText", comment);

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
            UnblockUI();
            let encCommentID = response.comments.encCommentID;
            if (filesAttachment.length == 0) {
                $(".chat-history").append('<li class="chat-message chat-message-right"><div class="d-flex overflow-hidden"><div class="chat-message-wrapper flex-grow-1 w-50"><div class="chat-message-text"><p class="mb-0">' + comment + '</p></div><div class="text-end text-muted mt-1"><small>' + getCurrentTime() + '</small></div></div><div class="user-avatar flex-shrink-0 ms-3"><div class="avatar avatar-sm"><img src="' + $('.loggedinUserAvatar').attr('src') + '" alt="Avatar" class="rounded-circle"></div></div></div></li>');
            }
            else {
                $(".chat-history").append('<li class="chat-message chat-message-right"><div class="d-flex overflow-hidden"><div class="chat-message-wrapper flex-grow-1 w-50"><div class="chat-message-text"><p class="mb-0">' + comment + '</p></div>  <div class="text-muted mt-1"><small style="float:right"><a href="javascript:void(0)" onclick="DownloadAttachments(\'' + encCommentID + '\')"><i class="bx bx-paperclip cursor-pointer"></i>Attachments</a></small></div><div class="text-end text-muted mt-1"><small>' + getCurrentTime() + '</small></div></div><div class="user-avatar flex-shrink-0 ms-3"><div class="avatar avatar-sm"><img src="' + $('.loggedinUserAvatar').attr('src') + '" alt="Avatar" class="rounded-circle"></div></div></div></li>');
            }

            //ViewProjectDetails(encProjectID);
            $("#CommentForm").find("[name='CommentText']").val("");
            $("#attached-files").empty();


            var commentSection = $('#CommentSection');
            commentSection.scrollTop(commentSection[0].scrollHeight);
            //CommentSection

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

function ActionOnMilestone(encMilestoneId, currObj, action) {



    Swal.fire({ title: 'Are you sure?', text: "You want to proceed!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encMilestoneId": encMilestoneId,
                "Action": action,
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTracker/ActionOnMilestone",
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
                    else if (data == 'Revise') {
                        $(currObj).parent().html('<a href="javascipt:void(0)" onclick="ActionOnMilestone(\'' + encMilestoneId + '\',this,\'MilestoneCompleted\')">Mark As Completed</a>');
                    }
                    else {
                        $(currObj).closest('tr').css("text-decoration", "line-through");
                        $(currObj).parent().html("");
                    }
                    //ViewProjectDetails(encProjectID);
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
                    ViewProjectDetails(encProjectID);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}


function ActionOnStatus(encProjectID, action) {

    let swalAction = action == "Approve" ? "This will approve the request of status change!" : action == "Revise" ? "This will revert back the original status!" : "";

    Swal.fire({ title: 'Are you sure?', text: swalAction, icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encProjectID": encProjectID,
                "Description": action
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTracker/ActionOnStatus",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Transaction Successful!");
                    UnblockUI();
                    ViewProjectDetails(encProjectID);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}

function setActive(element, action) {    
    // Get all the nav links
    var links = document.querySelectorAll('#projectNav .nav-link');

    // Remove the active class from all nav links
    links.forEach(function (link) {
        link.classList.remove('active');
    });

    // Add the active class to the clicked nav link
    element.classList.add('active');

    $("#projectNavValue").val(action);

    ProjectListPartialView();
}




function ViewProjectDetails(encProjectID) {

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

            $('html, body').animate({
                scrollTop: $('#DetailedProjectViewSection').offset().top
            }, 'slow');

            $("#SaveComment").click(function (e) {
                e.preventDefault();
                SaveComment();
            });

            $("#project-status-dropdown").change(function () {
                projectStatusChangeEvent(encProjectID);
            })






            AttachedFileEventAndCommentDate();



        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function AttachedFileEventAndCommentDate() {

    //document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.comment-date').forEach(function (element) {
        const dateStr = element.getAttribute('data-date');
        element.textContent = formatDateTime(dateStr);
    });
    //});

    document.getElementById('attach-doc').addEventListener('change', function (event) {
        const fileList = event.target.files;
        const fileNamesContainer = document.getElementById('attached-files');
        fileNamesContainer.innerHTML = ''; // Clear previous file names

        if (fileList.length > 0) {
            Array.from(fileList).forEach((file, index) => {
                const fileWrapper = document.createElement('div');
                fileWrapper.classList.add('file-item', 'd-flex', 'align-items-center', 'mb-2');

                const fileName = document.createElement('span');
                fileName.textContent = file.name;
                fileName.classList.add('me-2');

                const removeButton = document.createElement('button');
                removeButton.classList.add('btn', 'btn-sm', 'btn-outline-danger');
                removeButton.innerHTML = '&times;'; // Cross icon
                removeButton.dataset.fileIndex = index;

                removeButton.addEventListener('click', function () {
                    removeFile(index);
                });

                fileWrapper.appendChild(fileName);
                fileWrapper.appendChild(removeButton);
                fileNamesContainer.appendChild(fileWrapper);
            });
        }
    });

    function removeFile(index) {
        const input = document.getElementById('attach-doc');
        const dt = new DataTransfer();

        Array.from(input.files).forEach((file, i) => {
            if (i !== index) {
                dt.items.add(file); // Keep all files except the one that was removed
            }
        });

        input.files = dt.files; // Update the file input with the remaining files
        document.getElementById('attach-doc').dispatchEvent(new Event('change')); // Trigger change event to update the file list
    }
}

function projectStatusChangeEvent(encProjectID) {
    let StatusID = $("#project-status-dropdown").val();
    Swal.fire({ title: 'Are you sure?', text: "This will change the status of the project", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes. Change it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encProjectID": encProjectID,
                "StatusID": StatusID
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTracker/ProjectStatusChange",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    if (data == "Requested") {
                        $successalert("Transaction Successful!", "Requested For Status Change");
                    }
                    else {
                        $successalert("", "Transaction Successful!");
                    }
                    ViewProjectDetails(encProjectID);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}




function DownloadAttachments(encCommentID) {
    $.ajax({
        url: '/ProjectTracker/DownloadAttachments/' + encCommentID,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'// Set the response type to blob
        }, success: function (data) {
            // Create a temporary anchor element to trigger the download
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = "Attachments";
            // Set the filename for download
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            //$successalert("Success!", "Downloaded succesfully.");
        },
        error: function (xhr, status, error) {
            $erroralert("Oops!", 'Error downloading file:' + xhr.responseText);
        }
    });
}


function formatDateTime(date) {
    const now = new Date();
    const inputDate = new Date(date);

    // Compare dates
    const isToday = now.toDateString() === inputDate.toDateString();
    const yesterday = new Date();
    yesterday.setDate(now.getDate() - 1);
    const isYesterday = yesterday.toDateString() === inputDate.toDateString();

    const optionsTime = { hour: '2-digit', minute: '2-digit' };
    const optionsDate = { day: '2-digit', month: 'short', year: 'numeric' };

    const formatDate = inputDate.toLocaleDateString('en-GB', optionsDate).replace(',', '');
    const formatTime = inputDate.toLocaleTimeString([], optionsTime);

    if (isToday) {
        return formatTime;
    } else if (isYesterday) {
        return `Yesterday ${formatTime}`;
    } else {
        return `${formatDate} ${formatTime}`;
    }
}

