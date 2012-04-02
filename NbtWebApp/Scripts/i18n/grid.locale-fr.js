;(function($){
/**
 * jqGrid French Translation
 * Tony Tomov tony@trirand.com
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "Enregistrements {0} - {1} sur {2}",
		emptyrecords: "Aucun enregistrement Ã  afficher",
		loadtext: "Chargement...",
		pgtext : "Page {0} sur {1}"
	},
	search : {
		caption: "Recherche...",
		Find: "Chercher",
		Reset: "Annuler",
		odata : ['Ã©gal', 'diffÃ©rent', 'infÃ©rieur', 'infÃ©rieur ou Ã©gal','supÃ©rieur','supÃ©rieur ou Ã©gal', 'commence par','ne commence pas par','est dans',"n'est pas dans",'finit par','ne finit pas par','contient','ne contient pas'],
		groupOps: [	{ op: "AND", text: "tous" },	{ op: "OR",  text: "aucun" }	],
		matchText: " correspondance",
		rulesText: " rÃ¨gles"
	},
	edit : {
		addCaption: "Ajouter",
		editCaption: "Editer",
		bSubmit: "Valider",
		bCancel: "Annuler",
		bClose: "Fermer",
		saveData: "Les donnÃ©es ont changÃ© ! Enregistrer les modifications ?",
		bYes: "Oui",
		bNo: "Non",
		bExit: "Annuler",
		msg: {
			required: "Champ obligatoire",
			number: "Saisissez un nombre correct",
			minValue: "La valeur doit Ãªtre supÃ©rieure ou Ã©gale Ã ",
			maxValue: "La valeur doit Ãªtre infÃ©rieure ou Ã©gale Ã ",
			email: "n'est pas un email correct",
			integer: "Saisissez un entier correct",
			url: "n'est pas une adresse correcte. PrÃ©fixe requis ('http://' or 'https://')",
			nodefined : " n'est pas dÃ©fini!",
			novalue : " la valeur de retour est requise!",
			customarray : "Une fonction personnalisÃ©e devrait retourner un tableau (array)!",
			customfcheck : "Une fonction personnalisÃ©e devrait Ãªtre prÃ©sente dans le cas d'une vÃ©rification personnalisÃ©e!"
		}
	},
	view : {
		caption: "Voir les enregistrement",
		bClose: "Fermer"
	},
	del : {
		caption: "Supprimer",
		msg: "Supprimer les enregistrements sÃ©lectionnÃ©s ?",
		bSubmit: "Supprimer",
		bCancel: "Annuler"
	},
	nav : {
		edittext: " ",
		edittitle: "Editer la ligne sÃ©lectionnÃ©e",
		addtext:" ",
		addtitle: "Ajouter une ligne",
		deltext: " ",
		deltitle: "Supprimer la ligne sÃ©lectionnÃ©e",
		searchtext: " ",
		searchtitle: "Chercher un enregistrement",
		refreshtext: "",
		refreshtitle: "Recharger le tableau",
		alertcap: "Avertissement",
		alerttext: "Veuillez sÃ©lectionner une ligne",
		viewtext: "",
		viewtitle: "Afficher la ligne sÃ©lectionnÃ©e"
	},
	col : {
		caption: "Afficher/Masquer les colonnes",
		bSubmit: "Valider",
		bCancel: "Annuler"
	},
	errors : {
		errcap : "Erreur",
		nourl : "Aucune adresse n'est paramÃ©trÃ©e",
		norecords: "Aucun enregistrement Ã  traiter",
		model : "Nombre de titres (colNames) <> Nombre de donnÃ©es (colModel)!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam",
				"Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"
			],
			monthNames: [
				"Jan", "FÃ©v", "Mar", "Avr", "Mai", "Jui", "Jul", "Aou", "Sep", "Oct", "Nov", "DÃ©c",
				"Janvier", "FÃ©vrier", "Mars", "Avril", "Mai", "Juin", "Juillet", "Aout", "Septembre", "Octobre", "Novembre", "DÃ©cembre"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j == 1 ? 'er' : 'e';},
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
