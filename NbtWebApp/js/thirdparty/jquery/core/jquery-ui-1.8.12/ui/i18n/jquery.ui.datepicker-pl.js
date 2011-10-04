/* Polish initialisation for the jQuery UI date picker plugin. */
/* Written by Jacek Wysocki (jacek.wysocki@gmail.com). */
jQuery(function($){
	$.datepicker.regional['pl'] = {
		closeText: 'Zamknij',
		prevText: '&#x3c;Poprzedni',
		nextText: 'NastÄ™pny&#x3e;',
		currentText: 'DziÅ›',
		monthNames: ['StyczeÅ„','Luty','Marzec','KwiecieÅ„','Maj','Czerwiec',
		'Lipiec','SierpieÅ„','WrzesieÅ„','PaÅºdziernik','Listopad','GrudzieÅ„'],
		monthNamesShort: ['Sty','Lu','Mar','Kw','Maj','Cze',
		'Lip','Sie','Wrz','Pa','Lis','Gru'],
		dayNames: ['Niedziela','PoniedziaÅ‚ek','Wtorek','Åšroda','Czwartek','PiÄ…tek','Sobota'],
		dayNamesShort: ['Nie','Pn','Wt','Åšr','Czw','Pt','So'],
		dayNamesMin: ['N','Pn','Wt','Åšr','Cz','Pt','So'],
		weekHeader: 'Tydz',
		dateFormat: 'dd.mm.yy',
		firstDay: 1,
		isRTL: false,
		showMonthAfterYear: false,
		yearSuffix: ''};
	$.datepicker.setDefaults($.datepicker.regional['pl']);
});
