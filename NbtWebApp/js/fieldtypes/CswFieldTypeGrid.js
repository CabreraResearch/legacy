; (function ($) {
		
	var PluginName = 'CswFieldTypeGrid';

	var methods = {
		init: function(nodepk, $xml, cswnbtnodekey) {

				var $Div = $(this);
				$Div.children().remove();

				var ID = $xml.attr('id');
				var Required = ($xml.attr('required') == "true");
				var ReadOnly = ($xml.attr('readonly') == "true");

				var Value = $xml.children('value').text();
				console.log("grid prop XML" + $xml);
				//$Div.append("xml starts here" + $xml);
				if(ReadOnly)
				{
					$Div.append(Value);
				}
				else 
				{
					$($Div).CswNodeGrid( {}, {viewid:'1478', nodeid: nodepk, 'cswnbtnodekey': cswnbtnodekey} );
				}
			},
		save: function($propdiv, $xml) {
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
			}
	};
	
	// Method calling logic
	$.fn.CswFieldTypeGrid = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);
