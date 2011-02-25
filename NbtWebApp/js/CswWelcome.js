; (function ($) {
	$.fn.CswWelcome = function (options) {

		var o = {
			Url: '/NbtWebApp/wsNBT.asmx/getWelcomeItems',
			onLinkClick: function(optSelect) { }, //viewid, actionid, reportid
			onSearchClick: function(viewid) { },
			onAddClick: function(nodetypeid) { }
		};

		if (options) {
			$.extend(o, options);
		}
		var $this = $(this);

		CswAjaxXml({
			url: o.Url,
			data: "RoleId=",
			success: function ($xml) {
				var $WelcomeDiv = $('<div id="welcomediv"><table class="WelcomeTable" align="center" cellpadding="20"></table></div>')
									.appendTo($this);
				var $table = $WelcomeDiv.children('table');
				
				$xml.children().each(function() {

					//<item id=" + WelcomeRow["welcomeid"].ToString() + "\"";
					//      type=\"" + WelcomeRow["componenttype"].ToString() + "\"";
					//      buttonicon=\"" + IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString() + "\"";
					//      text=\"" + LinkText + "\"";
					//      displayrow=\"" + WelcomeRow["display_row"].ToString() + "\"";
					//      displaycol=\"" + WelcomeRow["display_col"].ToString() + "\"";

					var $item = $(this);
					var $cell = getTableCell($table, $item.attr('displayrow'), $item.attr('displaycol'));

					if($item.attr('buttonicon') != undefined && $item.attr('buttonicon') != '')
						$cell.append( $('<a href=""><img src="'+ $item.attr('buttonicon') +'"/></a><br/><br/>') );
					
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
							$cell.append( $('<a href="">' + optSelect.text + '</a>') );
							$cell.find('a').click(function() { o.onLinkClick(optSelect); return false; });
							break;
						case 'Search': 
							$cell.append( $('<a href="">' + optSelect.text + '</a>') );
							$cell.find('a').click(function() { o.onSearchClick(optSelect.viewid); return false; });
							break;
						case 'Text':
							$cell.text(optSelect.text);
							break;
						case 'Add': 
							$cell.append( $('<a href="">' + optSelect.text + '</a>') );
							$cell.find('a').click(function() { o.onAddClick(optSelect.nodetypeid); return false; });
							break;
					}
				});
				
			} // success{}
		});

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);


