﻿
function addToBasket(code, qty) {
    qty = 1;
    
    $.ajax(
        {
            url: "/cart",
            data: { code: code, qty: qty },
            type: "Post"
        }).done(function (result) {
            if (result !== true) {
                window.location = "/basket";
            }
        });
}



function addDiscountCode() {
    var coupon = $("#coupon").val();

    $('#errorDiv').css('display', 'none');
    if (coupon !== "") {
        $.ajax(
            {
                url: "/shop/DiscountRequestPost",
                data: { coupon: coupon },
                type: "GET"
            }).done(function (result) {
                if (result !== "Invald" && result !== "Used" && result !== "Expired") {
                    location.reload();
                }
                else if (result !== true) {
                    $('#errorDiv').css('display', 'block');
                    if (result.toLowerCase() === "used") {
                        $('#errorDiv').html("این کد تخفیف قبلا استفاده شده است.");
                    }
                    else if (result.toLowerCase() === "expired") {
                        $('#errorDiv').html("کد تخفیف وارد شده منقضی شده است.");
                    }
                    else if (result.toLowerCase() === "invald") {
                        $('#errorDiv').html("کد تخفیف وارد شده معتبر نمی باشد.");
                    }
                    else if (result.toLowerCase() === "true") {
                        $('#SuccessDiv').css('display', 'block');
                        $('#errorDiv').css('display', 'none');
                    }
                }
            });

    } else {
        $('#SuccessDiv').css('display', 'none');
        $('#errorDiv').html('کد تخفیف را وارد نمایید.');
        $('#errorDiv').css('display', 'block');
    }
}


function FinalizeOrder() {
    DisappearButton('btn-finalize', 'transfer-message2');
   
    var activationCode = $('#txtActivationCode').val();
    var cellNumber = $('#txtCellNumber').val();
    var company = $('#txtCompany').val();
    var email = $('#txtEmail').val();
    var address = $('#txtAddress').val();
    var postal = $('#txtPostal').val();
    var city = $('#txtCity').val();

    if (activationCode !== '') {
        $.ajax(
            {
                url: "/shop/Finalize",
                data: {
                    company: company,
                    email: email,
                    activationCode: activationCode,
                    cellNumber: cellNumber,
                    address: address,
                    postal: postal,
                    city: city
                  
                },
                type: "GET"
            }).done(function (result) {

                if (result === "invalid") {
                    $('#error-box2').css('display', 'block');
                    $('#error-box2').html('کد وارد شده صحیح نمی باشد.');
                    AppearButton('btn-finalize', 'transfer-message2');
                }
                else if (result !== "false"&&result!=="invalid") {
                    window.location = result;
                } else {
                    $('#error-box2').css('display', 'block');
                    $('#error-box2').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    AppearButton('btn-finalize', 'transfer-message2');

                }
            });
    } else {
        $('#error-box2').css('display', 'block');
        $('#error-box2').html('لطفا اطلاعات بالا را تکمیل کنید');
        AppearButton('btn-finalize', 'transfer-message2');
    }
}

function changeCellNum() {
    $('.activate').css('display', 'none');
    $('.register').css('display', 'block');
}

function registerUser(isPhysicalProduct) {
    DisappearButton('btn-register', 'transfer-message');
    var name = $('#txtFullName').val();
    var cellNumber = $('#txtCellNumber').val();
    var company = $('#txtCompany').val();
    var email = $('#txtEmail').val();

    var address = $('#txtAddress').val();
    var postal = $('#txtPostal').val();
    var city = $('#txtCity').val();

    if (isPhysicalProduct && (address === '' || city === '')) {
        $('#error-box').css('display', 'block');
        $('#error-box').html('لطفا فیلدهای ستاره دار را تکمیل کنید');
        AppearButton('btn-register', 'transfer-message');
    } else {
        if (name !== '' && cellNumber !== '') {
            $.ajax(
                {
                    url: "/shop/CheckUser",
                    data: {
                        company: company,
                        email: email,
                        fullName: name,
                        cellNumber: cellNumber,

                    },
                    type: "GET"
                }).done(function(result) {
                if (result === "true") {
                    $('.activate').css('display', 'block');
                    $('.register').css('display', 'none');
                } else if (result === "invalidMobile") {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('شماره موبایل وارد شده صحیح نمی باشد');
                    AppearButton('btn-register', 'transfer-message');
                } else if (result === "invalidEmail") {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('ایمیل وارد شده صحیح نمی باشد.');
                    AppearButton('btn-register', 'transfer-message');
                } else {
                    $('#error-box').css('display', 'block');
                    $('#error-box').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                    AppearButton('btn-register', 'transfer-message');

                }
            });
        } else {
            $('#error-box').css('display', 'block');
            $('#error-box').html('لطفا فیلدهای ستاره دار را تکمیل کنید');
            AppearButton('btn-register', 'transfer-message');
        }
    }
}



