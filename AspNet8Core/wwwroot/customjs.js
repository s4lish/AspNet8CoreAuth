
$(document).ready(function () {
  $('li a.active').removeClass('active').removeAttr('aria-current');
  $('a[href="' + location.pathname + '"]').closest('a').addClass('active').attr('aria-current', 'page');
});
