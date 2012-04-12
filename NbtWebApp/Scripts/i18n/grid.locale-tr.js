;(function($){
/**
 * jqGrid Turkish Translation
 * Erhan GÃ¼ndoÄŸan (erhan@trposta.net)
 * http://blog.zakkum.com
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "{0}-{1} listeleniyor. Toplam:{2}",
	    emptyrecords: "KayÄ±t bulunamadÄ±",
		loadtext: "YÃ¼kleniyor...",
		pgtext : "{0}/{1}. Sayfa"
	},
	search : {
	    caption: "Arama...",
	    Find: "Bul",
	    Reset: "Temizle",	    
	    odata : ['eÅŸit', 'eÅŸit deÄŸil', 'daha az', 'daha az veya eÅŸit', 'daha fazla', 'daha fazla veya eÅŸit', 'ile baÅŸlayan', 'ile baÅŸlamayan', 'iÃ§inde', 'iÃ§inde deÄŸil', 'ile biten', 'ile bitmeyen', 'iÃ§eren', 'iÃ§ermeyen'],
	    groupOps: [	{ op: "VE", text: "tÃ¼m" },	{ op: "VEYA",  text: "herhangi" }	],
		matchText: " uyan",
		rulesText: " kurallar"
	},
	edit : {
	    addCaption: "KayÄ±t Ekle",
	    editCaption: "KayÄ±t DÃ¼zenle",
	    bSubmit: "GÃ¶nder",
	    bCancel: "Ä°ptal",
		bClose: "Kapat",
		saveData: "Veriler deÄŸiÅŸti! KayÄ±t edilsin mi?",
		bYes : "Evet",
		bNo : "HayÄ±t",
		bExit : "Ä°ptal",
	    msg: {
	        required:"Alan gerekli",
	        number:"LÃ¼tfen bir numara giriniz",
	        minValue:"girilen deÄŸer daha bÃ¼yÃ¼k ya da buna eÅŸit olmalÄ±dÄ±r",
	        maxValue:"girilen deÄŸer daha kÃ¼Ã§Ã¼k ya da buna eÅŸit olmalÄ±dÄ±r",
	        email: "geÃ§erli bir e-posta adresi deÄŸildir",
	        integer: "LÃ¼tfen bir tamsayÄ± giriniz",
			url: "GeÃ§erli bir URL deÄŸil. ('http://' or 'https://') Ã¶n eki gerekli.",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
		}
	},
	view : {
	    caption: "KayÄ±t GÃ¶rÃ¼ntÃ¼le",
	    bClose: "Kapat"
	},
	del : {
	    caption: "Sil",
	    msg: "SeÃ§ilen kayÄ±tlar silinsin mi?",
	    bSubmit: "Sil",
	    bCancel: "Ä°ptal"
	},
	nav : {
		edittext: " ",
	    edittitle: "SeÃ§ili satÄ±rÄ± dÃ¼zenle",
		addtext:" ",
	    addtitle: "Yeni satÄ±r ekle",
	    deltext: " ",
	    deltitle: "SeÃ§ili satÄ±rÄ± sil",
	    searchtext: " ",
	    searchtitle: "KayÄ±tlarÄ± bul",
	    refreshtext: "",
	    refreshtitle: "Tabloyu yenile",
	    alertcap: "UyarÄ±",
	    alerttext: "LÃ¼tfen bir satÄ±r seÃ§iniz",
		viewtext: "",
		viewtitle: "SeÃ§ilen satÄ±rÄ± gÃ¶rÃ¼ntÃ¼le"
	},
	col : {
	    caption: "SÃ¼tunlarÄ± gÃ¶ster/gizle",
	    bSubmit: "GÃ¶nder",
	    bCancel: "Ä°ptal"	
	},
	errors : {
		errcap : "Hata",
		nourl : "Bir url yapÄ±landÄ±rÄ±lmamÄ±ÅŸ",
		norecords: "Ä°ÅŸlem yapÄ±lacak bir kayÄ±t yok",
	    model : "colNames uzunluÄŸu <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Paz", "Pts", "Sal", "Ã‡ar", "Per", "Cum", "Cts",
				"Pazar", "Pazartesi", "SalÄ±", "Ã‡arÅŸamba", "PerÅŸembe", "Cuma", "Cumartesi"
			],
			monthNames: [
				"Oca", "Åžub", "Mar", "Nis", "May", "Haz", "Tem", "AÄŸu", "Eyl", "Eki", "Kas", "Ara",
				"Ocak", "Åžubat", "Mart", "Nisan", "MayÄ±s", "Haziran", "Temmuz", "AÄŸustos", "EylÃ¼l", "Ekim", "KasÄ±m", "AralÄ±k"
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
