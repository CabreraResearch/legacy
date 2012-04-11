;(function($){
/**
 * jqGrid Chinese Translation for v3.6
 * waiting 2010.01.18
 * http://waiting.javaeye.com/
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 * 
 * update 2010.05.04
 *		add double u3000 SPACE for search:odata to fix SEARCH box display err when narrow width from only use of eq/ne/cn/in/lt/gt operator under IE6/7
**/
$.jgrid = {
	defaults : {
		recordtext: "{0} - {1}\u3000å…± {2} æ¡",	// å…±å­—å‰æ˜¯å…¨è§’ç©ºæ ¼
		emptyrecords: "æ— æ•°æ®æ˜¾ç¤º",
		loadtext: "è¯»å–ä¸­...",
		pgtext : " {0} å…± {1} é¡µ"
	},
	search : {
		caption: "æœç´¢...",
		Find: "æŸ¥æ‰¾",
		Reset: "é‡ç½®",
		odata : ['ç­‰äºŽ\u3000\u3000', 'ä¸ç­‰\u3000\u3000', 'å°äºŽ\u3000\u3000', 'å°äºŽç­‰äºŽ','å¤§äºŽ\u3000\u3000','å¤§äºŽç­‰äºŽ', 
			'å¼€å§‹äºŽ','ä¸å¼€å§‹äºŽ','å±žäºŽ\u3000\u3000','ä¸å±žäºŽ','ç»“æŸäºŽ','ä¸ç»“æŸäºŽ','åŒ…å«\u3000\u3000','ä¸åŒ…å«'],
		groupOps: [	{ op: "AND", text: "æ‰€æœ‰" },	{ op: "OR",  text: "ä»»ä¸€" }	],
		matchText: " åŒ¹é…",
		rulesText: " è§„åˆ™"
	},
	edit : {
		addCaption: "æ·»åŠ è®°å½•",
		editCaption: "ç¼–è¾‘è®°å½•",
		bSubmit: "æäº¤",
		bCancel: "å–æ¶ˆ",
		bClose: "å…³é—­",
		saveData: "æ•°æ®å·²æ”¹å˜ï¼Œæ˜¯å¦ä¿å­˜ï¼Ÿ",
		bYes : "æ˜¯",
		bNo : "å¦",
		bExit : "å–æ¶ˆ",
		msg: {
			required:"æ­¤å­—æ®µå¿…éœ€",
			number:"è¯·è¾“å…¥æœ‰æ•ˆæ•°å­—",
			minValue:"è¾“å€¼å¿…é¡»å¤§äºŽç­‰äºŽ ",
			maxValue:"è¾“å€¼å¿…é¡»å°äºŽç­‰äºŽ ",
			email: "è¿™ä¸æ˜¯æœ‰æ•ˆçš„e-mailåœ°å€",
			integer: "è¯·è¾“å…¥æœ‰æ•ˆæ•´æ•°",
			date: "è¯·è¾“å…¥æœ‰æ•ˆæ—¶é—´",
			url: "æ— æ•ˆç½‘å€ã€‚å‰ç¼€å¿…é¡»ä¸º ('http://' æˆ– 'https://')",
			nodefined : " æœªå®šä¹‰ï¼",
			novalue : " éœ€è¦è¿”å›žå€¼ï¼",
			customarray : "è‡ªå®šä¹‰å‡½æ•°éœ€è¦è¿”å›žæ•°ç»„ï¼",
			customfcheck : "Custom function should be present in case of custom checking!"
			
		}
	},
	view : {
		caption: "æŸ¥çœ‹è®°å½•",
		bClose: "å…³é—­"
	},
	del : {
		caption: "åˆ é™¤",
		msg: "åˆ é™¤æ‰€é€‰è®°å½•ï¼Ÿ",
		bSubmit: "åˆ é™¤",
		bCancel: "å–æ¶ˆ"
	},
	nav : {
		edittext: "",
		edittitle: "ç¼–è¾‘æ‰€é€‰è®°å½•",
		addtext:"",
		addtitle: "æ·»åŠ æ–°è®°å½•",
		deltext: "",
		deltitle: "åˆ é™¤æ‰€é€‰è®°å½•",
		searchtext: "",
		searchtitle: "æŸ¥æ‰¾",
		refreshtext: "",
		refreshtitle: "åˆ·æ–°è¡¨æ ¼",
		alertcap: "æ³¨æ„",
		alerttext: "è¯·é€‰æ‹©è®°å½•",
		viewtext: "",
		viewtitle: "æŸ¥çœ‹æ‰€é€‰è®°å½•"
	},
	col : {
		caption: "é€‰æ‹©åˆ—",
		bSubmit: "ç¡®å®š",
		bCancel: "å–æ¶ˆ"
	},
	errors : {
		errcap : "é”™è¯¯",
		nourl : "æ²¡æœ‰è®¾ç½®url",
		norecords: "æ²¡æœ‰è¦å¤„ç†çš„è®°å½•",
		model : "colNames å’Œ colModel é•¿åº¦ä¸ç­‰ï¼"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Sun", "Mon", "Tue", "Wed", "Thr", "Fri", "Sat",
		         "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
			],
			monthNames: [
				"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
				"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
			srcformat: 'Y-m-d',
			newformat: 'm-d-Y',
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "Y/j/n",
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
