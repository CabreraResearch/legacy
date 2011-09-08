/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeFile';

    var methods = {
        init: function(o) { 

                var $Div = $(this);
                $Div.contents().remove();

                var propVals = o.propData.values;
            
                var href = (false === o.Multi) ? tryParseString(propVals.href).trim() : CswMultiEditDefaultValue;
                var fileName = (false === o.Multi) ? tryParseString(propVals.name).trim() : CswMultiEditDefaultValue;

                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell13 = $table.CswTable('cell', 1, 3);

                $cell11.append('<a href="'+ href +'" target="_blank">'+ fileName +'</a>');

                if(!o.ReadOnly && o.EditMode != EditMode.AddInPopup.name)
                {
                    var $editButton = $('<div/>')
                        .appendTo($cell12)
                        .CswImageButton({   
                                            ButtonType: CswImageButton_ButtonType.Edit,
                                            AlternateText: 'Edit',
                                            ID: o.ID + '_edit',
                                            onClick: function ($ImageDiv) 
												{ 
													$.CswDialog( 'FileUploadDialog', {
														url: '/NbtWebApp/wsNBT.asmx/fileForProp',
														params: { 
																	PropId: o.propData.id
																  },
														onSuccess: function()
															{
																o.onReload();
															}
														});
													return CswImageButton_ButtonType.None; 
												}
                                        });
                    var $clearButton = $('<div/>')
                        .appendTo($cell13)
                        .CswImageButton({
                                            ButtonType: CswImageButton_ButtonType.Clear,
                                            AlternateText: 'Clear',
                                            ID: o.ID + '_clr',
                                            onClick: function ($ImageDiv) 
												{ 
													if(confirm("Are you sure you want to clear this file?"))
													{
														var dataJson = {
                                                            PropId: o.propData.id,
                                                            IncludeBlob: true
                                                        };
                                                        
                                                        CswAjaxJson({
															'url': '/NbtWebApp/wsNBT.asmx/clearProp',
															'data': dataJson,
															'success': function() { o.onReload(); }
														});
													}
													return CswImageButton_ButtonType.None; 
												}
                                        });
                }

            },
        save: function(o) {
//                var $TextBox = $propdiv.find('input');
//                o.propData.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeFile = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
