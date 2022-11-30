function ScreenDelete(id) {
    let result = confirm("Are you sure you want to delete?");
    if (result) {
        $.ajax({
            type: "get",
            url: "/Home/RemoveScreen?id=" + id,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location.reload(); }, 500);
                }
            },
            error: function () {
                alert("error");
            }
        });
    }
}

function getParam(name) {
    return (location.search.split(name + '=')[1] || '').split('&')[0];
}