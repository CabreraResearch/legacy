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
        if (options) $.extend(o, options);

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
                });

                function _makeAddLinksRecursive(addObj, parent) {
                    var ul = parent.ul();
                    function onEach(entryObj) {
                        var $li = Csw.handleMenuItem({
                            $ul: ul.$,
                            itemKey: entryObj.text,
                            itemJson: entryObj,
                            onAlterNode: o.onAddNode
                        }).appendTo(ul.$);
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

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


