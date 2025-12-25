$(function ($) {
  "use strict";

  jQuery(document).ready(function () {

    // preloader
    $("#preloader").delay(300).animate({
      "opacity": "0"
    }, 500, function () {
      $("#preloader").css("display", "none");
    });

    // Accordion Style
    $(".accordion-item").click(function () {
      if ($(".intro")[0]) {
        $(".accordion-item").removeClass("intro");
      }
      $(this).toggleClass("intro");
    });

    // Scroll Top
    var ScrollTop = $(".scrollToTop");
    $(window).on('scroll', function () {
      if ($(this).scrollTop() < 500) {
        ScrollTop.removeClass("active");
      } else {
        ScrollTop.addClass("active");
      }
    });
    $('.scrollToTop').on('click', function () {
      $('html, body').animate({
        scrollTop: 0
      }, 500);
      return false;
    });

    // Navbar Dropdown
    var dropdown_menu = $(".header-section .dropdown-menu");
    $(window).resize(function () {
      if ($(window).width() < 992) {
        dropdown_menu.removeClass('show');
      }
      else {
        dropdown_menu.addClass('show');
      }
    });
    if ($(window).width() < 992) {
      dropdown_menu.removeClass('show');
    }
    else {
      dropdown_menu.addClass('show');
    }

    // Sticky Header
    var fixed_top = $(".header-section");
    $(window).on("scroll", function () {
      if ($(window).scrollTop() > 50) {
        fixed_top.addClass("animated fadeInDown header-fixed");
      }
      else {
        fixed_top.removeClass("animated fadeInDown header-fixed");
      }
    });

    // Password Show Hide
    $('.showPass').on('click', function(){
        var passInput=$(".passInput");
        if(passInput.attr('type')==='password'){
          passInput.attr('type','text');
        }else{
          passInput.attr('type','password');
        }
    })
  });
});

$(document).ready(function () {
    $('.slider').slick({
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true, // Oklar ekleyin
        dots: true // Noktalar ekleyin
    });
});



/*==============================================================*/
// Hero slider
/*==============================================================*/
var $bannerSlider = jQuery('.banner-slider');
var $bannerFirstSlide = $('div.banner-slide:first-child');

$bannerSlider.on('init', function (e, slick) {
    var $firstAnimatingElements = $bannerFirstSlide.find('[data-animation]');
    slideanimate($firstAnimatingElements);
});
$bannerSlider.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
    var $animatingElements = $('div.slick-slide[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
    slideanimate($animatingElements);
});
$bannerSlider.slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    fade: true,
    dots: true,
    swipe: true,
    adaptiveHeight: true,
    autoplay: true, // Otomatik kaydýrmayý etkinleþtirin
    autoplaySpeed: 3000, // Kaydýrma hýzýný ayarlayýn (4 saniye)
    responsive: [
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1,
                autoplay: true,
                autoplaySpeed: 3000,
                swipe: true,
            }
        }
    ]
});
function slideanimate(elements) {
    var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
    elements.each(function () {
        var $this = $(this);
        var $animationDelay = $this.data('delay');
        var $animationType = 'animated ' + $this.data('animation');
        $this.css({
            'animation-delay': $animationDelay,
            '-webkit-animation-delay': $animationDelay
        });
        $this.addClass($animationType).one(animationEndEvents, function () {
            $this.removeClass($animationType);
        });
    });
}

// data color
jQuery("[data-color]").each(function () {
    jQuery(this).css('color', jQuery(this).attr('data-color'));
});
// data background color
jQuery("[data-bgcolor]").each(function () {
    jQuery(this).css('background-color', jQuery(this).attr('data-bgcolor'));
});
