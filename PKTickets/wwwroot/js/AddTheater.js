
$(document).ready(function () {
    $("#AddTheater").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveTheater",
            type: "Post",
            data: $("#AddTheater").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/TheatersList'; }, 5000);
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

