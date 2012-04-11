;(function($){
/**
 * jqGrid Arabic Translation
 * 
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "ØªØ³Ø¬ÙŠÙ„ {0} - {1} Ø¹Ù„Ù‰ {2}",
		emptyrecords: "Ù„Ø§ ÙŠÙˆØ¬Ø¯ ØªØ³Ø¬ÙŠÙ„",
		loadtext: "ØªØ­Ù…ÙŠÙ„...",
		pgtext : "ØµÙØ­Ø© {0} Ø¹Ù„Ù‰ {1}"
	},
	search : {
		caption: "Ø¨Ø­Ø«...",
		Find: "Ø¨Ø­Ø«",
		Reset: "Ø¥Ù„ØºØ§Ø¡",
		odata : ['ÙŠØ³Ø§ÙˆÙŠ', 'ÙŠØ®ØªÙ„Ù', 'Ø£Ù‚Ù„', 'Ø£Ù‚Ù„ Ø£Ùˆ ÙŠØ³Ø§ÙˆÙŠ','Ø£ÙƒØ¨Ø±','Ø£ÙƒØ¨Ø± Ø£Ùˆ ÙŠØ³Ø§ÙˆÙŠ', 'ÙŠØ¨Ø¯Ø£ Ø¨Ù€','Ù„Ø§ ÙŠØ¨Ø¯Ø£ Ø¨Ù€','est dans',"n'est pas dans",'ÙŠÙ†ØªÙ‡ Ø¨Ù€','Ù„Ø§ ÙŠÙ†ØªÙ‡ Ø¨Ù€','ÙŠØ­ØªÙˆÙŠ','Ù„Ø§ ÙŠØ­ØªÙˆÙŠ'],
		groupOps: [	{ op: "Ù…Ø¹", text: "Ø§Ù„ÙƒÙ„" },	{ op: "Ø£Ùˆ",  text: "Ù„Ø§ Ø£Ø­Ø¯" }	],
		matchText: " ØªÙˆØ§ÙÙ‚",
		rulesText: " Ù‚ÙˆØ§Ø¹Ø¯"
	},
	edit : {
		addCaption: "Ø§Ø¶Ø§ÙØ©",
		editCaption: "ØªØ­Ø¯ÙŠØ«",
		bSubmit: "ØªØ«Ø¨ÙŠØ«",
		bCancel: "Ø¥Ù„ØºØ§Ø¡",
		bClose: "ØºÙ„Ù‚",
		saveData: "ØªØºÙŠØ±Øª Ø§Ù„Ù…Ø¹Ø·ÙŠØ§Øª Ù‡Ù„ ØªØ±ÙŠØ¯ Ø§Ù„ØªØ³Ø¬ÙŠÙ„ ?",
		bYes: "Ù†Ø¹Ù…",
		bNo: "Ù„Ø§",
		bExit: "Ø¥Ù„ØºØ§Ø¡",
		msg: {
			required: "Ø®Ø§Ù†Ø© Ø¥Ø¬Ø¨Ø§Ø±ÙŠØ©",
			number: "Ø³Ø¬Ù„ Ø±Ù‚Ù… ØµØ­ÙŠØ­",
			minValue: "ÙŠØ¬Ø¨ Ø£Ù† ØªÙƒÙˆÙ† Ø§Ù„Ù‚ÙŠÙ…Ø© Ø£ÙƒØ¨Ø± Ø£Ùˆ ØªØ³Ø§ÙˆÙŠ 0",
			maxValue: "ÙŠØ¬Ø¨ Ø£Ù† ØªÙƒÙˆÙ† Ø§Ù„Ù‚ÙŠÙ…Ø© Ø£Ù‚Ù„ Ø£Ùˆ ØªØ³Ø§ÙˆÙŠ 0",
			email: "Ø¨Ø±ÙŠØ¯ ØºÙŠØ± ØµØ­ÙŠØ­",
			integer: "Ø³Ø¬Ù„ Ø¹Ø¯Ø¯ Ø·Ø¨ÙŠÙŠØ¹ÙŠ ØµØ­ÙŠØ­",
			url: "Ù„ÙŠØ³ Ø¹Ù†ÙˆØ§Ù†Ø§ ØµØ­ÙŠØ­Ø§. Ø§Ù„Ø¨Ø¯Ø§ÙŠØ© Ø§Ù„ØµØ­ÙŠØ­Ø© ('http://' Ø£Ùˆ 'https://')",
			nodefined : " Ù„ÙŠØ³ Ù…Ø­Ø¯Ø¯!",
			novalue : " Ù‚ÙŠÙ…Ø© Ø§Ù„Ø±Ø¬ÙˆØ¹ Ù…Ø·Ù„ÙˆØ¨Ø©!",
			customarray : "ÙŠØ¬Ø¨ Ø¹Ù„Ù‰ Ø§Ù„Ø¯Ø§Ù„Ø© Ø§Ù„Ø´Ø®ØµÙŠØ© Ø£Ù† ØªÙ†ØªØ¬ Ø¬Ø¯ÙˆÙ„Ø§",
			customfcheck : "Ø§Ù„Ø¯Ø§Ù„Ø© Ø§Ù„Ø´Ø®ØµÙŠØ© Ù…Ø·Ù„ÙˆØ¨Ø© ÙÙŠ Ø­Ø§Ù„Ø© Ø§Ù„ØªØ­Ù‚Ù‚ Ø§Ù„Ø´Ø®ØµÙŠ"
		}
	},
	view : {
		caption: "Ø±Ø£ÙŠØª Ø§Ù„ØªØ³Ø¬ÙŠÙ„Ø§Øª",
		bClose: "ØºÙ„Ù‚"
	},
	del : {
		caption: "Ø­Ø°Ù",
		msg: "Ø­Ø°Ù Ø§Ù„ØªØ³Ø¬ÙŠÙ„Ø§Øª Ø§Ù„Ù…Ø®ØªØ§Ø±Ø© ?",
		bSubmit: "Ø­Ø°Ù",
		bCancel: "Ø¥Ù„ØºØ§Ø¡"
	},
	nav : {
		edittext: " ",
		edittitle: "ØªØºÙŠÙŠØ± Ø§Ù„ØªØ³Ø¬ÙŠÙ„ Ø§Ù„Ù…Ø®ØªØ§Ø±",
		addtext:" ",
		addtitle: "Ø¥Ø¶Ø§ÙØ© ØªØ³Ø¬ÙŠÙ„",
		deltext: " ",
		deltitle: "Ø­Ø°Ù Ø§Ù„ØªØ³Ø¬ÙŠÙ„ Ø§Ù„Ù…Ø®ØªØ§Ø±",
		searchtext: " ",
		searchtitle: "Ø¨Ø­Ø« Ø¹Ù† ØªØ³Ø¬ÙŠÙ„",
		refreshtext: "",
		refreshtitle: "ØªØ­Ø¯ÙŠØ« Ø§Ù„Ø¬Ø¯ÙˆÙ„",
		alertcap: "ØªØ­Ø°ÙŠØ±",
		alerttext: "ÙŠØ±Ø¬Ù‰ Ø¥Ø®ØªÙŠØ§Ø± Ø§Ù„Ø³Ø·Ø±",
		viewtext: "",
		viewtitle: "Ø¥Ø¸Ù‡Ø§Ø± Ø§Ù„Ø³Ø·Ø± Ø§Ù„Ù…Ø®ØªØ§Ø±"
	},
	col : {
		caption: "Ø¥Ø¸Ù‡Ø§Ø±/Ø¥Ø®ÙØ§Ø¡ Ø§Ù„Ø£Ø¹Ù…Ø¯Ø©",
		bSubmit: "ØªØ«Ø¨ÙŠØ«",
		bCancel: "Ø¥Ù„ØºØ§Ø¡"
	},
	errors : {
		errcap : "Ø®Ø·Ø£",
		nourl : "Ù„Ø§ ÙŠÙˆØ¬Ø¯ Ø¹Ù†ÙˆØ§Ù† Ù…Ø­Ø¯Ø¯",
		norecords: "Ù„Ø§ ÙŠÙˆØ¬Ø¯ ØªØ³Ø¬ÙŠÙ„ Ù„Ù„Ù…Ø¹Ø§Ù„Ø¬Ø©",
		model : "Ø¹Ø¯Ø¯ Ø§Ù„Ø¹Ù†Ø§ÙˆÙŠÙ† (colNames) <> Ø¹Ø¯Ø¯ Ø§Ù„ØªØ³Ø¬ÙŠÙ„Ø§Øª (colModel)!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Ø§Ù„Ø£Ø­Ø¯", "Ø§Ù„Ø¥Ø«Ù†ÙŠÙ†", "Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡", "Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡", "Ø§Ù„Ø®Ù…ÙŠØ³", "Ø§Ù„Ø¬Ù…Ø¹Ø©", "Ø§Ù„Ø³Ø¨Øª",
				"Ø§Ù„Ø£Ø­Ø¯", "Ø§Ù„Ø¥Ø«Ù†ÙŠÙ†", "Ø§Ù„Ø«Ù„Ø§Ø«Ø§Ø¡", "Ø§Ù„Ø£Ø±Ø¨Ø¹Ø§Ø¡", "Ø§Ù„Ø®Ù…ÙŠØ³", "Ø§Ù„Ø¬Ù…Ø¹Ø©", "Ø§Ù„Ø³Ø¨Øª"
			],
			monthNames: [
				"Ø¬Ø§Ù†ÙÙŠ", "ÙÙŠÙØ±ÙŠ", "Ù…Ø§Ø±Ø³", "Ø£ÙØ±ÙŠÙ„", "Ù…Ø§ÙŠ", "Ø¬ÙˆØ§Ù†", "Ø¬ÙˆÙŠÙ„ÙŠØ©", "Ø£ÙˆØª", "Ø³Ø¨ØªÙ…Ø¨Ø±", "Ø£ÙƒØªÙˆØ¨Ø±", "Ù†ÙˆÙÙ…Ø¨Ø±", "Ø¯ÙŠØ³Ù…Ø¨Ø±",
				"Ø¬Ø§Ù†ÙÙŠ", "ÙÙŠÙØ±ÙŠ", "Ù…Ø§Ø±Ø³", "Ø£ÙØ±ÙŠÙ„", "Ù…Ø§ÙŠ", "Ø¬ÙˆØ§Ù†", "Ø¬ÙˆÙŠÙ„ÙŠØ©", "Ø£ÙˆØª", "Ø³Ø¨ØªÙ…Ø¨Ø±", "Ø£ÙƒØªÙˆØ¨Ø±", "Ù†ÙˆÙÙ…Ø¨Ø±", "Ø¯ÙŠØ³Ù…Ø¨Ø±"
			],
			AmPm : ["ØµØ¨Ø§Ø­Ø§","Ù…Ø³Ø§Ø¡Ø§","ØµØ¨Ø§Ø­Ø§","Ù…Ø³Ø§Ø¡Ø§"],
			S: function (j) {return j == 1 ? 'er' : 'e';},
			srcformat: 'Y-m-d',
			newformat: 'd/m/Y',
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "n/j/Y",
				LongDate: "l, F d, Y",
				FullDateTime: "l, F d, Y g:i:s A",
				MonthDay: "F d",
				ShortTime: "g:i A",
				LongTime: "g:i:s A",
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				YearMonth: "F, Y"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
};
})(jQuery);
