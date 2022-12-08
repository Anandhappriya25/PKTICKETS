﻿
$(document).ready(function () {
    $("#CreateUsers").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/Save",
            type: "Post",
            data: $("#CreateUsers").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    $.toast({
                        heading: 'Success',
                        text: response.message,
                        showHideTransition: 'slide',
                        icon: 'success',
                        hideAfter: 5000,
                        position: 'top-center',
                        stack: false,
                        loader: false, 
                        textAlign: 'center',
                        bgColor: 'green',
                        textColor: 'white'
                    })
                    setTimeout(function () { window.location = '/Home/UserList'; }, 4000);
                }
                else {
                    $.toast({
                        heading: 'Error',
                        loader: false, 
                        text: response.message,
                        showHideTransition: 'slide',
                        icon: 'error',
                        hideAfter: 5000,
                        position: 'top-center',
                        stack: false,
                        textAlign: 'center',
                        bgColor: 'red',
                        textColor: 'white'
                    })
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});
function GoIndex() {
    setTimeout(function () { window.location = '/Home/Index'; }, 100);
}
function GoUser() {
    setTimeout(function () { window.location = '/Home/UserList'; }, 100);
}
