/* Chinese initialisation for the jQuery UI date picker plugin. */
/* Written by Cloudream (cloudream@gmail.com). */
jQuery(function($){
	$.datepicker.regional['zh-CN'] = {
		closeText: 'å…³é—­',
		prevText: '&#x3c;ä¸Šæœˆ',
		nextText: 'ä¸‹æœˆ&#x3e;',
		currentText: 'ä»Šå¤©',
		monthNames: ['ä¸€æœˆ','äºŒæœˆ','ä¸‰æœˆ','å››æœˆ','äº”æœˆ','å…­æœˆ',
		'ä¸ƒæœˆ','å…«æœˆ','ä¹æœˆ','åæœˆ','åä¸€æœˆ','åäºŒæœˆ'],
		monthNamesShort: ['ä¸€','äºŒ','ä¸‰','å››','äº”','å…­',
		'ä¸ƒ','å…«','ä¹','å','åä¸€','åäºŒ'],
		dayNames: ['æ˜ŸæœŸæ—¥','æ˜ŸæœŸä¸€','æ˜ŸæœŸäºŒ','æ˜ŸæœŸä¸‰','æ˜ŸæœŸå››','æ˜ŸæœŸäº”','æ˜ŸæœŸå…­'],
		dayNamesShort: ['å‘¨æ—¥','å‘¨ä¸€','å‘¨äºŒ','å‘¨ä¸‰','å‘¨å››','å‘¨äº”','å‘¨å…­'],
		dayNamesMin: ['æ—¥','ä¸€','äºŒ','ä¸‰','å››','äº”','å…­'],
		weekHeader: 'å‘¨',
		dateFormat: 'yy-mm-dd',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: true,
		yearSuffix: 'å¹´'};
	$.datepicker.setDefaults($.datepicker.regional['zh-CN']);
});
