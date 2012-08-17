/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswDefaultContent = function (options) {

        var o = {
            ID: 'defcont',
            Url: '/NbtWebApp/wsNBT.asmx/getDefaultContent',
            viewid: '',
            viewmode: '',
            onAddNode: null
        };
        if (options) Csw.extend(o, options);

        var $parent = $(this);

        Csw.ajax.post({
            url: o.Url,
            data: { ViewId: o.viewid },
            success: function (data) {

                var addDiv = Csw.literals.div({
                    $parent: $parent,
                    ID: Csw.makeId({ id: o.ID, suffix: 'adddiv' }),
                    cssclass: 'adddiv',
                    text: 'Add New:'
                }).hide();

                function _makeAddLinksRecursive(addObj, parent) {
                    var ul = parent.ul();
                    function onEach(entryObj) {
                        var $li = handleItem({
                            itemJson: entryObj,
                            onAlterNode: o.onAddNode
                        }).appendTo(ul.$);
                        addDiv.show();
                    }

                    if (Csw.contains(addObj, 'entries')) {
                        Csw.each(addObj.entries, onEach);
                        if (ul.children().length() > 0) {
                            _makeAddLinksRecursive(addObj.children, ul);
                        } else {
                            _makeAddLinksRecursive(addObj.children, parent);
                        }
                    } // if(contains(addObj, 'entries'))
                } // _makeAddLinksRecursive()

                _makeAddLinksRecursive(data, addDiv);
            } // success
        }); // ajax

        function handleItem(options) {
            'use strict';
            var o = {
                itemJson: {},
                onAlterNode: null // function (nodeid, nodekey) { },
            };
            if (options) Csw.extend(o, options);
            var text = o.itemJson.text;
            var $li = $('<li><a href="#">' + text + '</a></li>');
            $li.css({ 'list-style-image': 'url(' + o.itemJson.icon + ')' });
            var $a = $li.children('a');

            $a.click(function () {
                $.CswDialog('AddNodeDialog', {
                    text: text,
                    nodetypeid: Csw.string(o.itemJson.nodetypeid),
                    relatednodeid: Csw.string(o.itemJson.relatednodeid), //for Grid Props
                    relatednodename: Csw.string(o.itemJson.relatednodename), //for Grid Props
                    relatednodetypeid: Csw.string(o.itemJson.relatednodetypeid), //for NodeTypeSelect
                    relatedobjectclassid: Csw.string(o.itemJson.relatedobjectclassid),
                    onAddNode: o.onAlterNode
                });
                return false;
            });
            return $li;
        } // handleItem()


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


