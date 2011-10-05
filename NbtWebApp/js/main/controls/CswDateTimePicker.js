/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswDateTimePicker';

    var methods = {
        init: function(options) {
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

            if(o.ReadOnly)
            {
                switch(o.DisplayMode)
				{
					case "Date":     $Div.append(o.Date);                break;
					case "Time":     $Div.append(o.Time);                break;
					case "DateTime": $Div.append(o.Date + " " + o.Time); break;
				}
            }
            else 
            {
                if( o.DisplayMode === "Date" || o.DisplayMode === "DateTime" )
				{
					var $DateBox = $Div.CswInput('init',{ ID: o.ID + "_date",
														  type: CswInput_Types.text,
														  value: o.Date,
														  onChange: o.OnChange,
														  width: '80px',
														  cssclass: 'textinput' // date' date validation broken by alternative formats
												  }); 
					$DateBox.datepicker({ 'dateFormat': o.DateFormat });
					if(o.Required) $DateBox.addClass("required");
				}

                if( o.DisplayMode === "Time" || o.DisplayMode === "DateTime" )
				{
					var $TimeBox = $Div.CswInput('init',{ ID: o.ID + "_time",
														  type: CswInput_Types.text,
														  cssclass: 'textinput', // validateTime',
														  onChange: o.onchange,
														  value: o.Time,
														  width: '80px'
													 }); 
					var $nowbutton = $Div.CswButton('init',{ 'ID': o.ID +'_now',
															'disableOnClick': false,
															'onclick': function() { $TimeBox.val( getTimeString(new Date(), o.TimeFormat) ); },
															'enabledText': 'Now'
													 }); 
                
	//				jQuery.validator.addMethod( "validateTime", function(value, element) { 
	//                            return (this.optional(element) || validateTime($(element).val()));
	//                        }, 'Enter a valid time (e.g. 12:30 PM)');

					if(o.Required) $TimeBox.addClass("required");
				}
			} // if-else(o.ReadOnly)
			return $Div;
        },
        value: function() {
            var $Div = $(this);
			var ID = $Div.CswAttrDom('id');
			var $DateBox = $Div.find( '#' + ID + '_date');
			var $TimeBox = $Div.find( '#' + ID + '_time');
			var ret = {};
			if($DateBox.length > 0) ret.Date = $DateBox.val();
			if($TimeBox.length > 0) ret.Time = $TimeBox.val();
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
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
