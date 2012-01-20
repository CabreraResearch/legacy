/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../pagecmp/CswDialog.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";        
    var pluginName = 'CswFieldTypeImage';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            
            if(o.Multi) {
                $Div.append(CswMultiEditDefaultValue);
            } else {

                var propVals = o.propData.values,
                    width, 
                    href = '/NbtWebApp/wsNBT.asmx/' + tryParseString(propVals.href);

                if(false === isNullOrEmpty(propVals.width) && 
                   isNumeric(propVals.width)) {
                    width = Math.abs(tryParseNumber(propVals.width, 100) - 36);
                } else {
                    width = '';
                }
                
                var fileName = tryParseString(propVals.name).trim();

                var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });
                var $cell11 = $table.CswTable('cell', 1, 1).CswAttrDom('colspan', '3');
                var $cell21 = $table.CswTable('cell', 2, 1).CswAttrDom('width', width);
                var $cell22 = $table.CswTable('cell', 2, 2).CswAttrDom('align', 'right');
                var $cell23 = $table.CswTable('cell', 2, 3).CswAttrDom('align', 'right');

                if ( false === isNullOrEmpty(fileName) ) {
                    //Case 24389: IE interprets height and width absolutely, better not to use them at all.
                    $('<a href="' + href + '" target="_blank"><img src="' + href + '" alt="' + fileName + '"/></a>')
                        .appendTo($cell11);
                    $cell21.append('<a href="' + href + '" target="_blank">' + fileName + '</a>');                
                } else {
                    $cell21.append('(no image selected)');
                }            


                if (!o.ReadOnly && o.EditMode != EditMode.AddInPopup.name) {
                    //Edit button
                    $('<div/>')
                        .appendTo($cell22)
                        .CswImageButton({
                                ButtonType: CswImageButton_ButtonType.Edit,
                                AlternateText: 'Edit',
                                ID: o.ID + '_edit',
                                onClick: function() {
                                    $.CswDialog('FileUploadDialog', {
                                        url: '/NbtWebApp/wsNBT.asmx/fileForProp',
                                        params: {
                                            PropId: o.propData.id
                                        },
                                        onSuccess: function() {
                                            o.onReload();
                                        }
                                    });
                                    return CswImageButton_ButtonType.None;
                                }
                            });
                    if( false === isNullOrEmpty(fileName) ) {
                        //Clear button
                        $('<div/>')
                            .appendTo($cell23)
                            .CswImageButton({
                                    ButtonType: CswImageButton_ButtonType.Clear,
                                    AlternateText: 'Clear',
                                    ID: o.ID + '_clr',
                                    onClick: function() {
                                        /* remember: confirm is globally blocking call */
                                        if (confirm("Are you sure you want to clear this image?")) {
                                            var dataJson = {
                                                PropId: o.propData.id,
                                                IncludeBlob: true
                                            };

                                            CswAjaxJson({
                                                    url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                                    data: dataJson,
                                                    success: function() { o.onReload(); }
                                                });
                                        }
                                        return CswImageButton_ButtonType.None;
                                    }
                                });
                    }
                }
            }
        },
        save: function(o) { 
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeImage = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
