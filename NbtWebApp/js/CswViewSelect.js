; (function ($) {
	$.fn.CswViewSelect = function (options) {

		var o = {
			ID: '',
			viewid: '',
			onSelect: function(optSelect) { 
				var x = {
							iconurl: '',
							type: '',
							viewid: '',
							viewname: '',
							viewmode: ''
						};
			},
			onSuccess: function() {},
			ClickDelay: 300
		};

		if (options) {
			$.extend(o, options);
		}

		var $selectdiv = $(this);
		$selectdiv.contents().remove();

		getViewSelect(o.viewid);

		function getViewSelect(optSelect) // selectedviewid
		{
			var m = {
				selectedviewid: '',
				viewid: ''
			};

			if (optSelect) {
				$.extend(m, optSelect);
			}

			$viewtreediv = $('<div/>');
			$selectdiv.CswComboBox('init', { 'ID': o.ID + '_combo', 
											 'TopContent': 'Select a View',
											 'SelectContent': $viewtreediv,
											 'Width': '266px' });

			$viewtreediv.CswViewTree({ 'onSelect': onTreeSelect, 'onSuccess': o.onSuccess });
			
		} // getViewSelect()

		function onTreeSelect(optSelect)
		{
			var x = {
					iconurl: '',
					type: '',
					viewid: '',
					viewname: '',
					viewmode: ''
					};
			if(options){
				$.extend(x, optSelect);
			}

			var $newTopContent = $('<div></div>');
            var $table = $newTopContent.CswTable('init', { ID: o.ID + 'selectedtbl' });
			var $cell1 = $table.CswTable('cell', 1, 1);
			var $icondiv = $('<div />').appendTo($cell1);
			$icondiv.css('background-image', x.iconurl);
			$icondiv.css('width', '18px');
			$icondiv.css('height' ,'18px');

			var $cell2 = $table.CswTable('cell', 1, 2);
			$cell2.append(x.viewname);

			$selectdiv.CswComboBox( 'TopContent', $newTopContent );
			setTimeout(function() { $selectdiv.CswComboBox( 'toggle'); }, o.ClickDelay);
			o.onSelect({
						iconurl: x.iconurl,
						type: x.type,
						viewid: x.viewid,
						viewname: x.viewname,
						viewmode: x.viewmode
						});
		}
		
		// For proper chaining support
		return this;

	}; // function(options) {
}) (jQuery);

