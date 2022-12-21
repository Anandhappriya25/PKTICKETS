$(document).ready(function () {
    $("#AddSchedule").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveSchedule",
            type: "Post",
            data: $("#AddSchedule").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/ScreenSchedules?id=' + $('#ScreenId').val(); }, 500);
                    $('#ScreenId').val();
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