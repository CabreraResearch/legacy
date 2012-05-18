/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    "use strict";

    Csw.controls.viewSelect = Csw.controls.viewSelect ||
        Csw.controls.register('viewSelect', function (cswParent, params) {

            var cswPrivateVar = {
                viewurl: '/NbtWebApp/wsNBT.asmx/getViewSelect',
                recenturl: '/NbtWebApp/wsNBT.asmx/getViewSelectRecent',
                ID: 'viewselect',
                onSelect: null,
                onSuccess: null,
                //ClickDelay: 300,
                issearchable: false,
                includeRecent: true,
                //usesession: true,
                hidethreshold: 5,
                maxHeight: '',

                div: null
            };
            if (params) {
                $.extend(cswPrivateVar, params);
            }

            var cswPublicRet = {};

            cswPrivateVar.addCategory = function (catobj) {

                var fieldsetid = Csw.makeId(cswPrivateVar.ID, '', catobj.category + '_fs', '', false);
                var $fieldset = cswPrivateVar.vsdiv.$.find('#' + fieldsetid);
                if ($fieldset.length === 0) {
                    $fieldset = $('<fieldset id="' + fieldsetid + '" class="viewselectfieldset"></fieldset>')
                                    .appendTo(cswPrivateVar.vsdiv.$);
                }

                $fieldset.contents().remove();
                $fieldset.append('<legend class="viewselectlegend">' + catobj.category + '</legend>');

                var morediv = Csw.literals.moreDiv({
                    ID: Csw.makeId(cswPrivateVar.ID, '', catobj.category + '_morediv'),
                    $parent: $fieldset
                });

                var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
                var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
                var row = 1;
                var tbl = showntbl;
                morediv.moreLink.hide();

                Csw.each(catobj.items, function (itemobj, itemname) {
                    if (row > cswPrivateVar.hidethreshold && tbl === showntbl) {
                        row = 1;
                        tbl = hiddentbl;
                        morediv.moreLink.show();
                    }

                    var iconcell = tbl.cell(row, 1).addClass('viewselecticoncell');
                    iconcell.img({ src: itemobj.iconurl });

                    var linkcell = tbl.cell(row, 2).addClass('viewselectitemcell');
                    linkcell.text(itemobj.name);

                    iconcell.bind('click', function () { cswPrivateVar.handleSelect(itemobj); });
                    linkcell.bind('click', function () { cswPrivateVar.handleSelect(itemobj); });

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

            cswPrivateVar.handleSelect = function (itemobj) {

                var $newTopContent = $('<div></div>');
                var table = Csw.literals.table({
                    $parent: $newTopContent,
                    ID: cswPrivateVar.ID + 'selectedtbl'
                });
                var iconDiv = table.cell(1, 1).div();

                iconDiv.css('background-image', itemobj.iconurl);
                iconDiv.css('width', '16px');
                iconDiv.css('height', '16px');

                table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

                cswPrivateVar.comboBox.topContent($newTopContent);
                cswPrivateVar.div.propNonDom('selectedType', itemobj.type);
                cswPrivateVar.div.propNonDom('selectedName', itemobj.name);
                switch (itemobj.type.toLowerCase()) {
                    case 'view':
                        cswPrivateVar.div.propNonDom('selectedValue', itemobj.viewid);
                        break;
                    case 'action':
                        cswPrivateVar.div.propNonDom('selectedValue', itemobj.actionid);
                        break;
                    case 'report':
                        cswPrivateVar.div.propNonDom('selectedValue', itemobj.reportid);
                        break;
                }

                //setTimeout(function () { cswPrivateVar.comboBox.toggle(); }, cswPrivateVar.ClickDelay);
                Csw.tryExec(cswPrivateVar.onSelect, itemobj);
            }; // cswPrivateVar.handleSelect()


            // Constructor
            (function () {
                cswPrivateVar.div = cswParent.div();
                cswPublicRet = Csw.dom({}, cswPrivateVar.div);

                cswPrivateVar.vsdiv = Csw.literals.div({ ID: Csw.makeId(cswPrivateVar.ID, '', 'vsdiv') });
                if (false == Csw.isNullOrEmpty(cswPrivateVar.maxHeight)) {
                    cswPrivateVar.vsdiv.css({ maxHeight: cswPrivateVar.maxHeight });
                }
                cswPrivateVar.comboBox = cswPrivateVar.div.comboBox({
                    ID: cswPrivateVar.ID + '_combo',
                    topContent: 'Select a View',
                    selectContent: cswPrivateVar.vsdiv.$, /* NO! Refactor to use Csw.literals and more wholesome methods. */
                    width: '266px'
                });

                $.extend(cswPublicRet, cswPrivateVar.comboBox);

                Csw.ajax.post({
                    url: cswPrivateVar.viewurl,
                    data: {
                        IsSearchable: cswPrivateVar.issearchable,
                        IncludeRecent: cswPrivateVar.includeRecent
                    },
                    stringify: false,
                    success: function (data) {
                        Csw.each(data.viewselectitems, cswPrivateVar.addCategory);
                        Csw.tryExec(cswPrivateVar.onSuccess);
                    }
                });
            })();


            cswPublicRet.value = function () {
                return {
                    type: cswPrivateVar.div.propNonDom('selectedType'),
                    value: cswPrivateVar.div.propNonDom('selectedValue'),
                    name: cswPrivateVar.div.propNonDom('selectedName')
                };
            };

            cswPublicRet.val = cswPublicRet.value;

            cswPublicRet.refreshRecent = function () {
                if (cswPrivateVar.includeRecent) {
                    Csw.ajax.post({
                        url: cswPrivateVar.recenturl,
                        success: function (data) {
                            Csw.each(data.viewselectitems, cswPrivateVar.addCategory);
                        }
                    });
                }
            }; // refreshRecent()

            return cswPublicRet;
        });
})();
