(function ($) {
    'use strict';
    $(function () {
        var body = $('body');
        var contentWrapper = $('.content-wrapper');
        var scroller = $('.container-scroller');
        var footer = $('.footer');
        var sidebar = $('.sidebar');

        //Close other submenu in sidebar on opening any

        sidebar.on('show.bs.collapse', '.collapse', function () {
            sidebar.find('.collapse.show').collapse('hide');
        });


        //Change sidebar

        $('[data-toggle="minimize"]').on("click", function () {
            body.toggleClass('sidebar-icon-only');
        });

        //checkbox and radios
        $(".form-check label,.form-radio label").append('<i class="input-helper"></i>');

        // Remove pro banner on close


        if ($.cookie('majestic-free-banner') != "true") {
            document.querySelector('#proBanner').classList.add('d-flex');
            document.querySelector('.navbar').classList.remove('fixed-top');
        }
        else {
            document.querySelector('#proBanner').classList.add('d-none');
            document.querySelector('.navbar').classList.add('fixed-top');
        }

        if ($(".navbar").hasClass("fixed-top")) {
            document.querySelector('.page-body-wrapper').classList.remove('pt-0');
            document.querySelector('.navbar').classList.remove('pt-5');
        }
        else {
            document.querySelector('.page-body-wrapper').classList.add('pt-0');
            document.querySelector('.navbar').classList.add('pt-5');
            document.querySelector('.navbar').classList.add('mt-3');

        }
        document.querySelector('#bannerClose').addEventListener('click', function () {
            document.querySelector('#proBanner').classList.add('d-none');
            document.querySelector('#proBanner').classList.remove('d-flex');
            document.querySelector('.navbar').classList.remove('pt-5');
            document.querySelector('.navbar').classList.add('fixed-top');
            document.querySelector('.page-body-wrapper').classList.add('proBanner-padding-top');
            document.querySelector('.navbar').classList.remove('mt-3');
            var date = new Date();
            date.setTime(date.getTime() + 24 * 60 * 60 * 1000);
            $.cookie('majestic-free-banner', "true", { expires: date });
        });
    });
})(jQuery);

$(document).ready(function () {
    $('#eye').click(function () {
        $(this).toggleClass('open');
        $(this).children('i').toggleClass('mdi-eye-off mdi-eye');
        if ($(this).hasClass('open')) {
            $(this).prev().attr('type', 'text');
        }
        else {
            $(this).prev().attr('type', 'password');
        }
    });
});

function ActiveClass() {
    var controller = location.pathname.split("/")[2];

    var dashboard = document.getElementById('dashboard');
    var candidate = document.getElementById('candidate');
    var manager = document.getElementById('manager');
    var role = document.getElementById('role');
    var hr = document.getElementById('humanresources');
    var examination = document.getElementById('examination');

    if (controller == null || controller.toLocaleLowerCase() === "home") {
        dashboard.classList.add('active');
        candidate.classList.remove('active');
        manager.classList.remove('active');
        role.classList.remove('active');
        hr.classList.remove('active');
        examination.classList.remove('active');
    }
    else if (controller.toLowerCase() === "candidate") {
        dashboard.classList.remove('active');
        candidate.classList.add('active');
        manager.classList.remove('active');
        role.classList.remove('active');
        hr.classList.remove('active');
        examination.classList.remove('active');
    }
    else if (controller.toLowerCase() === "manager") {
        dashboard.classList.remove('active');
        candidate.classList.remove('active');
        manager.classList.add('active');
        role.classList.remove('active');
        hr.classList.remove('active');
        examination.classList.remove('active');
    }
    else if (controller.toLowerCase() === "role") {
        dashboard.classList.remove('active');
        candidate.classList.remove('active');
        manager.classList.remove('active');
        role.classList.add('active');
        hr.classList.remove('active');
        examination.classList.remove('active');
    }
    else if (controller.toLowerCase() === "humanresources") {
        dashboard.classList.remove('active');
        candidate.classList.remove('active');
        manager.classList.remove('active');
        role.classList.remove('active');
        hr.classList.add('active');
        examination.classList.remove('active');
    }
    else {
        dashboard.classList.remove('active');
        candidate.classList.remove('active');
        manager.classList.remove('active');
        role.classList.remove('active');
        hr.classList.remove('active');
        examination.classList.add('active');
    }
}

ActiveClass();