function DisappearButton(id, apearId) {
    $('#' + id).css('display', 'none');
    $('#' + apearId).css('display', 'block');

}
function AppearButton(id, apearId) {
    $('#' + id).css('display', 'block');
    $('#' + apearId).css('display', 'none');

}



function loginUser() {
    DisappearButton('btn-login', 'loading-box');
    
    var cellNumber = $('#txtCellNum').val();
   
    if (cellNumber !== '') {
        $.ajax(
            {
                url: "/account/SendOtp",
                data: {
                    cellNumber: cellNumber 
                },
                type: "GET"
            }).done(function (result) {

            if (result === "true") {

                DisappearButton('login-form', 'otp-form');

            } else if (result === "false"){

                $('#error-box-login').css('display', 'block');

                $('#error-box-login').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');

                AppearButton('btn-login', 'loading-box');

            } else if (result === "invalidUser"){
              
                DisappearButton('login-form', 'register-form');
                
            } else if (result === "invalidCellNumber"){

                $('#error-box-login').css('display', 'block');

                $('#error-box-login').html('شماره موبایل وارد شده صحیح نمی باشد ');

                AppearButton('btn-login', 'loading-box');

            }
        });
    } else {
        $('#error-box-login').css('display', 'block');
        $('#error-box-login').html('لطفا شماره موبایل خود را وارد نمایید');
        AppearButton('btn-login', 'loading-box');
    }
}

function CompleteRegisterFrom() {
    DisappearButton('btn-register', 'register-loading-box');
   
    var name = $('#fullName').val();
    var empType;
    if ($("#karfarma").prop("checked")) {
        empType = 'karfarma';
    } else {
        empType = 'karmand';
    }
    var cellNumber = $('#txtCellNum').val();
   

    if (name !== '' && cellNumber !== '') {
        $.ajax(
            {
                url: "/account/CompleteRegister",
                data: {
                    fullName: name,
                    cellNumber: cellNumber,
                    employeeType: empType
                },
                type: "GET"
            }).done(function (result) {
            if (result !== "false") {
                DisappearButton('register-form', 'otp-form');

            } else {
                $('#error-box-register').css('display', 'block');
                $('#error-box-register').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                AppearButton('btn-register', 'register-loading-box');

            }
        });
    } else {
        $('#error-box-register').css('display', 'block');
        $('#error-box-register').html('لطفا اطلاعات بالا را تکمیل کنید');
        AppearButton('btn-register', 'register-loading-box');
    }
}

function checkUserOtp() {
    DisappearButton('btn-checkOtp', 'activation-loading-box');
   
    var cellNumber = $('#txtCellNum').val();
    var code = $('#txtCode').val();

    if (code !== '' ) {
        $.ajax(
            {
                url: "/account/CheckOtp",
                data: {
                    activationCode: code,
                    cellNumber: cellNumber,
                },
                type: "GET"
            }).done(function (result) {
                if (result === "invalid") {
                    $('#error-box-otp').css('display', 'block');
                    $('#error-box-otp').html('کد وارد شده صحیح نمی باشد');
                    AppearButton('btn-checkOtp', 'activation-loading-box');

                }
          else if (result === "true") {
                window.location = "/orders/list";
            }
                else {
                $('#error-box-otp').css('display', 'block');
                $('#error-box-otp').html('خطایی رخ داده است. لطفا مجددا تلاش کنید');
                AppearButton('btn-checkOtp', 'activation-loading-box');

            }
        });
    } else {
        $('#error-box-otp').css('display', 'block');
        $('#error-box-otp').html('کد فعال سازی را وارد کنید');
        AppearButton('btn-checkOtp', 'activation-loading-box');
    }
}