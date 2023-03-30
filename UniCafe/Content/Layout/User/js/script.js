(function ($) {
    'use strict'

    /* Mobile Menu */
    var app = {
        dropdown: function () {
            $('.drop-nav-arrow').on('click', function () {
                if (!$(this).parent().hasClass('open')) {
                    $(this).parent().addClass('open');
                    $(this).parent().find('> .drop-nav').slideDown();
                } else {
                    $(this).parent().removeClass('open');
                    $(this).parent().find('> .drop-nav').slideUp();
                }
            });
        }
    }

    $(document).ready(function () {
        'use strict'
        // Show hide menu icon
        $(".menu-icon a").on('click', function (e) {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open');
                $('.menu-nav-main').removeClass('open');
            } else {
                $(this).addClass('open');
                $('.menu-nav-main').addClass('open');
            }
            return false;
        });

        $('.menu-icon a').on('click', function () {
            $('body').toggleClass('hidden-body');
        });
        // Category icon manage
        $('.category-icon-menu .hambarger-icon').on('click', function () {
            $('.category-icon > ul').slideToggle();
            $(this).toggleClass('open');
            return false;
        });

        $('.language-change').on('click', function () {
            $('.language-menu ul').slideToggle();
        });

        /* Add Linit in textbox only number */
        $(".price-textbox input").on('keydown', function (e) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A, Command+A
                (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });
    });

    $(window).on('load', function () {
        initComponents();
        // Add icon in menu
        $('.menu-nav-main ul li').each(function () {
            if ($(this).find('.drop-nav').length) {
                $(this).append('<span class="drop-nav-arrow"><i class="fa fa-angle-down"></i></span>')
            }
        });
        app.dropdown();
        // Add pre loader time
        $('#pre-loader').delay(1000).fadeOut();

        headerFix();
    });

    $(window).resize(function () {
        initComponents();
    });

    function initComponents() {
        initBg();
    }

    function initBg() {
        // Set Background image
        $('.Banner-Bg').each(function () {
            var background = $(this).data('background');
            $(this).css('background-image', 'url("' + background + '")');
        });
    }

    // Page Bottom To Top Call
    $('.top-arrow').on('click', function () {
        $("html, body").animate({ scrollTop: 0 }, 600);
        return false;
    });

    // Scroll top arrow
    $(window).on('scroll', function () {

        headerFix();

        if ($(this).scrollTop() > 500) {
            $('.top-arrow').fadeIn();
        } else {
            $('.top-arrow').fadeOut();
        }
        return false;
    });

    /* Progress Bar Horizontal */
    jQuery.fn.progressBar = function (options) {
        //Default values for progress_bar
        var defaults = {
            height: "6",
            backgroundColor: "#eae7de",
            barColor: "#bc9540",
            percentage: true,
            shadow: false,
            border: false,
            animation: false,
            animateTarget: false,
        };

        var settings = $.extend({}, defaults, options);
        return this.each(function () {
            var elem = $(this);
            $.fn.replaceProgressBar(elem, settings);
        });
    };

    // Progress Bar element code
    $.fn.replaceProgressBar = function (item, settings) {
        var skill = item.text();
        var progress = item.data('width');
        var target = item.data('target');
        var bar_classes = ' ';
        var animation_class = '';

        // Set background color
        var bar_styles = 'background-color:' + settings.backgroundColor + ';height:' + settings.height + 'px;';
        if (settings.shadow) { bar_classes += 'shadow'; }
        if (settings.border) { bar_classes += ' border'; }
        if (settings.animation) { animation_class = ' animate'; }

        // Progress bar detail
        var overlay = '<div class="sonny_progressbar' + animation_class + '" data-width="' + progress + '">';
        overlay += '<p class="title">' + skill + '</p>';
        overlay += '<div class="bar-container' + bar_classes + '" style="' + bar_styles + '">';
        overlay += '<span class="backgroundBar"></span>';

        if (target) {
            if (settings.animateTarget) {
                overlay += '<span class="targetBar loader" style="width:' + target + '%;background-color:' + settings.targetBarColor + ';"></span>';
            } else {
                overlay += '<span class="targetBar" style="width:' + target + '%;background-color:' + settings.targetBarColor + ';"></span>';
            }
        }

        // Render the progress bar
        if (settings.animation) {
            overlay += '<span class="bar" style="background-color:' + settings.barColor + ';"></span>';
        } else {
            overlay += '<span class="bar" style="width:' + progress + '%;background-color:' + settings.barColor + ';"></span>';
        }

        // Render the percentage if enabled
        if (settings.percentage) {
            overlay += '<span class="progress-percent" style="line-height:' + settings.height + 'px;">' + progress + '%</span>';
        }

        // End
        overlay += '</div></div>';

        // Render the progress bar on the page
        $(item).replaceWith(overlay);

    };

    // Set animation
    var animate = function () {
        var doc_height = $(window).height();
        $('.sonny_progressbar.animate').each(function () {
            var position = $(this).offset().top;
            if (($(window).scrollTop() + doc_height - 60) > position) {
                var progress = $(this).data('width') + "%";
                $(this).removeClass('animate');
                $(this).find('.bar').css('opacity', '0.1');
                $(this).find('.bar').animate({
                    width: progress,
                    opacity: 1
                }, 3000);
            }
        });
    };

    // Looking for an animation element in the view
    $(window).on('scroll', function () {
        if ($('.sonny_progressbar.animate').length < 1) {
            return;
        }
        animate();
    });

    /* Progress Bar Call */
    $('.progressbar1').progressBar({
        shadow: true,
        percentage: false,
        animation: true,
    });

    // Header fix
    function headerFix() {

        // Check header position is top
        if ($(document).scrollTop() > $('header-part').height()) {
            if (($(window).height() < $(document).height()) && $('.header-part').hasClass('sticky')) {
                $('.header-part').addClass('sticky-fixed');
            }
        } else {
            $('.header-part').removeClass('sticky-fixed');
        }
    }

})(jQuery);

