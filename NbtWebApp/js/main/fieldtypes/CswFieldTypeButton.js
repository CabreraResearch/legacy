/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../controls/CswButton.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";        
    var pluginName = 'CswFieldTypeButton';

    var onButtonClick = function(propid, $button, o) {
        var propAttr = tryParseString(propid),
            params;

        $button.CswButton('disable');
        if(isNullOrEmpty(propAttr)) {
            CswError( ChemSW.makeClientSideError(ChemSW.enums.ErrorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.' ));
            $button.CswButton('enable');
        } else {
            params = {
                NodeTypePropAttr: propAttr
            };
            
            CswAjaxJson({
                    url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                    data: params,
                    success: function(data) {
                        $button.CswButton('enable');
                        if(isTrue(data.success)) {
                            switch (data.action) {
                                case ChemSW.enums.CswOnObjectClassClick.reauthenticate:
                                    $.CswCookie('clearAll');
                                    CswAjaxJson({
                                        url: '/NbtWebApp/wsNBT.asmx/reauthenticate',
                                        data: { PropId: propAttr },
                                        success: function () {
                                            unsetChanged(); // case 24669
                                            window.location = "Main.html";
                                        }
                                    });
                                    break;
                                case ChemSW.enums.CswOnObjectClassClick.refresh:
                                    o.onReload();
                                    break;
                                default:
                                    //Nada
                                    break;
                            }
                        }
                    },
                    error: function() {
                        $button.CswButton('enable');
                    }
                });
        }
    };
    
    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var propVals = o.propData.values,
                value = tryParseString(propVals.text, o.propData.name),
                mode = tryParseString(propVals.mode, 'button'),
                $button;

            //Read-only doesn't make sense for buttons
//            if(o.ReadOnly) {
//                $Div.append(value);
//            } else {
            if (mode === 'button'){
                $button = $Div.CswButton('init', {
                                ID: o.ID,
                                enabledText: value,
                                disabledText: value,
                                disableOnClick: true
                            });
            }
            else{
                $button = $Div.CswLink('init', {
                                ID: o.ID,
                                value: value,
                                href: '#'
                            });
            }
            $button.click(function() {
                onButtonClick(o.propid, $button, o);
            });

            if(o.Required) {
                $button.addClass('required');
            }
            //}
        },
        save: function(o) {
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeButton = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
