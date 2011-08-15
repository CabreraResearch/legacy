; (function ($) {
        
    var PluginName = 'CswFieldTypeUserSelect';
	var NameCol = "User Name";
	var KeyCol = "UserId";
    var StringKeyCol = "UserIdString";
	var ValueCol = "Include";

    var methods = {
        'init': function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var SelectedUserIds = o.propData.children('NodeId').text().trim();
				var $OptionsXml = o.propData.children('options');

				var $CBADiv = $('<div />')
								.appendTo($Div);

				// get data
				var data = new Array();
				var d = 0;
				$OptionsXml.children().each(function () {
					var $user = $(this);
					var $elm = { 
								 'label': $user.children('column[field="' + NameCol + '"]').CswAttrXml('value'),
								 'key': $user.children('column[field="' + KeyCol + '"]').CswAttrXml('value'),
								 'values': [ ($user.children('column[field="' + ValueCol + '"]').CswAttrXml('value') === "True") ]
							   };
					data[d] = $elm;
					d++;
				});

				$CBADiv.CswCheckBoxArray('init', {
					'ID': o.ID + '_cba',
					'cols': [ ValueCol ],
					'data': data,
					'UseRadios': false,
					'Required': o.Required,
					'ReadOnly': o.ReadOnly,
					'onchange': o.onchange
				});
            },
        'save': function(o) {
				var $OptionsXml = o.propData.children('options');
				var $CBADiv = o.$propdiv.children('div').first();
				var formdata = $CBADiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
				for (var r = 0; r < formdata.length; r++) {
					var checkitem = formdata[r][0];
					var $xmlitem = $OptionsXml.find('user:has(column[field="' + KeyCol + '"][value="' + checkitem.key + '"])');
					var $xmlvaluecolumn = $xmlitem.find('column[field="' + ValueCol + '"]');
					if (checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "False")
						$xmlvaluecolumn.CswAttrXml('value', 'True');
					else if (!checkitem.checked && $xmlvaluecolumn.CswAttrXml('value') === "True")
						$xmlvaluecolumn.CswAttrXml('value', 'False');
				} // for( var r = 0; r < formdata.length; r++)
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeUserSelect = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
