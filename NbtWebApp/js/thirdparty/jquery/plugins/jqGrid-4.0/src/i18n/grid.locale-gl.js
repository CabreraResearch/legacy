;(function($){
/**
 * jqGrid Galician Translation
 * Translated by Jorge Barreiro <yortx.barry@gmail.com>
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "Amosando {0} - {1} de {2}",
	    emptyrecords: "Sen rexistros que amosar",
		loadtext: "Cargando...",
		pgtext : "PÃ¡xina {0} de {1}"
	},
	search : {
	    caption: "BÃºsqueda...",
	    Find: "Buscar",
	    Reset: "Limpar",
	    odata : ['igual ', 'diferente a', 'menor que', 'menor ou igual que','maior que','maior ou igual a', 'empece por','non empece por','estÃ¡ en','non estÃ¡ en','termina por','non termina por','contÃ©n','non contÃ©n'],
	    groupOps: [	{ op: "AND", text: "todo" },	{ op: "OR",  text: "calquera" }	],
		matchText: " match",
		rulesText: " regras"
	},
	edit : {
	    addCaption: "Engadir rexistro",
	    editCaption: "Modificar rexistro",
	    bSubmit: "Gardar",
	    bCancel: "Cancelar",
		bClose: "Pechar",
		saveData: "ModificÃ¡ronse os datos, quere gardar os cambios?",
		bYes : "Si",
		bNo : "Non",
		bExit : "Cancelar",
	    msg: {
	        required:"Campo obrigatorio",
	        number:"Introduza un nÃºmero",
	        minValue:"O valor debe ser maior ou igual a ",
	        maxValue:"O valor debe ser menor ou igual a ",
	        email: "non Ã© un enderezo de correo vÃ¡lido",
	        integer: "Introduza un valor enteiro",
			date: "Introduza unha data correcta ",
			url: "non Ã© unha URL vÃ¡lida. Prefixo requerido ('http://' ou 'https://')",
			nodefined : " non estÃ¡ definido.",
			novalue : " o valor de retorno Ã© obrigatorio.",
			customarray : "A funciÃ³n persoalizada debe devolver un array.",
			customfcheck : "A funciÃ³n persoalizada debe estar presente no caso de ter validaciÃ³n persoalizada."
		}
	},
	view : {
	    caption: "Consultar rexistro",
	    bClose: "Pechar"
	},
	del : {
	    caption: "Eliminar",
	    msg: "Desexa eliminar os rexistros seleccionados?",
	    bSubmit: "Eliminar",
	    bCancel: "Cancelar"
	},
	nav : {
		edittext: " ",
	    edittitle: "Modificar a fila seleccionada",
		addtext:" ",
	    addtitle: "Engadir unha nova fila",
	    deltext: " ",
	    deltitle: "Eliminar a fila seleccionada",
	    searchtext: " ",
	    searchtitle: "Buscar informaciÃ³n",
	    refreshtext: "",
	    refreshtitle: "Recargar datos",
	    alertcap: "Aviso",
	    alerttext: "Seleccione unha fila",
		viewtext: "",
		viewtitle: "Ver fila seleccionada"
	},
	col : {
	    caption: "Mostrar/ocultar columnas",
	    bSubmit: "Enviar",
	    bCancel: "Cancelar"	
	},
	errors : {
		errcap : "Erro",
		nourl : "Non especificou unha URL",
		norecords: "Non hai datos para procesar",
	    model : "As columnas de nomes son diferentes das columnas de modelo"
	},
	formatter : {
		integer : {thousandsSeparator: ".", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: ".", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: ".", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Do", "Lu", "Ma", "Me", "Xo", "Ve", "Sa",
				"Domingo", "Luns", "Martes", "MÃ©rcoles", "Xoves", "Vernes", "SÃ¡bado"
			],
			monthNames: [
				"Xan", "Feb", "Mar", "Abr", "Mai", "XuÃ±", "Xul", "Ago", "Set", "Out", "Nov", "Dec",
				"Xaneiro", "Febreiro", "Marzo", "Abril", "Maio", "XuÃ±o", "Xullo", "Agosto", "Setembro", "Outubro", "Novembro", "Decembro"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
			srcformat: 'Y-m-d',
			newformat: 'd-m-Y',
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
