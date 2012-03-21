/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = "CswViewSelect";

    var methods = {
        'init': function (options) {

            var o = {
                ID: '',
                onSelect: null,
                onSuccess: null,
                ClickDelay: 300,
                issearchable: false,
                usesession: true
            };

            if (options) {
                $.extend(o, options);
            }

            var $parent = $(this);
            var div = Csw.controls.div({ $parent: $parent });

            var viewTreeDiv = Csw.controls.div();
            var comboBox = div.comboBox({ ID: o.ID + '_combo',
                topContent: 'Select a View',
                selectContent: viewTreeDiv.$,
                width: '266px'
            });

            viewTreeDiv.$.CswViewListTree({
                onSelect: function (optSelect) {
                    _onTreeSelect({
                        ID: o.ID,
                        ClickDelay: o.ClickDelay,
                        $item: optSelect.$item,
                        iconurl: optSelect.iconurl,
                        type: optSelect.type,
                        viewid: optSelect.viewid,
                        viewname: optSelect.viewname,
                        viewmode: optSelect.viewmode,
                        actionid: optSelect.actionid,
                        actionname: optSelect.actionname,
                        actionurl: optSelect.actionurl,
                        reportid: optSelect.reportid,
                        onSelect: o.onSelect,
                        selectDiv: div,
                        comboBox: comboBox
                    });
                },
                onSuccess: function () {
                    Csw.tryExec(o.onSuccess);
                },
                issearchable: o.issearchable,
                usesession: o.usesession
            });
            return div;
        },

        'value': function () {
            var $selectdiv = $(this);
            return {
                'type': $selectdiv.attr('selectedType'),
                'value': $selectdiv.attr('selectedValue')
            };
        }
    };

    // Method calling logic
    $.fn.CswViewSelect = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };

    function _onTreeSelect(optSelect) {
        var x = {
            ID: '',
            ClickDelay: 300,
            $item: '',
            iconurl: '',
            type: '',
            viewid: '',
            viewname: '',
            viewmode: '',
            actionid: '',
            actionname: '',
            actionurl: '',
            reportid: '',
            onSelect: null,
            selectDiv: ''
        };
        if (optSelect) {
            $.extend(x, optSelect);
        }

        var newTopDiv = Csw.controls.div({});
        var table = newTopDiv.table({
            ID: x.ID + 'selectedtbl'
        });
        var iconDiv = table.cell(1, 1).div();

        iconDiv.css('background-image', x.iconurl);
        iconDiv.css('width', '16px');
        iconDiv.css('height', '16px');

        table.cell(1, 2).text(x.viewname.substr(0, 30));

        x.comboBox.topContent(newTopDiv);
        x.selectDiv.propNonDom('selectedType', x.type);
        switch (x.type.toLowerCase()) {
            case 'view':
                x.selectDiv.propNonDom('selectedValue', x.viewid);
                break;
            case 'action':
                x.selectDiv.propNonDom('selectedValue', x.actionid);
                break;
            case 'report':
                x.selectDiv.propNonDom('selectedValue', x.reportid);
                break;
        }

        setTimeout(function () { x.comboBox.toggle(); }, x.ClickDelay);
        if (Csw.isFunction(x.onSelect)) {
            x.onSelect({
                iconurl: x.iconurl,
                type: x.type,
                viewid: x.viewid,
                viewname: x.viewname,
                viewmode: x.viewmode,
                actionid: x.actionid,
                actionname: x.actionname,
                actionurl: x.actionurl,
                reportid: x.reportid
            });
        }
    } // _onTreeSelect()


})(jQuery);

