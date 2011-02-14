; (function ($) {
        
    var PluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var Value = $xml.children('value').text();
                var MinValue = $xml.children('value').attr('minvalue');
                var MaxValue = $xml.children('value').attr('maxvalue');
                var Precision = $xml.children('value').attr('precision');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextBox = $('<input type="text" class="textinput" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($Div);
                    //$TextBox.change(function() { validate($Div, $TextBox, MinValue, MaxValue, Precision) });
                    
                    if(Precision == undefined || Precision <= 0)
                    {
                        // Integer
                        $TextBox.addClass("number");
                        $TextBox.min(MinValue);
                        $TextBox.max(MaxValue);
                    } else {
                        // Float
                        jQuery.validator.addMethod( ID + "_validateFloatMinValue", function(value, element) { 
                                return (this.optional(element) || validateFloatMinValue($(element).val(), MinValue))
                            }, 'Number must be greater than or equal to ' + MinValue);
                        jQuery.validator.addMethod( ID + "_validateFloatMaxValue", function(value, element) { 
                                return (this.optional(element) || validateFloatMaxValue($(element).val(), MaxValue))
                            }, 'Number must be less than or equal to ' + MaxValue);
                        jQuery.validator.addMethod( ID + "_validateFloatPrecision", function(value, element) { 
                                return (this.optional(element) || validateFloatPrecision($(element).val(), Precision))
                            }, 'Value must be numeric');
                        $TextBox.addClass( ID + "_validateFloatMinValue" );
                        $TextBox.addClass( ID + "_validateFloatMaxValue" );
                        $TextBox.addClass( ID + "_validateFloatPrecision" );
                    }   

                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function($propdiv, $xml) {
                var $TextBox = $propdiv.find('input');
                $xml.children('value').text($TextBox.val());
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
