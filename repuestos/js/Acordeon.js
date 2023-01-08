////////////////////////////
// http://www.adipalaz.com/experiments/jquery/accordion2.html
///////////////////////////
(function ($) {
    //http://www.mail-archive.com/jquery-en@googlegroups.com/msg43851.html
    $.fn.orphans = function () {
        var txt = [];
        this.each(function () {
            $.each(this.childNodes, function () {
                if (this.nodeType == 3 && $.trim(this.nodeValue)) txt.push(this)
            })
        });
        return $(txt);
    };
})(jQuery);
////////////////////////////
$(function () {
    $('#content .expand').orphans().wrap('<a href="#" title="expand/collapse"></a>');

    //different methods for selecting the desired item:
    //demo 1 - div.demo:eq(0)  - SlideFade Effects
    $('div.demo:eq(0) ul').find('ul.collapse').hide();
    $('div.demo:eq(0) .expand').click(function () {
        $(this).toggleClass('open').siblings().removeClass('open').end()
        .find('ul').slideToggle(400).end()
        .siblings('li').find('ul:visible').slideToggle(800);
        return false;
    });

    //demo 2 - div.demo:eq(1) - Slide Effects 
    $('div.demo:eq(1) ul').find('ul.collapse:gt(0)').hide().end().find('.expand:eq(0)').addClass('open');
    $('div.demo:eq(1) .expand').click(function () {
        $(this).toggleClass('open').siblings().removeClass('open').end()
        .find('ul').slideToggle().end()
        .siblings('li').find('ul:visible').slideToggle();
        return false;
    });

    //demo 3 - div.demo:eq(2) - Slide Effects 
    $('div.demo:eq(2) ul').find('ul.collapse:not(:eq(1))').hide().end().find('ul.collapse:visible').parent().addClass('open');
    $('div.demo:eq(2) .expand').click(function () {
        $(this).toggleClass('open').siblings().removeClass('open').end()
        .find('ul').slideToggle().end()
        .siblings('li').find('ul:visible').slideToggle();
        return false;
    });

    //demo 4 - div.demo:eq(3) - Slide Effects  - Always keep one sublist shown
    $('div.demo:eq(3) ul').find('ul.collapse:not(:last)').hide().end().find('.expand:last').addClass('open');
    $('div.demo:eq(3) .expand').click(function () {
        $(this).addClass('open').siblings().removeClass('open').end()
        .find('ul:hidden').slideToggle().end()
        .siblings('li').find('ul:visible').slideUp();
        return false;
    });

    //demo 5 - div.demo:eq(4) - Queued Slide Effects 
    $('div.demo:eq(4) ul').find('ul.collapse:not(:first)').hide().end().find('.expand:eq(0)').addClass('open');
    $('div.demo:eq(4) .expand').each(function () {
        $(this).click(function () {
            var $thisCllps = $(this).find('ul');
            var $cllpsVisible = $(this).siblings('li').find('ul:visible');
            ($cllpsVisible.length) ? $(this).toggleClass('open').siblings('li').removeClass('open')
                  .find('ul:visible').slideUp(400, function () {
                      $thisCllps.slideDown();
                  }) : $thisCllps.slideToggle().parent().toggleClass('open');
            return false;
        });
    });

    //demo 6 - div.demo:eq(5) - Queued Slide Effects - Always keep one sublist shown
    $('div.demo:eq(5) ul').find('ul.collapse:not(:first)').hide().end().find('.expand:eq(0)').addClass('open');
    $('div.demo:eq(5) .expand').each(function () {
        $(this).click(function () {
            var $thisCllps = $(this).find('ul');
            var $cllpsVisible = $(this).siblings('li').find('ul:visible');
            ($cllpsVisible.length) ? $(this).toggleClass('open').siblings('li').removeClass('open')
                  .find('ul:visible').slideUp(400, function () {
                      $thisCllps.slideDown();
                  }) : $(this).find('ul:hidden').slideToggle().parent().toggleClass('open');
            return false;
        });
    });
});