/* Russian (UTF-8) initialisation for the jQuery UI date picker plugin. */
/* Written by Andrew Stromnov (stromnov@gmail.com). */
jQuery(function($){
	$.datepicker.regional['ru'] = {
		closeText: 'Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ',
		prevText: '&#x3c;ÐŸÑ€ÐµÐ´',
		nextText: 'Ð¡Ð»ÐµÐ´&#x3e;',
		currentText: 'Ð¡ÐµÐ³Ð¾Ð´Ð½Ñ',
		monthNames: ['Ð¯Ð½Ð²Ð°Ñ€ÑŒ','Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ','ÐœÐ°Ñ€Ñ‚','ÐÐ¿Ñ€ÐµÐ»ÑŒ','ÐœÐ°Ð¹','Ð˜ÑŽÐ½ÑŒ',
		'Ð˜ÑŽÐ»ÑŒ','ÐÐ²Ð³ÑƒÑÑ‚','Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ','ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ','ÐÐ¾ÑÐ±Ñ€ÑŒ','Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ'],
		monthNamesShort: ['Ð¯Ð½Ð²','Ð¤ÐµÐ²','ÐœÐ°Ñ€','ÐÐ¿Ñ€','ÐœÐ°Ð¹','Ð˜ÑŽÐ½',
		'Ð˜ÑŽÐ»','ÐÐ²Ð³','Ð¡ÐµÐ½','ÐžÐºÑ‚','ÐÐ¾Ñ','Ð”ÐµÐº'],
		dayNames: ['Ð²Ð¾ÑÐºÑ€ÐµÑÐµÐ½ÑŒÐµ','Ð¿Ð¾Ð½ÐµÐ´ÐµÐ»ÑŒÐ½Ð¸Ðº','Ð²Ñ‚Ð¾Ñ€Ð½Ð¸Ðº','ÑÑ€ÐµÐ´Ð°','Ñ‡ÐµÑ‚Ð²ÐµÑ€Ð³','Ð¿ÑÑ‚Ð½Ð¸Ñ†Ð°','ÑÑƒÐ±Ð±Ð¾Ñ‚Ð°'],
		dayNamesShort: ['Ð²ÑÐº','Ð¿Ð½Ð´','Ð²Ñ‚Ñ€','ÑÑ€Ð´','Ñ‡Ñ‚Ð²','Ð¿Ñ‚Ð½','ÑÐ±Ñ‚'],
		dayNamesMin: ['Ð’Ñ','ÐŸÐ½','Ð’Ñ‚','Ð¡Ñ€','Ð§Ñ‚','ÐŸÑ‚','Ð¡Ð±'],
		weekHeader: 'ÐÐµÐ´',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['ru']);
});