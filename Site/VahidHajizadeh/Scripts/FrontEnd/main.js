"use strict";	
/* -------------------------------------
	CUSTOM FUNCTION WRITE HERE
 -------------------------------------- */
$(document).ready(function () {
	/* ---------------------------------------
		NEWS TICKER
	 -------------------------------------- */
	var ticker = $('#ticker'),
			container = ticker.find('ul'),
			tickWidth = 1,
			speed = 0.1, // Control pace
			distance,
			timing;
	container.find("li").each(function (i) {
		tickWidth += jQuery(this, i).outerWidth(true);
	});
	distance = tickWidth + (ticker.outerWidth() - jQuery('#gameLabel').outerWidth());
	timing = distance / speed;
	function scrollIt(dist, dur) {
		container.animate({
			left: '-=' + dist
		}, dur, 'linear', function () {
			container.css({
				'left': ticker.outerWidth()
			});
			scrollIt(distance, timing);
		});
	}
	scrollIt(distance, timing);
	ticker.hover(function () {
		container.stop();
	}, function () {
		var offset = container.offset(),
				newPosition = offset.left + tickWidth,
				newTime = newPosition / speed;
		scrollIt(newPosition, newTime);
	});
	/* ---------------------------------------
		DETAIL PAGE SLIDER
	 -------------------------------------- */
	$("#tg-banner-3").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 200,
		items: 7,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 5],
		itemsDesktopSmall: [991, 4],
		itemsTabletSmall: [767, 3],
		itemsMobile: [479, 1]
	});
	/* ---------------------------------------
		POST OF THE DAY
	 -------------------------------------- */
	$("#tg-postslider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 200,
		items: 3,
		pagination: true,
		paginationSpeed: 400,
		itemsDesktop: [1199, 3],
		itemsDesktopSmall: [991, 2]
	});
	/* ---------------------------------------
		MOST VIEWED
	 -------------------------------------- */
	$("#tg-mostviewdslider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 200,
		items: 4,
		pagination: true,
		paginationSpeed: 400,
		itemsDesktop: [1199, 3],
		itemsDesktopSmall: [991, 3]
	});
	/* ---------------------------------------
		FOOTER SLIDER
	 -------------------------------------- */
	$("#tg-footerpostslider").owlCarousel({
		autoPlay: false,
		navigation: true,
		slideSpeed: 300,
		items: 1,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 1],
		itemsDesktopSmall: [991, 1],
		itemsTabletSmall: [767, 1],
		navigationText: [
			"<i class='fa fa-angle-left'></i>",
			"<i class='fa fa-angle-right'></i>"
		]
	});
	/* ---------------------------------------
		WIDGET SLIDER
	 -------------------------------------- */
	$("#tg-widget-slider").owlCarousel({
		autoPlay: false,
		navigation: true,
		slideSpeed: 200,
		items: 1,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 1],
		itemsDesktopSmall: [991, 1],
		itemsTabletSmall: [768, 1],
		navigationText: [
			"<i class='fa fa-angle-left'></i>",
			"<i class='fa fa-angle-right'></i>"
		]
	});
	/* ---------------------------------------
		SLIDER POST
	 -------------------------------------- */
	$("#tg-slider-post").owlCarousel({
		autoPlay: false,
		navigation: true,
		slideSpeed: 300,
		items: 1,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 1],
		itemsDesktopSmall: [991, 1],
		itemsTabletSmall: [767, 1],
		navigationText: [
			"<i class='fa fa-angle-left'></i>",
			"<i class='fa fa-angle-right'></i>"
		]
	});
	/* ---------------------------------------
		FULLWIDHT SLIDER POST
	 -------------------------------------- */
	$("#tg-relatedimage-slider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 300,
		items: 4,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 3],
		itemsDesktopSmall: [991, 2],
		itemsTabletSmall: [767, 2],
		itemsMobile: [479, 1]
	});
	/* ---------------------------------------
		DETAIL PAGE SLIDER
	 -------------------------------------- */
	$("#tg-detailpage-slider").owlCarousel({
		autoPlay: false,
		navigation: false,
		slideSpeed: 300,
		items: 3,
		pagination: false,
		paginationSpeed: 400,
		itemsDesktop: [1199, 3],
		itemsDesktopSmall: [991, 2],
		itemsTabletSmall: [767, 2],
		itemsMobile: [479, 1]
	});
	/* ---------------------------------------
		PITREST SLIDER
	 -------------------------------------- */
	$("#tg-pintrest-slider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 300,
		singleItem: true,
		pagination: false,
		paginationSpeed: 400,
	});
	/* ---------------------------------------
		TWEETER SLIDER
	-------------------------------------- */
	$("#tg-tweeter-slider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 300,
		singleItem: true,
		pagination: false,
		paginationSpeed: 400,
	});
	/* ---------------------------------------
		FACEBOOK SLIDER
	-------------------------------------- */
	$("#tg-facebook-slider").owlCarousel({
		autoPlay: true,
		navigation: false,
		slideSpeed: 300,
		singleItem: true,
		pagination: false,
		paginationSpeed: 400,
	});
	/* ---------------------------------------
		SEARCH
	 -------------------------------------- */
	$(function () {
		$('a[href="#tg-search"]').on('click', function (event) {
			event.preventDefault();
			$('#tg-search').addClass('open');
			$('#tg-search > form > input[type="search"]').focus();
		});
		$('#tg-search, #tg-search button.close').on('click keyup', function (event) {
			if (event.target == this || event.target.className == 'close' || event.keyCode == 27) {
				$(this).removeClass('open');
			}
		});
		//Do not include! This prevents the form from submitting for DEMO purposes only!
		$('form').submit(function (event) {
			event.preventDefault();
			return false;
		})
	});
	/* ---------------------------------------
		SCROLL TO TOP
	-------------------------------------- */
	$('.tg-scroltop').on('click', function (){
		$("html, body").animate({
			scrollTop: 0
		}, 2000);
		return false;
	});
	/* -------------------------------------
		PRICE RANGE SLIDER
	 -------------------------------------- */
	function rangSlider() {
		$("#tg-pricerange").slider({
			min: 0,
			max: 5000,
			slide: function (event, ui) {
				$("#amount").val("$" + ui.value);
			}
		});
		$("#amount").val("$" + $("#tg-pricerange").slider("value"));
	}
	rangSlider();
	/*---------------------------------------
		PRODUCT SLIDER
	 -------------------------------------- */
	(function () {
		var sync1 = $("#tg-view-slider");
		var sync2 = $("#tg-thumbnail-slider");
		sync1.owlCarousel({
			singleItem: true,
			slideSpeed: 1000,
			navigation: false,
			pagination: false,
			afterAction: syncPosition,
			responsiveRefreshRate: 200,
		});
		sync2.owlCarousel({
			items: 4,
			itemsDesktop: [1199, 4],
			itemsDesktopSmall: [979, 4],
			itemsTablet: [768, 6],
			itemsMobile: [479, 4],
			pagination: false,
			navigation: false,
			navigationText: [
				"<i class='fa fa-angle-left'></i>",
				"<i class='fa fa-angle-right'></i>"
			],
			responsiveRefreshRate: 100,
			afterInit: function (el) {
				el.find(".owl-item").eq(0).addClass("active");
			}
		});
		function syncPosition(el) {
			var current = this.currentItem;
			$("#tg-thumbnail-slider")
					.find(".owl-item")
					.removeClass("active")
					.eq(current)
					.addClass("active")
			if ($("#tg-thumbnail-slider").data("owlCarousel") !== undefined) {
				center(current)
			}
		}
		$("#tg-thumbnail-slider").on("click", ".owl-item", function (e) {
			e.preventDefault();
			var number = $(this).data("owlItem");
			sync1.trigger("owl.goTo", number);
		});
		function center(number) {
			var sync2visible = sync2.data("owlCarousel").owl.visibleItems;
			var num = number;
			var found = false;
			for (var i in sync2visible) {
				if (num === sync2visible[i]) {
					var found = true;
				}
			}
			if (found === false) {
				if (num > sync2visible[sync2visible.length - 1]) {
					sync2.trigger("owl.goTo", num - sync2visible.length + 2)
				} else {
					if (num - 1 === -1) {
						num = 0;
					}
					sync2.trigger("owl.goTo", num);
				}
			} else if (num === sync2visible[sync2visible.length - 1]) {
				sync2.trigger("owl.goTo", sync2visible[1])
			} else if (num === sync2visible[0]) {
				sync2.trigger("owl.goTo", num - 1)
			}
		}
	}(jQuery));
	/*---------------------------------------
		PRODUCT SLIDER
	 ---------------------------------------*/
	$('em.minus').on('click', function () {
		$('#quantity1').val(parseInt($('#quantity1').val(), 10) - 1);
	});
	$('em.plus').on('click', function () {
		$('#quantity1').val(parseInt($('#quantity1').val(), 10) + 1);
	});
	/* -------------------------------------
		Google Map
	-------------------------------------- */
	$("#tg-map").gmap3({
		marker: {
			address: "1600 Elizabeth St, Melbourne, Victoria, Australia",
			options: {
				title: "Robert Frost Elementary School"
			}
		},
		map: {
			options: {
				zoom: 16,
				scrollwheel: false,
				disableDoubleClickZoom: true,
			}
		}
	});
	/* ---------------------------------------
		BANNER SLIDER
	 -------------------------------------- */
	$(".photosgallery-vertical").sliderkit({
		circular: true,
		shownavitems: 6,
		freeheight:true,
		verticalnav:true,
		navclipcenter: true,
		auto:true,
		panelfxspeed:300
	});
});
/* -------------------------------------
		PRELOADER
-------------------------------------- */
jQuery(window).load(function() {
	jQuery(".preloader-outer").delay(2000).fadeOut();
	jQuery(".preloader").delay(2000).fadeOut("slow");
});