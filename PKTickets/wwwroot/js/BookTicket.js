
$(document).ready(function () {
    $("#BookTicket").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveTicket",
            type: "Post",
            data: $("#BookTicket").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/ReservationsList'; }, 5000);
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

