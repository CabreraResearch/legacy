;(function($){
/**
 * jqGrid Russian Translation v1.0 02.07.2009 (based on translation by Alexey Kanaev v1.1 21.01.2009, http://softcore.com.ru)
 * Sergey Dyagovchenko
 * http://d.sumy.ua
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "ÐŸÑ€Ð¾ÑÐ¼Ð¾Ñ‚Ñ€ {0} - {1} Ð¸Ð· {2}",
	  emptyrecords: "ÐÐµÑ‚ Ð·Ð°Ð¿Ð¸ÑÐµÐ¹ Ð´Ð»Ñ Ð¿Ñ€Ð¾ÑÐ¼Ð¾Ñ‚Ñ€Ð°",
		loadtext: "Ð—Ð°Ð³Ñ€ÑƒÐ·ÐºÐ°...",
		pgtext : "Ð¡Ñ‚Ñ€. {0} Ð¸Ð· {1}"
	},
	search : {
    caption: "ÐŸÐ¾Ð¸ÑÐº...",
    Find: "ÐÐ°Ð¹Ñ‚Ð¸",
    Reset: "Ð¡Ð±Ñ€Ð¾Ñ",
    odata : ['Ñ€Ð°Ð²Ð½Ð¾', 'Ð½Ðµ Ñ€Ð°Ð²Ð½Ð¾', 'Ð¼ÐµÐ½ÑŒÑˆÐµ', 'Ð¼ÐµÐ½ÑŒÑˆÐµ Ð¸Ð»Ð¸ Ñ€Ð°Ð²Ð½Ð¾','Ð±Ð¾Ð»ÑŒÑˆÐµ','Ð±Ð¾Ð»ÑŒÑˆÐµ Ð¸Ð»Ð¸ Ñ€Ð°Ð²Ð½Ð¾', 'Ð½Ð°Ñ‡Ð¸Ð½Ð°ÐµÑ‚ÑÑ Ñ','Ð½Ðµ Ð½Ð°Ñ‡Ð¸Ð½Ð°ÐµÑ‚ÑÑ Ñ','Ð½Ð°Ñ…Ð¾Ð´Ð¸Ñ‚ÑÑ Ð²','Ð½Ðµ Ð½Ð°Ñ…Ð¾Ð´Ð¸Ñ‚ÑÑ Ð²','Ð·Ð°ÐºÐ°Ð½Ñ‡Ð¸Ð²Ð°ÐµÑ‚ÑÑ Ð½Ð°','Ð½Ðµ Ð·Ð°ÐºÐ°Ð½Ñ‡Ð¸Ð²Ð°ÐµÑ‚ÑÑ Ð½Ð°','ÑÐ¾Ð´ÐµÑ€Ð¶Ð¸Ñ‚','Ð½Ðµ ÑÐ¾Ð´ÐµÑ€Ð¶Ð¸Ñ‚'],
    groupOps: [	{ op: "AND", text: "Ð²ÑÐµ" },	{ op: "OR",  text: "Ð»ÑŽÐ±Ð¾Ð¹" }	],
    matchText: " ÑÐ¾Ð²Ð¿Ð°Ð´Ð°ÐµÑ‚",
    rulesText: " Ð¿Ñ€Ð°Ð²Ð¸Ð»Ð°"
	},
	edit : {
    addCaption: "Ð”Ð¾Ð±Ð°Ð²Ð¸Ñ‚ÑŒ Ð·Ð°Ð¿Ð¸ÑÑŒ",
    editCaption: "Ð ÐµÐ´Ð°ÐºÑ‚Ð¸Ñ€Ð¾Ð²Ð°Ñ‚ÑŒ Ð·Ð°Ð¿Ð¸ÑÑŒ",
    bSubmit: "Ð¡Ð¾Ñ…Ñ€Ð°Ð½Ð¸Ñ‚ÑŒ",
    bCancel: "ÐžÑ‚Ð¼ÐµÐ½Ð°",
		bClose: "Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ",
		saveData: "Ð”Ð°Ð½Ð½Ñ‹Ðµ Ð±Ñ‹Ð»Ð¸ Ð¸Ð·Ð¼ÐµÐ½ÐµÐ½Ð½Ñ‹! Ð¡Ð¾Ñ…Ñ€Ð°Ð½Ð¸Ñ‚ÑŒ Ð¸Ð·Ð¼ÐµÐ½ÐµÐ½Ð¸Ñ?",
		bYes : "Ð”Ð°",
		bNo : "ÐÐµÑ‚",
		bExit : "ÐžÑ‚Ð¼ÐµÐ½Ð°",
	    msg: {
        required:"ÐŸÐ¾Ð»Ðµ ÑÐ²Ð»ÑÐµÑ‚ÑÑ Ð¾Ð±ÑÐ·Ð°Ñ‚ÐµÐ»ÑŒÐ½Ñ‹Ð¼",
        number:"ÐŸÐ¾Ð¶Ð°Ð»ÑƒÐ¹ÑÑ‚Ð°, Ð²Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ð¿Ñ€Ð°Ð²Ð¸Ð»ÑŒÐ½Ð¾Ðµ Ñ‡Ð¸ÑÐ»Ð¾",
        minValue:"Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ðµ Ð´Ð¾Ð»Ð¶Ð½Ð¾ Ð±Ñ‹Ñ‚ÑŒ Ð±Ð¾Ð»ÑŒÑˆÐµ Ð»Ð¸Ð±Ð¾ Ñ€Ð°Ð²Ð½Ð¾",
        maxValue:"Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ðµ Ð´Ð¾Ð»Ð¶Ð½Ð¾ Ð±Ñ‹Ñ‚ÑŒ Ð¼ÐµÐ½ÑŒÑˆÐµ Ð»Ð¸Ð±Ð¾ Ñ€Ð°Ð²Ð½Ð¾",
        email: "Ð½ÐµÐºÐ¾Ñ€Ñ€ÐµÐºÑ‚Ð½Ð¾Ðµ Ð·Ð½Ð°Ñ‡ÐµÐ½Ð¸Ðµ e-mail",
        integer: "ÐŸÐ¾Ð¶Ð°Ð»ÑƒÐ¹ÑÑ‚Ð°, Ð²Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ñ†ÐµÐ»Ð¾Ðµ Ñ‡Ð¸ÑÐ»Ð¾",
        date: "ÐŸÐ¾Ð¶Ð°Ð»ÑƒÐ¹ÑÑ‚Ð°, Ð²Ð²ÐµÐ´Ð¸Ñ‚Ðµ Ð¿Ñ€Ð°Ð²Ð¸Ð»ÑŒÐ½ÑƒÑŽ Ð´Ð°Ñ‚Ñƒ",
        url: "Ð½ÐµÐ²ÐµÑ€Ð½Ð°Ñ ÑÑÑ‹Ð»ÐºÐ°. ÐÐµÐ¾Ð±Ñ…Ð¾Ð´Ð¸Ð¼Ð¾ Ð²Ð²ÐµÑÑ‚Ð¸ Ð¿Ñ€ÐµÑ„Ð¸ÐºÑ ('http://' or 'https://')",
		nodefined : " is not defined!",
		novalue : " return value is required!",
		customarray : "Custom function should return array!",
		customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
	    caption: "ÐŸÑ€Ð¾ÑÐ¼Ð¾Ñ‚Ñ€ Ð·Ð°Ð¿Ð¸ÑÐ¸",
	    bClose: "Ð—Ð°ÐºÑ€Ñ‹Ñ‚ÑŒ"
	},
	del : {
	    caption: "Ð£Ð´Ð°Ð»Ð¸Ñ‚ÑŒ",
	    msg: "Ð£Ð´Ð°Ð»Ð¸Ñ‚ÑŒ Ð²Ñ‹Ð±Ñ€Ð°Ð½Ð½ÑƒÑŽ Ð·Ð°Ð¿Ð¸ÑÑŒ(Ð¸)?",
	    bSubmit: "Ð£Ð´Ð°Ð»Ð¸Ñ‚ÑŒ",
	    bCancel: "ÐžÑ‚Ð¼ÐµÐ½Ð°"
	},
	nav : {
  		edittext: " ",
	    edittitle: "Ð ÐµÐ´Ð°ÐºÑ‚Ð¸Ñ€Ð¾Ð²Ð°Ñ‚ÑŒ Ð²Ñ‹Ð±Ñ€Ð°Ð½Ð½ÑƒÑŽ Ð·Ð°Ð¿Ð¸ÑÑŒ",
  		addtext:" ",
	    addtitle: "Ð”Ð¾Ð±Ð°Ð²Ð¸Ñ‚ÑŒ Ð½Ð¾Ð²ÑƒÑŽ Ð·Ð°Ð¿Ð¸ÑÑŒ",
	    deltext: " ",
	    deltitle: "Ð£Ð´Ð°Ð»Ð¸Ñ‚ÑŒ Ð²Ñ‹Ð±Ñ€Ð°Ð½Ð½ÑƒÑŽ Ð·Ð°Ð¿Ð¸ÑÑŒ",
	    searchtext: " ",
	    searchtitle: "ÐÐ°Ð¹Ñ‚Ð¸ Ð·Ð°Ð¿Ð¸ÑÐ¸",
	    refreshtext: "",
	    refreshtitle: "ÐžÐ±Ð½Ð¾Ð²Ð¸Ñ‚ÑŒ Ñ‚Ð°Ð±Ð»Ð¸Ñ†Ñƒ",
	    alertcap: "Ð’Ð½Ð¸Ð¼Ð°Ð½Ð¸Ðµ",
	    alerttext: "ÐŸÐ¾Ð¶Ð°Ð»ÑƒÐ¹ÑÑ‚Ð°, Ð²Ñ‹Ð±ÐµÑ€Ð¸Ñ‚Ðµ Ð·Ð°Ð¿Ð¸ÑÑŒ",
  		viewtext: "",
  		viewtitle: "ÐŸÑ€Ð¾ÑÐ¼Ð¾Ñ‚Ñ€ÐµÑ‚ÑŒ Ð²Ñ‹Ð±Ñ€Ð°Ð½Ð½ÑƒÑŽ Ð·Ð°Ð¿Ð¸ÑÑŒ"
	},
	col : {
	    caption: "ÐŸÐ¾ÐºÐ°Ð·Ð°Ñ‚ÑŒ/ÑÐºÑ€Ñ‹Ñ‚ÑŒ ÑÑ‚Ð¾Ð»Ð±Ñ†Ñ‹",
	    bSubmit: "Ð¡Ð¾Ñ…Ñ€Ð°Ð½Ð¸Ñ‚ÑŒ",
	    bCancel: "ÐžÑ‚Ð¼ÐµÐ½Ð°"	
	},
	errors : {
		errcap : "ÐžÑˆÐ¸Ð±ÐºÐ°",
		nourl : "URL Ð½Ðµ ÑƒÑÑ‚Ð°Ð½Ð¾Ð²Ð»ÐµÐ½",
		norecords: "ÐÐµÑ‚ Ð·Ð°Ð¿Ð¸ÑÐµÐ¹ Ð´Ð»Ñ Ð¾Ð±Ñ€Ð°Ð±Ð¾Ñ‚ÐºÐ¸",
    model : "Ð§Ð¸ÑÐ»Ð¾ Ð¿Ð¾Ð»ÐµÐ¹ Ð½Ðµ ÑÐ¾Ð¾Ñ‚Ð²ÐµÑ‚ÑÑ‚Ð²ÑƒÐµÑ‚ Ñ‡Ð¸ÑÐ»Ñƒ ÑÑ‚Ð¾Ð»Ð±Ñ†Ð¾Ð² Ñ‚Ð°Ð±Ð»Ð¸Ñ†Ñ‹!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Ð’Ñ", "ÐŸÐ½", "Ð’Ñ‚", "Ð¡Ñ€", "Ð§Ñ‚", "ÐŸÑ‚", "Ð¡Ð±",
				"Ð’Ð¾ÑÐºÑ€ÐµÑÐµÐ½Ð¸Ðµ", "ÐŸÐ¾Ð½ÐµÐ´ÐµÐ»ÑŒÐ½Ð¸Ðº", "Ð’Ñ‚Ð¾Ñ€Ð½Ð¸Ðº", "Ð¡Ñ€ÐµÐ´Ð°", "Ð§ÐµÑ‚Ð²ÐµÑ€Ð³", "ÐŸÑÑ‚Ð½Ð¸Ñ†Ð°", "Ð¡ÑƒÐ±Ð±Ð¾Ñ‚Ð°"
			],
			monthNames: [
				"Ð¯Ð½Ð²", "Ð¤ÐµÐ²", "ÐœÐ°Ñ€", "ÐÐ¿Ñ€", "ÐœÐ°Ð¹", "Ð˜ÑŽÐ½", "Ð˜ÑŽÐ»", "ÐÐ²Ð³", "Ð¡ÐµÐ½", "ÐžÐºÑ‚", "ÐÐ¾Ñ", "Ð”ÐµÐº",
				"Ð¯Ð½Ð²Ð°Ñ€ÑŒ", "Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ", "ÐœÐ°Ñ€Ñ‚", "ÐÐ¿Ñ€ÐµÐ»ÑŒ", "ÐœÐ°Ð¹", "Ð˜ÑŽÐ½ÑŒ", "Ð˜ÑŽÐ»ÑŒ", "ÐÐ²Ð³ÑƒÑÑ‚", "Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ", "ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ", "ÐÐ¾ÑÐ±Ñ€ÑŒ", "Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ"
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
