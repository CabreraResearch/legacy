/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeViewReference';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var viewId = tryParseString(o.propData.viewid).trim();
                var viewMode = tryParseString(o.propData.viewmode).trim().toLowerCase();
                var viewName = tryParseString(o.propData.name).trim();

				var $table = $Div.CswTable('init', { 'ID': o.ID + '_tbl' });

				if(o.EditMode != EditMode.AddInPopup.name)
				{
					$table.CswTable('cell', 1, 1).CswImageButton({
						ID: o.ID + '_view',
						ButtonType: CswImageButton_ButtonType.View,
						AlternateText: 'View',
						Required: o.Required,
						onClick: function ($ImageDiv) {
							setCurrentView(viewId, viewMode);
						
							// case 20958 - so that it doesn't treat the view as a Grid Property view
							$.CswCookie('clear', CswCookieName.CurrentNodeId);
							$.CswCookie('clear', CswCookieName.CurrentNodeKey);
						
							window.location = "Main.html";
							return CswImageButton_ButtonType.None; 
						}
					});
					if(!o.ReadOnly)
					{
						$table.CswTable('cell', 1, 2).CswImageButton({
							ID: o.ID + '_edit',
							ButtonType: CswImageButton_ButtonType.Edit,
							AlternateText: 'Edit',
							Required: o.Required,
							onClick: function ($ImageDiv) {
								window.location = "EditView.aspx?step=2&return=Main.html&viewid=" + viewId;
								return CswImageButton_ButtonType.None; 
							}
						});
					}
				} // if(o.EditMode != EditMode.AddInPopup.name)
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
