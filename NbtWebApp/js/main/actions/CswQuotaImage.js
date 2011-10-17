/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../controls/CswGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswQuotaImage';

    var methods = {
        init: function(options) {
			var o = {
				Url: '/NbtWebApp/wsNBT.asmx/getQuotaPercent',
				ID: 'action_quota_image'
			};
			if(options) $.extend(o, options);

			var $Div = $(this);

			// Quota table
			CswAjaxJson({
				url: o.Url,
				data: {},
				success: function(data) {
					var percentUsed = tryParseNumber( data.result, 0 );
					var image;
					if(percentUsed > 0)
					{
						if(percentUsed <= 33) {
							image = "good.gif";
						}
						else if(percentUsed > 33 && percentUsed <= 66) {
							image = "half.gif";
						}
						else if(percentUsed > 66 ) {
							image = "bad.gif";
						}
						$Div.append('<img src="Images/quota/'+ image +'" title="Quota Usage: '+ percentUsed+'%" />');
					}
				} // success
			}); // ajax()

        } // init
    }; // methods

    
    // Method calling logic
    $.fn.CswQuotaImage = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
