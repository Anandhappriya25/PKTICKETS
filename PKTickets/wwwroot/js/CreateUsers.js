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
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/UserList'; }, 4000);
                }
                else {
                    ToastMessage('Error', response.message, 'error', 'red')
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});