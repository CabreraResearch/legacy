;(function($){
/**
 * jqGrid Icelandic Translation
 * jtm@hi.is Univercity of Iceland
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "View {0} - {1} of {2}",
	    emptyrecords: "No records to view",
		loadtext: "HleÃ°ur...",
		pgtext : "Page {0} of {1}"
	},
	search : {
	    caption: "Leita...",
	    Find: "Leita",
	    Reset: "Endursetja",
	    odata : ['equal', 'not equal', 'less', 'less or equal','greater','greater or equal', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
	    groupOps: [	{ op: "AND", text: "all" },	{ op: "OR",  text: "any" }	],
		matchText: " match",
		rulesText: " rules"
	},
	edit : {
	    addCaption: "Add Record",
	    editCaption: "Edit Record",
	    bSubmit: "Vista",
	    bCancel: "HÃ¦tta viÃ°",
		bClose: "Loka",
		saveData: "Data has been changed! Save changes?",
		bYes : "Yes",
		bNo : "No",
		bExit : "Cancel",
	    msg: {
	        required:"Reitur er nauÃ°synlegur",
	        number:"Vinsamlega settu inn tÃ¶lu",
	        minValue:"gildi verÃ°ur aÃ° vera meira en eÃ°a jafnt og ",
	        maxValue:"gildi verÃ°ur aÃ° vera minna en eÃ°a jafnt og ",
	        email: "er ekki lÃ¶glegt email",
	        integer: "Vinsamlega settu inn tÃ¶lu",
			date: "Please, enter valid date value",
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
	    caption: "EyÃ°a",
	    msg: "EyÃ°a vÃ¶ldum fÃ¦rslum ?",
	    bSubmit: "EyÃ°a",
	    bCancel: "HÃ¦tta viÃ°"
	},
	nav : {
		edittext: " ",
	    edittitle: "Breyta fÃ¦rslu",
		addtext:" ",
	    addtitle: "NÃ½ fÃ¦rsla",
	    deltext: " ",
	    deltitle: "EyÃ°a fÃ¦rslu",
	    searchtext: " ",
	    searchtitle: "Leita",
	    refreshtext: "",
	    refreshtitle: "EndurhlaÃ°a",
	    alertcap: "ViÃ°vÃ¶run",
	    alerttext: "Vinsamlega veldu fÃ¦rslu",
		viewtext: "",
		viewtitle: "View selected row"
	},
	col : {
	    caption: "SÃ½na / fela dÃ¡lka",
	    bSubmit: "Vista",
	    bCancel: "HÃ¦tta viÃ°"	
	},
	errors : {
		errcap : "Villa",
		nourl : "Vantar slÃ³Ã°",
		norecords: "Engar fÃ¦rslur valdar",
	    model : "Length of colNames <> colModel!"
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