$(document).on('ready', function () {
    // Check map is available
    if ($('.map-outer').length > 0) {
        $('#map').height($(window).height());
    } else {
        $('#map').height('600px');
    }

    // Initialize google map
    var map;
    function initialize() {
        // Set google map property
        var myCenter = new google.maps.LatLng(40.7127837, -74.00594130000002);
        var mapProp = {
            center: myCenter,
            zoom: 11,
            scrollwheel: false,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            styles: [{ "featureType": "administrative.locality", "elementType": "labels", "stylers": [{ "visibility": "on" }] }, { "featureType": "administrative.locality", "elementType": "labels.text.fill", "stylers": [{ "color": "#000000" }, { "visibility": "on" }] }, { "featureType": "administrative.locality", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "on" }, { "color": "#ffffff" }, { "weight": "0.75" }] }, { "featureType": "landscape.natural", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ded7c6" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#ded7c6" }] }, { "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "road", "elementType": "labels", "stylers": [{ "visibility": "off" }] }, { "featureType": "transit.line", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "lightness": 700 }] }, { "featureType": "water", "elementType": "all", "stylers": [{ "color": "#c3a866" }] }, { "featureType": "water", "elementType": "labels", "stylers": [{ "visibility": "off" }] }]
        };
        var map = new google.maps.Map(document.getElementById("map"), mapProp);

        // Set google map marker
        var marker = new google.maps.Marker({
            position: myCenter,
            icon: 'images/map_marker.png'
        });
        marker.setMap(map);

        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            map.setOptions({ 'draggable': false });
        }
    }
    // Check google map ID is available or not
    if (document.getElementById('map') != null) {
        google.maps.event.addDomListener(window, 'load', initialize);
    }

    // Send mail to company
    $(".alert-container").hide();
    $("form[name='contact-form']").on('submit', function (e) {
        e.preventDefault();
        var url = "functions.php"; // the script where you handle the form input.
        var thisForm = $(this);
        var btnValue = $(this).find(".send_message").attr('value');
        $(this).find(".send_message").attr('value', 'SUBMITTING...');
        $(this).find(".send_message").attr('disabled', 'disabled');
        $.ajax({
            type: "POST",
            url: url,
            data: thisForm.serialize(), // serializes the form's elements.
            success: function (data) {
                // show response from the php script.
                $(".alert-container").html(data);
                $(".alert-container").show();
                thisForm.trigger("reset");
                thisForm.find(".send_message").attr('value', btnValue);
                thisForm.find(".send_message").removeAttr('disabled');
                $(".alert .close").on('click', function () {
                    $(".alert-container").hide();
                });
            }
        });
        return false; // avoid to execute the actual submit of the form.
    });
});