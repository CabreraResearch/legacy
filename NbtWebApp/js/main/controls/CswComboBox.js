/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswComboBox';

    $.fn.CswComboBox = function (method) {

        var methods = {
            init: function (options) {

                function handleClick() {
                    if (Csw.isFunction(o.onClick)) {
                        o.onClick();
                    }
                    toggle($TopDiv, $ChildDiv);
                }

                var o = {
                    ID: '',
                    TopContent: '',
                    SelectContent: 'This ComboBox Is Empty!',
                    Width: '180px',
                    onClick: null // function () { }
                };

                if (options) {
                    $.extend(o, options);
                }

                var $Div = $(this);
                $Div.contents().remove();

                var $TopDiv = $('<div id="' + o.ID + '_top" class="CswComboBox_TopDiv"></div>')
                            .appendTo($Div)
                            .css('width', o.Width);

                var table = Csw.controls.table({
                    $parent: $TopDiv,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                });
                table.propDom('width', '100%');

                var $cell1 = table.add(1, 1, o.TopContent);
                Csw.controls.dom.propDom($cell1, 'width', '100%');
                
                var $cell2 = table.cell(1, 2);
                $cell2.addClass("CswComboBox_ImageCell");

                var hideTo;
                var $ChildDiv = $('<div id="' + o.ID + '_child" class="CswComboBox_ChildDiv">')
                                  .appendTo($Div)
                                  .css('width', o.Width)
                                  .append(o.SelectContent)
                                  .hover(function () { clearTimeout(hideTo); }, function () { hideTo = setTimeout(function () { $ChildDiv.hide(); }, 750); });

                $cell1.click(handleClick);

                $cell2.CswImageButton({ 'ButtonType': Csw.enums.imageButton_ButtonType.Select,
                    'ID': o.ID + '_top_img',
                    'AlternateText': '',
                    'onClick': handleClick
                });

            },
            TopContent: function (content) {
                var $Div = $(this);
                var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                var $table = $TopDiv.children('table');
                /* Case 24440. We'll come back to this when we refactor this class. CswTable here. */
                var $cell1 = $table.CswTable('cell', 1, 1);
                $cell1.text('');
                $cell1.contents().remove();
                $cell1.append(content);
            },
            toggle: function () {
                var $Div = $(this);
                var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                var $ChildDiv = $Div.children('.CswComboBox_ChildDiv');
                toggle($TopDiv, $ChildDiv);
            },
            close: function () {
                var $Div = $(this);
                var $TopDiv = $Div.children('.CswComboBox_TopDiv');
                var $ChildDiv = $Div.children('.CswComboBox_ChildDiv');
                close($TopDiv, $ChildDiv);
            }
        };

        function toggle($TopDiv, $ChildDiv) {
            $TopDiv.toggleClass('CswComboBox_TopDiv_click');
            $ChildDiv.toggle();
        }
        function close($TopDiv, $ChildDiv) {
            $TopDiv.removeClass('CswComboBox_TopDiv_click');
            $ChildDiv.hide();
        }

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);