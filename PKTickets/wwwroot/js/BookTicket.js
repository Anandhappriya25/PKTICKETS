
$(document).ready(function () {
    $("#BookTicket").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveTicket",
            type: "Post",
            data: $("#BookTicket").serialize(),
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location = '/Home/ReservationsList'; }, 500);
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});

