/// <reference path="~app/CswApp-vsdoc.js" />


(function () {
    "use strict";

    Csw.controls.viewSelect = Csw.controls.viewSelect ||
        Csw.controls.register('viewSelect', function (cswParent, params) {

            var cswPrivate = {
                uri: 'Views',
                viewMethod: 'ViewSelect',
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
                Csw.extend(cswPrivate, params);
            }

            var cswPublic = {};

            cswPrivate.addCategory = function (catobj) {

                var fieldsetid = Csw.makeSafeId(cswPrivate.ID, '', catobj.category + '_fs', '', false);
                var $fieldset = cswPrivate.vsdiv.$.find('#' + fieldsetid);
                if ($fieldset.length === 0) {
                    $fieldset = $('<fieldset id="' + fieldsetid + '" class="viewselectfieldset"></fieldset>')
                                    .appendTo(cswPrivate.vsdiv.$);
                }

                $fieldset.contents().remove();
                $fieldset.append('<legend class="viewselectlegend">' + catobj.category + '</legend>');

                var morediv = Csw.literals.moreDiv({
                    ID: Csw.makeSafeId(cswPrivate.ID, '', catobj.category + '_morediv'),
                    $parent: $fieldset
                });

                var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
                var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
                var row = 1;
                var tbl = showntbl;
                morediv.moreLink.hide();

                Csw.each(catobj.items, function (itemobj, itemname) {
                    if (row > cswPrivate.hidethreshold && tbl === showntbl) {
                        row = 1;
                        tbl = hiddentbl;
                        morediv.moreLink.show();
                    }

                    var iconcell = tbl.cell(row, 1).addClass('viewselecticoncell');
                    iconcell.img({ src: itemobj.iconurl });

                    var linkcell = tbl.cell(row, 2).addClass('viewselectitemcell');
                    linkcell.text(itemobj.name);

                    iconcell.bind('click', function () { cswPrivate.handleSelect(itemobj); });
                    linkcell.bind('click', function () { cswPrivate.handleSelect(itemobj); });

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

            cswPrivate.handleSelect = function (itemobj) {

                var $newTopContent = $('<div></div>');
                var table = Csw.literals.table({
                    $parent: $newTopContent,
                    ID: cswPrivate.ID + 'selectedtbl'
                });
                var iconDiv = table.cell(1, 1).div();

                iconDiv.css('background-image', itemobj.iconurl);
                iconDiv.css('width', '16px');
                iconDiv.css('height', '16px');

                table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

                cswPrivate.comboBox.topContent($newTopContent);
                cswPrivate.div.propNonDom('selectedType', itemobj.type);
                cswPrivate.div.propNonDom('selectedName', itemobj.name);
                cswPrivate.div.propNonDom('selectedValue', itemobj.itemid);
                
                Csw.tryExec(cswPrivate.onSelect, itemobj);
            }; // cswPrivate.handleSelect()


            // Constructor
            (function () {
                cswPrivate.div = cswParent.div();
                cswPublic = Csw.dom({}, cswPrivate.div);

                cswPrivate.vsdiv = Csw.literals.div({ ID: Csw.makeId(cswPrivate.ID, 'vsdiv') });
                if (false == Csw.isNullOrEmpty(cswPrivate.maxHeight)) {
                    cswPrivate.vsdiv.css({ maxHeight: cswPrivate.maxHeight });
                }
                cswPrivate.comboBox = cswPrivate.div.comboBox({
                    ID: cswPrivate.ID + '_combo',
                    topContent: 'Select a View',
                    selectContent: cswPrivate.vsdiv.$, /* NO! Refactor to use Csw.literals and more wholesome methods. */
                    width: '266px'
                });

                Csw.extend(cswPublic, cswPrivate.comboBox);

                cswPrivate.ajax = Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.viewMethod,
                    data: {
                        IsSearchable: cswPrivate.issearchable,
                        IncludeRecent: cswPrivate.includeRecent
                    },
                    success: function (data) {
                        Csw.each(data.categories, cswPrivate.addCategory);
                        Csw.tryExec(cswPrivate.onSuccess);
                    }
                });
            })();


            cswPublic.value = function () {
                return {
                    type: cswPrivate.div.propNonDom('selectedType'),
                    value: cswPrivate.div.propNonDom('selectedValue'),
                    name: cswPrivate.div.propNonDom('selectedName')
                };
            };

            cswPublic.val = cswPublic.value;

            cswPublic.refreshRecent = function () {
                if (cswPrivate.includeRecent) {
                    Csw.ajaxWcf.post({
                        urlMethod: cswPrivate.viewMethod,
                        data: {
                            LimitToRecent: true
                        },
                        success: function (data) {
                            Csw.each(data.categories, cswPrivate.addCategory);
                        }
                    });
                }
            }; // refreshRecent()

            return cswPublic;
        });
})();
