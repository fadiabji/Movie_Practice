// ajax function to stop refreshing page after add something to the cart
function addToCart(movieId) {
    event.preventDefault();
    var requestUrl = "/Cart/AddToCart/movieId";
    $.ajax({
        type: "POST",
        url: requestUrl,
        dataType: "json",
        cach: false,
        data: { id: movieId },
        success: function (data) {
            var newData = data.value;
            var x = newData.length;
            $('#shopcount').html(x);
            //$('#shopcount').css({
            //    "display": "block",
            //    "text- align": "center",
            //    "color": "#fff",
            //    "background-color": "red",
            //    "width": "20px",
            //    "height": "25px",
            //    "border-radius": "50%",
            //    "font-size": "14px"
            //});
        }
    })
}