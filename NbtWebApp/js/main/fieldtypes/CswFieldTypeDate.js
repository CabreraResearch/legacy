; (function ($) {
        
    var PluginName = 'CswFieldTypeDate';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('value').CswAttrXml('date');
            var Format = ServerDateFormatToJQuery(o.$propxml.children('value').CswAttrXml('dateformat'));

//            if (Value === '1/1/0001')
//                Value = '';

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                     type: CswInput_Types.text,
                                                     value: Value,
                                                     onChange: o.onchange,
                                                     cssclass: 'textinput' // date' date validation broken by alternative formats
                                              }); 
                $TextBox.datepicker({ 'dateFormat': Format });
                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
				$TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) { //$propdiv, $xml
                var $TextBox = o.$propdiv.find('input');
                //o.$propxml.children('value').text($TextBox.val());
				o.$propxml.children('value').CswAttrXml('date', $TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDate = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
