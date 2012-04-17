/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    "use strict";

    Csw.controls.viewSelect = Csw.controls.viewSelect ||
        Csw.controls.register('viewSelect', function (cswParent, params) {

            var internal = {
                viewurl: '/NbtWebApp/wsNBT.asmx/getViewSelect',
                recenturl: '/NbtWebApp/wsNBT.asmx/getViewSelectRecent',
                ID: 'viewselect',
                onSelect: null,
                onSuccess: null,
                //ClickDelay: 300,
                issearchable: false,
                //usesession: true,
                hidethreshold: 5,
                maxHeight: '',

                div: null
            };
            if (params) {
                $.extend(internal, params);
            }

            var external = {};

            internal.addCategory = function (catobj) {

                var fieldsetid = Csw.makeId(internal.ID, '', catobj.category + '_fs', '', false);
                var $fieldset = internal.vsdiv.$.find('#' + fieldsetid);
                if ($fieldset.length === 0) {
                    $fieldset = $('<fieldset id="' + fieldsetid + '" class="viewselectfieldset"></fieldset>')
                                    .appendTo(internal.vsdiv.$);
                }

                $fieldset.contents().remove();
                $fieldset.append('<legend class="viewselectlegend">' + catobj.category + '</legend>');

                var morediv = Csw.literals.moreDiv({
                    ID: Csw.makeId(internal.ID, '', catobj.category + '_morediv'),
                    $parent: $fieldset
                });

                var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
                var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
                var row = 1;
                var tbl = showntbl;
                morediv.moreLink.hide();

                Csw.each(catobj.items, function (itemobj, itemname) {
                    if (row > internal.hidethreshold && tbl === showntbl) {
                        row = 1;
                        tbl = hiddentbl;
                        morediv.moreLink.show();
                    }

                    var iconcell = tbl.cell(row, 1).addClass('viewselecticoncell');
                    iconcell.img({ src: itemobj.iconurl });

                    var linkcell = tbl.cell(row, 2).addClass('viewselectitemcell');
                    linkcell.text(itemobj.name);

                    iconcell.bind('click', function () { internal.handleSelect(itemobj); });
                    linkcell.bind('click', function () { internal.handleSelect(itemobj); });

                    linkcell.$.hover(
                        function () {
                            iconcell.addClass('viewselectitemhover');
                            linkcell.addClass('viewselectitemhover');
                        },
                        function () {
                            iconcell.removeClass('viewselectitemhover');
                            linkcell.removeClass('viewselectitemhover');
                        });

                    row += 1;
                }); // Csw.each() items
            }; // addCategory()

            internal.handleSelect = function (itemobj) {

                var $newTopContent = $('<div></div>');
                var table = Csw.literals.table({
                    $parent: $newTopContent,
                    ID: internal.ID + 'selectedtbl'
                });
                var iconDiv = table.cell(1, 1).div();

                iconDiv.css('background-image', itemobj.iconurl);
                iconDiv.css('width', '16px');
                iconDiv.css('height', '16px');

                table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

                internal.comboBox.topContent($newTopContent);
                internal.div.propNonDom('selectedType', itemobj.type);
                internal.div.propNonDom('selectedName', itemobj.name);
                switch (itemobj.type.toLowerCase()) {
                    case 'view':
                        internal.div.propNonDom('selectedValue', itemobj.viewid);
                        break;
                    case 'action':
                        internal.div.propNonDom('selectedValue', itemobj.actionid);
                        break;
                    case 'report':
                        internal.div.propNonDom('selectedValue', itemobj.reportid);
                        break;
                }

                //setTimeout(function () { internal.comboBox.toggle(); }, internal.ClickDelay);
                Csw.tryExec(internal.onSelect, itemobj);
            }; // internal.handleSelect()


            // Constructor
            (function () {
                internal.div = cswParent.div();
                external = Csw.dom({}, internal.div);
                
                internal.vsdiv = Csw.literals.div({ ID: Csw.makeId(internal.ID, '', 'vsdiv') });
                if (false == Csw.isNullOrEmpty(internal.maxHeight)) {
                    internal.vsdiv.css({ maxHeight: internal.maxHeight });
                }
                internal.comboBox = internal.div.comboBox({
                    ID: internal.ID + '_combo',
                    topContent: 'Select a View',
                    selectContent: internal.vsdiv.$, /* NO! Refactor to use Csw.literals and more wholesome methods. */
                    width: '266px'
                });

                $.extend(external, internal.comboBox);

                Csw.ajax.post({
                    url: internal.viewurl,
                    data: { IsSearchable: internal.issearchable },
                    stringify: false,
                    success: function (data) {
                        Csw.each(data.viewselectitems, internal.addCategory);
                        Csw.tryExec(internal.onSuccess);
                    }
                });
            })();


            external.value = function () {
                return {
                    type: internal.div.propNonDom('selectedType'),
                    value: internal.div.propNonDom('selectedValue'),
                    name: internal.div.propNonDom('selectedName')
                };
            };

            external.val = external.value;

            external.refreshRecent = function () {
                Csw.ajax.post({
                    url: internal.recenturl,
                    success: function (data) {
                        Csw.each(data.viewselectitems, internal.addCategory);
                    }
                });
            };

            return external;
        });
})();
