/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeLink';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();

            var propVals = o.propData.values;
            var text = (false === o.Multi) ? tryParseString(propVals.text).trim() : CswMultiEditDefaultValue;
            var href = (false === o.Multi) ? tryParseString(propVals.href).trim() : CswMultiEditDefaultValue;

            var $Link = $('<a href="' + href + '" target="_blank">' + text + '</a>&nbsp;&nbsp;');

            if(o.ReadOnly) {
                $Div.append($Link);
            } else {
                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

				$Link.appendTo($table.CswTable('cell', 1, 1));

                $('<div/>')
                    .appendTo($table.CswTable('cell', 1, 2))
                    .CswImageButton({
                        ButtonType: CswImageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: o.ID + '_edit',
                        Required: o.Required,
                        onClick: function ($ImageDiv) { 
								$edittable.show();
								return CswImageButton_ButtonType.None; 
							}
                    });

				var $edittable = $Div.CswTable('init', { ID: o.ID + '_edittbl' })
									.hide();

                $( '<span>Text</span>' )
                    .appendTo($edittable.CswTable('cell', 1, 1));
                
                var $edittextcell = $edittable.CswTable('cell', 1, 2);
                var $edittext = $edittextcell.CswInput('init',{ID: o.ID + '_text',
                                                                type: CswInput_Types.text,
                                                                value: text,
                                                                onChange: o.onchange
                                                                }); 
                
                $( '<span>URL</span>' )
                    .appendTo($edittable.CswTable('cell', 2, 1));
                
                var $edithrefcell = $edittable.CswTable('cell', 2, 2);
				var $edithref = $edithrefcell.CswInput('init',{ID: o.ID + '_href',
                                                               type: CswInput_Types.text,
                                                               value: href,
                                                               onChange: o.onchange
                                                       }); 

                if(o.Required && href === '') {
                    $edittable.show();
					$edittext.addClass("required");
					$edithref.addClass("required");
                }
				$edittext.clickOnEnter(o.$savebtn);
				$edithref.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) { 
            var $edittext = o.$propdiv.find('#' + o.ID + '_text');
            var $edithref = o.$propdiv.find('#' + o.ID + '_href');
			var propVals = o.propData.values;	
            propVals.text = $edittext.val();
			propVals.href = $edithref.val();
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeLink = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
