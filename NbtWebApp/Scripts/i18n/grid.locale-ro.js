;(function($){
/**
 * jqGrid Romanian Translation
 * Alexandru Emil Lupu contact@alecslupu.ro
 * http://www.alecslupu.ro/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "Vizualizare {0} - {1} din {2}",
		emptyrecords: "Nu existÄƒ Ã®nregistrÄƒri de vizualizat",
		loadtext: "ÃŽncÄƒrcare...",
		pgtext : "Pagina {0} din {1}"
	},
	search : {
		caption: "CautÄƒ...",
		Find: "CautÄƒ",
		Reset: "Resetare",
		odata : ['egal', 'diferit', 'mai mic', 'mai mic sau egal','mai mare','mai mare sau egal', 'Ã®ncepe cu','nu Ã®ncepe cu','se gÄƒseÈ™te Ã®n','nu se gÄƒseÈ™te Ã®n','se terminÄƒ cu','nu se terminÄƒ cu','conÈ›ine',''],
		groupOps: [	{ op: "AND", text: "toate" },	{ op: "OR",  text: "oricare" }	],
		matchText: " gÄƒsite",
		rulesText: " reguli"
	},
	edit : {
		addCaption: "AdÄƒugare Ã®nregistrare",
		editCaption: "Modificare Ã®nregistrare",
		bSubmit: "SalveazÄƒ",
		bCancel: "Anulare",
		bClose: "ÃŽnchide",
		saveData: "InformaÈ›iile au fost modificate! SalvaÈ›i modificÄƒrile?",
		bYes : "Da",
		bNo : "Nu",
		bExit : "Anulare",
		msg: {
			required:"CÃ¢mpul este obligatoriu",
			number:"VÄƒ rugÄƒm introduceÈ›i un numÄƒr valid",
			minValue:"valoarea trebuie sa fie mai mare sau egalÄƒ cu",
			maxValue:"valoarea trebuie sa fie mai micÄƒ sau egalÄƒ cu",
			email: "nu este o adresÄƒ de e-mail validÄƒ",
			integer: "VÄƒ rugÄƒm introduceÈ›i un numÄƒr valid",
			date: "VÄƒ rugÄƒm sÄƒ introduceÈ›i o datÄƒ validÄƒ",
			url: "Nu este un URL valid. Prefixul  este necesar('http://' or 'https://')",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
		caption: "Vizualizare Ã®nregistrare",
		bClose: "ÃŽnchidere"
	},
	del : {
		caption: "È˜tegere",
		msg: "È˜tergeÈ›i Ã®nregistrarea (Ã®nregistrÄƒrile) selectate?",
		bSubmit: "È˜terge",
		bCancel: "Anulare"
	},
	nav : {
		edittext: "",
		edittitle: "ModificÄƒ rÃ¢ndul selectat",
		addtext:"",
		addtitle: "AdaugÄƒ rÃ¢nd nou",
		deltext: "",
		deltitle: "È˜terge rÃ¢ndul selectat",
		searchtext: "",
		searchtitle: "CÄƒutare Ã®nregistrÄƒri",
		refreshtext: "",
		refreshtitle: "ReÃ®ncarcare Grid",
		alertcap: "Avertisment",
		alerttext: "VÄƒ rugÄƒm sÄƒ selectaÈ›i un rÃ¢nd",
		viewtext: "",
		viewtitle: "VizualizeazÄƒ rÃ¢ndul selectat"
	},
	col : {
		caption: "AratÄƒ/Ascunde coloanele",
		bSubmit: "SalveazÄƒ",
		bCancel: "Anulare"
	},
	errors : {
		errcap : "Eroare",
		nourl : "Niciun url nu este setat",
		norecords: "Nu sunt Ã®nregistrÄƒri de procesat",
		model : "Lungimea colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Dum", "Lun", "Mar", "Mie", "Joi", "Vin", "SÃ¢m",
				"DuminicÄƒ", "Luni", "MarÈ›i", "Miercuri", "Joi", "Vineri", "SÃ¢mbÄƒtÄƒ"
			],
			monthNames: [
				"Ian", "Feb", "Mar", "Apr", "Mai", "Iun", "Iul", "Aug", "Sep", "Oct", "Noi", "Dec",
				"Ianuarie", "Februarie", "Martie", "Aprilie", "Mai", "Iunie", "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie"
			],
			AmPm : ["am","pm","AM","PM"],
			/*
			 Here is a problem in romanian: 
					M	/	F
			 1st = primul / prima
			 2nd = Al doilea / A doua
			 3rd = Al treilea / A treia 
			 4th = Al patrulea/ A patra
			 5th = Al cincilea / A cincea 
			 6th = Al È™aselea / A È™asea
			 7th = Al È™aptelea / A È™aptea
			 .... 
			 */
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
