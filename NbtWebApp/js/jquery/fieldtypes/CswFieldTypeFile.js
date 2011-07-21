/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.2-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeFile';

    var methods = {
        init: function(o) { //o.nodeid, o.$propxml, o.onchange

                var $Div = $(this);
                $Div.contents().remove();

                var Href = o.$propxml.children('href').text().trim();
                var FileName = o.$propxml.children('name').text().trim();

                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
                var $cell11 = $table.CswTable('cell', 1, 1);
                var $cell12 = $table.CswTable('cell', 1, 2);
                var $cell13 = $table.CswTable('cell', 1, 3);

                $cell11.append('<a href="'+ Href +'" target="_blank">'+ FileName +'</a>');

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
														'url': '/NbtWebApp/wsNBT.asmx/fileForProp',
														'params': { 
																	'PropId': o.$propxml.CswAttrXml('id')
																  },
														'onSuccess': function()
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
                                                            PropId: o.$propxml.CswAttrXml('id'),
                                                            IncludeBlob: true
                                                        };
                                                        
                                                        CswAjaxJSON({
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
//                o.$propxml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeFile = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
