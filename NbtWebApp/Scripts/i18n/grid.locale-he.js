;(function($){
/**
 * jqGrid Hebrew Translation
 * Shuki Shukrun shukrun.shuki@gmail.com
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "×ž×¦×™×’ {0} - {1} ×ž×ª×•×š {2}",
		emptyrecords: "××™×Ÿ ×¨×©×•×ž×•×ª ×œ×”×¦×™×’",
		loadtext: "×˜×•×¢×Ÿ...",
		pgtext : "×“×£ {0} ×ž×ª×•×š {1}"
	},
	search : {
		caption: "×ž×—×¤×©...",
		Find: "×—×¤×©",
		Reset: "×”×ª×—×œ",
		odata : ['×©×•×•×”', '×œ× ×©×•×•×”', '×§×˜×Ÿ', '×§×˜×Ÿ ××• ×©×•×•×”','×’×“×•×œ','×’×“×•×œ ××• ×©×•×•×”', '×ž×ª×—×™×œ ×‘','×œ× ×ž×ª×—×™×œ ×‘','× ×ž×¦× ×‘','×œ× × ×ž×¦× ×‘','×ž×¡×ª×™×™× ×‘','×œ× ×ž×¡×ª×™×™× ×‘','×ž×›×™×œ','×œ× ×ž×›×™×œ'],
		groupOps: [	{ op: "AND", text: "×”×›×œ" },	{ op: "OR",  text: "××—×“ ×ž" }	],
		matchText: " ×ª×•××",
		rulesText: " ×—×•×§×™×"
	},
	edit : {
		addCaption: "×”×•×¡×£ ×¨×©×•×ž×”",
		editCaption: "×¢×¨×•×š ×¨×©×•×ž×”",
		bSubmit: "×©×œ×—",
		bCancel: "×‘×˜×œ",
		bClose: "×¡×’×•×¨",
		saveData: "× ×ª×•× ×™× ×”×©×ª× ×•! ×œ×©×ž×•×¨?",
		bYes : "×›×Ÿ",
		bNo : "×œ×",
		bExit : "×‘×˜×œ",
		msg: {
			required:"×©×“×” ×—×•×‘×”",
			number:"×× ×, ×”×›× ×¡ ×ž×¡×¤×¨ ×ª×§×™×Ÿ",
			minValue:"×¢×¨×š ×¦×¨×™×š ×œ×”×™×•×ª ×’×“×•×œ ××• ×©×•×•×” ×œ ",
			maxValue:"×¢×¨×š ×¦×¨×™×š ×œ×”×™×•×ª ×§×˜×Ÿ ××• ×©×•×•×” ×œ ",
			email: "×”×™× ×œ× ×›×ª×•×‘×ª ××™×™×ž×œ ×ª×§×™× ×”",
			integer: "×× ×, ×”×›× ×¡ ×ž×¡×¤×¨ ×©×œ×",
			date: "×× ×, ×”×›× ×¡ ×ª××¨×™×š ×ª×§×™×Ÿ",
			url: "×”×›×ª×•×‘×ª ××™× ×” ×ª×§×™× ×”. ×“×¨×•×©×” ×ª×—×™×œ×™×ª ('http://' ××• 'https://')",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
		caption: "×”×¦×’ ×¨×©×•×ž×”",
		bClose: "×¡×’×•×¨"
	},
	del : {
		caption: "×ž×—×§",
		msg: "×”×× ×œ×ž×—×•×§ ××ª ×”×¨×©×•×ž×”/×•×ª ×”×ž×¡×•×ž× ×•×ª?",
		bSubmit: "×ž×—×§",
		bCancel: "×‘×˜×œ"
	},
	nav : {
		edittext: "",
		edittitle: "×¢×¨×•×š ×©×•×¨×” ×ž×¡×•×ž× ×ª",
		addtext:"",
		addtitle: "×”×•×¡×£ ×©×•×¨×” ×—×“×©×”",
		deltext: "",
		deltitle: "×ž×—×§ ×©×•×¨×” ×ž×¡×•×ž× ×ª",
		searchtext: "",
		searchtitle: "×—×¤×© ×¨×©×•×ž×•×ª",
		refreshtext: "",
		refreshtitle: "×˜×¢×Ÿ ×’×¨×™×“ ×ž×—×“×©",
		alertcap: "××–×”×¨×”",
		alerttext: "×× ×, ×‘×—×¨ ×©×•×¨×”",
		viewtext: "",
		viewtitle: "×”×¦×’ ×©×•×¨×” ×ž×¡×•×ž× ×ª"
	},
	col : {
		caption: "×”×¦×’/×”×¡×ª×¨ ×¢×ž×•×“×•×ª",
		bSubmit: "×©×œ×—",
		bCancel: "×‘×˜×œ"
	},
	errors : {
		errcap : "×©×’×™××”",
		nourl : "×œ× ×”×•×’×“×¨×” ×›×ª×•×‘×ª url",
		norecords: "××™×Ÿ ×¨×©×•×ž×•×ª ×œ×¢×‘×“",
		model : "××•×¨×š ×©×œ colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"×", "×‘", "×’", "×“", "×”", "×•", "×©",
				"×¨××©×•×Ÿ", "×©× ×™", "×©×œ×™×©×™", "×¨×‘×™×¢×™", "×—×ž×™×©×™", "×©×™×©×™", "×©×‘×ª"
			],
			monthNames: [
				"×™× ×•", "×¤×‘×¨", "×ž×¨×¥", "××¤×¨", "×ž××™", "×™×•× ", "×™×•×œ", "××•×’", "×¡×¤×˜", "××•×§", "× ×•×‘", "×“×¦×ž",
				"×™× ×•××¨", "×¤×‘×¨×•××¨", "×ž×¨×¥", "××¤×¨×™×œ", "×ž××™", "×™×•× ×™", "×™×•×œ×™", "××•×’×•×¡×˜", "×¡×¤×˜×ž×‘×¨", "××•×§×˜×•×‘×¨", "× ×•×‘×ž×‘×¨", "×“×¦×ž×‘×¨"
			],
			AmPm : ["×œ×¤× ×™ ×”×¦×”×¨×™×","××—×¨ ×”×¦×”×¨×™×","×œ×¤× ×™ ×”×¦×”×¨×™×","××—×¨ ×”×¦×”×¨×™×"],
			S: function (j) {return j < 11 || j > 13 ? ['', '', '', ''][Math.min((j - 1) % 10, 3)] : ''},
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
