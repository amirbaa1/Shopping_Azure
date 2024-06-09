function ShowEdit(BasketItemId, quantity) {
    $("#BasketItemId").val(BasketItemId);
    $("#quantity").val(quantity);
    $("#editQuantityModal").modal({
        fadeDuration: 400,
        fadeDelay: 0.10
    });
//}
//function ApplyDiscountCode() {

//    var code = $("#txtDiscountCode").val();

//    var postData = { 'discountCode': code };

//    $.ajax({
//        contentType: 'application/x-www-form-urlencoded',
//        dataType: 'json',
//        type: "POST",
//        url: "Basket/ApplyDiscount",
//        data: postData,
//        success: function (data) {
//            if (data.isSuccess) {
//                swal({
//                    title: "بسیار خوب!",
//                    text: data.message,
//                    icon: "warning",
//                    button: "خب",
//                });
//            }
//            else {
//                swal({
//                    title: "هشدار!",
//                    text: data.message,
//                    icon: "warning",
//                    button: "خب",
//                });
//            }
//        }
//    });
//}


