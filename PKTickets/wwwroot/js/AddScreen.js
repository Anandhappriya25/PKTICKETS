$(document).ready(function () {
    $("#AddScreen").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveScreen",
            type: "Post",
            data: $("#AddScreen").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/TheaterScreens?id=' + $('#TheaterId').val(); }, 5000);
                    $('#TheaterId').val();
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