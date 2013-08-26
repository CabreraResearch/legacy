/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    "use strict";

    //Case 30479: gods help you if you need more than viewSelect in the same page at the same time.
    //until that day, let's self-satisfy our promises.
    var promise = null;

    Csw.composites.viewSelect = Csw.composites.viewSelect ||
        Csw.composites.register('viewSelect', function (cswParent, params) {

            var cswPrivate = {
                viewMethod: 'Views/ViewSelect',
                name: 'viewselect',
                onSelect: null,
                onSuccess: null,
                //ClickDelay: 300,
                issearchable: false,
                includeRecent: true,
                //usesession: true,
                hidethreshold: 5,
                maxHeight: '',
                fieldSets: {

                },
                useCache: true,
                div: null
            };

            Csw.extend(cswPrivate, params);

            var cswPublic = {};

            cswPrivate.addCategory = function (catobj) {
                cswPrivate.fieldSets[catobj.category + '_fs'] = cswPrivate.fieldSets[catobj.category + '_fs'] ||
                    cswPrivate.vsdiv.fieldSet({ cssclass: 'viewselectfieldset' });


                cswPrivate.fieldSets[catobj.category + '_fs'].empty();
                cswPrivate.fieldSets[catobj.category + '_fs'].legend({ cssclass: 'viewselectlegend', value: catobj.category });

                var morediv = cswPrivate.fieldSets[catobj.category + '_fs'].moreDiv({
                    name: 'morediv'
                });

                var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
                var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
                var row = 1;
                var tbl = showntbl;
                morediv.moreLink.hide();

                Csw.iterate(catobj.items, function (itemobj, itemname) {
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
                    name: cswPrivate.name + '_selectedtbl'
                });
                var iconDiv = table.cell(1, 1).div();

                iconDiv.css('background-image', itemobj.iconurl);
                iconDiv.css('width', '16px');
                iconDiv.css('height', '16px');

                table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

                cswPrivate.comboBox.topContent($newTopContent);
                cswPrivate.div.data('selectedType', itemobj.type);
                cswPrivate.div.data('selectedName', itemobj.name);
                cswPrivate.div.data('selectedValue', itemobj.itemid);

                Csw.tryExec(cswPrivate.onSelect, itemobj);
            }; // cswPrivate.handleSelect()

            var toDo = [];

            // Constructor
            (function ctor() {
                toDo.push(ctor);

                var getAjaxPromise = function () {
                    if (promise && promise.abort) {
                        promise.abort();
                    }
                    promise = Csw.ajaxWcf.post({
                        urlMethod: cswPrivate.viewMethod,
                        watchGlobal: false,
                        data: {
                            IsSearchable: cswPrivate.issearchable,
                            IncludeRecent: cswPrivate.includeRecent
                        },
                        success: function (ret) {
                            makeSelect(ret);
                            Csw.setCachedWebServiceCall(cswPrivate.viewMethod, ret);
                            return Csw.tryExec(cswPrivate.onSuccess);
                        }
                    });
                    return promise;
                };

                var makeSelect = function (data) {
                    if (data) {
                        cswParent.empty();
                        cswPrivate.div = cswParent.div();
                        cswPublic = Csw.dom({}, cswPrivate.div);

                        cswPrivate.vsdiv = Csw.literals.div();
                        if (false == Csw.isNullOrEmpty(cswPrivate.maxHeight)) {
                            cswPrivate.vsdiv.css({ maxHeight: cswPrivate.maxHeight });
                        }
                        cswPrivate.comboBox = cswPrivate.div.comboBox({
                            name: cswPrivate.name + '_combo',
                            topContent: 'Select a View',
                            selectContent: cswPrivate.vsdiv.$, /* NO! Refactor to use Csw.literals and more wholesome methods. */
                            width: '266px'
                        });

                        Csw.extend(cswPublic, cswPrivate.comboBox);

                        Csw.iterate(data.categories, cswPrivate.addCategory);
                    }
                };

                if (true === cswPrivate.useCache) {
                    Csw.getCachedWebServiceCall(cswPrivate.viewMethod)
                        .then(makeSelect)
                        .then(function () {
                            cswPrivate.useCache = false;
                            return cswParent.viewSelect({
                                onSelect: cswPrivate.onSelect,
                                onSuccess: cswPrivate.onSuccess,
                                issearchable: cswPrivate.issearchable,
                                includeRecent: cswPrivate.includeRecent,
                                hidethreshold: cswPrivate.hidethreshold,
                                maxHeight: cswPrivate.maxHeight,
                                useCache: false
                            });
                        });
                } else {
                    getAjaxPromise();
                }
                //getAjaxPromise();

                toDo.push(promise);
                return promise;
            })();


            cswPublic.value = function () {
                return {
                    type: cswPrivate.div.data('selectedType'),
                    value: cswPrivate.div.data('selectedValue'),
                    name: cswPrivate.div.data('selectedName')
                };
            };

            cswPublic.hide = function () {
                cswPrivate.comboBox.hide();
            };

            cswPublic.show = function () {
                cswPrivate.comboBox.show();
            };

            cswPublic.val = cswPublic.value;

            cswPublic.promise = Q.all(toDo);

            return cswPublic;
        });
})();
