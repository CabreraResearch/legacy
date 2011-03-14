; (function ($) {
	$.fn.CswWelcome = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getWelcomeItems',
			onLinkClick: function(optSelect) { }, //viewid, actionid, reportid
			onSearchClick: function(optSelect) { }, //viewid
			onAddClick: function(optSelect) { } //nodetypeid
		};

		if (options) {
			$.extend(o, options);
		}
		var $this = $(this);

		CswAjaxXml({
			url: o.Url,
			data: "RoleId=",
			success: function ($xml) {
				var $WelcomeDiv = $('<div id="welcomediv"></div>')
									.appendTo($this)
                                    .css('text-align', 'center')
                                    .css('font-size', '1.2em');

				var $table = $WelcomeDiv.CswLayoutTable('init', {
                                                                 'ID': 'welcometable',
                                                                 'cellset': { rows: 2, columns: 1 },
                                                                 'TableCssClass': 'WelcomeTable',
                                                                 'cellpadding': 10,
                                                                 'align': 'center',
                                                                 'onSwap': function(e, onSwapData) {}
                                                                });
				
				$xml.children().each(function() {

					var $item = $(this);
                    var $cellset = $table.CswLayoutTable('cellset', $item.attr('displayrow'), $item.attr('displaycol'));
					var $imagecell = $cellset[1][1];
					var $textcell = $cellset[2][1];

					if($item.attr('buttonicon') != undefined && $item.attr('buttonicon') != '')
						$imagecell.append( $('<a href=""><img border="0" src="'+ $item.attr('buttonicon') +'"/></a>') );
					
					var optSelect = {
						type: $item.attr('type'),
						viewmode: $item.attr('viewmode'),
						itemid: $item.attr('itemid'), 
						text: $item.attr('text'), 
						iconurl: $item.attr('iconurl'),
						viewid: $item.attr('viewid'),
						actionid: $item.attr('actionid'),
						reportid: $item.attr('reportid'),
						nodetypeid: $item.attr('nodetypeid'),
						linktype: $item.attr('linktype')
					};

					switch(optSelect.linktype)
					{
						case 'Link':
							$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
							$textcell.find('a').click(function() { o.onLinkClick(optSelect); return false; });
							break;
						case 'Search': 
							$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
							$textcell.find('a').click(function() { o.onSearchClick(optSelect); return false; }); //viewid
							break;
						case 'Text':
							$textcell.text(optSelect.text);
							break;
						case 'Add': 
							$textcell.append( $('<a href="">' + optSelect.text + '</a>') );
							$textcell.find('a').click(function() { o.onAddClick(optSelect); return false; }); //nodetypeid
							break;
					}
				});
				
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);


