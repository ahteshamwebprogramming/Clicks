function AddPartialView(encPriorityId) {
    var inputDTO = {};
    inputDTO.encPriorityId = encPriorityId;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ProjectTrackerMasters/AddProjectPriorityPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_AddPartial').html(data);
            UnblockUI();
            initErrorValidate();
        },
        error: function (result) {
            $erroralert("Transaction Failed!", result.responseText + '!');
            UnblockUI();
        }
    });
}

function ListPartialView() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ProjectTrackerMasters/ListProjectPriorityPartialView',
        //data: JSON.stringify(resignationListDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_ListPartial').html(data);
            $(".List").DataTable({
                "order": []
            });
            UnblockUI();
        },
        error: function (result) {
            $erroralert("Transaction Failed!", result.responseText + '!');
            UnblockUI();
        }
    });
}

function Reset() {
    AddPartialView("");
}
function isValidateForm() {
    let res = true;
    let Priority = $("#ProjectPriorityForm").find("[name='Priority']").val();

    if (!(Priority != null && Priority != undefined && $.trim(Priority) != "")) {
        $("#ProjectPriorityForm").find("[name='Priority']").parent().find(".text-danger").text("This field is mandatory");
        $("#ProjectPriorityForm").find("[name='Priority']").parent().find(".text-danger").show();
        res = false;
    }
    return res;
}

function Save() {
    BlockUI();
    if (!isValidateForm()) {
        UnblockUI();
        return;
    }
    let inputDTO = {};
    inputDTO.Priority = $("#ProjectPriorityForm").find("[name='Priority']").val();
    inputDTO.encPriorityId = $("#ProjectPriorityForm").find("[name='encPriorityId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/ProjectTrackerMasters/SaveProjectPriority",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            AddPartialView("");
            ListPartialView();
            $successalert("", "Transaction Successful!");
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });

}



function deleteRecord(encPriorityId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encPriorityId": encPriorityId
            };
            $.ajax({
                type: "POST",
                url: "/ProjectTrackerMasters/DeleteProjectPriority",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    AddPartialView("");
                    ListPartialView();
                    $successalert("", "Transaction Successful!");
                },
                error: function (error) {
                    $erroralert("Error!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}