/* Tajiki (UTF-8) initialisation for the jQuery UI date picker plugin. */
/* Written by Abdurahmon Saidov (saidovab@gmail.com). */
jQuery(function($){
	$.datepicker.regional['tj'] = {
		closeText: 'Ð˜Ð´Ð¾Ð¼Ð°',
		prevText: '&#x3c;ÒšÐ°Ñ„Ð¾',
		nextText: 'ÐŸÐµÑˆ&#x3e;',
		currentText: 'Ð˜Ð¼Ñ€Ó¯Ð·',
		monthNames: ['Ð¯Ð½Ð²Ð°Ñ€','Ð¤ÐµÐ²Ñ€Ð°Ð»','ÐœÐ°Ñ€Ñ‚','ÐÐ¿Ñ€ÐµÐ»','ÐœÐ°Ð¹','Ð˜ÑŽÐ½',
		'Ð˜ÑŽÐ»','ÐÐ²Ð³ÑƒÑÑ‚','Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€','ÐžÐºÑ‚ÑÐ±Ñ€','ÐÐ¾ÑÐ±Ñ€','Ð”ÐµÐºÐ°Ð±Ñ€'],
		monthNamesShort: ['Ð¯Ð½Ð²','Ð¤ÐµÐ²','ÐœÐ°Ñ€','ÐÐ¿Ñ€','ÐœÐ°Ð¹','Ð˜ÑŽÐ½',
		'Ð˜ÑŽÐ»','ÐÐ²Ð³','Ð¡ÐµÐ½','ÐžÐºÑ‚','ÐÐ¾Ñ','Ð”ÐµÐº'],
		dayNames: ['ÑÐºÑˆÐ°Ð½Ð±Ðµ','Ð´ÑƒÑˆÐ°Ð½Ð±Ðµ','ÑÐµÑˆÐ°Ð½Ð±Ðµ','Ñ‡Ð¾Ñ€ÑˆÐ°Ð½Ð±Ðµ','Ð¿Ð°Ð½Ò·ÑˆÐ°Ð½Ð±Ðµ','Ò·ÑƒÐ¼ÑŠÐ°','ÑˆÐ°Ð½Ð±Ðµ'],
		dayNamesShort: ['ÑÐºÑˆ','Ð´ÑƒÑˆ','ÑÐµÑˆ','Ñ‡Ð¾Ñ€','Ð¿Ð°Ð½','Ò·ÑƒÐ¼','ÑˆÐ°Ð½'],
		dayNamesMin: ['Ð¯Ðº','Ð”Ñˆ','Ð¡Ñˆ','Ð§Ñˆ','ÐŸÑˆ','Ò¶Ð¼','Ð¨Ð½'],
		weekHeader: 'Ð¥Ñ„',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['tj']);
});