/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../../thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var PluginName = "CswMobileFieldType";
    
    var methods = {
	
        'init': function(options) 
		{
            var o = {
                'ID': '',
				'name': '',
                'placeholder': '',
                'cssclass': '',
                ParentId: '', 
                'fieldtype': '', 
                Answer: '', 
                CompliantAnswers: '',
                'Suffix': 'ans',
                'onChange': function() {}
            };
            if (options) $.extend(o, options);

			o.name = tryParseString(o.name,o.ID);
            o.ID = tryParseString(o.ID,o.name);

            var $parent = $(this);
            var $fieldcontain = $('<div class="csw_fieldset" data-role="fieldcontain"></div>');
            var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
    								    .appendTo($fieldcontain)
    								    .CswAttrDom({
								        'id': o.ID + '_fieldset'
								    })
    								.CswAttrXml({
								        'data-role': 'controlgroup',
								        'data-type': 'horizontal'
								    });
            
            switch (FieldType) {
                case "Date":
                    $prop = $retLi.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value });
                    break;
                case "Link":
                    $prop = $retLi.CswLink('init', { ID: propId, href: sf_href, rel: 'external', value: sf_text });
                    break;
                case "List":
                    $prop = $('<select class="csw_prop_select" name="' + propId + '" id="' + propId + '"></select>')
                                                .appendTo($retLi)
                        .selectmenu();
                    var selectedvalue = sf_value;
                    var optionsstr = sf_options;
                    var options = optionsstr.split(',');
                    for (var i = 0; i < options.length; i++) {
                        var $option = $('<option value="' + options[i] + '"></option>')
                                                    .appendTo($prop);
                        if (selectedvalue === options[i]) {
                            $option.CswAttrDom('selected', 'selected');
                        }

                        if (!isNullOrEmpty(options[i])) {
                            $option.val(options[i]);
                        } else {
                            $option.valueOf('[blank]');
                        }
                    }
                    $prop.selectmenu('refresh');
                    break;
                case "Logical":
                    //CswMobileLogical
                    break;
                case "Memo":
                    $prop = $('<textarea name="' + propId + '">' + sf_text + '</textarea>')
                                                    .appendTo($retLi);
                    break;
                case "Number":
                    sf_value = tryParseNumber(sf_value, '');
                    $prop = $retLi.CswInput('init', { type: CswInput_Types.number, ID: propId, value: sf_value });
                    break;
                case "Password":
                        //nada
                    break;
                case "Quantity":
                    $prop = $retLi.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value })
                                                .append(sf_units);
                    break;
                case "Question":
                    //CswMobileQuestion
                    break;
                case "Static":
                    $retLi.append($('<p id="' + propId + '">' + sf_text + '</p>'));
                    break;
                case "Text":
                    $prop = $retLi.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_text });
                    break;
                case "Time":
                    $prop = $retLi.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value });
                    break;
                default:
                    $retLi.append($('<p id="' + propId + '">' + $xmlitem.CswAttrXml('gestalt') + '</p>'));
                    break;
                } // switch (FieldType)
            
            return $fieldcontain;
        }

    };
    	// Method calling logic
	$.fn.CswMobileFieldType = function (method) { /// <param name="$" type="jQuery" />
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
    };


})(jQuery);
