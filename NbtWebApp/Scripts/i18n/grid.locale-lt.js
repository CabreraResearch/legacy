;(function($){
/**
 * jqGrid Lithuanian Translation
 * aur1mas aur1mas@devnet.lt
 * http://aur1mas.devnet.lt
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "PerÅ¾iÅ«rima {0} - {1} iÅ¡ {2}",
		emptyrecords: "Ä®raÅ¡Å³ nÄ—ra",
		loadtext: "Kraunama...",
		pgtext : "Puslapis {0} iÅ¡ {1}"
	},
	search : {
		caption: "PaieÅ¡ka...",
		Find: "IeÅ¡koti",
		Reset: "Atstatyti",
		odata : ['lygu', 'nelygu', 'maÅ¾iau', 'maÅ¾iau arba lygu','daugiau','daugiau arba lygu', 'prasideda','neprasideda','reikÅ¡mÄ— yra','reikÅ¡mÄ—s nÄ—ra','baigiasi','nesibaigia','yra sudarytas','nÄ—ra sudarytas'],
		groupOps: [	{ op: "AND", text: "visi" },	{ op: "OR",  text: "bet kuris" }	],
		matchText: " match",
		rulesText: " rules"
	},
	edit : {
		addCaption: "Sukurti Ä¯raÅ¡Ä…",
		editCaption: "Redaguoti Ä¯raÅ¡Ä…",
		bSubmit: "IÅ¡saugoti",
		bCancel: "AtÅ¡aukti",
		bClose: "UÅ¾daryti",
		saveData: "Duomenys buvo pakeisti! IÅ¡saugoti pakeitimus?",
		bYes : "Taip",
		bNo : "Ne",
		bExit : "AtÅ¡aukti",
		msg: {
			required:"Privalomas laukas",
			number:"Ä®veskite tinkamÄ… numerÄ¯",
			minValue:"reikÅ¡mÄ— turi bÅ«ti didesnÄ— arba lygi ",
			maxValue:"reikÅ¡mÄ— turi bÅ«ti maÅ¾esnÄ— arba lygi",
			email: "neteisingas el. paÅ¡to adresas",
			integer: "Ä®veskite teisingÄ… sveikÄ…jÄ¯ skaiÄiÅ³",
			date: "Ä®veskite teisingÄ… datÄ…",
			url: "blogas adresas. NepamirÅ¡kite pridÄ—ti ('http://' arba 'https://')",
			nodefined : " nÄ—ra apibrÄ—Å¾ta!",
			novalue : " turi bÅ«ti graÅ¾inama kokia nors reikÅ¡mÄ—!",
			customarray : "Custom f-ja turi grÄ…Å¾inti masyvÄ…!",
			customfcheck : "Custom f-ja tÅ«rÄ—tÅ³ bÅ«ti sukurta, prieÅ¡ bandant jÄ… naudoti!"
			
		}
	},
	view : {
		caption: "PerÅ¾iÅ«rÄ—ti Ä¯raÅ¡us",
		bClose: "UÅ¾daryti"
	},
	del : {
		caption: "IÅ¡trinti",
		msg: "IÅ¡trinti paÅ¾ymÄ—tus Ä¯raÅ¡us(-Ä…)?",
		bSubmit: "IÅ¡trinti",
		bCancel: "AtÅ¡aukti"
	},
	nav : {
		edittext: "",
		edittitle: "Redaguoti paÅ¾ymÄ—tÄ… eilutÄ™",
		addtext:"",
		addtitle: "PridÄ—ti naujÄ… eilutÄ™",
		deltext: "",
		deltitle: "IÅ¡trinti paÅ¾ymÄ—tÄ… eilutÄ™",
		searchtext: "",
		searchtitle: "Rasti Ä¯raÅ¡us",
		refreshtext: "",
		refreshtitle: "Perkrauti lentelÄ™",
		alertcap: "Ä®spÄ—jimas",
		alerttext: "Pasirinkite eilutÄ™",
		viewtext: "",
		viewtitle: "PerÅ¾iÅ«rÄ—ti pasirinktÄ… eilutÄ™"
	},
	col : {
		caption: "Pasirinkti stulpelius",
		bSubmit: "Gerai",
		bCancel: "AtÅ¡aukti"
	},
	errors : {
		errcap : "Klaida",
		nourl : "Url reikÅ¡mÄ— turi bÅ«ti perduota",
		norecords: "NÄ—ra Ä¯raÅ¡Å³, kuriuos bÅ«tÅ³ galima apdoroti",
		model : "colNames skaiÄius <> colModel skaiÄiui!"
	},
	formatter : {
		integer : {thousandsSeparator: "", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: "", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:",", thousandsSeparator: "", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Sek", "Pir", "Ant", "Tre", "Ket", "Pen", "Å eÅ¡",
				"Sekmadienis", "Pirmadienis", "Antradienis", "TreÄiadienis", "Ketvirtadienis", "Penktadienis", "Å eÅ¡tadienis"
			],
			monthNames: [
				"Sau", "Vas", "Kov", "Bal", "Geg", "Bir", "Lie", "Rugj", "Rugs", "Spa", "Lap", "Gru",
				"Sausis", "Vasaris", "Kovas", "Balandis", "GeguÅ¾Ä—", "BirÅ¾elis", "Liepa", "RugpjÅ«tis", "RugsÄ—jis", "Spalis", "Lapkritis", "Gruodis"
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
