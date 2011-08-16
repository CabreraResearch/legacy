/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeViewPickList';
	var nameCol = "View Name";
    var keyCol = "nodeviewid";
    var valueCol = "Include";

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var selectedViewIds = tryParseString(o.propData.viewid).trim();
				var optionData = o.propData.options;
				var selectMode = o.propData.selectmode;
				var $CBADiv = $('<div />')
								.appendTo($Div);

				// get data
				var data = [];
				var d = 0;
				for (var optId in optionData) {
				    if (optionData.hasOwnProperty(optId)) {
				        var thisOpt = optionData[optId];
				        var $elm = {
				            label: thisOpt[nameCol],
				            key: thisOpt[keyCol],
				            values: [isTrue(thisOpt[valueCol])]
				        };
				        data[d] = $elm;
				        d++;
				    }
				}

            $CBADiv.CswCheckBoxArray('init', {
				ID: o.ID + '_cba',
				cols: [ valueCol ],
				data: data,
				UseRadios: (selectMode === 'Single'),
				Required: o.Required,
				ReadOnly: o.ReadOnly,
				onchange: o.onchange
			});
            },
        'save': function(o) {
				var optionData = o.propData.options;
				var $CBADiv = o.$propdiv.children('div').first();
				var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
            alert('Come back to fix save.');
                for (var r = 0; r < formdata.length; r++) {
					var checkitem = formdata[r][0];
					var $xmlitem = optionData.find('user:has(column[field="' + keyCol + '"][value="' + checkitem.key + '"])');
					var $xmlvaluecolumn = $xmlitem.find('column[field="' + valueCol + '"]');
					if (checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "False")
						$xmlvaluecolumn.CswAttrXml('value', 'True');
					else if (!checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "True")
						$xmlvaluecolumn.CswAttrXml('value', 'False');
				} // for( var r = 0; r < formdata.length; r++)
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeViewPickList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
