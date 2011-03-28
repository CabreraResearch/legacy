; (function ($) {
        
    var PluginName = 'CswFieldTypeViewReference';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var ViewId = o.$propxml.children('viewid').text().trim();
                var ViewMode = o.$propxml.children('viewmode').text().trim();
                var ViewName = o.$propxml.children('name').text().trim();

				var $table = $Div.CswTable('init', { 'ID': o.ID + '_tbl' });

                $table.CswTable('cell', 1, 1).CswImageButton({
					'ID': o.ID + '_view',
					'ButtonType': CswImageButton_ButtonType.View,
					'AlternateText': 'View',
					'Required': o.Required,
					'onClick': function ($ImageDiv) {
						$.CswCookie('set', CswCookieName.CurrentView.ViewId, ViewId);
						$.CswCookie('set', CswCookieName.CurrentView.ViewMode, ViewMode);
						window.location = "NewMain.html";
						return CswImageButton_ButtonType.None; 
					}
				});
                if(!o.ReadOnly)
                {
					$table.CswTable('cell', 1, 2).CswImageButton({
						'ID': o.ID + '_edit',
						'ButtonType': CswImageButton_ButtonType.Edit,
						'AlternateText': 'Edit',
						'Required': o.Required,
						'onClick': function ($ImageDiv) {
							window.location = "EditView.aspx?step=2&return=NewMain.html&viewid=" + ViewId;
							return CswImageButton_ButtonType.None; 
						}
					});
				}
            },
        save: function(o) {
				// nothing to save
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeViewReference = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
