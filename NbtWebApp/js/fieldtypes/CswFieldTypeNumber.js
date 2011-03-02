; (function ($) {
        
    var PluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(nodepk, $xml, onchange) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Value = $xml.children('value').text().trim();
                if(Value == "NaN") Value = '';
                var MinValue = $xml.children('value').attr('minvalue');
                var MaxValue = $xml.children('value').attr('maxvalue');
                var Precision = $xml.children('value').attr('precision');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextBox = $('<input type="text" class="textinput number" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($Div)
                                     .change(onchange);
                    
                    if(MinValue != undefined)
                    {
                        jQuery.validator.addMethod( ID + "_validateFloatMinValue", function(value, element) { 
                                return (this.optional(element) || validateFloatMinValue($(element).val(), MinValue));
                            }, 'Number must be greater than or equal to ' + MinValue);
                        $TextBox.addClass( ID + "_validateFloatMinValue" );
                    }
                    if(MaxValue != undefined)
                    {
                        jQuery.validator.addMethod( ID + "_validateFloatMaxValue", function(value, element) { 
                                return (this.optional(element) || validateFloatMaxValue($(element).val(), MaxValue));
                            }, 'Number must be less than or equal to ' + MaxValue);
                        $TextBox.addClass( ID + "_validateFloatMaxValue" );
                    }
                    if(Precision == undefined || Precision <= 0)
                    {
                        jQuery.validator.addMethod( ID + "_validateInteger", function(value, element) { 
                                return (this.optional(element) || validateInteger($(element).val()));
                            }, 'Value must be an integer');
                        $TextBox.addClass( ID + "_validateInteger" );
                    } else {
                        jQuery.validator.addMethod( ID + "_validateFloatPrecision", function(value, element) { 
                                return (this.optional(element) || validateFloatPrecision($(element).val(), Precision));
                            }, 'Value must be numeric');
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
