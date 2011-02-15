; (function ($) {
        
    var PluginName = 'CswFieldTypeLogicalSet';

    var methods = {
        init: function(nodepk, $xml) {

                $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var LogicalSetXml = $xml.children('LogicalSetXml').first();
                
                var $CBADiv = $('<div />')
                                .appendTo($Div);
                
                var $xml = $($(LogicalSetXml).text());
                
                // get columns
                var cols = new Array();
                var c = 0;

                $xml.find('yvalue')
                    .first()
                    .children()
                    .each(function() {
                        cols[c] = this.nodeName;
                        c++;
                    });

                // get data
                var data = new Array();
                var d = 0;
                $xml.find('yvalue').each(function () {
                    var $this = $(this);
                    var values = new Array();
                    var r = 0;
                    for(var c = 0; c < cols.length; c++)
                    {
                        values[r] = ($this.children(cols[c]).text() == "1");
                        r++;
                    }

                    var $elm = { 'label': $this.attr('y'),
                                 'key': $this.attr('y'),
                                 'values': values };
                    data[d] = $elm;
                    d++;
                });

                $CBADiv.CswCheckBoxArray('init', {
                                            'ID': ID + '_cba',
                                            'cols': cols,
                                            'data': data
                                        });


            },
        save: function($propdiv, $xml) {
                var $CBADiv = $propdiv.children('div').first();
                var data = $CBADiv.CswCheckBoxArray( 'getdata' );
                

//                var $CheckboxImage = $propdiv.find('div');
//                $xml.children('checked').text($CheckboxImage.attr('alt'));
            }
    };
    

//    function onClick($ImageDiv, Required) 
//    {
//        var currentValue = $ImageDiv.attr('alt');
//	    var newValue = CswImageButton_ButtonType.CheckboxNull;
//	    var newAltText = "null";
//        if (currentValue == "null") {
//		    newValue = CswImageButton_ButtonType.CheckboxTrue;
//            newAltText = "true";
//	    } else if (currentValue == "false") {
//		    if (Required == "true") {
//			    newValue = CswImageButton_ButtonType.CheckboxTrue;
//                newAltText = "true";
//		    } else {
//			    newValue = CswImageButton_ButtonType.CheckboxNull;
//                newAltText = "null";
//		    }
//	    } else if (currentValue == "true") {
//		    newValue = CswImageButton_ButtonType.CheckboxFalse;
//            newAltText = "false";
//	    }
//        $ImageDiv.attr('alt', newAltText);
//        return newValue;
//    } // onClick()



    // Method calling logic
    $.fn.CswFieldTypeLogicalSet = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);





