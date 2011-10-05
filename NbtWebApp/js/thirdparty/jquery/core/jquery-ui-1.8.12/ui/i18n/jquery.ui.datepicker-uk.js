/* Ukrainian (UTF-8) initialisation for the jQuery UI date picker plugin. */
/* Written by Maxim Drogobitskiy (maxdao@gmail.com). */
jQuery(function($){
	$.datepicker.regional['uk'] = {
		closeText: 'Ð—Ð°ÐºÑ€Ð¸Ñ‚Ð¸',
		prevText: '&#x3c;',
		nextText: '&#x3e;',
		currentText: 'Ð¡ÑŒÐ¾Ð³Ð¾Ð´Ð½Ñ–',
		monthNames: ['Ð¡Ñ–Ñ‡ÐµÐ½ÑŒ','Ð›ÑŽÑ‚Ð¸Ð¹','Ð‘ÐµÑ€ÐµÐ·ÐµÐ½ÑŒ','ÐšÐ²Ñ–Ñ‚ÐµÐ½ÑŒ','Ð¢Ñ€Ð°Ð²ÐµÐ½ÑŒ','Ð§ÐµÑ€Ð²ÐµÐ½ÑŒ',
		'Ð›Ð¸Ð¿ÐµÐ½ÑŒ','Ð¡ÐµÑ€Ð¿ÐµÐ½ÑŒ','Ð’ÐµÑ€ÐµÑÐµÐ½ÑŒ','Ð–Ð¾Ð²Ñ‚ÐµÐ½ÑŒ','Ð›Ð¸ÑÑ‚Ð¾Ð¿Ð°Ð´','Ð“Ñ€ÑƒÐ´ÐµÐ½ÑŒ'],
		monthNamesShort: ['Ð¡Ñ–Ñ‡','Ð›ÑŽÑ‚','Ð‘ÐµÑ€','ÐšÐ²Ñ–','Ð¢Ñ€Ð°','Ð§ÐµÑ€',
		'Ð›Ð¸Ð¿','Ð¡ÐµÑ€','Ð’ÐµÑ€','Ð–Ð¾Ð²','Ð›Ð¸Ñ','Ð“Ñ€Ñƒ'],
		dayNames: ['Ð½ÐµÐ´Ñ–Ð»Ñ','Ð¿Ð¾Ð½ÐµÐ´Ñ–Ð»Ð¾Ðº','Ð²Ñ–Ð²Ñ‚Ð¾Ñ€Ð¾Ðº','ÑÐµÑ€ÐµÐ´Ð°','Ñ‡ÐµÑ‚Ð²ÐµÑ€','Ð¿â€™ÑÑ‚Ð½Ð¸Ñ†Ñ','ÑÑƒÐ±Ð¾Ñ‚Ð°'],
		dayNamesShort: ['Ð½ÐµÐ´','Ð¿Ð½Ð´','Ð²Ñ–Ð²','ÑÑ€Ð´','Ñ‡Ñ‚Ð²','Ð¿Ñ‚Ð½','ÑÐ±Ñ‚'],
		dayNamesMin: ['ÐÐ´','ÐŸÐ½','Ð’Ñ‚','Ð¡Ñ€','Ð§Ñ‚','ÐŸÑ‚','Ð¡Ð±'],
		weekHeader: 'ÐÐµ',
		dateFormat: 'dd/mm/yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['uk']);
});