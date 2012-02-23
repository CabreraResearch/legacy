/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswDiv.js" />
/// <reference path="../controls/CswTable.js" />

; (function ($) {

    var pluginName = 'CswFieldTypeComments';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();

            //var Value = extractCDataValue($xml.children('text'));
            var propVals = o.propData.values;
            var rows = tryParseString(propVals.rows);
            var columns = tryParseString(propVals.columns);


            //comments:  [ { datetime: '12/31/2012', commenter: 'david', message: 'yuck' }, { ... } ]

            var $commentsDiv = $Div.CswDiv('init',{
                'ID': makeId({ID: o.ID, suffix: 'commentsDiv'}),
                'value': '',
                'cssclass': 'scrollingdiv'
            });
            var $myTable = $commentsDiv.CswTable('init', {
							ID: makeId({ID: o.ID, suffix: 'commentsTbl'}),
							TableCssClass: '',
							CellCssClass: '',
							cellpadding: 2,
							cellspacing: 0,
							align: '',
							width: '',
							cellalign: 'top',
							cellvalign: 'top',
							//onCreateCell: function (e, $table, $cell, row, column) { },
							FirstCellRightAlign: false,
							OddCellRightAlign: false,
							border: 0
						});
            var arow=0;
            var bgclass='';
            each(propVals.comments,function(acomment){
               arow+=1;
               var $cell1 = $myTable.CswTable('cell',arow*2,1); 
               var $cell2 = $myTable.CswTable('cell',arow*2,2); 
                if( (arow % 2)===0 ){
                    bgclass='OddRow';
                }
                else{
                    bgclass='EvenRow';
                }
                $cell1.addClass(bgclass);
                $cell2.addClass(bgclass);
                $cell1.append(acomment.datetime);
                $cell2.append(acomment.commenter);
               var $cell3 = $myTable.CswTable('cell',arow*2 + 1,1); 
                $cell3.CswAttrDom('colspan','2');
                $cell3.addClass(bgclass);
                $cell3.append(acomment.message);
            });
            if (false === o.ReadOnly) {
                var $TextArea = $('<textarea id="' + o.ID + '" name="' + o.ID + '" rows="' + rows + '" cols="' + columns + '"></textarea>')
                                    .appendTo($Div)
                                    .change(o.onchange);
//                $Div.CswButton('init', {
//				ID: makeId({ID: o.ID, suffix: 'commentBtn'}),
//				enabledText: 'Add',
//				disabledText: 'Adding...',
//			    cssclass: '',
//				hasText: true,
//				disableOnClick: true,
//				inputType: CswInput_Types.button.name,
//				primaryicon: '',
//				secondaryicon: '',
//				ReadOnly: false,
//                //'Required': false,
//				onclick: null //function () { }
//            });
       
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { newmessage: null };
            var $TextArea = o.$propdiv.find('textarea');
            if (false === isNullOrEmpty($TextArea)) {
                attributes.newmessage = $TextArea.val();
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeComments = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
