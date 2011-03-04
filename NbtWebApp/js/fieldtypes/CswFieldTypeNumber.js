; (function ($) {
        
    var PluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('value').text().trim();
            if(Value == "NaN") Value = '';
            var MinValue = o.$propxml.children('value').attr('minvalue');
            var MaxValue = o.$propxml.children('value').attr('maxvalue');
            var Precision = o.$propxml.children('value').attr('precision');

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $('<input type="text" class="textinput number" id="'+ o.ID +'" name="' + o.ID + '" value="'+ Value +'" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange);
                    
                if(MinValue != undefined)
                {
                    jQuery.validator.addMethod( o.ID + "_validateFloatMinValue", function(value, element) { 
                            return (this.optional(element) || validateFloatMinValue($(element).val(), MinValue));
                        }, 'Number must be greater than or equal to ' + MinValue);
                    $TextBox.addClass( o.ID + "_validateFloatMinValue" );
                }
                if(MaxValue != undefined)
                {
                    jQuery.validator.addMethod( o.ID + "_validateFloatMaxValue", function(value, element) { 
                            return (this.optional(element) || validateFloatMaxValue($(element).val(), MaxValue));
                        }, 'Number must be less than or equal to ' + MaxValue);
                    $TextBox.addClass( o.ID + "_validateFloatMaxValue" );
                }
                if(Precision == undefined || Precision <= 0)
                {
                    jQuery.validator.addMethod( o.ID + "_validateInteger", function(value, element) { 
                            return (this.optional(element) || validateInteger($(element).val()));
                        }, 'Value must be an integer');
                    $TextBox.addClass( o.ID + "_validateInteger" );
                } else {
                    jQuery.validator.addMethod( o.ID + "_validateFloatPrecision", function(value, element) { 
                            return (this.optional(element) || validateFloatPrecision($(element).val(), Precision));
                        }, 'Value must be numeric');
                    $TextBox.addClass( o.ID + "_validateFloatPrecision" );
                }

                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
            }
        },
        save: function(o) { //$propdiv, $xml
                var $TextBox = o.$propdiv.find('input');
                o.$propxml.children('value').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNumber = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
