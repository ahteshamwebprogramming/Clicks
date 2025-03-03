function ValidForm(formId) {
    let isValid = true;
    let ctrls = $("#" + formId).find("[required]");

    // Remove previous error messages
    ctrls.removeClass("error");
    $("#" + formId + " .error").remove();

    ctrls.each((i, v) => {        
        let tagName = $(v)[0].tagName.toUpperCase();
        if (tagName == "INPUT") {
            let tagType = $(v)[0].type.toUpperCase();
            if (tagType == "TEXT" || tagType == "EMAIL" || tagType == "FILE" || tagType == "NUMBER") {
                if ($.trim($(v).val()) == "") {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                    }
                }
            }
            // Minimum length validation for PAN
            if (tagType == "TEXT" && $(v).attr('id') == "Pannumber") {
                let minLength = 10;
                if ($(v).val().length < minLength) {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">PAN number must be at least ' + minLength + ' characters long</label>').insertAfter(v);
                    }
                }
            }
            // Minimum length validation for Aadhar
            if (tagType == "NUMBER" && $(v).attr('id') == "AadharNumber") {
                let minLength = 12;
                if ($(v).val().length < minLength) {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">Aadhar number must be at least ' + minLength + ' digits long</label>').insertAfter(v);
                    }
                }
            }
            // Minimum value validation for numbers
            if (tagType == "NUMBER") {
                let min = $(v).attr('min');
                if (min !== undefined && parseFloat($(v).val()) < parseFloat(min)) {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">Please enter a number greater than or equal to ' + min + '</label>').insertAfter(v);
                    }
                }
            }
        } else if (tagName == "SELECT") {
            if ($.trim($(v).val()) == "" || $(v).val() == undefined || $(v).val() == "0") {
                isValid = false;
                let name = $(v).attr('name');
                if (!$(v).hasClass("error")) {
                    $(v).addClass("error");
                    $(v).parent().append('<label class="error" for="' + name + '">This field is required</label>');
                }
            }
        }
    });

    ctrls = $("#" + formId).find("[minlength]");
    ctrls.each((i, v) => {
        let tagName = $(v)[0].tagName.toUpperCase();

        if (tagName == "INPUT") {
            let tagType = $(v)[0].type.toUpperCase();
            if (tagType == "FILE") {
                var filePath = $(v)[0].value;
                var fileName = filePath.split('\\').pop();
                let minlength = $(v).attr('minlength');
                if (!isNaN(minlength)) {
                    if (fileName.length > parseInt(minlength)) {
                        isValid = false;
                        let name = $(v).attr('name');
                        if (!$(v).hasClass("error")) {
                            $(v).addClass("error");
                            $('<label class="error" for="' + name + '">File name is too long. Please choose a file with a name less than ' + minlength + ' characters</label>').insertAfter(v);
                        }
                    }
                }
            }
        }
    });
    ctrls = $("#" + formId).find("[filemaxsize]");
    ctrls.each((i, v) => {
        let tagName = $(v)[0].tagName.toUpperCase();

        if (tagName == "INPUT") {
            let tagType = $(v)[0].type.toUpperCase();
            if (tagType == "FILE") {
                if ($(v)[0].files[0] != undefined && $(v)[0].files.length > 0) {
                    var file = $(v)[0].files[0];
                    var fileSize = file.size / 1024 / 1024; // Convert size to MB
                    let filemaxsize = $(v).attr('filemaxsize');
                    if (!isNaN(filemaxsize)) {
                        if (fileSize > parseInt(filemaxsize)) {
                            isValid = false;
                            let name = $(v).attr('name');
                            if (!$(v).hasClass("error")) {
                                $(v).addClass("error");
                                $('<label class="error" for="' + name + '">File size is too large. Please choose a file smaller than ' + filemaxsize + ' MB.</label>').insertAfter(v);
                            }
                        }
                    }
                }
            }
        }
    });
    return isValid;
}


function ValidForm1(formId) {
    let isValid = true;
    let ctrls = $("#" + formId).find("[required]");

    ctrls.each((i, v) => {
        let tagName = $(v)[0].tagName.toUpperCase();
        if (tagName == "INPUT") {
            let tagType = $(v)[0].type.toUpperCase();
            if (tagType == "TEXT") {
                if ($.trim($(v).val()) == "") {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                    }
                }
            }
            else if (tagType == "NUMBER") {
                if ($.trim($(v).val()) == "") {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                    }
                }
            }
            else if (tagType == "FILE") {
                if ($.trim($(v).val()) == "") {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                    }
                }
            }
            else if (tagType == "EMAIL") {
                if ($.trim($(v).val()) == "") {
                    isValid = false;
                    let name = $(v).attr('name');
                    if (!$(v).hasClass("error")) {
                        $(v).addClass("error");
                        $('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                    }
                }
            }
        }
        else if (tagName == "SELECT") {
            if ($.trim($(v).val()) == "" || $(v).val() == undefined || $(v).val() == "0") {
                isValid = false;
                let name = $(v).attr('name');
                if (!$(v).hasClass("error")) {
                    $(v).addClass("error");
                    $(v).parent().append('<label class="error" for="' + name + '">This field is required</label>');
                    //$('<label class="error" for="' + name + '">This field is required</label>').insertAfter(v);
                }

            }
        }
    });
    return isValid;
}
