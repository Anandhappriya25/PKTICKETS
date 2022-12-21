function MovieDelete(id) {
    let result = confirm("Are you sure you want to delete?");
    if (result) {
        $.ajax({
            type: "get",
            url: "/Home/RemoveMovie?id=" + id,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    ToastMessage('Success', response.message, 'success', 'green')
                    setTimeout(function () { window.location = '/Home/Movies' }, 5000);
                }
                else {
                    ToastMessage('Error', response.message, 'error', 'red')
                }
            },
            error: function () {
                alert("error");
            }
        });
    }
}
