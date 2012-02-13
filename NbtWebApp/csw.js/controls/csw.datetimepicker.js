/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

; (function ($) {
    "use strict";    
    var pluginName = 'CswDateTimePicker';

    var methods = {
        init: function (options) {
            var o = {
                ID: '',
                Date: '',
                Time: '',
                DateFormat: '',
                TimeFormat: '',
                DisplayMode: 'Date',    // Date, Time, DateTime
                ReadOnly: false,
                Required: false,
                OnChange: null
            };
            if(options) $.extend(o, options);

            var $ParentDiv = $(this);
            var $Div = $('<div id="'+ o.ID +'"></div>')
                        .appendTo($ParentDiv);

            if(o.ReadOnly) {
                switch(o.DisplayMode) {
                    case "Date":     
                        $Div.CswDiv({ ID: o.ID + "_date", value: o.Date });
                        break;
                    case "Time":
                        $Div.CswDiv({ ID: o.ID + "_time", value: o.Time });
                        break;
                    case "DateTime":
                        $Div.CswDiv({ ID: o.ID + "_time", value: o.Date + " " + o.Time });
                        break;
                }
            } else {
                if( o.DisplayMode === "Date" || o.DisplayMode === "DateTime" ) {
                    var $DateBox = $Div.CswInput('init',{ ID: o.ID + "_date",
                                                          type: Csw.enums.inputTypes.text,
                                                          value: o.Date,
                                                          onChange: o.OnChange,
                                                          width: '80px',
                                                          cssclass: 'textinput' 
                                                  }); 
                    $DateBox.datepicker({ 'dateFormat': Csw.serverDateFormatToJQuery(o.DateFormat) });
                    if(o.Required) $DateBox.addClass("required");
                }

                if( o.DisplayMode === "Time" || o.DisplayMode === "DateTime" ) {
                    var $TimeBox = $Div.CswInput('init',{ ID: o.ID + "_time",
                                                          type: Csw.enums.inputTypes.text,
                                                          cssclass: 'textinput', 
                                                          onChange: o.onchange,
                                                          value: o.Time,
                                                          width: '80px'
                                                     }); 
                    $Div.CswButton('init',{ 'ID': o.ID +'_now',
                                                            'disableOnClick': false,
                                                            'onclick': function () { $TimeBox.val( Csw.getTimeString(new Date(), o.TimeFormat) ); },
                                                            'enabledText': 'Now'
                                                     }); 
                
                    if(o.Required) $TimeBox.addClass("required");
                }
            } // if-else(o.ReadOnly)
            return $Div;
        },
        value: function (readOnly) {
            var $Div = $(this),
                id = this.prop('id'),
                $DateBox = $Div.find( '#' + id + '_date'),
                $TimeBox = $Div.find( '#' + id + '_time'),
                ret = {};
            if ($DateBox.length > 0) {
                ret.date = (false === Csw.bool(readOnly)) ? $DateBox.val() : $DateBox.text(); 
            } 
            if ($TimeBox.length > 0) {
                ret.time = (false === Csw.bool(readOnly)) ? $TimeBox.val() : $TimeBox.text();
            }
            return ret;
        }
    };
    
    // Method calling logic
    $.fn.CswDateTimePicker = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
