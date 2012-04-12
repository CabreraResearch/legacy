;(function($){
/**
 * jqGrid Greek (el) Translation
 * Alex Cicovic
 * http://www.alexcicovic.com
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "View {0} - {1} of {2}",
	    emptyrecords: "No records to view",
		loadtext: "Î¦ÏŒÏÏ„Ï‰ÏƒÎ·...",
		pgtext : "Page {0} of {1}"
	},
	search : {
	    caption: "Î‘Î½Î±Î¶Î®Ï„Î·ÏƒÎ·...",
	    Find: "Î•ÏÏÎµÏƒÎ·",
	    Reset: "Î•Ï€Î±Î½Î±Ï†Î¿ÏÎ¬",
	    odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
	    groupOps: [	{ op: "AND", text: "all" },	{ op: "OR",  text: "any" }	],
		matchText: " match",
		rulesText: " rules"
	},
	edit : {
	    addCaption: "Î•Î¹ÏƒÎ±Î³Ï‰Î³Î® Î•Î³Î³ÏÎ±Ï†Î®Ï‚",
	    editCaption: "Î•Ï€ÎµÎ¾ÎµÏÎ³Î±ÏƒÎ¯Î± Î•Î³Î³ÏÎ±Ï†Î®Ï‚",
	    bSubmit: "ÎšÎ±Ï„Î±Ï‡ÏŽÏÎ·ÏƒÎ·",
	    bCancel: "Î†ÎºÏ…ÏÎ¿",
		bClose: "ÎšÎ»ÎµÎ¯ÏƒÎ¹Î¼Î¿",
		saveData: "Data has been changed! Save changes?",
		bYes : "Yes",
		bNo : "No",
		bExit : "Cancel",
	    msg: {
	        required:"Î¤Î¿ Ï€ÎµÎ´Î¯Î¿ ÎµÎ¯Î½Î±Î¹ Î±Ï€Î±ÏÎ±Î¯Ï„Î·Ï„Î¿",
	        number:"Î¤Î¿ Ï€ÎµÎ´Î¯Î¿ Î´Î­Ï‡ÎµÏ„Î±Î¹ Î¼ÏŒÎ½Î¿ Î±ÏÎ¹Î¸Î¼Î¿ÏÏ‚",
	        minValue:"Î— Ï„Î¹Î¼Î® Ï€ÏÎ­Ï€ÎµÎ¹ Î½Î± ÎµÎ¯Î½Î±Î¹ Î¼ÎµÎ³Î±Î»ÏÏ„ÎµÏÎ· Î® Î¯ÏƒÎ· Ï„Î¿Ï… ",
	        maxValue:"Î— Ï„Î¹Î¼Î® Ï€ÏÎ­Ï€ÎµÎ¹ Î½Î± ÎµÎ¯Î½Î±Î¹ Î¼Î¹ÎºÏÏŒÏ„ÎµÏÎ· Î® Î¯ÏƒÎ· Ï„Î¿Ï… ",
	        email: "Î— Î´Î¹ÎµÏÎ¸Ï…Î½ÏƒÎ· e-mail Î´ÎµÎ½ ÎµÎ¯Î½Î±Î¹ Î­Î³ÎºÏ…ÏÎ·",
	        integer: "Î¤Î¿ Ï€ÎµÎ´Î¯Î¿ Î´Î­Ï‡ÎµÏ„Î±Î¹ Î¼ÏŒÎ½Î¿ Î±ÎºÎ­ÏÎ±Î¹Î¿Ï…Ï‚ Î±ÏÎ¹Î¸Î¼Î¿ÏÏ‚",
			url: "is not a valid URL. Prefix required ('http://' or 'https://')",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
	    caption: "View Record",
	    bClose: "Close"
	},
	del : {
	    caption: "Î”Î¹Î±Î³ÏÎ±Ï†Î®",
	    msg: "Î”Î¹Î±Î³ÏÎ±Ï†Î® Ï„Ï‰Î½ ÎµÏ€Î¹Î»ÎµÎ³Î¼Î­Î½Ï‰Î½ ÎµÎ³Î³ÏÎ±Ï†ÏŽÎ½;",
	    bSubmit: "ÎÎ±Î¹",
	    bCancel: "Î†ÎºÏ…ÏÎ¿"
	},
	nav : {
		edittext: " ",
	    edittitle: "Î•Ï€ÎµÎ¾ÎµÏÎ³Î±ÏƒÎ¯Î± ÎµÏ€Î¹Î»ÎµÎ³Î¼Î­Î½Î·Ï‚ ÎµÎ³Î³ÏÎ±Ï†Î®Ï‚",
		addtext:" ",
	    addtitle: "Î•Î¹ÏƒÎ±Î³Ï‰Î³Î® Î½Î­Î±Ï‚ ÎµÎ³Î³ÏÎ±Ï†Î®Ï‚",
	    deltext: " ",
	    deltitle: "Î”Î¹Î±Î³ÏÎ±Ï†Î® ÎµÏ€Î¹Î»ÎµÎ³Î¼Î­Î½Î·Ï‚ ÎµÎ³Î³ÏÎ±Ï†Î®Ï‚",
	    searchtext: " ",
	    searchtitle: "Î•ÏÏÎµÏƒÎ· Î•Î³Î³ÏÎ±Ï†ÏŽÎ½",
	    refreshtext: "",
	    refreshtitle: "Î‘Î½Î±Î½Î­Ï‰ÏƒÎ· Î Î¯Î½Î±ÎºÎ±",
	    alertcap: "Î ÏÎ¿ÏƒÎ¿Ï‡Î®",
	    alerttext: "Î”ÎµÎ½ Î­Ï‡ÎµÏ„Îµ ÎµÏ€Î¹Î»Î­Î¾ÎµÎ¹ ÎµÎ³Î³ÏÎ±Ï†Î®",
		viewtext: "",
		viewtitle: "View selected row"
	},
	col : {
	    caption: "Î•Î¼Ï†Î¬Î½Î¹ÏƒÎ· / Î‘Ï€ÏŒÎºÏÏ…ÏˆÎ· Î£Ï„Î·Î»ÏŽÎ½",
	    bSubmit: "ÎŸÎš",
	    bCancel: "Î†ÎºÏ…ÏÎ¿"
	},
	errors : {
		errcap : "Î£Ï†Î¬Î»Î¼Î±",
		nourl : "Î”ÎµÎ½ Î­Ï‡ÎµÎ¹ Î´Î¿Î¸ÎµÎ¯ Î´Î¹ÎµÏÎ¸Ï…Î½ÏƒÎ· Ï‡ÎµÎ¹ÏÎ¹ÏƒÎ¼Î¿Ï Î³Î¹Î± Ï„Î· ÏƒÏ…Î³ÎºÎµÎºÏÎ¹Î¼Î­Î½Î· ÎµÎ½Î­ÏÎ³ÎµÎ¹Î±",
		norecords: "Î”ÎµÎ½ Ï…Ï€Î¬ÏÏ‡Î¿Ï…Î½ ÎµÎ³Î³ÏÎ±Ï†Î­Ï‚ Ï€ÏÎ¿Ï‚ ÎµÏ€ÎµÎ¾ÎµÏÎ³Î±ÏƒÎ¯Î±",
		model : "Î†Î½Î¹ÏƒÎ¿Ï‚ Î±ÏÎ¹Î¸Î¼ÏŒÏ‚ Ï€ÎµÎ´Î¯Ï‰Î½ colNames/colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"ÎšÏ…Ï", "Î”ÎµÏ…", "Î¤ÏÎ¹", "Î¤ÎµÏ„", "Î ÎµÎ¼", "Î Î±Ï", "Î£Î±Î²",
				"ÎšÏ…ÏÎ¹Î±ÎºÎ®", "Î”ÎµÏ…Ï„Î­ÏÎ±", "Î¤ÏÎ¯Ï„Î·", "Î¤ÎµÏ„Î¬ÏÏ„Î·", "Î Î­Î¼Ï€Ï„Î·", "Î Î±ÏÎ±ÏƒÎºÎµÏ…Î®", "Î£Î¬Î²Î²Î±Ï„Î¿"
			],
			monthNames: [
				"Î™Î±Î½", "Î¦ÎµÎ²", "ÎœÎ±Ï", "Î‘Ï€Ï", "ÎœÎ±Î¹", "Î™Î¿Ï…Î½", "Î™Î¿Ï…Î»", "Î‘Ï…Î³", "Î£ÎµÏ€", "ÎŸÎºÏ„", "ÎÎ¿Îµ", "Î”ÎµÎº",
				"Î™Î±Î½Î¿Ï…Î¬ÏÎ¹Î¿Ï‚", "Î¦ÎµÎ²ÏÎ¿Ï…Î¬ÏÎ¹Î¿Ï‚", "ÎœÎ¬ÏÏ„Î¹Î¿Ï‚", "Î‘Ï€ÏÎ¯Î»Î¹Î¿Ï‚", "ÎœÎ¬Î¹Î¿Ï‚", "Î™Î¿ÏÎ½Î¹Î¿Ï‚", "Î™Î¿ÏÎ»Î¹Î¿Ï‚", "Î‘ÏÎ³Î¿Ï…ÏƒÏ„Î¿Ï‚", "Î£ÎµÏ€Ï„Î­Î¼Î²ÏÎ¹Î¿Ï‚", "ÎŸÎºÏ„ÏŽÎ²ÏÎ¹Î¿Ï‚", "ÎÎ¿Î­Î¼Î²ÏÎ¹Î¿Ï‚", "Î”ÎµÎºÎ­Î¼Î²ÏÎ¹Î¿Ï‚"
			],
			AmPm : ["Ï€Î¼","Î¼Î¼","Î Îœ","ÎœÎœ"],
			S: function (j) {return j == 1 || j > 1 ? ['Î·'][Math.min((j - 1) % 10, 3)] : ''},
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
