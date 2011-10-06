/* Kazakh (UTF-8) initialisation for the jQuery UI date picker plugin. */
/* Written by Dmitriy Karasyov (dmitriy.karasyov@gmail.com). */
jQuery(function($){
	$.datepicker.regional['kz'] = {
		closeText: 'Ð–Ð°Ð±Ñƒ',
		prevText: '&#x3c;ÐÐ»Ð´Ñ‹Ò£Ò“Ñ‹',
		nextText: 'ÐšÐµÐ»ÐµÑÑ–&#x3e;',
		currentText: 'Ð‘Ò¯Ð³Ñ–Ð½',
		monthNames: ['ÒšÐ°Ò£Ñ‚Ð°Ñ€','ÐÒ›Ð¿Ð°Ð½','ÐÐ°ÑƒÑ€Ñ‹Ð·','Ð¡Ó™ÑƒÑ–Ñ€','ÐœÐ°Ð¼Ñ‹Ñ€','ÐœÐ°ÑƒÑÑ‹Ð¼',
		'Ð¨Ñ–Ð»Ð´Ðµ','Ð¢Ð°Ð¼Ñ‹Ð·','ÒšÑ‹Ñ€ÐºÒ¯Ð¹ÐµÐº','ÒšÐ°Ð·Ð°Ð½','ÒšÐ°Ñ€Ð°ÑˆÐ°','Ð–ÐµÐ»Ñ‚Ð¾Ò›ÑÐ°Ð½'],
		monthNamesShort: ['ÒšÐ°Ò£','ÐÒ›Ð¿','ÐÐ°Ñƒ','Ð¡Ó™Ñƒ','ÐœÐ°Ð¼','ÐœÐ°Ñƒ',
		'Ð¨Ñ–Ð»','Ð¢Ð°Ð¼','ÒšÑ‹Ñ€','ÒšÐ°Ð·','ÒšÐ°Ñ€','Ð–ÐµÐ»'],
		dayNames: ['Ð–ÐµÐºÑÐµÐ½Ð±Ñ–','Ð”Ò¯Ð¹ÑÐµÐ½Ð±Ñ–','Ð¡ÐµÐ¹ÑÐµÐ½Ð±Ñ–','Ð¡Ó™Ñ€ÑÐµÐ½Ð±Ñ–','Ð‘ÐµÐ¹ÑÐµÐ½Ð±Ñ–','Ð–Ò±Ð¼Ð°','Ð¡ÐµÐ½Ð±Ñ–'],
		dayNamesShort: ['Ð¶ÐºÑ','Ð´ÑÐ½','ÑÑÐ½','ÑÑ€Ñ','Ð±ÑÐ½','Ð¶Ð¼Ð°','ÑÐ½Ð±'],
		dayNamesMin: ['Ð–Ðº','Ð”Ñ','Ð¡Ñ','Ð¡Ñ€','Ð‘Ñ','Ð–Ð¼','Ð¡Ð½'],
		weekHeader: 'ÐÐµ',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['kz']);
});
