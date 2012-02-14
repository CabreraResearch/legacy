/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypeImage';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();
            
            if(o.Multi) {
                $Div.append(Csw.enums.multiEditDefaultValue);
            } else {

                var propVals = o.propData.values,
                    width, 
                    href = '/NbtWebApp/' + Csw.string(propVals.href);

                if(false === Csw.isNullOrEmpty(propVals.width) && 
                   Csw.isNumeric(propVals.width)) {
                    width = Math.abs(Csw.number(propVals.width, 100) - 36);
                } else {
                    width = '';
                }
                
                var fileName = Csw.string(propVals.name).trim();

                var table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                });
                var cell11 = table.cell(1, 1).propDom('colspan', '3');
                var cell21 = table.cell(2, 1).propDom('width', width);
                var cell22 = table.add(2, 2, '<div />').propDom('align', 'right');
                var cell23 = table.add(2, 3, '<div />').propDom('align', 'right');

                if ( false === Csw.isNullOrEmpty(fileName) ) {
                    //Case 24389: IE interprets height and width absolutely, better not to use them at all.
                    cell11.append('<a href="' + href + '" target="_blank"><img src="' + href + '" alt="' + fileName + '"/></a>');
                    cell21.append('<a href="' + href + '" target="_blank">' + fileName + '</a>');                
                } else {
                    cell21.append('(no image selected)');
                }            


                if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    //Edit button
                    cell22.children('div')
                        .$.CswImageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                                AlternateText: 'Edit',
                                ID: Csw.controls.dom.makeId(o.ID, 'edit'),
                                onClick: function () {
                                    $.CswDialog('FileUploadDialog', {
                                        url: '/NbtWebApp/wsNBT.asmx/fileForProp',
                                        params: {
                                            PropId: o.propData.id
                                        },
                                        onSuccess: function () {
                                            o.onReload();
                                        }
                                    });
                                    return Csw.enums.imageButton_ButtonType.None;
                                }
                            });
                    if( false === Csw.isNullOrEmpty(fileName) ) {
                        //Clear button
                        cell23.children('div')
                            .$.CswImageButton({
                                    ButtonType: Csw.enums.imageButton_ButtonType.Clear,
                                    AlternateText: 'Clear',
                                    ID: Csw.controls.dom.makeId(o.ID, 'clr'),
                                    onClick: function () {
                                        /* remember: confirm is globally blocking call */
                                        if (confirm("Are you sure you want to clear this image?")) {
                                            var dataJson = {
                                                PropId: o.propData.id,
                                                IncludeBlob: true
                                            };

                                            Csw.ajax.post({
                                                    url: '/NbtWebApp/wsNBT.asmx/clearProp',
                                                    data: dataJson,
                                                    success: function () { o.onReload(); }
                                                });
                                        }
                                        return Csw.enums.imageButton_ButtonType.None;
                                    }
                                });
                    }
                }
            }
        },
        save: function (o) { 
            Csw.preparePropJsonForSave(o.propData);
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
