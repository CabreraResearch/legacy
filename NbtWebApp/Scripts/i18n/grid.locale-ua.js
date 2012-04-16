;(function($){
/**
 * jqGrid Ukrainian Translation v1.0 02.07.2009
 * Sergey Dyagovchenko
 * http://d.sumy.ua
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "ÐŸÐµÑ€ÐµÐ³Ð»ÑÐ´ {0} - {1} Ð· {2}",
	  emptyrecords: "ÐÐµÐ¼Ð°Ñ” Ð·Ð°Ð¿Ð¸ÑÑ–Ð² Ð´Ð»Ñ Ð¿ÐµÑ€ÐµÐ³Ð»ÑÐ´Ñƒ",
		loadtext: "Ð—Ð°Ð²Ð°Ð½Ñ‚Ð°Ð¶ÐµÐ½Ð½Ñ...",
		pgtext : "Ð¡Ñ‚Ð¾Ñ€. {0} Ð· {1}"
	},
	search : {
    caption: "ÐŸÐ¾ÑˆÑƒÐº...",
    Find: "Ð—Ð½Ð°Ð¹Ñ‚Ð¸",
    Reset: "Ð¡ÐºÐ¸Ð´Ð°Ð½Ð½Ñ",
    odata : ['Ñ€Ñ–Ð²Ð½Ð¾', 'Ð½Ðµ Ñ€Ñ–Ð²Ð½Ð¾', 'Ð¼ÐµÐ½ÑˆÐµ', 'Ð¼ÐµÐ½ÑˆÐµ Ð°Ð±Ð¾ Ñ€Ñ–Ð²Ð½Ðµ','Ð±Ñ–Ð»ÑŒÑˆÐµ','Ð±Ñ–Ð»ÑŒÑˆÐµ Ð°Ð±Ð¾ Ñ€Ñ–Ð²Ð½Ðµ', 'Ð¿Ð¾Ñ‡Ð¸Ð½Ð°Ñ”Ñ‚ÑŒÑÑ Ð·','Ð½Ðµ Ð¿Ð¾Ñ‡Ð¸Ð½Ð°Ñ”Ñ‚ÑŒÑÑ Ð·','Ð·Ð½Ð°Ñ…Ð¾Ð´Ð¸Ñ‚ÑŒÑÑ Ð²','Ð½Ðµ Ð·Ð½Ð°Ñ…Ð¾Ð´Ð¸Ñ‚ÑŒÑÑ Ð²','Ð·Ð°ÐºÑ–Ð½Ñ‡ÑƒÑ”Ñ‚ÑŒÑÑ Ð½Ð°','Ð½Ðµ Ð·Ð°ÐºÑ–Ð½Ñ‡ÑƒÑ”Ñ‚ÑŒÑÑ Ð½Ð°','Ð¼Ñ–ÑÑ‚Ð¸Ñ‚ÑŒ','Ð½Ðµ Ð¼Ñ–ÑÑ‚Ð¸Ñ‚ÑŒ'],
    groupOps: [	{ op: "AND", text: "Ð²ÑÐµ" },	{ op: "OR",  text: "Ð±ÑƒÐ´ÑŒ-ÑÐºÐ¸Ð¹" }	],
    matchText: " Ð·Ð±Ñ–Ð³Ð°Ñ”Ñ‚ÑŒÑÑ",
    rulesText: " Ð¿Ñ€Ð°Ð²Ð¸Ð»Ð°"
	},
	edit : {
    addCaption: "Ð”Ð¾Ð´Ð°Ñ‚Ð¸ Ð·Ð°Ð¿Ð¸Ñ",
    editCaption: "Ð—Ð¼Ñ–Ð½Ð¸Ñ‚Ð¸ Ð·Ð°Ð¿Ð¸Ñ",
    bSubmit: "Ð—Ð±ÐµÑ€ÐµÐ³Ñ‚Ð¸",
    bCancel: "Ð’Ñ–Ð´Ð¼Ñ–Ð½Ð°",
		bClose: "Ð—Ð°ÐºÑ€Ð¸Ñ‚Ð¸",
		saveData: "Ð”Ð¾ Ð´Ð°Ð½Ð½Ð¸Ñ… Ð±ÑƒÐ»Ð¸ Ð²Ð½ÐµÑÐµÐ½Ñ– Ð·Ð¼Ñ–Ð½Ð¸! Ð—Ð±ÐµÑ€ÐµÐ³Ñ‚Ð¸ Ð·Ð¼Ñ–Ð½Ð¸?",
		bYes : "Ð¢Ð°Ðº",
		bNo : "ÐÑ–",
		bExit : "Ð’Ñ–Ð´Ð¼Ñ–Ð½Ð°",
	    msg: {
        required:"ÐŸÐ¾Ð»Ðµ Ñ” Ð¾Ð±Ð¾Ð²'ÑÐ·ÐºÐ¾Ð²Ð¸Ð¼",
        number:"Ð‘ÑƒÐ´ÑŒ Ð»Ð°ÑÐºÐ°, Ð²Ð²ÐµÐ´Ñ–Ñ‚ÑŒ Ð¿Ñ€Ð°Ð²Ð¸Ð»ÑŒÐ½Ðµ Ñ‡Ð¸ÑÐ»Ð¾",
        minValue:"Ð·Ð½Ð°Ñ‡ÐµÐ½Ð½Ñ Ð¿Ð¾Ð²Ð¸Ð½Ð½Ðµ Ð±ÑƒÑ‚Ð¸ Ð±Ñ–Ð»ÑŒÑˆÐµ Ð°Ð±Ð¾ Ð´Ð¾Ñ€Ñ–Ð²Ð½ÑŽÑ”",
        maxValue:"Ð·Ð½Ð°Ñ‡ÐµÐ½Ð½Ñ Ð¿Ð¾Ð²Ð¸Ð½Ð½Ð¾ Ð±ÑƒÑ‚Ð¸ Ð¼ÐµÐ½ÑˆÐµ Ð°Ð±Ð¾ Ð´Ð¾Ñ€Ñ–Ð²Ð½ÑŽÑ”",
        email: "Ð½ÐµÐºÐ¾Ñ€ÐµÐºÑ‚Ð½Ð° Ð°Ð´Ñ€ÐµÑÐ° ÐµÐ»ÐµÐºÑ‚Ñ€Ð¾Ð½Ð½Ð¾Ñ— Ð¿Ð¾ÑˆÑ‚Ð¸",
        integer: "Ð‘ÑƒÐ´ÑŒ Ð»Ð°ÑÐºÐ°, Ð²Ð²ÐµÐ´ÐµÐ½Ð½Ñ Ð´Ñ–Ð¹ÑÐ½Ðµ Ñ†Ñ–Ð»Ðµ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð½Ñ",
        date: "Ð‘ÑƒÐ´ÑŒ Ð»Ð°ÑÐºÐ°, Ð²Ð²ÐµÐ´ÐµÐ½Ð½Ñ Ð´Ñ–Ð¹ÑÐ½Ðµ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð½Ñ Ð´Ð°Ñ‚Ð¸",
        url: "Ð½Ðµ Ð´Ñ–Ð¹ÑÐ½Ð¸Ð¹ URL. ÐÐµÐ¾Ð±Ñ…Ñ–Ð´Ð½Ð° Ð¿Ñ€Ð¸ÑÑ‚Ð°Ð²ÐºÐ° ('http://' or 'https://')",
		nodefined : " is not defined!",
		novalue : " return value is required!",
		customarray : "Custom function should return array!",
		customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
	    caption: "ÐŸÐµÑ€ÐµÐ³Ð»ÑÐ½ÑƒÑ‚Ð¸ Ð·Ð°Ð¿Ð¸Ñ",
	    bClose: "Ð—Ð°ÐºÑ€Ð¸Ñ‚Ð¸"
	},
	del : {
	    caption: "Ð’Ð¸Ð´Ð°Ð»Ð¸Ñ‚Ð¸",
	    msg: "Ð’Ð¸Ð´Ð°Ð»Ð¸Ñ‚Ð¸ Ð¾Ð±Ñ€Ð°Ð½Ð¸Ð¹ Ð·Ð°Ð¿Ð¸Ñ(Ð¸)?",
	    bSubmit: "Ð’Ð¸Ð´Ð°Ð»Ð¸Ñ‚Ð¸",
	    bCancel: "Ð’Ñ–Ð´Ð¼Ñ–Ð½Ð°"
	},
	nav : {
  		edittext: " ",
	    edittitle: "Ð—Ð¼Ñ–Ð½Ð¸Ñ‚Ð¸ Ð²Ð¸Ð±Ñ€Ð°Ð½Ð¸Ð¹ Ð·Ð°Ð¿Ð¸Ñ",
  		addtext:" ",
	    addtitle: "Ð”Ð¾Ð´Ð°Ñ‚Ð¸ Ð½Ð¾Ð²Ð¸Ð¹ Ð·Ð°Ð¿Ð¸Ñ",
	    deltext: " ",
	    deltitle: "Ð’Ð¸Ð´Ð°Ð»Ð¸Ñ‚Ð¸ Ð²Ð¸Ð±Ñ€Ð°Ð½Ð¸Ð¹ Ð·Ð°Ð¿Ð¸Ñ",
	    searchtext: " ",
	    searchtitle: "Ð—Ð½Ð°Ð¹Ñ‚Ð¸ Ð·Ð°Ð¿Ð¸ÑÐ¸",
	    refreshtext: "",
	    refreshtitle: "ÐžÐ½Ð¾Ð²Ð¸Ñ‚Ð¸ Ñ‚Ð°Ð±Ð»Ð¸Ñ†ÑŽ",
	    alertcap: "ÐŸÐ¾Ð¿ÐµÑ€ÐµÐ´Ð¶ÐµÐ½Ð½Ñ",
	    alerttext: "Ð‘ÑƒÐ´ÑŒ Ð»Ð°ÑÐºÐ°, Ð²Ð¸Ð±ÐµÑ€Ñ–Ñ‚ÑŒ Ð·Ð°Ð¿Ð¸Ñ",
  		viewtext: "",
  		viewtitle: "ÐŸÐµÑ€ÐµÐ³Ð»ÑÐ½ÑƒÑ‚Ð¸ Ð¾Ð±Ñ€Ð°Ð½Ð¸Ð¹ Ð·Ð°Ð¿Ð¸Ñ"
	},
	col : {
	    caption: "ÐŸÐ¾ÐºÐ°Ð·Ð°Ñ‚Ð¸/ÐŸÑ€Ð¸Ñ…Ð¾Ð²Ð°Ñ‚Ð¸ ÑÑ‚Ð¾Ð²Ð¿Ñ†Ñ–",
	    bSubmit: "Ð—Ð±ÐµÑ€ÐµÐ³Ñ‚Ð¸",
	    bCancel: "Ð’Ñ–Ð´Ð¼Ñ–Ð½Ð°"
	},
	errors : {
		errcap : "ÐŸÐ¾Ð¼Ð¸Ð»ÐºÐ°",
		nourl : "URL Ð½Ðµ Ð·Ð°Ð´Ð°Ð½",
		norecords: "ÐÐµÐ¼Ð°Ñ” Ð·Ð°Ð¿Ð¸ÑÑ–Ð² Ð´Ð»Ñ Ð¾Ð±Ñ€Ð¾Ð±ÐºÐ¸",
    model : "Ð§Ð¸ÑÐ»Ð¾ Ð¿Ð¾Ð»Ñ–Ð² Ð½Ðµ Ð²Ñ–Ð´Ð¿Ð¾Ð²Ñ–Ð´Ð°Ñ” Ñ‡Ð¸ÑÐ»Ñƒ ÑÑ‚Ð¾Ð²Ð¿Ñ†Ñ–Ð² Ñ‚Ð°Ð±Ð»Ð¸Ñ†Ñ–!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"ÐÐ´", "ÐŸÐ½", "Ð’Ñ‚", "Ð¡Ñ€", "Ð§Ñ‚", "ÐŸÑ‚", "Ð¡Ð±",
				"ÐÐµÐ´Ñ–Ð»Ñ", "ÐŸÐ¾Ð½ÐµÐ´Ñ–Ð»Ð¾Ðº", "Ð’Ñ–Ð²Ñ‚Ð¾Ñ€Ð¾Ðº", "Ð¡ÐµÑ€ÐµÐ´Ð°", "Ð§ÐµÑ‚Ð²ÐµÑ€", "ÐŸ'ÑÑ‚Ð½Ð¸Ñ†Ñ", "Ð¡ÑƒÐ±Ð¾Ñ‚Ð°"
			],
			monthNames: [
				"Ð¡Ñ–Ñ‡", "Ð›ÑŽÑ‚", "Ð‘ÐµÑ€", "ÐšÐ²Ñ–", "Ð¢Ñ€Ð°", "Ð§ÐµÑ€", "Ð›Ð¸Ð¿", "Ð¡ÐµÑ€", "Ð’ÐµÑ€", "Ð–Ð¾Ð²", "Ð›Ð¸Ñ", "Ð“Ñ€Ñƒ",
				"Ð¡Ñ–Ñ‡ÐµÐ½ÑŒ", "Ð›ÑŽÑ‚Ð¸Ð¹", "Ð‘ÐµÑ€ÐµÐ·ÐµÐ½ÑŒ", "ÐšÐ²Ñ–Ñ‚ÐµÐ½ÑŒ", "Ð¢Ñ€Ð°Ð²ÐµÐ½ÑŒ", "Ð§ÐµÑ€Ð²ÐµÐ½ÑŒ", "Ð›Ð¸Ð¿ÐµÐ½ÑŒ", "Ð¡ÐµÑ€Ð¿ÐµÐ½ÑŒ", "Ð’ÐµÑ€ÐµÑÐµÐ½ÑŒ", "Ð–Ð¾Ð²Ñ‚ÐµÐ½ÑŒ", "Ð›Ð¸ÑÑ‚Ð¾Ð¿Ð°Ð´", "Ð“Ñ€ÑƒÐ´ÐµÐ½ÑŒ"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
			srcformat: 'Y-m-d',
			newformat: 'd.m.Y',
			masks : {
	            ISO8601Long:"Y-m-d H:i:s",
	            ISO8601Short:"Y-m-d",
	            ShortDate: "n.j.Y",
	            LongDate: "l, F d, Y",
	            FullDateTime: "l, F d, Y G:i:s",
	            MonthDay: "F d",
	            ShortTime: "G:i",
	            LongTime: "G:i:s",
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
